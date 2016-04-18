using UnityEngine;
using System.Collections;
using MonsterLove.StateMachine;

public class BetterEnemy : Entity
{
	public enum State
	{
		Approach,
		Danger,
		Attack,
		Attacking,
		//Wait,
		Avoid,
		Hit,
		Death
	}

	[SerializeField]
	protected EnemyConfig _config;

	protected StateMachine<State> _fsm;

	protected Player _player;
	private float _waitTime;

	protected BetterEnemy[] _otherEnemies;
	private float _attackTime;
	private BoxCollider2D _attackZone;
	private bool _inAttackZone;
	private bool _attackedPlayer;
	private float _hitTime;

	protected float attackDistanceSqr { get { return _config.attackDistance * _config.attackDistance; } }
	protected float dangerDistanceSqr { get { return _config.dangerDistance * _config.dangerDistance; } }
	protected float avoidDistanceSqr { get { return _config.avoidDistance * _config.avoidDistance; } }
	protected float avoidEnemiesDistanceSqr { get { return _config.avoidEnemiesDistance * _config.avoidEnemiesDistance; } }



	protected override void Awake()
	{
		base.Awake();

		_fsm = StateMachine<State>.Initialize(this, State.Approach);
		_fsm.Changed += (state) =>
		{
			if (state == State.Death)
			{
				if (_config.deathSounds.Length > 0)
				{
					var index = Random.Range(0, _config.deathSounds.Length);
					Music.PlayClipAtPoint(_config.deathSounds[index], transform.position);
				}
				StartCoroutine(DeathFlicker());
			}
		};

		_config.basicAttack.Init();
		_shapeshift.Init(_config.powerStates, _config.powerStates.candyState);
	}

	protected override void Start()
	{
		base.Start();
	}

	public virtual void RandomizePowerState()
	{
		_shapeshift.FSM.ChangeState((Shapeshift.ShapeshiftState)Random.Range(0, (int)Shapeshift.ShapeshiftState.Count));
	}

	public void PlayRandomBasicAttackAnimation()
	{
		var state = _shapeshift.CurrentState;
		var rand = Random.Range(0, _config.basicAttack.animations[state].Length);
		_animator.Play(_config.basicAttack.animations[state][rand].clip.name, _stateLayers.attackLayer, 0);
	}

	protected override void OnDamage(float amount)
	{
		_fsm.ChangeState(State.Hit);
	}

	protected override void OnDeath()
	{
		_fsm.ChangeState(State.Death, StateTransition.Overwrite);
	}

	protected Vector2 GetPlayerDirection()
	{
		if (!_player)
		{
			_player = FindObjectOfType<Player>();
		}
		return _player == null ? Vector2.zero : (Vector2)(_player.transform.position - transform.position);
	}

	void Update()
	{
		if (isDead)
		{
			return;
		}
		
		var dir = GetPlayerDirection();
		if ((_fsm.State == State.Danger)
			&& dir.sqrMagnitude < avoidDistanceSqr)
		{
			_fsm.ChangeState(State.Avoid);
		}

		if (_fsm.State != State.Attack && _fsm.State != State.Attacking &&
			_movable.isMoving)
		{
			_player.RemoveAttacker(this);
		}

		if (_movable.isMoving)
		{
			_shapeshift.PlayCurrentMove();
		}
		else if (_fsm.State != State.Attack && _fsm.State != State.Attacking && _fsm.State != State.Death)
		{
			_shapeshift.PlayCurrentIdle();
		}
			
	}

