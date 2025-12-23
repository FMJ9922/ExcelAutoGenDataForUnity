using UnityEngine;
using UnityEditor;
using System.IO;
using OfficeOpenXml;
using System.Collections.Generic;
using System;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

/// <summary>
/// 使用EPPlus获取表格数据，同时导出对应的Json以及Class.
/// </summary>
public class ExcelExporter
{
    /// <summary>
    /// Excel表格路径
    /// </summary>
    private const string excelPath = "../Assets/Excel";

    /// <summary>
    /// 导出的Json路径
    /// </summary>
    private const string configPath = "../Assets/Json";

    /// <summary>
    /// 导出的类路径
    /// </summary>
    private const string classPath = "../Assets/Scripts/Data";

    /// <summary>
    /// 属性行
    /// </summary>
    private const int propertyIndex = 2;

    /// <summary>
    /// 类型行
    /// </summary>
    private const int typeIndex = 3;

    /// <summary>
    /// 值行
    /// </summary>
    private const int valueIndex = 4;
    
    private static Dictionary<string, List<string>> enumValues = new Dictionary<string, List<string>>();


    [MenuItem("Tools/ExportExcel")]
    private static void ExportConfigs()
    {
        try
        {
            enumValues.Clear();
            FileInfo[] files = Files.LoadFiles(excelPath);
            string[] fileNames = new string[files.Length];
            for (int i = 0;i<files.Length;i++)
            {
                var file = files[i];
                //过滤文件
                if (file.Extension != ".xlsx") continue;
                ExcelPackage excelPackage = new ExcelPackage(file);
                ExcelWorksheets worksheets = excelPackage.Workbook.Worksheets;
                //只导表1
                ExcelWorksheet worksheet = worksheets[1];

                ExportJson(worksheet, Path.GetFileNameWithoutExtension(file.FullName));
                ExportClass(worksheet, Path.GetFileNameWithoutExtension(file.FullName));
                fileNames[i] = Path.GetFileNameWithoutExtension(file.FullName);
            }

            ExportDataManagaerDefine(fileNames);

            AssetDatabase.Refresh();
        }
        catch (Exception e)
        {
            Debug.LogError(e.ToString());
        }
    }

    private static void ExportClass(ExcelWorksheet worksheet, string fileName)
    {
        string[] properties = GetProperties(worksheet);
        StringBuilder sb = new StringBuilder();

        // 收集所有的enum类型
        Dictionary<string, string> enumFields = new Dictionary<string, string>();
        for (int col = 1; col <= properties.Length; col++)
        {
            string fieldType = GetType(worksheet, col);
            string fieldName = properties[col - 1];

            if (fieldType == "enum")
            {
                enumFields[fieldName] = fieldName;
            }
        }

        sb.Append("using System;\t\n");
        sb.Append("using Newtonsoft.Json;\t\n");
        sb.Append("[Serializable]\t\n");
        sb.Append($"public class {fileName}: IData\n"); //类名
        sb.Append("{\n");

        for (int col = 1; col <= properties.Length; col++)
        {
            string fieldType = GetType(worksheet, col);
            string fieldName = properties[col - 1];
            sb.Append($"\t[JsonProperty(\"{fieldName}\")]\n");
            if (fieldType == "enum")
            {
                sb.Append($"\tpublic {fieldName} {fieldName}Value {{ get; set; }}\n");
            }
            else
            {
                sb.Append($"\tpublic {fieldType} {fieldName} {{ get; set; }}\n");
            }
        }

        sb.Append("}\n\n");

        foreach (var enumField in enumFields)
        {
            string enumName = enumField.Key;
            if (enumValues.ContainsKey(enumName))
            {
                sb.Append($"public enum {enumName}\n");
                sb.Append("{\n");

                var values = enumValues[enumName];
                for (int i = 0; i < values.Count; i++)
                {
                    string enumValue = values[i];
                    // 处理枚举值命名（确保是有效的C#标识符）
                    string validEnumValue = MakeValidEnumIdentifier(enumValue, i);
                    sb.Append($"\t{validEnumValue} = {i}");
                    if (i < values.Count - 1)
                    {
                        sb.Append(",");
                    }

                    sb.Append("\n");
                }

                sb.Append("}\n\n");
            }

            Files.SaveFile(classPath, string.Format("{0}.cs", fileName), sb.ToString());

        }
    }
    
