using System;	
using Newtonsoft.Json;	
[Serializable]	
public class WaveDataConfig: IData
{
	[JsonProperty("WaveId")]
	public int WaveId { get; set; }
	[JsonProperty("WaveInterval")]
	public int[] WaveInterval { get; set; }
	[JsonProperty("IsBossWave")]
	public int IsBossWave { get; set; }
	[JsonProperty("MonsterIds")]
	public int[] MonsterIds { get; set; }
	[JsonProperty("MonsterNums")]
	public int[] MonsterNums { get; set; }
	[JsonProperty("SpawnPoints")]
	public int[] SpawnPoints { get; set; }
	[JsonProperty("HeroIds")]
	public int[] HeroIds { get; set; }
	[JsonProperty("FinishOptions")]
	public int[] FinishOptions { get; set; }
	[JsonProperty("Range")]
	public int[] Range { get; set; }
}

