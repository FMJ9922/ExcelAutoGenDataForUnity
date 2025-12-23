using System;	
using Newtonsoft.Json;	
[Serializable]	
public class EntityDataConfig: IData
{
	[JsonProperty("CharacterId")]
	public int CharacterId { get; set; }
	[JsonProperty("EntityName")]
	public string EntityName { get; set; }
	[JsonProperty("ModelId")]
	public int ModelId { get; set; }
	[JsonProperty("type")]
	public int type { get; set; }
	[JsonProperty("SkillDes")]
	public string SkillDes { get; set; }
	[JsonProperty("MissileId")]
	public int MissileId { get; set; }
	[JsonProperty("SkillMissileId")]
	public int[] SkillMissileId { get; set; }
	[JsonProperty("MuzzleHitId")]
	public int MuzzleHitId { get; set; }
	[JsonProperty("SpawnEffectId")]
	public int SpawnEffectId { get; set; }
	[JsonProperty("AttackPower")]
	public float AttackPower { get; set; }
	[JsonProperty("Defense")]
	public float Defense { get; set; }
	[JsonProperty("MaxHP")]
	public float MaxHP { get; set; }
	[JsonProperty("MagicPower")]
	public float MagicPower { get; set; }
	[JsonProperty("MagicDefense")]
	public float MagicDefense { get; set; }
	[JsonProperty("CritRate")]
	public float CritRate { get; set; }
	[JsonProperty("CritValue")]
	public float CritValue { get; set; }
	[JsonProperty("SearchRange")]
	public float SearchRange { get; set; }
	[JsonProperty("AttackRange")]
	public float AttackRange { get; set; }
	[JsonProperty("AttackSpeed")]
	public float AttackSpeed { get; set; }
	[JsonProperty("SkillRate")]
	public float SkillRate { get; set; }
	[JsonProperty("AttackAnimDelay")]
	public float AttackAnimDelay { get; set; }
	[JsonProperty("SkillAnimDelay")]
	public float[] SkillAnimDelay { get; set; }
	[JsonProperty("Acceleration")]
	public float Acceleration { get; set; }
	[JsonProperty("MaxSpeed")]
	public float MaxSpeed { get; set; }
	[JsonProperty("RotationSpeed")]
	public float RotationSpeed { get; set; }
	[JsonProperty("StartSmokeScale")]
	public float StartSmokeScale { get; set; }
}

