using UnityEngine;
using System.Collections;
using MonsterLove.StateMachine;

public class BasicEnemy : MonoBehaviour
{
	public enum State
	{
		Walking,
		Idle,
		Attack,
		Hit
	}

	[SerializeField]
	BasicEnemyConfig _config;

	Animator _animator;
	Movable _movable;
	Player _player;

	StateMachine<State> _fsm;
	Shapeshift _shapeshift;

	float _staggerTime;
	float _attackTime;

	Transform _transform;
	public new Transform transform
	{
		get { return _transform == null ? (_transform = GetComponent<Transform>()) : _transform; }
	}

	void Awake()
	{
		_movable = GetComponent<Movable>();
		_animator = GetComponent<Animator>();

		_fsm = StateMachine<State>.Initialize(this);
		_fsm.Changed += StateChanged;
		_fsm.ChangeState(State.Walking);

		_shapeshift = GetComponent<Shapeshift>();
	}

	void Start()
	{
		_player = FindObjectOfType<Player>();
	}

	void Update()
	{

	}

	public void ChangeState(State state)
	{
		_fsm.ChangeState(state);
	}

	public void Event_OnDamage(float health)
	{
		if (health <= 0)
		{
			WaveController.instance.RemoveEnemy(gameObject);
			Destroy(gameObject);
		}
		else
		{
			_fsm.ChangeState(State.Hit);
		}
		//Debug.LogFormat("{0} health: {1}", name, health);
	}

	void StateChanged(State state)
	{
		//Debug.LogFormat("Enemy state changed: {0}", state.ToString());
	}

	void Hit_Enter()
	{
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
		if (moveDir.sqrMagnitude <= _config.attackRange)
		{
			_fsm.ChangeState(State.Attack);
			_movable.Move(Vector2.zero);
			return;
		}
		_movable.Move(moveDir.normalized);
	}

	void Idle_Enter()
	{
		_fsm.ChangeState(State.Walking);
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

		if (_attackTime + _config.attackInterval <= Time.time)
		{
			_attackTime = Time.time;
			_animator.SetTrigger("Attack_Punch");
		}

		if (length > _config.attackRange)
		{
			_fsm.ChangeState(State.Walking);
			return;
		}
	}
}
