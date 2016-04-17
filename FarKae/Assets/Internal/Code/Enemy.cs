using UnityEngine;
using System.Collections;
using MonsterLove.StateMachine;

public class Enemy : Entity
{
	public enum EnemyState
	{
		Approach,
		Idle,
		Attacking,
		Attack,
		//Block,
		Hit
	}

	[SerializeField]
	protected EnemyConfig _config;

	protected bool _attackedPlayer;

	protected Player _player;

	protected StateMachine<EnemyState> _fsm;

	protected float _attackCooldown;
	protected float _blockTime;
	protected float _hitTime;
	public EnemyState State
	{
		get { return _fsm.State; }
		set { _fsm.ChangeState(value); }
	}

	public virtual void RandomizePowerState()
	{
		_shapeshift.FSM.ChangeState((Shapeshift.ShapeshiftState)Random.Range(0, (int)Shapeshift.ShapeshiftState.Count));
	}

	protected override void Awake()
	{
		base.Awake();
		_config.powerStates.SetupFrameRates();

		_fsm = StateMachine<EnemyState>.Initialize(this);
		_fsm.ChangeState(EnemyState.Approach);

		_config.basicAttack.Init();

		_shapeshift.Init(_config.powerStates, _config.powerStates.candyState);
	}

	protected override void Start()
	{
		base.Start();
		_player = FindObjectOfType<Player>();
	}

	public override void PlayRandomBasicAttackAnimation()
	{
		var state = _shapeshift.CurrentState;
		var rand = Random.Range(0, _config.basicAttack.animations[state].Length);
		_animator.Play(_config.basicAttack.animations[state][rand].clip.name, _stateLayers.attackLayer, 0);
	}

	protected override void OnDamage(float amount)
	{
		_fsm.ChangeState(EnemyState.Hit);
	}

	protected override void OnDeath()
	{
		WaveController.instance.RemoveEnemy(this);
		Destroy(gameObject);
	}

	public void Block(float strength = 1f)
	{
		if (_fsm.State != EnemyState.Attacking)
		{
			_shapeshift.PlayCurrentBlock();
		}
		//_fsm.ChangeState(EnemyState.Block);
	}

	public bool Blocking()
	{
		return _stateLayers.CheckState(_shapeshift.currentPowerState.block.clip.name, _stateLayers.blockLayer);
	}

	protected virtual void Hit_Enter()
	{
		BlinkManager.instance.AddBlink(gameObject, Color.white, 0.1f);
		_shapeshift.PlayCurrentHit();
		_hitTime = Time.time;
	}

	protected virtual void Hit_Update()
	{
		if (_hitTime + _config.hitStaggerDuration <= Time.time)
		{
			_fsm.ChangeState(EnemyState.Approach);
		}
	}

	protected virtual void Block_Enter()
	{
		_shapeshift.PlayCurrentBlock();
		_blockTime = Time.time;
	}

	protected virtual void Block_Update()
	{
		if (_blockTime + _config.blockStaggerDuration <= Time.time)
		{
			_fsm.ChangeState(EnemyState.Approach);
		}
	}

	protected virtual void Approach_Enter()
	{
	}

	protected virtual void Approach_Update()
	{
		var playerPos = _player.transform.position;
		var dir = playerPos - transform.position;

		var move = Vector2.zero;

		if (dir.x > 0f)
		{
			move.x = 1f;
		}
		else if (dir.x < 0f)
		{
			move.x = -1f;
		}
		if (dir.y > 0f)
		{
			move.y = 1f;
		}
		else if (dir.y < 0f)
		{
			move.y = -1f;
		}

		if (dir.sqrMagnitude <= _config.attackRange)
		{
			_fsm.ChangeState(EnemyState.Attack);
			_movable.Move(Vector2.zero);
			return;
		}

		if (Blocking())
		{
			return;
		}

		_movable.Move(move);
		if (_movable.isMoving)
		{
			_shapeshift.PlayCurrentMove();
		}
	}

	protected virtual void Idle_Enter()
	{
		_fsm.ChangeState(EnemyState.Approach);
	}

	protected virtual void Idle_Update()
	{
		_shapeshift.PlayCurrentIdle();
	}

	protected virtual void Attack_Enter()
	{
		_attackCooldown = Time.time + Random.Range(_config.minAttackInterval, _config.maxAttackInterval);
		_attackedPlayer = false;
		_shapeshift.PlayCurrentIdle();
	}

	protected virtual void Attack_Update()
	{
		var playerPos = _player.transform.position;
		var length = (playerPos - transform.position).sqrMagnitude;

		if (_attackCooldown <= Time.time)
		{
			_attackCooldown = Time.time + Random.Range(_config.minAttackInterval, _config.maxAttackInterval);
			_attackedPlayer = false;
			_fsm.ChangeState(EnemyState.Attacking);
		}
		else
		{
			if (!Blocking())
			{
				_shapeshift.PlayCurrentIdle();
			}
		}

		if (length > _config.attackRange)
		{
			_fsm.ChangeState(EnemyState.Approach);
			return;
		}
	}

	protected virtual void Attacking_Enter()
	{
		PlayRandomBasicAttackAnimation();
	}

	protected virtual void Attacking_Update()
	{
		if (!_stateLayers.CheckState("None", _stateLayers.attackLayer))
		{
			CheckPlayerHit();
		}
		else
		{
			_fsm.ChangeState(EnemyState.Attack);
		}
	}

	protected virtual void Attack_Finally()
	{
		_attackedPlayer = false;
	}

	protected void CheckPlayerHit()
	{
		if (!_attackCollider || !_attackCollider.enabled)
		{
			return;
		}

		var pointA = _attackCollider.offset;
		pointA.x *= _movable.GetDirection();
		pointA = (pointA + (Vector2)transform.position) - _attackCollider.size / 2f;
		var pointB = _attackCollider.offset;
		pointB.x *= _movable.GetDirection();
		pointB = (pointB + (Vector2)transform.position) + _attackCollider.size / 2f;

		Collider2D[] colliders = new Collider2D[8];

		if (Physics2D.OverlapAreaNonAlloc(pointA, pointB, colliders, (1 << _hitboxColliderLayer.value) | (1 << _attackColliderLayer.value)) <= 0)
		{
			return;
		}

		for (int i = 0; i < colliders.Length; i++)
		{
			var otherCollider = colliders[i];
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
}
