using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;

public class RainbowMeter : MonoBehaviour
{
	public static RainbowMeter instance;

	public Slider slider;

	public float maxRainbow { get; set; }
	public float decayDuration { get; set; }
	public float currentRainbow { get; private set; }

	void Awake()
	{
		instance = this;
	}

	public void AddRainbow(float amount)
	{
		currentRainbow += amount;
		if (currentRainbow >= maxRainbow)
		{
			slider.value = currentRainbow / maxRainbow;
			currentRainbow = maxRainbow;
			slider.DOValue(0f, decayDuration);
		}
		else
		{
			slider.DOValue(slider.value, 0.1f)
				.SetEase(Ease.OutExpo);
		}
	}

	public void ResetRainbow()
	{
		currentRainbow = 0f;
		slider.value = 0f;
	}
}
