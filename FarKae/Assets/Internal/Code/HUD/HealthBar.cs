using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class HealthBar : MonoBehaviour
{
	public RectTransform frame;
	public Slider slider;
	public Text text;

	public float shakeDuration = 0.2f;
	public float shakeStrength = 30f;
	public int shakeVibrato = 30;
	public Ease shakeEase = Ease.Linear;

	Player _player;
	Tween _pulseTween;
	bool _isShaking;

	void Update()
	{
		if (!_player)
		{
			_player = FindObjectOfType<Player>();
			return;
		}

		var normalizedHealth = _player.normalizedHealth;
		if (slider.value != normalizedHealth)
		{
			Shake(slider.value - normalizedHealth);
			slider.value = normalizedHealth;
			text.text = _player.health.ToString();
		}
	}

	void Shake(float strength)
	{
		if (_isShaking)
		{
			return;
		}

		_isShaking = true;
		frame.DOShakeAnchorPos(shakeDuration, shakeStrength, shakeVibrato)
			.SetEase(shakeEase)
			.OnComplete(() =>
			{
				_isShaking = false;
			});
	}
}
