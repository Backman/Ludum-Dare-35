using UnityEngine;
using System.Collections;

[CreateAssetMenu]
public class EnemySpawnSettings : ScriptableObject
{
	[System.Serializable]
	public struct EnemyData
	{
		public string name;
		public GameObject prefab;
		public float spawnChance;
	}

	public EnemyData[] enemies;
}
