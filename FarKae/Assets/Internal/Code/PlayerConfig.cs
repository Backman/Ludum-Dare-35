using UnityEngine;
using System.Collections;
using UnityEngine.Experimental.Director;

[CreateAssetMenu]
public class PlayerConfig : ScriptableObject
{
	[System.Serializable]
	public class AnimationState
	{
		public AnimationClip clip;
		public AnimationClipPlayable playable;
	}

	[System.Serializable]
	public class Attack
	{
		public float damage;
		public bool doDash = false;
		public AnimationCurve dashCurve;
		public float dashLength = 0.3f;
		public float dashDuration = 0.1f;
		public float freezeDuration;
		public float freezeValue;
	}

	public int basicAttackCount;
	public Attack basicAttack;
	public Attack attackOne;
	public Attack attackTwo;
	public Attack superAttack;

	public float attackStaggerDuration = 0.2f;
	public float attackSwingDuration = 0.2f;
	public float attackCooldown = 0.2f;
}
