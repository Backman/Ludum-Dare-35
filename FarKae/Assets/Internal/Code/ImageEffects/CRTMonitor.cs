using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class CRTMonitor : MonoBehaviour
{
	[System.Serializable]
	public class LineSettings
	{
		public float Frequency = 100f;
		public float Speed = 0f;
		public float Thickness = 0f;
	}

	public LineSettings lineOne;
	public LineSettings lineTwo;

	[SerializeField]
	private Shader _shader;
	public Shader shader
	{
		get
		{
			if (_shader == null)
				_shader = Shader.Find("Hidden/CRTMonitor");

			return _shader;
		}
	}

	private Material _material;
	public Material material
	{
		get
		{
			if (_material == null)
				_material = ImageEffectHelper.CheckShaderAndCreateMaterial(shader);

			return _material;
		}
	}

	void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		if (!material)
		{
			Graphics.Blit(source, destination);
			return;
		}

		material.SetFloat("_LineOneFrequency", lineOne.Frequency);
		material.SetFloat("_LineOneSpeed", lineOne.Speed);
		material.SetFloat("_LineOneThickness", lineOne.Thickness);
		material.SetFloat("_LineTwoFrequency", lineTwo.Frequency);
		material.SetFloat("_LineTwoSpeed", lineTwo.Speed);
		material.SetFloat("_LineTwoThickness", lineTwo.Thickness);

		Graphics.Blit(source, destination, material);
	}
}
