using UnityEngine;
using System.Collections;
using DG.Tweening;

public class ScreenShake : MonoBehaviour
{

	private Camera cam;
	private bool _isShaking;

	void Start()
	{
		cam = GetComponent<Camera> ();
	}
	
	public void Shake(float duration = 0.05f, float strength = 0.05f)
	{
		if (_isShaking)
		{
			return;
		}

		_isShaking = true;
		cam.DOShakePosition(duration, strength, 10, 10f)
			.OnComplete(() =>
			{
				_isShaking = false;
			});
	}
}
