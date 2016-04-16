using UnityEngine;
using System.Collections;

[CreateAssetMenu]
public class BasicEnemyConfig : ScriptableObject
{
	public float attackRange = 0.5f;
	public float attackInterval = 2f;
	public float hitStaggerDuration = 0.5f;
}
