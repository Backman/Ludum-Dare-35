using UnityEngine;
using System.Collections;

public class AttackCollider : MonoBehaviour
{
	static LayerMask _hitboxLayer = LayerMask.NameToLayer("HitboxCollider");
	void OnTriggerEnter2D(Collider2D collider)
	{
		var iAmEnemy = GetComponentInParent<BasicEnemy>();
		if (collider.gameObject.layer != _hitboxLayer
			|| (collider.GetComponentInParent<BasicEnemy>() && iAmEnemy))
		{
			return;
		}

		var iAmPlayer = GetComponentInParent<Player>();
		var receiverIsEnemy = collider.GetComponentInParent<BasicEnemy>();
		if (iAmPlayer && receiverIsEnemy)
		{
			var playerState = GetComponentInParent<Shapeshift>().CurrentState;
			var enemyState = collider.GetComponentInParent<Shapeshift>().CurrentState;
			if (playerState != enemyState)
			{
				return;
			}
		}

		var health = collider.GetComponentInParent<Health>();
		if (health)
		{
			health.Damage(50);
		}
	}
}
