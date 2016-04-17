using UnityEngine;
using System.Collections;

[CreateAssetMenu]
public class EnemyConfig : EntityConfig
{
	[System.Serializable]
	public class EnemyAttack : AttackState
	{
	}

	public float blockStaggerDuration = 0.5f;

	public EnemyAttack basicAttack;

	public float attackRange = 0.5f;
	public float minAttackInterval = 1f;
	public float maxAttackInterval = 2f;
	public float attackInterval = 2f;
}
