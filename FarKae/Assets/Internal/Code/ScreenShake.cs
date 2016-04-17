using UnityEngine;
using System.Collections;
using DG.Tweening;

public class ScreenShake : MonoBehaviour
{
	private Camera cam;

	Tweener _shaking;

	void Start()
	{
		cam = GetComponent<Camera> ();
	}
	
	public void Shake(float duration = 0.05f, float strength = 0.05f)
	{
		if (_shaking != null && _shaking.IsPlaying())
		{
			return;
		}

		_shaking = cam.DOShakePosition(duration, strength, 10, 10f);
	}
}
