using System;	
using Newtonsoft.Json;	
[Serializable]	
public class ItemDataConfig: IData
{
	[JsonProperty("ItemID")]
	public int ItemID { get; set; }
	[JsonProperty("GmsName")]
	public string GmsName { get; set; }
	[JsonProperty("Name")]
	public string Name { get; set; }
	[JsonProperty("NamePara")]
	public int NamePara { get; set; }
	[JsonProperty("Description")]
	public string Description { get; set; }
	[JsonProperty("DesPara")]
	public int DesPara { get; set; }
	[JsonProperty("Icon")]
	public string Icon { get; set; }
	[JsonProperty("Type")]
	public int Type { get; set; }
	[JsonProperty("Rank")]
	public int Rank { get; set; }
	[JsonProperty("GemID")]
	public int GemID { get; set; }
	[JsonProperty("SelectedAwardID")]
	public int SelectedAwardID { get; set; }
	[JsonProperty("RewardInBagType")]
	public int RewardInBagType { get; set; }
	[JsonProperty("RewardInBagID")]
	public string RewardInBagID { get; set; }
	[JsonProperty("RewardParam")]
	public int RewardParam { get; set; }
	[JsonProperty("DoubleReward")]
	public int DoubleReward { get; set; }
	[JsonProperty("ShowRollingTips")]
	public int ShowRollingTips { get; set; }
	[JsonProperty("ItemSourceID")]
	public int[] ItemSourceID { get; set; }
	[JsonProperty("ShowOwned")]
	public int ShowOwned { get; set; }
	[JsonProperty("IfDisplayInBag")]
	public int IfDisplayInBag { get; set; }
	[JsonProperty("RMBValue")]
	public float RMBValue { get; set; }
}

