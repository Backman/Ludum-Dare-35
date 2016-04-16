using UnityEngine;
using System.Collections;
using UnityEngine.Events;

[System.Serializable]
public class FloatEvent : UnityEvent<float>
{
}

public class Health : MonoBehaviour
{
	[SerializeField]
	HealthConfig _config;
	[SerializeField]
	FloatEvent onDamage;

	int _health;

	public float health { get { return _health; } }

	private void Awake()
	{
		_health = _config.maxHealth;
	}

	public void Damage(int amount)
	{
		_health -= amount;
		onDamage.Invoke(_health);
		BlinkManager.instance.AddBlink(gameObject, Color.white, 0.1f);
	}

	public void Heal(int amount)
	{
		_health += amount;
	}
}
