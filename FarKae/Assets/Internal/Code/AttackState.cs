using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class AnimationState
{
	public AnimationClip clip;
	public int frameRate = 10;

	public void SetFramRate()
	{
		if (clip)
		{
			clip.frameRate = frameRate == 0 ? 10 : frameRate;
		}
	}
}

[System.Serializable]
public class AttackState
{
	public float damage;
	[SerializeField]
	private AnimationState[] _candyAnimations;
	[SerializeField]
	private AnimationState[] _lightningAnimations;
	[SerializeField]
	private AnimationState[] _magicAnimations;
	[SerializeField]
	private AnimationState[] _avocadoAnimations;

	public Dictionary<Shapeshift.ShapeshiftState, AnimationState[]> animations;

	public void Init()
	{
		animations = new Dictionary<Shapeshift.ShapeshiftState, AnimationState[]>();

		SetFrameRate(ref _candyAnimations);
		SetFrameRate(ref _lightningAnimations);
		SetFrameRate(ref _magicAnimations);
		SetFrameRate(ref _avocadoAnimations);

		animations[Shapeshift.ShapeshiftState.Candy] = _candyAnimations;
		animations[Shapeshift.ShapeshiftState.Lightning] = _lightningAnimations;
		animations[Shapeshift.ShapeshiftState.Magic] = _magicAnimations;
		animations[Shapeshift.ShapeshiftState.Avocado] = _avocadoAnimations;
	}

	private void SetFrameRate(ref AnimationState[] animations)
	{
		for (int i = 0; i < animations.Length; i++)
		{
			animations[i].SetFramRate();
		}
	}
}
