using System;	
using Newtonsoft.Json;	
[Serializable]	
public class MissileDataConfig: IData
{
	[JsonProperty("MissileId")]
	public int MissileId { get; set; }
	[JsonProperty("EntityName")]
	public string EntityName { get; set; }
	[JsonProperty("ModelId")]
	public int ModelId { get; set; }
	[JsonProperty("type")]
	public int type { get; set; }
	[JsonProperty("HitId")]
	public int HitId { get; set; }
	[JsonProperty("AttackRange")]
	public float AttackRange { get; set; }
	[JsonProperty("Speed")]
	public float Speed { get; set; }
	[JsonProperty("LifeTime")]
	public float LifeTime { get; set; }
	[JsonProperty("RotationSpeed")]
	public int[] RotationSpeed { get; set; }
	[JsonProperty("Target")]
	public int Target { get; set; }
	[JsonProperty("TargetPara")]
	public int TargetPara { get; set; }
	[JsonProperty("DmgRate")]
	public float DmgRate { get; set; }
	[JsonProperty("ChaseType")]
	public int ChaseType { get; set; }
	[JsonProperty("Pierce")]
	public int Pierce { get; set; }
	[JsonProperty("Knockback")]
	public float Knockback { get; set; }
	[JsonProperty("Shake")]
	public float Shake { get; set; }
	[JsonProperty("MuzzleHitId")]
	public int MuzzleHitId { get; set; }
	[JsonProperty("AttachOnTarget")]
	public int AttachOnTarget { get; set; }
	[JsonProperty("StartZ")]
	public float StartZ { get; set; }
	[JsonProperty("MissileNum")]
	public int MissileNum { get; set; }
	[JsonProperty("AttackTimes")]
	public int AttackTimes { get; set; }
}

