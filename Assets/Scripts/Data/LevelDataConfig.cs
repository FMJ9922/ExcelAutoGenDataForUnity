using System;	
using Newtonsoft.Json;	
[Serializable]	
public class LevelDataConfig: IData
{
	[JsonProperty("LevelId")]
	public int LevelId { get; set; }
	[JsonProperty("LevelName")]
	public string LevelName { get; set; }
	[JsonProperty("WaveIds")]
	public int[] WaveIds { get; set; }
	[JsonProperty("HasWall")]
	public int HasWall { get; set; }
}

