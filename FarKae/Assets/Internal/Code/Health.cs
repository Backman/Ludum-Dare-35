using UnityEngine;
using System.Collections;

public class Health : MonoBehaviour
{
	[SerializeField]
	HealthConfig _config;

	[HideInInspector]
	public int health;

	private void Awake()
	{
		health = _config.maxHealth;
	}
}
