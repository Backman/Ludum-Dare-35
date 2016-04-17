using UnityEngine;
using System.Collections;
using DG.Tweening;

public class Movable : MonoBehaviour
{
	public enum MoveDirection
	{
		Left,
		Right
	}

	public MovableConfig config;
	public float multiplier = 1f;

	[HideInInspector]
	public bool canMove = true;

	[HideInInspector]
	public MoveDirection direction;
	
	Vector2 _direction;
	Vector2 _velocity;

	Animator _animator;
	SpriteRenderer _renderer;
	Rigidbody2D _rb;

	Tweener _pushTween;

	Transform _transform;

	public new Transform transform
	{
		get { return _transform == null ? (_transform = GetComponent<Transform>()) : _transform; }
	}

	public bool isMoving
	{
		get;
		private set;
	}

	void Awake()
	{
		_rb  = GetComponent<Rigidbody2D>();
		_animator  = GetComponent<Animator>();
		_renderer  = GetComponent<SpriteRenderer>();

		direction = MoveDirection.Right;
	}

	void Update()
	{
		_direction.x *= config.baseSpeed;
		_direction.y *= config.verticalSpeed;

		_velocity = canMove ? 
			_direction * multiplier : Vector2.zero;

		isMoving = _velocity.sqrMagnitude > Mathf.Epsilon;

		if (isMoving)
		{
			direction = _velocity.x > 0f ?
				MoveDirection.Right : MoveDirection.Left;
		}

		if (_renderer && isMoving)
		{
			if (direction == MoveDirection.Left
				&& transform.localScale.x > 0f)
			{
				var scale = transform.localScale;
				scale.x = -1f;
				transform.localScale = scale;
			}
			else if (direction == MoveDirection.Right
				&& transform.localScale.x < 0f)
			{
				var scale = transform.localScale;
				scale.x = 1f;
				transform.localScale = scale;
			}
			//_renderer.flipX = _velocity.x < 0f;
		}
		_direction = Vector2.zero;
	}

	void FixedUpdate()
	{
		if (_pushTween != null && _pushTween.IsPlaying())
		{
			return;
		}
		_rb.velocity = _velocity;
	}

	public void Move(Vector2 direction)
	{
		_direction = direction.normalized;
	}

	public void Push(float length, float duration, AnimationCurve pushCurve = null)
	{
		AnimationCurve curve = pushCurve ?? AnimationCurve.Linear(0f, 0f, 1f, 1f);

		if (_pushTween != null && _pushTween.IsPlaying())
		{
			_pushTween.Kill(false);
		}

		var end = _rb.position.x + (GetDirection() * length);
		_pushTween = _rb.DOMoveX(end, duration)
			.SetEase(curve)
			.OnStart(() =>
			{
				_rb.velocity = Vector2.zero;
			});
	}

	public float GetDirection()
	{
		return direction == MoveDirection.Right ? 1f : -1f;
	}
}
