using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class ImageDistort : MonoBehaviour
{
	public float distortion = 0.5f;

	[SerializeField]
	private Shader _shader;
	public Shader shader
	{
		get
		{
			if (_shader == null)
				_shader = Shader.Find("Hidden/ImageDistort");

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

		material.SetFloat("_Distortion", distortion);
		Graphics.Blit(source, destination, material);
	}
}
