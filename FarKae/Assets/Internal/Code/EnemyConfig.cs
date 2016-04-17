using UnityEngine;
using System.Collections;

[CreateAssetMenu]
public class EnemyConfig : EntityConfig
{
	[System.Serializable]
	public class EnemyAttack : AttackState
	{
		public AudioSettings attackSound;
	}

	public float blockStaggerDuration = 0.5f;

	public EnemyAttack basicAttack;

	public float xAttackRange = 0.5f;
	public float yAttackRange = 0.1f;
	public float separateThreshold = 2f;
	public float approachSeparateDistance = 1f;
	public float attackSeparateDistance = 2f;
	public float stillWorthIt = 0.8f;
	public float minAttackInterval = 1f;
	public float maxAttackInterval = 2f;
	public float attackInterval = 2f;
}
