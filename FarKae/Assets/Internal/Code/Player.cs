using UnityEngine;
using System.Collections;
using MonsterLove.StateMachine;
using System;
using InControl;
using System.Collections.Generic;
using UnityEngine.Experimental.Director;

public class Player : MonoBehaviour
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
		AttackOne,
		AttackTwo,
		SuperAttack,
	}

	[SerializeField]
	PlayerConfig _config;
	[SerializeField]
	HealthConfig _healthConfig;

	public BoxCollider2D attackCollider;
	public BoxCollider2D hitCollider;

	AudioSource _audioSource;

	int _attackIndex;

	LayerMask _hitboxColliderLayer;
	LayerMask _attackColliderLayer;

	BoxCollider2D[] _overlappedHitColliders = new BoxCollider2D[8];
	List<BoxCollider2D> _attackedEnemies = new List<BoxCollider2D>();

	Animator _animator;
	SpriteRenderer _renderer;
	Movable _movable;
	PlayerActions _actions;
	Shapeshift _shapeshift;

	StateMachine<PlayerState> _fsm;
	StateMachine<AttackState> _attackFSM;
	Shapeshift.ShapeshiftState _shapeshiftWhenAttack;

	float _health;

	ScreenShake _screenShake;

	public PlayerState State
	{
		get { return _fsm.State; }
		set { _fsm.ChangeState(value); }
	}

	Transform _transform;
	private float _swingTime;
	private float _attackStaggerTime;
	private float _attackCooldownTimer;

	public new Transform transform
	{
		get { return _transform == null ? (_transform = GetComponent<Transform>()) : _transform; }
	}

	private void Awake()
	{
		_hitboxColliderLayer = LayerMask.NameToLayer("HitboxCollider");
		_attackColliderLayer = LayerMask.NameToLayer("AttackCollider");

		_movable = GetComponent<Movable>();
		_animator = GetComponent<Animator>();
		_renderer = GetComponent<SpriteRenderer>();
		_audioSource = GetComponent<AudioSource>();

		_shapeshift = GetComponent<Shapeshift>();

		_fsm = StateMachine<PlayerState>.Initialize(this, PlayerState.Normal);

		_attackFSM = StateMachine<AttackState>.Initialize(this, AttackState.NoneAttack);
		_attackFSM.Changed += (state) =>
		{
			attackCollider.enabled = false;
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

	private void Start()
	{
		_shapeshift.FSM.Changed += ShapeshiftStateChanged;
	}

	private void OnEnable()
	{
		_actions = PlayerActions.CreateWithDefaultBindings();
	}

	private void OnDisable()
	{
		_actions.Destroy();
	}

	public void Damage(float amount)
	{
		_health -= amount;
		BlinkManager.instance.AddBlink(gameObject, Color.white, 0.1f);
		if (_health > 0f)
		{
			State = PlayerState.Hit;
		}
		else
		{
			State = PlayerState.Hit;
			Debug.LogErrorFormat("You fucking died");
		}
	}

	void TryDash(PlayerConfig.Attack attack)
	{
		if (attack.doDash)
		{
			_movable.Dash(attack.dashLength, attack.dashDuration, attack.dashCurve);
		}
	}

	private void ShapeshiftStateChanged(Shapeshift.ShapeshiftState state)
	{
		_screenShake.Shake();
	}

	void BasicAttack_Enter()
	{
		Debug.Log("BasicAttack");
		_swingTime = Time.time;

		_attackIndex++;
		_shapeshift.SetRandomBasicAttackAnimation();
		_animator.Play("BasicAttack", 1, 0f);
		_swingTime = Time.time + _config.attackCooldown;
		_attackCooldownTimer = Time.time;
	}

	void BasicAttack_Update()
	{
		if (_swingTime + _config.attackSwingDuration < Time.time)
		{
			_attackFSM.ChangeState(AttackState.NoneAttack);
			return;
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
				_shapeshift.SetRandomBasicAttackAnimation();
				_animator.Play("BasicAttack", 1, 0f);
				_swingTime = Time.time + _config.attackCooldown;
				_attackCooldownTimer = Time.time;
			}
			return;
		}

		CheckEnemyHit(_config.basicAttack);
	}

	void BasicAttack_Finally()
	{
		attackCollider.enabled = false;
	}

	private void SuperAttack_Enter()
	{
		Debug.Log("SuperAttack");
		_animator.Play("BoomMOFO", 1, 0f);

		_attackStaggerTime = Time.time;
		TryDash(_config.superAttack);
	}

	void SuperAttack_Finally()
	{
		attackCollider.enabled = false;
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

	private bool CheckEnemyHit(PlayerConfig.Attack attack)
	{
		if (!attackCollider || !attackCollider.enabled)
		{
			return false;
		}

		var min = attackCollider.offset;
		min.x *= _movable.GetDirection();
		min += (Vector2)transform.position;
		var max = min + (Vector2)attackCollider.bounds.max;

		if (Physics2D.OverlapAreaNonAlloc(min, max, _overlappedHitColliders, (1 << _hitboxColliderLayer.value) | (1 << _attackColliderLayer.value)) <= 0)
		{
			return false;
		}

		bool result = false;
		for (int i = 0; i < _overlappedHitColliders.Length; i++)
		{
			var otherCollider = _overlappedHitColliders[i];
			if (otherCollider && otherCollider.gameObject != gameObject)
			{
				var enemy = otherCollider.GetComponentInParent<BasicEnemy>();
				if (enemy && _shapeshift.CurrentState == enemy.ShapeshiftState
					&& !_attackedEnemies.Contains(otherCollider))
				{
					if (_attackedEnemies.Count == 0 && !_audioSource.isPlaying) {
						_audioSource.Play ();
					}
					
					result = true;
					Camera.main.GetComponent<ScreenShake>().Shake();
					enemy.Damage(attack.damage);
					_attackedEnemies.Add(otherCollider);
				}
			}
		}

		if (result)
		{
			FreezeFrameController.instance.DoFreeze(attack.freezeDuration, attack.freezeValue);
		}
		return result;
	}

	private void Hit_Enter()
	{
		_animator.SetTrigger("Hit");
	}

	private void Hit_Update()
	{
	}

	private void Normal_Update()
	{
		var move = _actions.Move.Value;

		_movable.Move(move);

		var candy = _actions.Candy.WasPressed;
		var lightning = _actions.Lightning.WasPressed;
		var magic = _actions.Magic.WasPressed;
		var avocado = _actions.Avocado.WasPressed;

		if (candy)
		{
			if (_shapeshiftWhenAttack != Shapeshift.ShapeshiftState.Candy)
			{
				//_attackFSM.ChangeState(AttackState.NoneAttack, StateTransition.Overwrite);
				_attackIndex = 0;
			}
			_shapeshift.FSM.ChangeState(Shapeshift.ShapeshiftState.Candy);
		}
		else if (lightning)
		{
			if (_shapeshiftWhenAttack != Shapeshift.ShapeshiftState.Lightning)
			{
				//_attackFSM.ChangeState(AttackState.NoneAttack, StateTransition.Overwrite);
				_attackIndex = 0;
			}
			_shapeshift.FSM.ChangeState(Shapeshift.ShapeshiftState.Lightning);
		}
		else if (magic)
		{
			if (_shapeshiftWhenAttack != Shapeshift.ShapeshiftState.Magic)
			{
				//_attackFSM.ChangeState(AttackState.NoneAttack, StateTransition.Overwrite);
				_attackIndex = 0;
			}
			_shapeshift.FSM.ChangeState(Shapeshift.ShapeshiftState.Magic);
		}
		else if (avocado)
		{
			if (_shapeshiftWhenAttack != Shapeshift.ShapeshiftState.Avocado)
			{
				_attackFSM.ChangeState(AttackState.NoneAttack, StateTransition.Overwrite);
				_attackIndex = 0;
			}
			_shapeshift.FSM.ChangeState(Shapeshift.ShapeshiftState.Avocado);
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

#if UNITY_EDITOR
	private void OnDrawGizmos()
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