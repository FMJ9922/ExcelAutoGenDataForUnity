# Excel Auto Gen Data For Unity - User Manual

## Overview

This is an Excel data sheet export tool for Unity projects. It can automatically convert Excel tables into JSON configuration files and corresponding C# data classes. The tool uses the EPPlus library to read Excel files and supports various data types, including enum types and array types.

## Main Features

### 1. Data Export
- Export Excel tables as JSON configuration files
- Automatically generate corresponding C# data classes
- Generate static accessors for the data manager (DataManager)

### 2. Supported Data Types
- **Basic Types**: `int`, `int32`, `int64`, `long`, `float`, `double`, `string`
- **Array Types**: `int[]`, `float[]`
- **Enum Types**: `enum` (automatically extracts enum values from data and generates enum definitions)

### 3. Automatic Code Generation
- Generate corresponding C# classes for each Excel table
- Automatically generate enum type definitions
- Generate a unified data manager class for easy data access

## Excel Table Format Requirements

### Fixed Row Structure
| Row Number | Content Description | Example |
|------------|---------------------|---------|
| Row 2 | Field Name (Property Name) | `id`, `name`, `type` |
| Row 3 | Field Type (Data Type) | `int`, `string`, `enum` |
| Row 4 onwards | Data Rows (Actual Data) | `1`, `"Weapon"`, `"Sword"` |

### Excel File Structure Example
| Column A | Column B | Column C |
|----------|----------|----------|
| (Row 2) Field Name | `id` | `name` | `itemType` |
| (Row 3) Field Type | `int` | `string` | `enum` |
| (Row 4) Data Row 1 | `1001` | `"Long Sword"` | `"Weapon"` |
| (Row 5) Data Row 2 | `1002` | `"Shield"` | `"Armor"` |

## Output Files

### 1. JSON Configuration Files
- Location: `../Assets/Json/`
- Format: JSON array containing all data rows
```json
{
  "datas": [
    {
      "id": 1001,
      "name": "Long Sword",
      "itemType": 0
    },
    {
      "id": 1002,
      "name": "Shield",
      "itemType": 1
    }
  ]
}
```

### 2. C# Data Classes
- Location: `../Assets/Scripts/Data/`
- Format: Serializable classes containing all fields
```csharp
using System;
using Newtonsoft.Json;

[Serializable]
public class ItemData: IData
{
    [JsonProperty("id")]
    public int id { get; set; }
    
    [JsonProperty("name")]
    public string name { get; set; }
    
    [JsonProperty("itemType")]
    public itemType itemTypeValue { get; set; }
}
```

### 3. Enum Definitions (if enum fields exist)
```csharp
public enum itemType
{
    Weapon = 0,
    Armor = 1
}
```

### 4. Data Manager Definition
- File: `DataManagerDefine.cs`
- Function: Provides static access interfaces for all data tables

## Usage Instructions

### Step 1: Prepare Excel Files
1. Place Excel files in the `../Assets/Excel/` directory
2. Ensure files use the correct format (see format requirements above)
3. Use supported field types

### Step 2: Export Data
In the Unity Editor, click menu: **Tools → ExportExcel**

### Step 3: Use Data
```csharp
// Access data in code
var item = DataManager.ItemData.GetById(1001);
Debug.Log(item.name); // Output: "Long Sword"
var item1 = DataManager.ItemData.GetByIndex(0);
var count = DataManager.ItemData.Count;
```

## Detailed Field Type Support

### 1. Integer (`int`, `int32`)
- **Excel Format**: Plain numbers (e.g., `123`, `-456`)
- **JSON Output**: `123`
- **C# Type**: `int`

### 2. Long Integer (`int64`, `long`)
- **Excel Format**: Plain numbers (supports large numbers)
- **JSON Output**: `123456789012`
- **C# Type**: `long`

### 3. Floating Point (`float`)
- **Excel Format**: Decimals (e.g., `3.14`, `.5`, `-2.5`)
- **JSON Output**: `3.14`
- **Special Handling**: Supports formats with omitted leading zeros (e.g., `.5` → `0.5`)

