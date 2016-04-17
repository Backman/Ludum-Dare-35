using UnityEngine;
using System.Collections;

[CreateAssetMenu]
public class EnemyConfig : EntityConfig
{
	[System.Serializable]
	public class EnemyAttack : AttackState
	{
	}

	public EnemyAttack basicAttack;

	public float attackRange = 0.5f;
	public float attackInterval = 2f;
}
