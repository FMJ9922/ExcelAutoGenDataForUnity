using System.Collections.Generic;	
public partial class DataManager : Singleton<DataManager>	
{	
	private List<string> loadList = new List<string>(){	
		"BuildingDataConfig",
		"EntityDataConfig",
		"ItemDataConfig",
		"LevelDataConfig",
		"MissileDataConfig",
		"WaveDataConfig",
	};
	public static DataEntry<int, BuildingDataConfig> BuildingDataConfig { get; private set; }
	public static DataEntry<int, EntityDataConfig> EntityDataConfig { get; private set; }
	public static DataEntry<int, ItemDataConfig> ItemDataConfig { get; private set; }
	public static DataEntry<int, LevelDataConfig> LevelDataConfig { get; private set; }
	public static DataEntry<int, MissileDataConfig> MissileDataConfig { get; private set; }
	public static DataEntry<int, WaveDataConfig> WaveDataConfig { get; private set; }
	private void BuildData()	
	{
		BuildingDataConfig = DataEntry<int, BuildingDataConfig>.FromJson(_jsonDic["BuildingDataConfig"]);
		EntityDataConfig = DataEntry<int, EntityDataConfig>.FromJson(_jsonDic["EntityDataConfig"]);
		ItemDataConfig = DataEntry<int, ItemDataConfig>.FromJson(_jsonDic["ItemDataConfig"]);
		LevelDataConfig = DataEntry<int, LevelDataConfig>.FromJson(_jsonDic["LevelDataConfig"]);
		MissileDataConfig = DataEntry<int, MissileDataConfig>.FromJson(_jsonDic["MissileDataConfig"]);
		WaveDataConfig = DataEntry<int, WaveDataConfig>.FromJson(_jsonDic["WaveDataConfig"]);
	}
}
