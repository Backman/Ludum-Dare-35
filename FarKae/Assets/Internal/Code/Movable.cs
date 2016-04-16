﻿using UnityEngine;
using System.Collections;

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

	void Awake()
	{
		_rb  = GetComponent<Rigidbody2D>();
		_animator  = GetComponent<Animator>();
		_renderer  = GetComponent<SpriteRenderer>();
	}

	void Update()
	{
		_direction.x *= config.baseSpeed;
		_direction.y *= config.verticalSpeed;

		_velocity = canMove ? 
			_direction * multiplier : Vector2.zero;

		var isMoving = _velocity.sqrMagnitude > Mathf.Epsilon;

		if (isMoving)
		{
			direction = _velocity.x > 0f ?
				MoveDirection.Right : MoveDirection.Left;
		}

		if (_renderer && isMoving)
		{
			_renderer.flipX = _velocity.x < 0f;
		}

		if (_animator)
		{
			_animator.SetBool("IsMoving", isMoving);
		}
	}

	void FixedUpdate()
	{
		_rb.velocity = _velocity;
	}

	public void Move(Vector2 direction)
	{
		_direction = direction;
	}
}
