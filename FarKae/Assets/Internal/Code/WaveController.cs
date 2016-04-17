using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WaveController : MonoBehaviour
{
	public static WaveController instance;

	public EnemySpawner spawner = new EnemySpawner();

	public BasicEnemy basicEnemy;
	public NarwhalGuy narwhalGuy;

	public Dictionary<int, Enemy> aliveEnemies = new Dictionary<int, Enemy>();

	void Awake()
	{
		instance = this;
	}

	void Start()
	{
		SpawnEnemies(1, narwhalGuy);
		SpawnEnemies(4, basicEnemy);
	}

	public void RemoveEnemy(Enemy enemy)
	{
		if (aliveEnemies.ContainsKey(enemy.GetInstanceID()))
		{
			aliveEnemies.Remove(enemy.GetInstanceID());
		}

		if (aliveEnemies.Count == 0)
		{
			SpawnEnemies(1, narwhalGuy);
			SpawnEnemies(4, basicEnemy);
		}
	}

	void SpawnEnemies(int count, Enemy enemyPrefab)
	{
		for (int i = 0; i < count; i++)
		{
			var newEnemy = spawner.SpawnEnemy(enemyPrefab.gameObject);
			if (newEnemy)
			{
				aliveEnemies.Add(newEnemy.GetInstanceID(), newEnemy);
			}
		}
	}
}