	void OnCollisionEnter2D(Collision2D other)
	{
		if (other.gameObject.layer == LayerMask.NameToLayer("Environment"))
		{
			if (_fsm.State == State.Avoid)
			{
				_fsm.ChangeState(State.Approach);
			}
		}
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.gameObject.layer == LayerMask.NameToLayer("AttackZone"))
		{
			_inAttackZone = true;
		}
	}

	void OnTriggerExit2D(Collider2D other)
	{
		if (other.gameObject.layer == LayerMask.NameToLayer("AttackZone"))
		{
			_inAttackZone = false;
		}
	}

	public void Block()
	{
		if (_fsm.State != State.Attacking)
		{
			_shapeshift.PlayCurrentBlock();
		}
	}

	protected void CheckPlayerHit(AttackState attack)
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
					player.Damage(attack.damage);
				}
			}
		}
	}

	IEnumerator DeathFlicker()
	{
		_shapeshift.PlayCurrentDeath();
		var start = Time.time;
		var renderer = GetComponent<SpriteRenderer>();
		while (start + _config.deathFlickerDuration >= Time.time)
		{
			renderer.enabled = false;
			yield return new WaitForSeconds(_config.deathFlickerInterval);
			renderer.enabled = true;
			yield return new WaitForSeconds(_config.deathFlickerInterval);
		}

		WaveController.instance.RemoveEnemy(gameObject);
		Destroy(gameObject);
	}

	#region State Methods

	protected virtual void Approach_Enter()
	{
		_otherEnemies = FindObjectsOfType<BetterEnemy>();
	}

	protected virtual void Approach_Update()
	{
		var dir = GetPlayerDirection();

		if (dir.sqrMagnitude < dangerDistanceSqr)
		{
			_fsm.ChangeState(State.Danger);
			return;
		}

		Vector2 separation = Vector2.zero;
		for (int i = 0; i < _otherEnemies.Length; i++)
		{
			var other = _otherEnemies[i];
			if (!other || other == this)
			{
				continue;
			}

			var offset = (Vector2)other.transform.position - (Vector2)transform.position;
			if (offset.sqrMagnitude > avoidEnemiesDistanceSqr)
			{
				continue;
			}
			separation = -offset;
			break;
		}


		if (separation != Vector2.zero)
		{
			_movable.Move(dir + separation, false);
			return;
		}
		_movable.Move(dir);
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
			_fsm.ChangeState(State.Approach);
		}
	}

	protected virtual void Danger_Enter()
	{
		_otherEnemies = FindObjectsOfType<BetterEnemy>();
		_attackTime = Time.time + Random.Range(_config.minAttackInterval, _config.maxAttackInterval);
	}

	protected virtual void Danger_Update()
	{
		if (_attackTime < Time.time)
		{
			_attackTime = Time.time + Random.Range(_config.minAttackInterval, _config.maxAttackInterval);
			if(_player.RequestAttack(this))
			{
				_fsm.ChangeState(State.Attack);
				return;
			}
		}

		if (GetPlayerDirection().sqrMagnitude >= dangerDistanceSqr)
		{
			_fsm.ChangeState(State.Approach);
			return;
		}

		Vector2 separation = Vector2.zero;
		int count = 0;
		for (int i = 0; i < _otherEnemies.Length; i++)
		{
			var other = _otherEnemies[i];
			if (!other || other == this)
			{
				continue;
			}

			var offset = (Vector2)other.transform.position - (Vector2)transform.position;
			var dist = offset.sqrMagnitude;
			if (dist > avoidEnemiesDistanceSqr)
			{
				continue;
			}
			separation += offset / -offset.magnitude;
			count++;
		}
		if (count > 0)
		{
			separation = separation / count;
			_movable.Move(separation);
		}
	}

	protected virtual void Attack_Enter()
	{
		_attackZone = _player.ClosestAttackZone(transform.position);
	}

	protected virtual void Attack_Update()
	{
		if(_inAttackZone)
		{
			_movable.SetDirection(GetPlayerDirection().x);
			_fsm.ChangeState(State.Attacking);
			return;
		}

		var dir = (_attackZone.offset + ((Vector2)_player.transform.position) - (Vector2)transform.position);
		_movable.Move(dir);
	}

	protected virtual void Attack_Exit()
	{
		_inAttackZone = false;
	}

	protected virtual void Attacking_Enter()
	{
		PlayRandomBasicAttackAnimation();

		if (_config.basicAttack.voiceOverSounds.Length > 0)
		{
			var random = Random.Range(0, 1f);
			if (random <= _config.basicAttack.voiceOverChance)
			{
				var audioSetting = _config.basicAttack.voiceOverSounds[Random.Range(0, _config.basicAttack.voiceOverSounds.Length)];
				Music.PlayClipAtPoint(audioSetting, transform.position, Random.Range(0.05f, 0.15f));
			}
		}

		Music.PlayClipAtPoint(_config.basicAttack.attackSound, transform.position);
	}

	protected virtual void Attacking_Update()
	{
		if (_stateLayers.CheckState("None", _stateLayers.attackLayer))
		{
			_fsm.ChangeState(State.Danger);
			_player.RemoveAttacker(this);
			_attackedPlayer = false;
		}
		else
		{
			CheckPlayerHit(_config.basicAttack);
		}
	}

	protected virtual void Attacking_Finally()
	{
		_player.RemoveAttacker(this);
		_attackedPlayer = false;
	}

	protected virtual void Avoid_Enter()
	{
	}

	protected virtual void Avoid_Update()
	{
		var dir = GetPlayerDirection();

		if (dir.sqrMagnitude <= avoidDistanceSqr)
		{
			_movable.Move(-dir, false);
		}
		else
		{
			_fsm.ChangeState(State.Approach);
		}
	}

	protected virtual void Death_Enter()
	{

	}

	#endregion
}
