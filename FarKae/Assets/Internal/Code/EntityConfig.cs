using UnityEngine;
using System.Collections;

public abstract class EntityConfig : ScriptableObject
{
	[System.Serializable]
	public class PowerState
	{
		public Color color = Color.white;
		public GameObject effect;

		public AnimationState idle;
		public AnimationState move;

		public virtual void SetupFrameRates()
		{
			idle.SetFramRate();
			move.SetFramRate();
		}
	}

	[System.Serializable]
	public class PowerStates
	{
		public PowerState candyState;
		public PowerState lightningState;
		public PowerState magicState;
		public PowerState avocadoState;

		public void SetupFrameRates()
		{
			candyState.SetupFrameRates();
			lightningState.SetupFrameRates();
			magicState.SetupFrameRates();
			avocadoState.SetupFrameRates();
		}
	}

	public float hitStaggerDuration = 0.5f;

	public PowerStates powerStates;
}
