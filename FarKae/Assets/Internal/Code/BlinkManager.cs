using UnityEngine;
using System.Collections.Generic;

public class BlinkManager : MonoBehaviour
{
	struct BlinkState
	{
		public Color Color;
		public float EndTime;
		public float StartTime;
		public AnimationCurve Curve;
		public Renderer[] Renderers;
	}

	Dictionary<GameObject, BlinkState> _blinks = new Dictionary<GameObject, BlinkState>();
	MaterialPropertyBlock _block;


	List<GameObject> toRemove = new List<GameObject>();

	static BlinkManager _instance;
	public static BlinkManager instance
	{
		get
		{
			if (!_instance)
			{
				_instance = new GameObject("Blink Manager").AddComponent<BlinkManager>();
			}
			return _instance;
		}
	}

	void Awake()
	{
		_block = new MaterialPropertyBlock();
	}

	void Update()
	{
		foreach (var blink in _blinks)
		{
			if (!blink.Key)
			{
				toRemove.Add(blink.Key);
				continue;
			}
			Debug.LogFormat("Updating blink: {0}", blink.Key.name);
			var state = blink.Value;
			if (Time.unscaledTime > state.EndTime)
			{
				toRemove.Add(blink.Key);
				continue;
			}
			float duration = state.EndTime - state.StartTime;


			float t = (state.EndTime - Time.unscaledTime) / duration;
			var color = state.Color;
			if (state.Curve != null)
				color.a *= state.Curve.Evaluate(t);
			else
				color.a *= t;
			for (int i = 0; i < state.Renderers.Length; i++)
			{
				var renderer = state.Renderers[i];
				renderer.GetPropertyBlock(_block);
				_block.SetColor("_BlinkColor", color);
				renderer.SetPropertyBlock(_block);
			}
		}
		_block.Clear();
		for (int i = 0; i < toRemove.Count; i++)
		{
			var state = _blinks[toRemove[i]];

			for (int j = 0; j < state.Renderers.Length; j++)
			{
				var renderer = state.Renderers[j];
				if (!renderer)
				{
					continue;
				}
				renderer.GetPropertyBlock(_block);
				_block.SetColor("_BlinkColor", new Color(0, 0, 0, 0));
				renderer.SetPropertyBlock(_block);
			}

			_blinks.Remove(toRemove[i]);
		}
		toRemove.Clear();

	}

	public void AddBlink(GameObject source, Color color, float duration, AnimationCurve curve = null)
	{
		BlinkState state;
		state.Color = color;
		state.StartTime = Time.unscaledTime;
		state.EndTime = Time.unscaledTime + duration;
		state.Curve = curve;
		state.Renderers = source.GetComponentsInChildren<Renderer>();
		_blinks[source] = state;
	}
}