using UnityEngine;
using System.Collections;
using MonsterLove.StateMachine;
using System;
using InControl;
using System.Collections.Generic;

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
	Shapeshift.ShapeshiftState _shapeshiftState;

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
		_audioSource = GetComponent<AudioSource> ();

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
			Debug.LogErrorFormat("You fuckign dieadf");
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

	private void AttackOne_Enter()
	{
		_shapeshiftState = _shapeshift.CurrentState;
		_animator.SetTrigger("AttackOne");

		_swingTime = Time.time;
		TryDash(_config.attackOne);
	}

	private void AttackOne_Update()
	{
		if (_swingTime + _config.attackSwingDuration < Time.time)
		{
			_attackFSM.ChangeState(AttackState.NoneAttack);
			return;
		}
		if (_actions.StateActions[_shapeshift.CurrentState].WasPressed)
		{
			_attackFSM.ChangeState(AttackState.AttackTwo);
			return;
		}
		CheckEnemyHit(_config.attackOne.damage);
	}

	private void AttackTwo_Enter()
	{
		_animator.SetTrigger("AttackTwo");

		_swingTime = Time.time;
		TryDash(_config.attackTwo);
	}

	private void AttackTwo_Update()
	{
		if (_swingTime + _config.attackSwingDuration < Time.time)
		{
			_attackFSM.ChangeState(AttackState.NoneAttack);
			return;
		}
		if (_actions.StateActions[_shapeshift.CurrentState].WasPressed)
		{
			_attackFSM.ChangeState(AttackState.SuperAttack);
			return;
		}
		CheckEnemyHit(_config.attackTwo.damage);
	}

	private void SuperAttack_Enter()
	{
		_animator.SetTrigger("SuperAttack");

		_attackStaggerTime = Time.time;
		TryDash(_config.superAttack);
	}

	private void SuperAttack_Update()
	{
		CheckEnemyHit(_config.superAttack.damage);

		if (_attackStaggerTime + _config.attackStaggerDuration < Time.time)
		{
			_attackFSM.ChangeState(AttackState.NoneAttack);
			_fsm.ChangeState(PlayerState.Normal);
		}
	}

	private void CheckEnemyHit(float damage)
	{
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
			if (otherCollider && otherCollider.gameObject != gameObject)
			{
				var enemy = otherCollider.GetComponentInParent<BasicEnemy>();
				if (enemy && _shapeshift.CurrentState == enemy.ShapeshiftState
					&& !_attackedEnemies.Contains(otherCollider))
				{
					if (_attackedEnemies.Count == 0 && !_audioSource.isPlaying) {
						_audioSource.Play ();
					}
						
					Camera.main.GetComponent<ScreenShake>().Shake();
					enemy.Damage(damage);
					_attackedEnemies.Add(otherCollider);
				}
			}
		}
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
			_shapeshift.FSM.ChangeState(Shapeshift.ShapeshiftState.Candy);
			if (_shapeshiftState != Shapeshift.ShapeshiftState.Candy)
			{
				_attackFSM.ChangeState(AttackState.NoneAttack, StateTransition.Overwrite);
			}
		}
		else if (lightning)
		{
			_shapeshift.FSM.ChangeState(Shapeshift.ShapeshiftState.Lightning);
			if (_shapeshiftState != Shapeshift.ShapeshiftState.Lightning)
			{
				_attackFSM.ChangeState(AttackState.NoneAttack, StateTransition.Overwrite);
			}
		}
		else if (magic)
		{
			_shapeshift.FSM.ChangeState(Shapeshift.ShapeshiftState.Magic);
			if (_shapeshiftState != Shapeshift.ShapeshiftState.Magic)
			{
				_attackFSM.ChangeState(AttackState.NoneAttack, StateTransition.Overwrite);
			}
		}
		else if (avocado)
		{
			_shapeshift.FSM.ChangeState(Shapeshift.ShapeshiftState.Avocado);
			if (_shapeshiftState != Shapeshift.ShapeshiftState.Avocado)
			{
				_attackFSM.ChangeState(AttackState.NoneAttack, StateTransition.Overwrite);
			}
		}
	}

	void NoneAttack_Update()
	{
		bool attack = _actions.StateActions[_shapeshift.CurrentState].WasPressed;

		if (attack)
		{
			_attackFSM.ChangeState(AttackState.AttackOne);
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