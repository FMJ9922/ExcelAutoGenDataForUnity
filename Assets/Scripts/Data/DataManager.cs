using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets; // 需要安装 Newtonsoft.Json 包

public interface IData
{

}

public class DataEntry<TKey, TData> where TData : class, IData, new()
{
    private readonly Dictionary<TKey, TData> _dataMap = new();
    private readonly List<TData> _dataList = new();

    public int Count => _dataList.Count;

    // 根据 Key 获取数据
    public TData GetById(TKey key) => 
        _dataMap.TryGetValue(key, out var data) ? data : null;

    // 根据索引获取数据
    public TData GetByIndex(int index) => 
        (index >= 0 && index < _dataList.Count) ? _dataList[index] : null;


// 直接反序列化 JSON
    public static DataEntry<TKey, TData> FromJson(string jsonText)
    {
        Debug.Log(typeof(TData).ToString());
        var jObject = JObject.Parse(jsonText);
        var datasArray = jObject["datas"] as JArray;
        if (datasArray == null || !datasArray.Any())
            throw new ArgumentException("JSON must contain a non-empty 'datas' array");
        var firstData = datasArray.First as JObject;
        var jsonKeyName = firstData.Properties().First().Name;// 通过反射找到匹配的属性
        var keyProperty = typeof(TData).GetProperties()
            .FirstOrDefault(p => 
                p.GetCustomAttribute<JsonPropertyAttribute>()?.PropertyName == jsonKeyName ||
                p.Name.Equals(jsonKeyName, StringComparison.OrdinalIgnoreCase));

        if (keyProperty == null)
            throw new ArgumentException($"Class {typeof(TData)} has no property mapped to JSON key '{jsonKeyName}'");

        
        var container = JsonConvert.DeserializeObject<DataContainer<TData>>(jsonText);
        var entry = new DataEntry<TKey, TData>();

        foreach (var data in container.Datas)
        {
            var key = (TKey)keyProperty.GetValue(data);
            entry._dataMap.Add(key, data);
            entry._dataList.Add(data);
        }
        Debug.Log(typeof(TData).ToString());
        return entry;
    }

    private static TKey GetKeyValue(TData data, string propertyName)
    {
        // 方法1：使用反射获取属性值
        var prop = typeof(TData).GetProperty(propertyName);
        if (prop != null) return (TKey)prop.GetValue(data);
        throw new ArgumentException($"Invalid key property: {propertyName}");
    }
}

public class DataContainer<T>
{
    [JsonProperty("datas")]
    public T[] Datas { get; set; }
}

public class DataEntry2<TKey1, TKey2, TData> where TData : class, IData, new()
{
    private readonly Dictionary<TKey1, Dictionary<TKey2, TData>> _dataMap
        = new Dictionary<TKey1, Dictionary<TKey2, TData>>();
    private readonly List<TData> _dataArray = new List<TData>();
    // 缓存属性信息提升性能
    private static System.Reflection.PropertyInfo _key1Prop;
    private static System.Reflection.PropertyInfo _key2Prop;
    private string _keyName1;
    private string _keyName2;

    public int Count => _dataArray.Count;

    public int GetCount(TKey1 key1)
    {
        return _dataMap.TryGetValue(key1, out var innerDict) ? innerDict.Count : 0;
    }

    public int GetKey1Count() => _dataMap.Count;

    public TData GetByID(TKey1 key1, TKey2 key2)
    {
        if (_dataMap.TryGetValue(key1, out var innerDict))
        {
            if (innerDict.TryGetValue(key2, out var data))
            {
                return data;
            }
        }
        throw new KeyNotFoundException($"Keys {key1}/{key2} not found");
    }

    public TData GetByIndex(int index)
    {
        if (index < 0 || index >= _dataArray.Count)
        {
            throw new IndexOutOfRangeException();
        }
        return _dataArray[index];
    }

