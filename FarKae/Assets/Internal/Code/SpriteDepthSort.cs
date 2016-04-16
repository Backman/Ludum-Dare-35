using UnityEngine;
using System.Collections;

/// <summary>
/// http://breadcrumbsinteractive.com/two-unity-tricks-isometric-games/
/// </summary>
[ExecuteInEditMode]
[RequireComponent(typeof(SpriteRenderer))]
public class SpriteDepthSort : MonoBehaviour
{
	const int IsometricRangePerYUnit = 100;

	SpriteRenderer _renderer;

	Transform _transform;
	public new Transform transform
	{
		get { return _transform == null ? (_transform = GetComponent<Transform>()) : _transform; }
	}

	void Update()
	{
		if (!_renderer)
		{
			_renderer = GetComponent<SpriteRenderer>();
		}

		_renderer.sortingOrder = -(int)(transform.position.y * IsometricRangePerYUnit);
	}
}
