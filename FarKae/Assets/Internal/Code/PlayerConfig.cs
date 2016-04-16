using UnityEngine;
using System.Collections;

[CreateAssetMenu]
public class PlayerConfig : ScriptableObject
{
	public int firstAttackDamage = 5;
	public int secondAttackDamage = 10;
	public int superAttackDamage = 15;

	public float attackStaggerDuration = 0.2f;
	public float attackSwingDuration = 0.2f;
}
