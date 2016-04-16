using UnityEngine;
using System.Collections;

public class HitCollider : MonoBehaviour
{
	void OnTriggerEnter2D(Collider2D collider)
	{
		var health = GetComponentInParent<Health>();
		if (health)
		{
			health.Damage(10);
		}
	}
}
