using UnityEngine;
using System.Collections;
using MonsterLove.StateMachine;
using System.Collections.Generic;

public class Player : Entity
{
	public enum PlayerState
	{
		Normal,
		Hit,
		Attacking
	}

	public enum AttackState
	{
		NoneAttack,
		BasicAttack,
		SuperAttack,
	}

	[SerializeField]
	PlayerConfig _config;

	AudioSource _audioSource;

	int _attackIndex;

	List<Collider2D> _attackedEnemies = new List<Collider2D>();

	PlayerActions _actions;

	StateMachine<PlayerState> _fsm;
	StateMachine<AttackState> _attackFSM;
	Shapeshift.ShapeshiftState _shapeshiftWhenAttack;

	ScreenShake _screenShake;

	public PlayerState State
	{
		get { return _fsm.State; }
		set { _fsm.ChangeState(value); }
	}

	private float _swingTime;
	private float _attackStaggerTime;
	private float _attackCooldownTimer;

	protected override void Awake()
	{
		base.Awake();
		_audioSource = GetComponent<AudioSource>();

		_shapeshift.Init(_config.powerStates, _config.powerStates.candyState);

		_config.powerStates.SetupFrameRates();
		_config.basicAttack.Init();
		_config.superAttack.Init();

		_fsm = StateMachine<PlayerState>.Initialize(this, PlayerState.Normal);

		_attackFSM = StateMachine<AttackState>.Initialize(this, AttackState.NoneAttack);
		_attackFSM.Changed += (state) =>
		{
			_attackCollider.enabled = false;
			_attackedEnemies.Clear();
			if (state != AttackState.NoneAttack)
			{
				_fsm.ChangeState(PlayerState.Attacking);
			}
			else
			{
				_attackIndex = 0;
				_fsm.ChangeState(PlayerState.Normal);
			}
		};

		_health = _healthConfig.maxHealth;
		_screenShake = Camera.main.GetComponent<ScreenShake>();
	}

	protected override void Start()
	{
		base.Start();
		_shapeshift.FSM.Changed += ShapeshiftStateChanged;
		_shapeshift.FSM.ChangeState(Shapeshift.ShapeshiftState.Candy);
	}

	private void OnEnable()
	{
		_actions = PlayerActions.CreateWithDefaultBindings();
	}

	private void OnDisable()
	{
		_actions.Destroy();
	}

	protected override void OnDamage(float amount)
	{
		State = PlayerState.Hit;
	}

	protected override void OnDeath()
	{
		State = PlayerState.Hit;
		Debug.LogErrorFormat("You fucking died");
	}

	private void ShapeshiftStateChanged(Shapeshift.ShapeshiftState state)
	{
		_screenShake.Shake();
	}

	void BasicAttack_Enter()
	{
		_swingTime = Time.time;

		_attackIndex++;
		_swingTime = Time.time + _config.attackCooldown;
		_attackCooldownTimer = Time.time;
		PlayRandomBasicAttackAnimation();

		_attackedEnemies.Clear();
	}

	public override void PlayRandomBasicAttackAnimation()
	{
		var state = _shapeshift.CurrentState;
		var rand = Random.Range(0, _config.basicAttack.animations[state].Length);
		_animator.Play(_config.basicAttack.animations[state][rand].clip.name, 1, 0);
	}

	public void PlayRandomSuperAttackAnimation()
	{
		var state = _shapeshift.CurrentState;
		var rand = Random.Range(0, _config.superAttack.animations[state].Length);
		_animator.Play(_config.superAttack.animations[state][rand].clip.name, 1, 0);
	}

