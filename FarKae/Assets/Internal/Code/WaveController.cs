using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WaveController : MonoBehaviour
{
	public static WaveController instance;

	public EnemySpawner spawner = new EnemySpawner();

	public GameObject enemyPrefab;

	public Dictionary<int, GameObject> aliveEnemies = new Dictionary<int, GameObject>();

	void Awake()
	{
		instance = this;
	}

	void Start()
	{
		SpawnEnemies(4);
	}

	public void RemoveEnemy(GameObject enemy)
	{
		if (aliveEnemies.ContainsKey(enemy.GetInstanceID()))
		{
			aliveEnemies.Remove(enemy.GetInstanceID());
		}

		if (aliveEnemies.Count == 0)
		{
			SpawnEnemies(4);
		}
	}

	void SpawnEnemies(int count)
	{
		for (int i = 0; i < count; i++)
		{
			var newEnemy = spawner.SpawnEnemy(enemyPrefab);
			if (newEnemy)
			{
				aliveEnemies.Add(newEnemy.GetInstanceID(), newEnemy);
			}
		}
	}
}
