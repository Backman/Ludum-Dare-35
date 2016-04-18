using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WaveController : MonoBehaviour
{
	public static WaveController instance;

	public EnemySpawner spawner = new EnemySpawner();

	public BasicEnemy basicEnemy;
	public NarwhalGuy narwhalGuy;
	public BetterEnemy betterEnemy;
	public BetterNarwhalGuy betterNarwhalGuy;

	public Dictionary<int, GameObject> aliveEnemies = new Dictionary<int, GameObject>();

	void Awake()
	{
		instance = this;
	}

	void Start()
	{
		SpawnEnemies(5, betterEnemy.gameObject);
		SpawnEnemies(1, betterNarwhalGuy.gameObject);
		//SpawnEnemies(1, narwhalGuy);
		//SpawnEnemies(4, basicEnemy);
	}

	void Update()
	{
		if (aliveEnemies.Count == 0)
		{
			SpawnEnemies(3, betterEnemy.gameObject);
		}
	}

	public void RemoveEnemy(GameObject enemy)
	{
		if (aliveEnemies.ContainsKey(enemy.GetInstanceID()))
		{
			aliveEnemies.Remove(enemy.GetInstanceID());
		}

		if (aliveEnemies.Count == 0)
		{
			SpawnEnemies(5, betterEnemy.gameObject);
			SpawnEnemies(1, betterNarwhalGuy.gameObject);
		}
	}

	void SpawnEnemies(int count, GameObject enemy)
	{
		for (int i = 0; i < count; i++)
		{
			var newEnemy = spawner.SpawnEnemy(enemy);
			if (newEnemy)
			{
				aliveEnemies.Add(newEnemy.GetInstanceID(), newEnemy);
			}
		}
	}
}
