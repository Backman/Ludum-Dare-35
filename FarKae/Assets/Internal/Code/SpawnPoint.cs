using UnityEngine;
using System.Collections;

public class SpawnPoint : MonoBehaviour
{
	void Start()
	{
		var waveController = FindObjectOfType<WaveController>();
		if (waveController)
		{
			waveController.spawner.spawnPoints.Add(transform);
		}
	}
}
