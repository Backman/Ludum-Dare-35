using UnityEngine;
using System.Collections;

public class FreezeFrameController : MonoBehaviour
{
	public static FreezeFrameController instance;

	private void Awake()
	{
		instance = this;
	}

	public void DoFreeze(float duration = 0.2f, float freeze = 0f)
	{
		if (Time.timeScale >= 1f)
		{
			StartCoroutine(Freeze(duration, freeze));
		}
	}

	IEnumerator Freeze(float duration, float freeze)
	{
		float start = Time.unscaledTime;
		Time.timeScale = freeze;
		while (start + duration >= Time.unscaledTime)
		{
			yield return null;
		}

		Time.timeScale = 1f;
	}
}
