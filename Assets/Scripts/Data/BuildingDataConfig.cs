using System;	
using Newtonsoft.Json;	
[Serializable]	
public class BuildingDataConfig: IData
{
	[JsonProperty("Id")]
	public int Id { get; set; }
	[JsonProperty("Name")]
	public string Name { get; set; }
	[JsonProperty("ConfigType")]
	public ConfigType ConfigTypeValue { get; set; }
}

public enum ConfigType
{
	OfficalBuilding = 0
}

