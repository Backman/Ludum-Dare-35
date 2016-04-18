using UnityEngine;
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

	protected LayerMask _hitboxColliderLayer;
	protected LayerMask _attackColliderLayer;

	protected Transform _transform;

	protected Animator _animator;
	protected Movable _movable;

	protected AnimatorStateLayers _stateLayers;

	public float normalizedHealth
	{ get { return health / maxHealth; } }

	public float maxHealth
	{ get { return _healthConfig.maxHealth; } }

	public float health
	{ get; protected set; }

	public bool isDead
	{ get; protected set; }

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

		health = _healthConfig.maxHealth;
		_shapeshift = GetComponent<Shapeshift>();

		_stateLayers = new AnimatorStateLayers(_animator);
	}

	protected virtual void Start()
	{
	}

	public virtual void Damage(float amount)
	{
		health -= amount;
		if (health > 0f)
		{
			OnDamage(amount);
		}
		else
		{
			health = 0f;
			if (!isDead)
			{
				isDead = true;
				OnDeath();
			}
		}
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