### 4. Double Precision Floating Point (`double`)
- **Excel Format**: Decimals
- **JSON Output**: `3.14159265358979`
- **C# Type**: `double`

### 5. String (`string`)
- **Excel Format**: Any text
- **JSON Output**: `"Text content"`
- **Special Handling**: Automatically escapes double quotes

### 6. Integer Array (`int[]`)
- **Excel Format**: Comma-separated numbers (e.g., `1,2,3` or `[1,2,3]`)
- **JSON Output**: `[1,2,3]`
- **C# Type**: `int[]`

### 7. Float Array (`float[]`)
- **Excel Format**: Comma-separated decimals (e.g., `1.5,2.3,3.14`)
- **JSON Output**: `[1.5,2.3,3.14]`
- **C# Type**: `float[]`

### 8. Enum Type (`enum`)
- **Excel Format**: Text representation of enum values (e.g., `"Weapon"`, `"Armor"`)
- **JSON Output**: Enum index (integer, starting from 0)
- **C# Type**: Automatically generated enum type
- **Features**:
  - Automatically extracts all enum values from data
  - Generates corresponding C# enum definitions
  - Automatically handles invalid identifiers (spaces, special characters, etc.)

## Unsupported Features

### 1. Excel Function Limitations
- ❌ Only supports the first worksheet (Worksheet[1])
- ❌ Does not support Excel formulas (only reads displayed values)
- ❌ Does not support merged cells
- ❌ Does not support multiple worksheet export

### 2. Data Type Limitations
- ❌ Does not support nested objects (e.g., `object`, `class` types)
- ❌ Does not support dictionary types
- ❌ Does not support multidimensional arrays
- ❌ Does not support `bool` types (use `int` 0/1 as alternative)
- ❌ Does not support `DateTime` types (use `string` or `long` timestamp as alternative)

### 3. Other Limitations
- ❌ Does not support dynamic row/column expansion
- ❌ Does not support conditional formatting and styles
- ❌ Does not preserve comments in Excel
- ❌ Does not support custom export formats

## Error Handling

### 1. Null Value Handling
- All empty cells are automatically converted to default values of corresponding types
- Default values: `int`→`0`, `float`→`0`, `string`→`""`, `array`→`[]`

### 2. Type Conversion Failures
- When type conversion fails, default values are used and error logs are output
- Example: Converting "abc" to int will result in 0 and log an error

### 3. Excel Format Errors
- If Row 2 (property row) has null values, an exception is thrown
- An exception is thrown if the file path does not exist

## Directory Structure Requirements

```
Project Root/
├── Assets/
│   ├── Excel/          # Place Excel files
│   ├── Json/           # Exported JSON files
│   └── Scripts/
│       └── Data/       # Exported C# class files
```

## Dependencies

1. **EPPlus**: Used to read Excel files (requires separate installation)
2. **Newtonsoft.Json**: Used for JSON serialization (requires separate installation)
3. **Unity Editor**: Needs to run in Unity Editor environment

## Notes

1. **File Encoding**: Ensure Excel files use UTF-8 encoding
2. **File Locking**: Close Excel files before exporting
3. **Data Type Consistency**: Data types in the same column must be consistent
4. **Enum Value Uniqueness**: Enum values in Excel should maintain consistent case and format
5. **Field Naming**: Field names should comply with C# identifier naming conventions

## Extension Suggestions

For extending functionality, you may consider:
1. Adding `bool` type support
2. Supporting more array types (e.g., `string[]`)
3. Adding data validation functionality
4. Supporting export to other formats (e.g., XML, binary)
5. Adding incremental export functionality

## Troubleshooting

### Common Issues
1. **Export Failure**: Check if the Excel file is open in another program
2. **Data Type Error**: Check if type declarations in Row 3 are correct
3. **JSON Format Error**: Check if strings contain unescaped double quotes
4. **Enum Generation Error**: Check if enum values contain illegal characters

### Viewing Logs
All error messages are output to the Unity console, including detailed row/column information and error causes.

---

**Version**: 1.0  
**Last Updated**: 2025/12/24  
**Maintainer**: Feimaijun
