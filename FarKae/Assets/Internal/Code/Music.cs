using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum SoundSourceType
{
	Unknown,
}

public class Music : MonoBehaviour
{
	struct AudioState
	{
		public AudioClip clip;
		public Vector3 position;
		public float volume;
		public float pitch;
		public float delay;
		public float start;
	}

	public float MinLowPassValue = 3000f;
	public float MaxLowPassValue = 22000f;
	public AudioLowPassFilter LowPassFilter;
	//private GameObject MusicPlayer;
	private AudioSource[] Songs;
	public float transition = 0;
	[Range (0.0f, 1.0f)]
	public float Musicvolume = 0.33f;
	public float fadespeed = 0.025f;
	[Range (0.0f, 1.0f)]
	public float sfxv = 1f;
	public static Music instance;

	static List<AudioState> _toPlay = new List<AudioState>();

	// Use this for initialization
	void Start ()
	{
		instance = this;
		//MusicPlayer = GameObject.Find ("MusicPlayer");
		Songs = GetComponents<AudioSource> ();

		var menuMusic = GameObject.Find ("MenuMusic");
		if (menuMusic) {
			Destroy (menuMusic);
		}
	}

	public static AudioSource PlayClipAtPoint(AudioSettings soundSettings, Vector3 pos,
		float delay = 0f,
		SoundSourceType source = SoundSourceType.Unknown)
	{
		if (soundSettings != null)
		{
			return PlayClipAtPoint(soundSettings.audioClip, pos,
				soundSettings.volume, soundSettings.RandomPitch(), delay, source);
		}
		return null;
	}

	public static AudioSource PlayClipAtPoint (AudioClip clip, Vector3 pos, float volume, float pitch,
		float delay = 0f,
		SoundSourceType source = SoundSourceType.Unknown)
	{
		if (!clip)
		{
			return null;
		}

		if (delay > float.Epsilon)
		{
			_toPlay.Add(new AudioState()
			{
				clip = clip,
				position = pos,
				pitch = pitch,
				volume = volume,
				start = Time.time,
				delay = delay
			});
		}

		return CreateAudioSourceObject(clip, pos, volume, pitch); // return the AudioSource reference
	}

	static AudioSource CreateAudioSourceObject(AudioClip clip, Vector3 pos, float volume, float pitch)
	{
		var tempGO = new GameObject ("TempAudio"); // create the temp object
		tempGO.transform.position = pos; // set its position
		var aSource = tempGO.AddComponent<AudioSource> (); // add an audio source
		aSource.clip = clip; // define the clip
		aSource.volume = volume;
		aSource.spatialBlend = 1.0f;
		aSource.minDistance = 5f;
		aSource.maxDistance = 100f;
		aSource.pitch = pitch;
		// set other aSource properties here, if desired
		aSource.Play (); // start the sound
		Destroy (tempGO, clip.length); // destroy object after clip duration
		return aSource;
	}

	public void PlayRegularMusic ()
	{
		//Songs [3].Stop ();
		//Songs [0].Play ();
		//Songs [1].Play ();
	}
	
	// Update is called once per frame
	void Update ()
	{
		for (int i = 0; i < _toPlay.Count; i++)
		{
			var toPlay = _toPlay[i];
			if (toPlay.start + toPlay.delay <= Time.time)
			{
				CreateAudioSourceObject(toPlay.clip, toPlay.position, toPlay.volume, toPlay.pitch);
			}
		}
		//Songs [0].volume = (1 - transition) * Musicvolume;
		//Songs [1].volume = transition * Musicvolume;
		//Songs [2].volume = transition * Musicvolume;
		//Songs [3].volume = Musicvolume;


		//var lowPassValue = Mathf.Lerp (MinLowPassValue, MaxLowPassValue, 1 - transition);
		//LowPassFilter.cutoffFrequency = lowPassValue;

		//if (Player.transform.position.y < threshold - 0.75f) {
		//	if (!Songs [2].isPlaying) {
		//		Songs [2].Play ();
		//	}
		//	//Songs[0].volume = (1 - Player.Hype.NormalizedHype) * Musicvolume;
		//	//Songs[1].volume = Player.Hype.NormalizedHype * Musicvolume;
		//	transition += fadespeed * Time.deltaTime;

		//}
		//if (Player.transform.position.y > threshold - 0.75f) {
		//	if (Songs [2].isPlaying && Songs [2].volume == 0f) {
		//		Songs [2].Stop ();
		//	}

		//	transition -= 10 * fadespeed * Time.deltaTime;
		//	//Songs[0].volume = Musicvolume;
		//	//Songs[1].volume = 0;
		//}
		transition = Mathf.Clamp01 (transition);
	}
}
