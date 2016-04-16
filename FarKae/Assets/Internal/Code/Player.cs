using UnityEngine;
using System.Collections;
using MonsterLove.StateMachine;

public class Player : MonoBehaviour
{
	public enum State
	{
		Normal,
		Attacking,
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
	Shapeshift _shapeshift;

	Transform _transform;
	public new Transform transform
	{
		get { return _transform == null ? (_transform = GetComponent<Transform>()) : _transform; }
	}

	void Awake()
	{
		_movable = GetComponent<Movable>();
		_animator = GetComponent<Animator>();
		_renderer = GetComponent<SpriteRenderer>();

		_shapeshift = GetComponent<Shapeshift>();
	}

	void Start()
	{
		_shapeshift.FSM.Changed += ShapeshiftStateChanged;
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
		var magic = _actions.Magic.WasPressed;
		var avocado = _actions.Avocado.WasPressed;

		if (candy)
		{
			attack = true;
			_shapeshift.FSM.ChangeState(Shapeshift.State.Candy);
		}
		else if (lightning)
		{
			attack = true;
			_shapeshift.FSM.ChangeState(Shapeshift.State.Lightning);
		}
		else if (magic)
		{
			attack = true;
			_shapeshift.FSM.ChangeState(Shapeshift.State.Magic);
		}
		else if (avocado)
		{
			attack = true;
			_shapeshift.FSM.ChangeState(Shapeshift.State.Avocado);
		}

		if (attack)
		{
			_animator.SetTrigger("Attack_Punch");
		}
	}

	void ShapeshiftStateChanged(Shapeshift.State state)
	{
		switch (state)
		{
			case Shapeshift.State.Candy:
				_animator.CrossFade("Candy.Idle", 0f, 0, 0f);
				break;
			case Shapeshift.State.Lightning:
				_animator.CrossFade("Lightning.Idle", 0f, 0, 0f);
				break;
			case Shapeshift.State.Magic:
				_animator.CrossFade("Magic.Idle", 0f, 0, 0f);
				break;
			case Shapeshift.State.Avocado:
				_animator.CrossFade("Avocado.Idle", 0f, 0, 0f);
				break;
			default:
				break;
		}
	}

	void OnCandyEnter()
	{
		_animator.CrossFade("Candy.Idle", 0f, 0, 0f);
	}

	void OnLightningEnter()
	{
		_animator.CrossFade("Lightning.Idle", 0f, 0, 0f);
	}

	void OnMagicEnter()
	{
		_animator.CrossFade("Magic.Idle", 0f, 0, 0f);
	}

	void OnAvocadoEnter()
	{
		_animator.CrossFade("Avocado.Idle", 0f, 0, 0f);
	}
}