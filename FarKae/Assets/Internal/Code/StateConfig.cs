using UnityEngine;
using System.Collections;

[CreateAssetMenu]
public class StateConfig : ScriptableObject
{
	[System.Serializable]
	public class AnimationState
	{
		public AnimationClip clip;
		public int frameRate = 10;

		public void SetFramRate()
		{
			if (clip)
			{
				clip.frameRate = frameRate;
			}
		}
	}

	public Color color = Color.white;
	public GameObject effect;

	public AnimationState idle;
	public AnimationState move;
	public AnimationState attackOne;
	public AnimationState attackTwo;
	public AnimationState superAttack;

	void SetupFrameRates()
	{
		idle.SetFramRate();
		move.SetFramRate();
		attackOne.SetFramRate();
		attackTwo.SetFramRate();
		superAttack.SetFramRate();
	}
}
