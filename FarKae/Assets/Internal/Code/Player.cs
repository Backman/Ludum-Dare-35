using UnityEngine;
using System.Collections;
using MonsterLove.StateMachine;
using System;
using InControl;

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
		None,
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

	public bool canAttack = true;

	BoxCollider2D[] _overlappedHitColliders = new BoxCollider2D[8];

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
		_movable = GetComponent<Movable>();
		_animator = GetComponent<Animator>();
		_renderer = GetComponent<SpriteRenderer>();

		_shapeshift = GetComponent<Shapeshift>();

		_fsm = StateMachine<PlayerState>.Initialize(this, PlayerState.Normal);
		_attackFSM = StateMachine<AttackState>.Initialize(this, AttackState.None);

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
		if (State == PlayerState.Hit)
		{
			return;
		}
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

	public void AllowAttack()
	{
		canAttack = true;
	}

	private void ShapeshiftStateChanged(Shapeshift.ShapeshiftState state)
	{
		_screenShake.Shake();
		switch (state)
		{
			case Shapeshift.ShapeshiftState.Candy:
				//_animator.CrossFade("Candy.Idle", 0f, 0, 0f);
				break;
			case Shapeshift.ShapeshiftState.Lightning:
				//_animator.CrossFade("Lightning.Idle", 0f, 0, 0f);
				break;
			case Shapeshift.ShapeshiftState.Magic:
				//_animator.CrossFade("Magic.Idle", 0f, 0, 0f);
				break;
			case Shapeshift.ShapeshiftState.Avocado:
				//_animator.CrossFade("Avocado.Idle", 0f, 0, 0f);
				break;
			default:
				break;
		}
	}

	private void AttackOne_Enter()
	{
		_fsm.ChangeState(PlayerState.Attacking);
		canAttack = false;
		_shapeshiftState = _shapeshift.CurrentState;
		_movable.Move(Vector2.zero);
		_animator.SetTrigger("AttackOne");

		_swingTime = Time.time;
	}

	private void AttackOne_Update()
	{
		if (_swingTime + _config.attackSwingDuration < Time.time)
		{
			_attackFSM.ChangeState(AttackState.None);
			return;
		}
		if (_actions.StateActions[_shapeshift.CurrentState].WasPressed)
		{
			_attackFSM.ChangeState(AttackState.AttackTwo);
			return;
		}
		CheckEnemyHit(_config.firstAttackDamage);
	}

	private void AttackTwo_Enter()
	{
		canAttack = false;
		_movable.Move(Vector2.zero);
		_animator.SetTrigger("AttackTwo");

		_swingTime = Time.time;
	}

	private void AttackTwo_Update()
	{
		if (_swingTime + _config.attackSwingDuration < Time.time)
		{
			_attackFSM.ChangeState(AttackState.None);
			return;
		}
		if (_actions.StateActions[_shapeshift.CurrentState].WasPressed)
		{
			_attackFSM.ChangeState(AttackState.SuperAttack);
			return;
		}
		CheckEnemyHit(_config.secondAttackDamage);
	}
	private void SuperAttack_Enter()
	{
		_movable.Move(Vector2.zero);
		_animator.SetTrigger("SuperAttack");

		_attackStaggerTime = Time.time;
	}

	private void SuperAttack_Update()
	{
		CheckEnemyHit(_config.superAttackDamage);

		if (_attackStaggerTime + _config.attackStaggerDuration < Time.time)
		{
			_attackFSM.ChangeState(AttackState.None);
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
				var enemy = otherCollider.GetComponentInParent<BasicEnemy>();
				if (enemy && _shapeshift.CurrentState == enemy.ShapeshiftState)
				{
					Camera.main.GetComponent<ScreenShake>().Shake();
					enemy.Damage(damage);
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
	}

	void None_Update()
	{
		bool attack = false;

		var candy = _actions.Candy.WasPressed;
		var lightning = _actions.Lightning.WasPressed;
		var magic = _actions.Magic.WasPressed;
		var avocado = _actions.Avocado.WasPressed;

		if (candy)
		{
			attack = true;
			_shapeshift.FSM.ChangeState(Shapeshift.ShapeshiftState.Candy);
		}
		else if (lightning)
		{
			attack = true;
			_shapeshift.FSM.ChangeState(Shapeshift.ShapeshiftState.Lightning);
		}
		else if (magic)
		{
			attack = true;
			_shapeshift.FSM.ChangeState(Shapeshift.ShapeshiftState.Magic);
		}
		else if (avocado)
		{
			attack = true;
			_shapeshift.FSM.ChangeState(Shapeshift.ShapeshiftState.Avocado);
		}

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