    private static string MakeValidEnumIdentifier(string value, int index)
    {
        if (string.IsNullOrEmpty(value))
        {
            return $"Value{index}";
        }
        
        // 移除空格和特殊字符，只保留字母、数字和下划线
        string result = Regex.Replace(value, @"[^\w]", "_");
        
        // 如果以数字开头，添加下划线前缀
        if (char.IsDigit(result[0]))
        {
            result = "_" + result;
        }
        
        // 确保不是C#关键字
        string[] keywords = { "abstract", "as", "base", "bool", "break", "byte", "case", "catch", "char", 
            "checked", "class", "const", "continue", "decimal", "default", "delegate", "do", "double", 
            "else", "enum", "event", "explicit", "extern", "false", "finally", "fixed", "float", "for", 
            "foreach", "goto", "if", "implicit", "in", "int", "interface", "internal", "is", "lock", 
            "long", "namespace", "new", "null", "object", "operator", "out", "override", "params", 
            "private", "protected", "public", "readonly", "ref", "return", "sbyte", "sealed", "short", 
            "sizeof", "stacked", "static", "string", "struct", "switch", "this", "throw", "true", 
            "try", "typeof", "uint", "ulong", "unchecked", "unsafe", "ushort", "using", "virtual", 
            "void", "volatile", "while" };
        
        if (keywords.Contains(result.ToLower()))
        {
            result = "@" + result;
        }
        
        // 如果为空字符串，使用默认值
        if (string.IsNullOrEmpty(result))
        {
            result = $"Value{index}";
        }
        
        return result;
    }



    private static void ExportDataManagaerDefine(string[] fileNames)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("using System.Collections.Generic;\t\n");
        sb.Append("public partial class DataManager : Singleton<DataManager>\t\n");
        sb.Append("{\t\n");
        sb.Append("\tprivate List<string> loadList = new List<string>(){\t\n");
        for (int i = 0; i < fileNames.Length; i++)
        {
            sb.Append($"\t\t\"{fileNames[i]}\",\n");
        }
        sb.Append("\t};\n");
        
        for (int i = 0; i < fileNames.Length; i++)
        {
            sb.Append($"\tpublic static DataEntry<int, {fileNames[i]}> {fileNames[i]} {{ get; private set; }}\n");
        }
        
        sb.Append("\tprivate void BuildData()\t\n");
        sb.Append("\t{\n");
        for (int i = 0; i < fileNames.Length; i++)
        {
            sb.Append($"\t\t{fileNames[i]} = DataEntry<int, {fileNames[i]}>.FromJson(_jsonDic[\"{fileNames[i]}\"]);\n");
        }
        
        sb.Append("\t}\n");
        
        sb.Append("}\n");
        Files.SaveFile(classPath, "DataManagerDefine.cs", sb.ToString());
    }

    private static void ExportJson(ExcelWorksheet worksheet, string fileName)
    {
        List<string> jsonEntries = new List<string>();
        string[] properties = GetProperties(worksheet);
        int rowCount = worksheet.Dimension.End.Row;

        for (int row = 4; row <= rowCount; row++)
        {
            List<string> keyValues = new List<string>();
            for (int col = 1; col <= properties.Length; col++)
            {
                string fieldName = properties[col - 1];
                string fieldType = GetType(worksheet, col);
                string value = worksheet.Cells[row, col].Text;
                string jsonValue = Convert(fieldType, value, fieldName);
                keyValues.Add($"\t\t\"{fieldName}\": {jsonValue}");
            }

            string rowJson = "\t{\n" + string.Join(",\n", keyValues) + "\n\t}";
            jsonEntries.Add(rowJson);
        }

        string finalJson = "{\n" +
                           $"\t\"datas\": [\n" +
                           string.Join(",\n", jsonEntries) + "\n" +
                           "\t]\n" +
                           "}";
    
        Files.SaveFile(configPath, $"{fileName}.json", finalJson);
    }

    private static string[] GetProperties(ExcelWorksheet worksheet)
    {
        string[] properties = new string[worksheet.Dimension.End.Column];
        for (int col = 1; col <= worksheet.Dimension.End.Column; col++)
        {
            if (worksheet.Cells[propertyIndex, col].Text == "")
                throw new System.Exception(string.Format("第{0}行第{1}列为空", propertyIndex, col));
            properties[col - 1] = worksheet.Cells[propertyIndex, col].Text;
        }

        return properties;
    }

    private static string[] GetValues(ExcelWorksheet worksheet, int col)
    {
        //容量减去前三行
        string[] values = new string[worksheet.Dimension.End.Row - 3];
        for (int row = valueIndex; row <= worksheet.Dimension.End.Row; row++)
        {
            values[row - valueIndex] = worksheet.Cells[row, col].Text;
        }

        return values;
    }

    private static string GetType(ExcelWorksheet worksheet, int col)
    {
        return worksheet.Cells[typeIndex, col].Text;
    }

    private static string Convert(string type, string value, string fieldName)
    {
        // 统一处理空值输入
        if (string.IsNullOrWhiteSpace(value))
        {
            return GetDefaultValueByType(type); // 专门处理默认值
        }

        try
        {
            switch (type.ToLower()) // 统一转小写增强兼容性
            {
                case "int":
                case "int32":
                    return int.TryParse(value, out int i) ? i.ToString() : "0";

                case "int64":
                case "long":
                    return long.TryParse(value, out long l) ? l.ToString() : "0";

                case "float":
                    return ParseFloatWithSuffix(value);

                case "double":
                    return double.TryParse(value, out double d) ? d.ToString("0.0############") : "0.0";

                case "string":
                    return $"\"{value.Trim().Replace("\"", "\\\"")}\""; // 处理双引号转义

                case "int[]":
                    return ParseIntArray(value);
                case "float[]":
                    return ParseFloatArray(value);
                case "enum":
                    return ParseEnumValue(value, fieldName);

                // 可扩展其他类型
                default:
                    throw new Exception($"Unsupported type: {type}");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"转换失败: 类型={type}, 值={value}, 错误={ex.Message}");
            return GetDefaultValueByType(type); // 确保容错
        }
    }
    
    private static string ParseEnumValue(string value, string enumName)
    {
        // 收集枚举值
        if (!enumValues.ContainsKey(enumName))
        {
            enumValues[enumName] = new List<string>();
        }

        // 去重并保持顺序
        var enumList = enumValues[enumName];
        if (!enumList.Contains(value))
        {
            enumList.Add(value);
        }

        // 返回枚举值的索引
        return enumList.IndexOf(value).ToString();
    }

