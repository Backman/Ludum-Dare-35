using UnityEngine;
using System.Collections;
using MonsterLove.StateMachine;

public class BasicEnemy : MonoBehaviour
{
	public enum State
	{
		Walking,
		Waiting,
		Attack
	}

	[SerializeField]
	BasicEnemyConfig _config;

	Movable _movable;

	Player _player;

	StateMachine<State> _fsm;

	Transform _transform;
	public new Transform transform
	{
		get { return _transform == null ? (_transform = GetComponent<Transform>()) : _transform; }
	}

	void Awake()
	{
		_movable = GetComponent<Movable>();

		_fsm = StateMachine<State>.Initialize(this);
		_fsm.Changed += StateChanged;
		_fsm.ChangeState(State.Walking);
	}

	void Start()
	{
		_player = FindObjectOfType<Player>();
	}

	void Update()
	{
		var playerPos = _player.transform.position;
		var moveDir = playerPos - transform.position;

		if (moveDir.sqrMagnitude < _config.attackRange)
		{

		}

		_movable.Move(moveDir.normalized);
	}

	void StateChanged(State state)
	{
	}
}
