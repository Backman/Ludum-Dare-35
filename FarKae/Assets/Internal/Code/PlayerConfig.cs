﻿using UnityEngine;
using System.Collections;
using UnityEngine.Experimental.Director;

[CreateAssetMenu]
public class PlayerConfig : EntityConfig
{
	[System.Serializable]
	public class PlayerAttack : AttackState
	{
		public float freezeDuration;
		public float freezeValue;
		public AudioSettings hitSound;
		public AudioSettings blockSound;
		public AudioSettings punchSound;
	}

	public int basicAttackCount;
	public PlayerAttack basicAttack;
	public PlayerAttack superAttack;

	public AudioSettings deathSound;
	public AudioSettings coinPickupSound;
	public AudioSettings rainbowSound;
	public Material rainbowRaveMaterial;
	public float maxRainbow;

	public float rainbowRaveDuration = 5f;
	public float attackStaggerDuration = 0.2f;
	public float attackSwingDuration = 0.2f;
	public float attackCooldown = 0.2f;

	public int simultaneousAttackers = 3;
}
