using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;

public class TextPulse : MonoBehaviour
{
	public Gradient gradient;
	public float speed = 1f;

	IEnumerator Start()
	{

		var texts = GetComponentsInChildren<Text>(true);

		var t = 0f;
		while (true)
		{
			t += Time.deltaTime * speed;


			var color = gradient.Evaluate(Mathf.Repeat(t, 1f));

			for (int i = 0; i < texts.Length; i++)
			{
				texts[i].color = color;
			}

			yield return null;
		}
	}
}
