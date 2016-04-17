using UnityEngine;
using System.Collections;

public class EnvironmentCollider : MonoBehaviour
{
	void OnTriggerExit2D(Collider2D other)
	{
		if (other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
		{
			other.isTrigger = false;
		}
	}
}
