﻿using UnityEngine;
using System.Collections;

public abstract class Entity : MonoBehaviour
{
	[SerializeField]
	protected HealthConfig _healthConfig;

	[SerializeField]
	protected BoxCollider2D _attackCollider;
	[SerializeField]
	protected BoxCollider2D _hitCollider;

	protected Shapeshift _shapeshift;
	protected float _health;

	protected LayerMask _hitboxColliderLayer;
	protected LayerMask _attackColliderLayer;

	protected Transform _transform;

	protected Animator _animator;
	protected Movable _movable;

	public Shapeshift.ShapeshiftState ShapeshiftState
	{
		get { return _shapeshift.CurrentState; }
		set { _shapeshift.CurrentState = value; }
	}

	public new Transform transform
	{
		get { return _transform == null ? (_transform = GetComponent<Transform>()) : _transform; }
	}

	protected virtual void Awake()
	{
		_hitboxColliderLayer = LayerMask.NameToLayer("HitboxCollider");
		_attackColliderLayer = LayerMask.NameToLayer("AttackCollider");

		_movable = GetComponent<Movable>();
		_animator = GetComponent<Animator>();

		_health = _healthConfig.maxHealth;
		_shapeshift = GetComponent<Shapeshift>();
	}

	protected virtual void Start()
	{
	}

	public void Damage(float amount)
	{
		_health -= amount;
		if (_health > 0f)
		{
			OnDamage(amount);
		}
		else
		{
			OnDeath();
		}
	}

	public virtual void PlayRandomBasicAttackAnimation()
	{
	}

	protected bool CheckAnimatorState(string name, int layer)
	{
		return _animator.GetCurrentAnimatorStateInfo(layer).IsName(name);
	}

	protected abstract void OnDamage(float amount);
	protected abstract void OnDeath();

#if UNITY_EDITOR
	void OnDrawGizmos()
	{
		var moveCollider = GetComponent<BoxCollider2D>();
		if (moveCollider)
		{
			var origin = moveCollider.offset;
			origin.x *= _movable ? _movable.GetDirection() : 1f;
			origin += (Vector2)transform.position;

			Gizmos.color = new Color(0f, 1f, 0f, 0.3f);
			Gizmos.DrawCube(origin, moveCollider.size);
		}

		if (_attackCollider)
		{
			var origin = _attackCollider.offset;
			origin.x *= _movable ? _movable.GetDirection() : 1f;
			origin += (Vector2)transform.position;

			var dir = _movable ? _movable.GetDirection() : 1f;

			var pointA = _attackCollider.offset;
			pointA.x *= dir;
			pointA = (pointA + (Vector2)transform.position) - _attackCollider.size / 2f;
			var pointB = _attackCollider.offset;
			pointB.x *= dir;
			pointB = (pointB + (Vector2)transform.position) + _attackCollider.size / 2f;

			Gizmos.color = new Color(1f, 0f, 0f, 0.8f);
			Gizmos.DrawLine(pointA, pointB);
			Gizmos.color = new Color(0f, 1f, 0f, 0.8f);
			Gizmos.DrawCube(pointA, Vector3.one * 0.01f);
			Gizmos.color = new Color(0f, 0f, 1f, 0.8f);
			Gizmos.DrawCube(pointB, Vector3.one * 0.01f);

			Gizmos.color = new Color(1f, 0f, 0f, 0.3f);
			Gizmos.DrawCube(origin, _attackCollider.size);
		}
		if (_hitCollider)
		{
			var origin = _hitCollider.offset;
			origin.x *= _movable ? _movable.GetDirection() : 1f;
			origin += (Vector2)transform.position;

			Gizmos.color = new Color(1f, 1f, 0f, 0.3f);
			Gizmos.DrawCube(origin, _hitCollider.size);
		}
	}
#endif
}