	void BasicAttack_Update()
	{
		if (_swingTime + _config.attackSwingDuration < Time.time)
		{
			_attackFSM.ChangeState(AttackState.NoneAttack);
			return;
		}

		if (_shapeshiftWhenAttack != _shapeshift.CurrentState)
		{
			_attackIndex = 0;
		}

		if (_attackCooldownTimer + _config.attackCooldown < Time.time
			&& _actions.StateActions[_shapeshift.CurrentState].WasPressed)
		{
			if (++_attackIndex >= _config.basicAttackCount)
			{
				_attackFSM.ChangeState(AttackState.SuperAttack);
			}
			else
			{
				PlayRandomBasicAttackAnimation();
				_swingTime = Time.time + _config.attackCooldown;
				_attackCooldownTimer = Time.time;
				_attackedEnemies.Clear();
			}
			return;
		}

		CheckEnemyHit(_config.basicAttack);
	}

	private void SuperAttack_Enter()
	{
		PlayRandomSuperAttackAnimation();

		_attackStaggerTime = Time.time;
		_attackedEnemies.Clear();
	}

	private void SuperAttack_Update()
	{
		if (_attackStaggerTime + _config.attackStaggerDuration < Time.time)
		{
			_attackFSM.ChangeState(AttackState.NoneAttack);
			_fsm.ChangeState(PlayerState.Normal);
			return;
		}

		if (CheckEnemyHit(_config.superAttack))
		{
			StartCoroutine(_actions.Vibrate(1f, 0.3f));
		}
	}

	private void Hit_Enter()
	{
		BlinkManager.instance.AddBlink(gameObject, Color.white, 0.1f);
		_animator.SetTrigger("Hit");
	}

	private void Hit_Update()
	{
	}

	private void Normal_Update()
	{
		var move = _actions.Move.Value;

		_movable.Move(move);

		if (_movable.isMoving)
		{
			_shapeshift.PlayCurrentMove();
		}
		else
		{
			_shapeshift.PlayCurrentIdle();
		}

		var candy = _actions.Candy.WasPressed;
		var lightning = _actions.Lightning.WasPressed;
		var magic = _actions.Magic.WasPressed;
		var avocado = _actions.Avocado.WasPressed;

		if (candy)
		{
			NewShapeShiftState(Shapeshift.ShapeshiftState.Candy);
		}
		else if (lightning)
		{
			NewShapeShiftState(Shapeshift.ShapeshiftState.Lightning);
		}
		else if (magic)
		{
			NewShapeShiftState(Shapeshift.ShapeshiftState.Magic);
		}
		else if (avocado)
		{
			NewShapeShiftState(Shapeshift.ShapeshiftState.Avocado);
		}
	}

	void NewShapeShiftState(Shapeshift.ShapeshiftState state)
	{
		if (_shapeshiftWhenAttack != state)
		{
			_shapeshiftWhenAttack = state;
			_attackIndex = 0;
			_shapeshift.FSM.ChangeState(state, StateTransition.Overwrite);
		}
	}

	void NoneAttack_Update()
	{
		bool attack = _actions.StateActions[_shapeshift.CurrentState].WasPressed;

		if (attack)
		{
			_attackFSM.ChangeState(AttackState.BasicAttack);
		}
	}

	private bool CheckEnemyHit(PlayerConfig.PlayerAttack attack)
	{
		if (!_attackCollider || !_attackCollider.enabled)
		{
			return false;
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
			return false;
		}

		bool hit = false;
		for (int i = 0; i < colliders.Length; i++)
		{
			var otherCollider = colliders[i];
			if (otherCollider && otherCollider.gameObject != gameObject)
			{
				var enemy = otherCollider.GetComponentInParent<Enemy>();
				if (!enemy || _attackedEnemies.Contains(otherCollider))
				{
					continue;
				}
				if (_attackedEnemies.Count == 0)
				{
					_audioSource.Play();
				}
				Camera.main.GetComponent<ScreenShake>().Shake();
				_attackedEnemies.Add(otherCollider);
				hit = true;

				if (_shapeshift.CurrentState == enemy.ShapeshiftState)
				{
					enemy.Damage(attack.damage);
				}
				else
				{
					enemy.Block();
				}
			}
		}

		if (hit)
		{
			FreezeFrameController.instance.DoFreeze(attack.freezeDuration, attack.freezeValue);
		}
		return hit;
	}
}