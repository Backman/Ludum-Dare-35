using UnityEngine;
using System.Collections;
using MonsterLove.StateMachine;

public class Player : MonoBehaviour
{
	public enum PlayerState
	{
		Normal,
		Attack,
		Hit
	}

	public float dashRepeatDuration = 0.3f;

	public BoxCollider2D attackCollider;
	public BoxCollider2D hitCollider; 

	BoxCollider2D[] _overlappedHitColliders = new BoxCollider2D[8];

	Animator _animator;
	SpriteRenderer _renderer;
	Movable _movable;
	PlayerActions _actions;
	Shapeshift _shapeshift;

	StateMachine<PlayerState> _fsm;
	Shapeshift.ShapeshiftState _shapeshiftState;

	public PlayerState State
	{
		get { return _fsm.State; }
		set { _fsm.ChangeState(value); }
	}

	public void ChangeState(PlayerState state)
	{
		_fsm.ChangeState(state);
	}

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

		_fsm = StateMachine<PlayerState>.Initialize(this, PlayerState.Normal);
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

	void ShapeshiftStateChanged(Shapeshift.ShapeshiftState state)
	{
		//switch (state)
		//{
		//	case Shapeshift.State.Candy:
		//		_animator.CrossFade("Candy.Idle", 0f, 0, 0f);
		//		break;
		//	case Shapeshift.State.Lightning:
		//		_animator.CrossFade("Lightning.Idle", 0f, 0, 0f);
		//		break;
		//	case Shapeshift.State.Magic:
		//		_animator.CrossFade("Magic.Idle", 0f, 0, 0f);
		//		break;
		//	case Shapeshift.State.Avocado:
		//		_animator.CrossFade("Avocado.Idle", 0f, 0, 0f);
		//		break;
		//	default:
		//		break;
		//}
	}

	void Attack_Enter()
	{
		_movable.Move(Vector2.zero);
		_animator.SetTrigger("Attack_Punch");
	}

	void Attack_Update()
	{
		if (!attackCollider || !attackCollider.enabled)
		{
			return;
		}

		var min = attackCollider.offset + (Vector2)transform.position + attackCollider.size / 2.0f;
		var max = min + attackCollider.offset;

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
				Debug.LogFormat("Hit {0}", otherCollider.name);
				var enemy = otherCollider.GetComponentInParent<BasicEnemy>();
				if (enemy && _shapeshift.State == enemy.ShapeshiftState)
				{
					enemy.Damage(50f);
				}
			}
		}
	}

	void Hit_Enter()
	{
		//_animator.SetTrigger("Hit");
	}

	void Hit_Update()
	{
	}

	void Normal_Update()
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
			_fsm.ChangeState(PlayerState.Attack);
		}
	}
}