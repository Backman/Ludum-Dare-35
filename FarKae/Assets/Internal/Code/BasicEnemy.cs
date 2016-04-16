using UnityEngine;
using System.Collections;
using MonsterLove.StateMachine;

public class BasicEnemy : MonoBehaviour
{
	public enum EnemyState
	{
		Walking,
		Idle,
		Attack,
		Hit
	}

	[SerializeField]
	BasicEnemyConfig _enemyConfig;
	[SerializeField]
	HealthConfig _healthConfig;

	public BoxCollider2D attackCollider;
	public BoxCollider2D hitCollider;

	LayerMask _hitboxColliderLayer;
	LayerMask _attackColliderLayer;

	bool _attackedPlayer;

	BoxCollider2D[] _overlappedHitColliders = new BoxCollider2D[8];

	Animator _animator;
	Movable _movable;
	Player _player;

	StateMachine<EnemyState> _fsm;
	Shapeshift _shapeshift;

	float _staggerTime;
	float _attackTime;

	public EnemyState State
	{
		get { return _fsm.State; }
		set { _fsm.ChangeState(value); }
	}

	public Shapeshift.ShapeshiftState ShapeshiftState
	{
		get { return _shapeshift.CurrentState; }
		set { _shapeshift.CurrentState = value; }
	}

	Transform _transform;
	float _health;

	public new Transform transform
	{
		get { return _transform == null ? (_transform = GetComponent<Transform>()) : _transform; }
	}

	void Awake()
	{
		_hitboxColliderLayer = LayerMask.NameToLayer("HitboxCollider");
		_attackColliderLayer = LayerMask.NameToLayer("AttackCollider");

		_movable = GetComponent<Movable>();
		_animator = GetComponent<Animator>();

		_fsm = StateMachine<EnemyState>.Initialize(this);
		_fsm.Changed += StateChanged;
		_fsm.ChangeState(EnemyState.Walking);

		_shapeshift = GetComponent<Shapeshift>();
		_health = _healthConfig.maxHealth;
	}

	void Start()
	{
		_player = FindObjectOfType<Player>();
	}

	public void Damage(float amount)
	{
		_health -= amount;
		Debug.LogFormat("Damage");
		if (_health <= 0)
		{
			WaveController.instance.RemoveEnemy(gameObject);
			Destroy(gameObject);
		}
		else
		{
			_fsm.ChangeState(EnemyState.Hit);
		}
	}

	void StateChanged(EnemyState state)
	{
		//Debug.LogFormat("Enemy state changed: {0}", state.ToString());
	}

	void Hit_Enter()
	{
		BlinkManager.instance.AddBlink(gameObject, Color.white, 0.1f);
		_animator.SetTrigger("Hit");
	}

	void Hit_Update()
	{
	}

	void Walking_Enter()
	{
	}

	void Walking_Update()
	{
		var playerPos = _player.transform.position;
		var moveDir = playerPos - transform.position;
		if (moveDir.sqrMagnitude <= _enemyConfig.attackRange)
		{
			_fsm.ChangeState(EnemyState.Attack);
			_movable.Move(Vector2.zero);
			return;
		}
		_movable.Move(moveDir.normalized);
	}

	void Idle_Enter()
	{
		_fsm.ChangeState(EnemyState.Walking);
	}

	void Idle_Update()
	{
	}

	void Attack_Enter()
	{
		_attackTime = Time.time;
	}

	void Attack_Update()
	{
		var playerPos = _player.transform.position;
		var length = (playerPos - transform.position).sqrMagnitude;

		if (_attackTime + _enemyConfig.attackInterval <= Time.time)
		{
			_attackTime = Time.time;
			_animator.SetTrigger("Attack");
		}

		if (length > _enemyConfig.attackRange)
		{
			_fsm.ChangeState(EnemyState.Walking);
			return;
		}

		if (!attackCollider || !attackCollider.enabled)
		{
			return;
		}

		var min = attackCollider.offset;
		min.x *= _movable.GetDirection();
		min += (Vector2)transform.position;
		var max = min + (Vector2)attackCollider.bounds.max;

		if (Physics2D.OverlapAreaNonAlloc(min, max, _overlappedHitColliders, (1 << _hitboxColliderLayer.value) | (1 << _attackColliderLayer.value)) <= 0)
		{
			return;
		}

		for (int i = 0; i < _overlappedHitColliders.Length; i++)
		{
			var otherCollider = _overlappedHitColliders[i];
			if (otherCollider
				&& LayerMask.LayerToName(otherCollider.gameObject.layer) == "HitboxCollider"
				&& otherCollider.gameObject != gameObject)
			{
				var player = otherCollider.GetComponentInParent<Player>();
				if (player && !_attackedPlayer)
				{
					_attackedPlayer = true;
					player.Damage(50f);
				}
			}
		}
	}

	void Attack_Finally()
	{
		_attackedPlayer = false;
	}

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

		if (attackCollider)
		{
			var origin = attackCollider.offset;
			origin.x *= _movable ? _movable.GetDirection() : 1f;
			origin += (Vector2)transform.position;

			Gizmos.color = new Color(1f, 0f, 0f, 0.3f);
			Gizmos.DrawCube(origin, attackCollider.size);
		}
		if (hitCollider)
		{
			var origin = hitCollider.offset;
			origin.x *= _movable ? _movable.GetDirection() : 1f;
			origin += (Vector2)transform.position;

			Gizmos.color = new Color(1f, 1f, 0f, 0.3f);
			Gizmos.DrawCube(origin, hitCollider.size);
		}
	}
#endif
}
