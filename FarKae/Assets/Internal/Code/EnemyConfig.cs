using UnityEngine;
using System.Collections;

[CreateAssetMenu]
public class EnemyConfig : EntityConfig
{
	[System.Serializable]
	public class EnemyAttack : AttackState
	{
		public AudioSettings attackSound;
		[Range(0f, 1f)]
		public float voiceOverChance = 0.3f;
		public AudioSettings[] voiceOverSounds;
	}

	public float blockStaggerDuration = 0.5f;

	public AudioSettings[] deathSounds;

	public EnemyAttack basicAttack;

	public float attackDistance = 1f;
	public float dangerDistance = 1.5f;
	public float avoidDistance = 1.5f;
	public float avoidEnemiesDistance = 0.3f;

	public float xAttackRange = 0.5f;
	public float yAttackRange = 0.1f;
	public float separateThreshold = 2f;
	public float approachSeparateDistance = 1f;
	public float attackSeparateDistance = 2f;
	public float stillWorthIt = 0.8f;
	public float minAttackInterval = 1f;
	public float maxAttackInterval = 2f;
	public float attackInterval = 2f;

	public float deathFlickerDuration = 0.5f;
	public float deathFlickerInterval = 0.05f;
}
