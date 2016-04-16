using UnityEngine;
using System.Collections;
using DG.Tweening;

public class ScreenShake : MonoBehaviour {

	private Camera cam;


	// Use this for initialization
	void Start () {
		cam = GetComponent<Camera> ();
	}
	
	// Update is called once per frame
	void Update () {
	}

	public void Screenshake (float duration = 0.05f, float strength = 0.05f){
		cam.DOShakePosition (duration, strength, 10, 10f);

	}

}
