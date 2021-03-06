﻿using UnityEngine;
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
	private bool _flipSprite;

	public new Transform transform
	{
		get { return _transform == null ? (_transform = GetComponent<Transform>()) : _transform; }
	}

	public bool isMoving
	{
		get;
		private set;
	}

	public void SetDirection(float dir)
	{
		direction = dir > 0f ?
			MoveDirection.Right : MoveDirection.Left;

		Flip();
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
				Flip();
			}
			else if (direction == MoveDirection.Right
				&& transform.localScale.x < 0f)
			{
				Flip();
			}
		}
		_direction = Vector2.zero;
	}

	void Flip()
	{
		if (!_flipSprite)
		{
			return;
		}

		if (direction == MoveDirection.Right)
		{
			var scale = transform.localScale;
			scale.x = 1f;
			transform.localScale = scale;
		}
		else
		{
			var scale = transform.localScale;
			scale.x = -1f;
			transform.localScale = scale;
		}
	}

	void FixedUpdate()
	{
		if (_pushTween != null && _pushTween.IsPlaying())
		{
			return;
		}
		_rb.velocity = _velocity;

		if (GetComponent<Player>())
		{
			Stats.unitsMoved += _rb.velocity.magnitude;
		}
	}

	public void Move(Vector2 direction, bool flipSprite = true)
	{
		_flipSprite = flipSprite;
		_direction = direction.normalized;
	}

	public void Push(int direction, float length, float duration, AnimationCurve pushCurve = null)
	{
		AnimationCurve curve = pushCurve ?? AnimationCurve.Linear(0f, 0f, 1f, 1f);

		if (_pushTween != null && _pushTween.IsPlaying())
		{
			_pushTween.Kill(false);
		}

		var end = _rb.position.x + (direction * length);
		_pushTween = _rb.DOMoveX(end, duration)
			.SetEase(curve)
			.OnStart(() =>
			{
				_rb.velocity = Vector2.zero;
			});
	}

	public int GetDirection()
	{
		return direction == MoveDirection.Right ? 1 : -1;
	}
}
