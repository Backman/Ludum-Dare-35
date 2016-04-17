using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public struct IntRect
{
	public int xMin;
	public int yMin;
	public int xMax;
	public int yMax;
}

[System.Serializable]
public class EnemySpawner
{
	public List<Transform> spawnPoints = new List<Transform>();
	float _spawnAccumulator;

	void Update()
	{

	}

	public Enemy SpawnEnemy(GameObject enemyPrefab)
	{
		//List<Transform> points = new List<Transform>();
		//for (int i = 0; i < spawnPoints.Count; i++)
		//{
		//	Debug.LogFormat("{0}", spawnPoints[i].name);
		//	var sp = spawnPoints[i];
		//	Vector3 viewportPoint = Camera.main.WorldToViewportPoint(sp.transform.position);
		//	if (Mathf.Abs(viewportPoint.x) < 1f && Mathf.Abs(viewportPoint.y) < 1f)
		//	{
		//		Debug.LogFormat("x: {0}, y: {1}", Mathf.Abs(viewportPoint.x), Mathf.Abs(viewportPoint.y));
		//		continue;
		//	}
		//	points.Add(spawnPoints[i]);
		//}

		if (spawnPoints.Count == 0)
			return null;
		var point = spawnPoints[Random.Range(0, spawnPoints.Count)];
		var go = (GameObject)Object.Instantiate(enemyPrefab, point.transform.position, Quaternion.identity);
		var enemy = go.GetComponent<Enemy>();
		enemy.RandomizePowerState();
		return enemy;
	}

	public static Vector2 GetFrustumSize(float layerDepth, out Vector2 centerPoint)
	{
		centerPoint = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0)).GetPoint(layerDepth);
		float frustumHeight = 2.0f * (layerDepth - Camera.main.transform.position.z) * Mathf.Tan((float)(Camera.main.fieldOfView * 0.5 * Mathf.Deg2Rad));
		float frustumWidth = frustumHeight * Camera.main.aspect;
		return new Vector2(frustumWidth, frustumHeight);
	}

	public static IntRect GetCurrentRect(float layerDepth, float gridSize, Vector2 frustumSizeModifier)
	{
		Vector2 centerPoint;
		Vector2 frustumSize = GetFrustumSize(layerDepth, out centerPoint);
		float minX = centerPoint.x - frustumSize.x * frustumSizeModifier.x;
		float maxX = centerPoint.x + frustumSize.x * frustumSizeModifier.x;
		float minY = centerPoint.y - frustumSize.y * frustumSizeModifier.y;
		float maxY = centerPoint.y + frustumSize.y * frustumSizeModifier.y;


		IntRect newVisible;
		newVisible.xMin = Mathf.RoundToInt(gridSize * minX - 0.5f);
		newVisible.yMin = Mathf.RoundToInt(gridSize * minY - 0.5f);
		newVisible.xMax = Mathf.RoundToInt(gridSize * maxX + 0.5f);
		newVisible.yMax = Mathf.RoundToInt(gridSize * maxY + 0.5f);

		return newVisible;
	}
}
