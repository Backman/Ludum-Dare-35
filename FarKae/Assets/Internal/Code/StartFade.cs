using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;

public class StartFade : MonoBehaviour
{
	public float duration;
	public Image image;
	private bool _fading;

	void Awake()
	{
		_fading = true;
		image.DOFade(0f, duration)
			.OnComplete(() =>
			{
			});
	}

	void Start()
	{
	}
}
