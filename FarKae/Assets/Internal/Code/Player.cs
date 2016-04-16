using UnityEngine;
using System.Collections;
using MonsterLove.StateMachine;

public class Player : MonoBehaviour
{
	public enum State
	{
		Candy,
		Lightning,
		Crystal,
		Avocado
	}

	public float dashRepeatDuration = 0.3f;

	[SerializeField]
	StateConfig _candyConfig;
	[SerializeField]
	StateConfig _lightningConfig;
	[SerializeField]
	StateConfig _crystalConfig;
	[SerializeField]
	StateConfig _avocadoConfig;

	Animator _animator;
	SpriteRenderer _renderer;
	Movable _movable;
	PlayerActions _actions;


	StateMachine<State> _fsm;

	Transform _transform;
	public new Transform transform
	{
		get { return _transform == null ? (_transform = GetComponent<Transform>()) : _transform; }
	}

	public State CurrentState
	{
		get { return _fsm.State; }
		set { _fsm.ChangeState(value); }
	}

	void Awake()
	{
		_movable = GetComponent<Movable>();
		_animator = GetComponent<Animator>();
		_renderer = GetComponent<SpriteRenderer>();

		_fsm = StateMachine<State>.Initialize(this);
		_fsm.Changed += StateChanged;
		_fsm.ChangeState(State.Candy);
	}

	void OnEnable()
	{
		_actions = PlayerActions.CreateWithDefaultBindings();
	}

	void OnDisable()
	{
		_actions.Destroy();
	}

	void Update()
	{
		var move = _actions.Move.Value;

		_movable.Move(move);


		bool attack = false;

		var candy = _actions.Candy.WasPressed;
		var lightning = _actions.Lightning.WasPressed;
		var piss = _actions.Crystal.WasPressed;
		var poop = _actions.Avocado.WasPressed;

		switch (_fsm.State)
		{
			case State.Candy:
				attack = candy;
				if (lightning)
				{
					_fsm.ChangeState(State.Lightning);
				}
				if (piss)
				{
					_fsm.ChangeState(State.Crystal);
				}
				if (poop)
				{
					_fsm.ChangeState(State.Avocado);
				}
				break;
			case State.Lightning:
				attack = lightning;
				if (candy)
				{
					_fsm.ChangeState(State.Candy);
				}
				if (piss)
				{
					_fsm.ChangeState(State.Crystal);
				}
				if (poop)
				{
					_fsm.ChangeState(State.Avocado);
				}
				break;
			case State.Crystal:
				attack = piss;
				if (candy)
				{
					_fsm.ChangeState(State.Candy);
				}
				if (lightning)
				{
					_fsm.ChangeState(State.Lightning);
				}
				if (poop)
				{
					_fsm.ChangeState(State.Avocado);
				}
				break;
			case State.Avocado:
				attack = poop;
				if (candy)
				{
					_fsm.ChangeState(State.Candy);
				}
				if (lightning)
				{
					_fsm.ChangeState(State.Lightning);
				}
				if (piss)
				{
					_fsm.ChangeState(State.Crystal);
				}
				break;
			default:
				break;
		}

		if (attack)
		{
			_animator.SetTrigger("Attack_Punch");
		}
	}

	void StateChanged(State state)
	{
		switch (state)
		{
			case State.Candy:
				_renderer.color = _candyConfig.color;
				break;
			case State.Lightning:
				_renderer.color = _lightningConfig.color;
				break;
			case State.Crystal:
				_renderer.color = _crystalConfig.color;
				break;
			case State.Avocado:
				_renderer.color = _avocadoConfig.color;
				break;
			default:
				break;
		}
	}

	void Candy_Enter()
	{
		_animator.CrossFade("Candy", 0f, 0, 0f);
	}

	void Candy_Update()
	{
	}

	void Lightning_Enter()
	{
		_animator.CrossFade("Lightning", 0f, 0, 0f);
	}

	void Lightning_Update()
	{
		Debug.Log("Lightning State");
	}

	void Crystal_Enter()
	{
		_animator.CrossFade("Crystal", 0f, 0, 0f);
	}

	void Crystal_Update()
	{
		Debug.Log("Crystal State");
	}

	void Avocado_Enter()
	{
		_animator.CrossFade("Avocado", 0f, 0, 0f);
	}

	void Avocado_Update()
	{
		Debug.Log("Avocado State");
	}
}