using UnityEngine;
using System.Collections;

public class SpawnPoint : MonoBehaviour
{
	void Start()
	{
		WaveController.instance.spawner.spawnPoints.Add(transform);
	}
}
