using UnityEngine;
using System.Collections;

[CreateAssetMenu]
public class PlayerConfig : ScriptableObject
{
	[System.Serializable]
	public class Attack
	{
		public float damage;
		public bool doDash = false;
		public AnimationCurve dashCurve;
		public float dashLength = 0.3f;
		public float dashDuration = 0.1f;
	}

	public Attack attackOne;
	public Attack attackTwo;
	public Attack superAttack;

	public float attackStaggerDuration = 0.2f;
	public float attackSwingDuration = 0.2f;
}