    public IEnumerable<TData> GetIterator()
    {
        foreach (var innerDict in _dataMap.Values)
        {
            foreach (var data in innerDict.Values)
            {
                yield return data;
            }
        }
    }

    public IEnumerable<TData> GetIterator(TKey1 key1)
    {
        if (_dataMap.TryGetValue(key1, out var innerDict))
        {
            foreach (var data in innerDict.Values)
            {
                yield return data;
            }
        }
    }

    public static async Task<DataEntry2<TKey1, TKey2, TData>> LoadData(
        string path, string keyName1, string keyName2)
    {
        var entry = new DataEntry2<TKey1, TKey2, TData>
        {
            _keyName1 = keyName1,
            _keyName2 = keyName2
        };
        CacheKeyProperties(typeof(TData), keyName1, keyName2);

        var jsonText = await AssetLoader.LoadJsonAsync(path);
        var jsonArray = JsonConvert.DeserializeObject<TData[]>(jsonText);

        foreach (var data in jsonArray)
        {
            var key1 = (TKey1)_key1Prop.GetValue(data);
            var key2 = (TKey2)_key2Prop.GetValue(data);

            if (!entry._dataMap.TryGetValue(key1, out var innerDict))
            {
                innerDict = new Dictionary<TKey2, TData>();
                entry._dataMap.Add(key1, innerDict);
            }

            innerDict.Add(key2, data);
            entry._dataArray.Add(data);
        }

        return entry;
    }

    private static void CacheKeyProperties(Type dataType, string key1, string key2)
    {
        // 使用缓存避免重复反射
        if (_key1Prop == null)
        {
            _key1Prop = dataType.GetProperty(key1)
                ?? throw new ArgumentException($"Invalid key1 property: {key1}");

            _key2Prop = dataType.GetProperty(key2)
                ?? throw new ArgumentException($"Invalid key2 property: {key2}");
        }
    }
}


public partial class DataManager : Singleton<DataManager>
{
    private Dictionary<string, string> _jsonDic;
    void Awake()
    {
        _jsonDic = new Dictionary<string, string>();
    }

    void Start()
    {
        Invoke("DoTask", 0.01f);
    }

    async Task DoTask()
    {
        await OnStartup();
        BuildData();
    }

    public async Task OnStartup()
    {
        for (int i = 0; i < loadList.Count; i++)
        {
            string str = await AssetLoader.LoadJsonAsync($"Assets/Json/{loadList[i]}.json");
            _jsonDic.Add(loadList[i],str);
        }
    }

    // 数据访问入口

    public Task OnShutdown() => Task.CompletedTask;
}

// 辅助类（需要根据实际引擎/框架实现）
public static class AssetLoader
{
    public static async Task<string> LoadJsonAsync(string assetKey)
    {
        // 创建加载句柄
        var handle = Addressables.LoadAssetAsync<TextAsset>(assetKey);
        // 等待加载完成
        var textAsset = await handle.Task;

        // 验证加载结果
        if (textAsset == null)
        {
            Addressables.Release(handle); // 加载失败时释放
            throw new JsonLoadingException($"Failed to load JSON: {assetKey}");
        }

        return textAsset.text;
    }

    public static async Task<T> LoadAndParseJsonAsync<T>(string assetKey) where T : new()
    {
        var jsonText = await LoadJsonAsync(assetKey);
        return ParseJson<T>(jsonText);
    }
    public static T ParseJson<T>(string json) where T : new()
    {
        try
        {
            return JsonUtility.FromJson<T>(json);
        }
        catch (System.Exception e)
        {
            throw new JsonParsingException($"JSON parsing failed: {typeof(T)}", e);
        }
    }
}
public class JsonLoadingException : System.Exception
{
    public JsonLoadingException(string message) : base(message) { }
    public JsonLoadingException(string message, System.Exception inner) : base(message, inner) { }
}

public class JsonParsingException : System.Exception
{
    public JsonParsingException(string message) : base(message) { }
    public JsonParsingException(string message, System.Exception inner) : base(message, inner) { }
}
