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
		get { return _shapeshift.State; }
		set { _shapeshift.State = value; }
	}

	Transform _transform;
	float _health;

	public new Transform transform
	{
		get { return _transform == null ? (_transform = GetComponent<Transform>()) : _transform; }
	}

	void Awake()
	{
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
		if (_fsm.State == EnemyState.Hit)
		{
			return;
		}

		_health -= amount;
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
			_animator.SetTrigger("Attack_Punch");
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

		var min = attackCollider.offset + (Vector2)transform.position + attackCollider.size / 2.0f;
		var max = min + attackCollider.offset;

		if (Physics2D.OverlapAreaNonAlloc(min, max, _overlappedHitColliders) <= 0)
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
				Debug.LogFormat("Hit {0}", otherCollider.name);
				var enemy = otherCollider.GetComponentInParent<BasicEnemy>();
				if (enemy)
				{
					enemy.Damage(50f);
				}
			}
		}
	}
}
