using UnityEngine;
using System.Collections;

[System.Serializable]
public class AudioSettings
{
	public AudioClip audioClip;
	public float volume = 1f;
	public float minPitch = 1f;
	public float maxPitch = 1f;

	public float RandomPitch()
	{
		return Random.Range(minPitch, maxPitch);
	}
}