// 辅助方法：获取类型默认值
    private static string GetDefaultValueByType(string type)
    {
        switch (type.ToLower())
        {
            case "int":
            case "int32":
            case "int64":
            case "long":
                return "0";

            case "float":
                return "0";

            case "double":
                return "0.0";

            case "string":
                return "\"\"";

            case "int[]":
                return "[]";
            case "float[]":
                return "[]";
                
            case "enum":
                return "Null";

            default:
                return "null";
        }
    }

// 辅助方法：处理浮点数特殊格式
    private static string ParseFloatWithSuffix(string value)
    {
        // 清理非法字符（保留数字、小数点和负号）
        string sanitized = Regex.Replace(value, @"[^\d\.-]", "");

        // 处理前导零缺失（如 ".01" → "0.01"）
        if (sanitized.StartsWith(".")) sanitized = "0" + sanitized;
        if (sanitized.StartsWith("-.")) sanitized = "-0." + sanitized.Substring(2);

        // 解析并添加 'f' 后缀
        if (float.TryParse(sanitized, NumberStyles.Any, CultureInfo.InvariantCulture, out float f))
        {
            return $"{f.ToString("0.0#####")}"; // 控制精度避免科学计数法
        }

        return "0"; // 解析失败返回默认
    }

// 辅助方法：处理数组
    private static string ParseIntArray(string value)
    {
        string cleanValue = Regex.Replace(value, @"[\[\]\s]", "");
        if (string.IsNullOrEmpty(cleanValue)) return "[]";

        var elements = cleanValue.Split(new[] { ',', '，' }, StringSplitOptions.RemoveEmptyEntries)
            .Select(e =>
            {
                string sanitized = Regex.Replace(e, @"[^\d-]", "");
                return int.TryParse(sanitized, out int num) ? num.ToString() : "0";
            });

        return $"[{string.Join(",", elements)}]";
    }
    
    private static string ParseFloatArray(string value)
    {
        string cleanValue = Regex.Replace(value, @"[\[\]\s]", "");
        if (string.IsNullOrEmpty(cleanValue)) return "[]";

        var elements = cleanValue.Split(new[] { ',', '，' }, StringSplitOptions.RemoveEmptyEntries)
            .Select(e =>
            {
                return ParseFloatWithSuffix(e);
            });

        return $"[{string.Join(",", elements)}]";
    }
}

public class Files
{
    public static FileInfo[] LoadFiles(string path)
    {
        path = string.Format("{0}/{1}", Application.dataPath, path);

        if (Directory.Exists(path))
        {
            DirectoryInfo directory = new DirectoryInfo(path);
            List<FileInfo> files = new List<FileInfo>();
            foreach (var file in directory.GetFiles("*"))
            {
                if (file.Name.EndsWith(".meta"))
                    continue;
                if (file.Name.StartsWith("~")) continue;

                files.Add(file);
            }

            return files.ToArray();
        }
        else
        {
            throw new System.Exception("路径不存在");
        }
    }

    public static void SaveFile(string path, string fileName, string fileContent)
    {
        path = string.Format("{0}/{1}", Application.dataPath, path);

        if (Directory.Exists(path))
        {
            path = string.Format("{0}/{1}", path, fileName);
            if (File.Exists(path))
            {
                File.Delete(path);
            }

            File.WriteAllText(path, fileContent);
        }
        else
        {
            throw new System.Exception("路径不存在");
        }
    }
}