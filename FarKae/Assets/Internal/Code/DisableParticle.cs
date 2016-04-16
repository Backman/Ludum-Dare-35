using UnityEngine;
using System.Collections;

public class DisableParticle : MonoBehaviour
{
	public bool destroy;

	ParticleSystem _system;

	void Awake()
	{
		_system = GetComponent<ParticleSystem>();
		if (!_system)
		{
			if (destroy)
			{
				Destroy(gameObject);
			}
			else
			{
				gameObject.SetActive(false);
			}
		}
	}

	void Update()
	{
		if (!_system.IsAlive(true))
		{
			if (destroy)
			{
				Destroy(gameObject);
			}
			else
			{
				gameObject.SetActive(false);
			}
		}
	}
}
