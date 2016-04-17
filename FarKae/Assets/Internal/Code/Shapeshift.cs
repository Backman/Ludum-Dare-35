using UnityEngine;
using System.Collections;
using MonsterLove.StateMachine;
using UnityEngine.Experimental.Director;

public class Shapeshift : MonoBehaviour
{
	public enum ShapeshiftState
	{
		Candy = 0,
		Lightning,
		Magic,
		Avocado,
		Count
	}

	public bool playStateChangeEffects;
	public bool applyColors = true;

	[SerializeField]
	Transform _stateEffectSocket;

	public EntityConfig.PowerStates _powerStates;

	public StateMachine<ShapeshiftState> FSM;

	public System.Action onCandyEnter;
	public System.Action onCandyUpdate;
	public System.Action onLightningEnter;
	public System.Action onLightningUpdate;
	public System.Action onMagicEnter;
	public System.Action onMagicUpdate;
	public System.Action onAvocadoEnter;
	public System.Action onAvocadoUpdate;

	GameObject _candyEffect;
	GameObject _lightningEffect;
	GameObject _magicEffect;
	GameObject _avocadoEffect;

	Movable _movable;

	public EntityConfig.PowerState currentPowerState;

	private Animator _animator;

	public ShapeshiftState CurrentState
	{
		get { return FSM.State; }
		set { FSM.ChangeState(value); }
	}

	void Awake()
	{
		FSM = StateMachine<ShapeshiftState>.Initialize(this);
		FSM.Changed += StateChanged;

		_movable = GetComponent<Movable>();
		_animator = GetComponent<Animator>();
	}

	public void Init(EntityConfig.PowerStates states,
		EntityConfig.PowerState state)
	{
		_powerStates = states;

		CreateStateEffect(_powerStates.candyState.effect, out _candyEffect);
		CreateStateEffect(_powerStates.lightningState.effect, out _lightningEffect);
		CreateStateEffect(_powerStates.magicState.effect, out _magicEffect);
		CreateStateEffect(_powerStates.avocadoState.effect, out _avocadoEffect);

		currentPowerState = state;
	}

	void CreateStateEffect(GameObject prefab, out GameObject effect)
	{
		effect = null;
		if (prefab && playStateChangeEffects)
		{
			effect = Instantiate(prefab);
			effect.transform.SetParent(_stateEffectSocket, false);
			effect.SetActive(false);
		}
	}

	void PlayStateEffect(GameObject effect)
	{
		if (effect && playStateChangeEffects)
		{
			effect.GetComponent<ParticleSystem>().time = 0f;
			effect.GetComponent<ParticleSystem>().Play(true);
			effect.SetActive(true);
		}
	}

	bool CheckState(string name, int layer)
	{
		var state = _animator.GetCurrentAnimatorStateInfo(layer);
		return state.IsName(name);
	}

	void ChangeState(EntityConfig.PowerState powerState, GameObject effect)
	{
		PlayStateEffect(effect);
		if (applyColors)
		{
			GetComponent<SpriteRenderer>().color = powerState.color;
		}

		currentPowerState = powerState;
	}

	void StateChanged(ShapeshiftState state)
	{
		switch (state)
		{
			case ShapeshiftState.Candy:
				ChangeState(_powerStates.candyState, _candyEffect);
				break;
			case ShapeshiftState.Lightning:
				ChangeState(_powerStates.lightningState, _lightningEffect);
				break;
			case ShapeshiftState.Magic:
				ChangeState(_powerStates.magicState, _magicEffect);
				break;
			case ShapeshiftState.Avocado:
				ChangeState(_powerStates.avocadoState, _avocadoEffect);
				break;
			default:
				break;
		}
	}

	public void PlayCurrentHit()
	{
		//if (!CheckState(currentPowerState.superAttack.clip.name, 1))
		//{
		//	_animator.Play(currentPowerState.superAttack.clip.name, 1, 0f);
		//}
	}

	public void PlayCurrentBlock()
	{
		//if (!CheckState(currentPowerState.superAttack.clip.name, 1))
		//{
		//	_animator.Play(currentPowerState.superAttack.clip.name, 1, 0f);
		//}
	}

	public void PlayCurrentIdle()
	{
		if (!CheckState(currentPowerState.idle.clip.name, 0))
		{
			_animator.Play(currentPowerState.idle.clip.name, 0, 0f);
		}
	}

	public void PlayCurrentMove()
	{
		if (!CheckState(currentPowerState.move.clip.name, 0))
		{
			_animator.Play(currentPowerState.move.clip.name, 0, 0f);
		}
	}

	void Candy_Enter()
	{
		if (onCandyEnter != null)
		{
			onCandyEnter();
		}
	}

	void Candy_Update()
	{
		if (onCandyUpdate != null)
		{
			onCandyUpdate();
		}
	}

	void Lightning_Enter()
	{
		if (onLightningEnter != null)
		{
			onLightningEnter();
		}
	}

	void Lightning_Update()
	{
		if (onLightningUpdate != null)
		{
			onLightningUpdate();
		}

	}

	void Magic_Enter()
	{
		if (onMagicEnter != null)
		{
			onMagicEnter();
		}
	}

	void Magic_Update()
	{
		if (onMagicUpdate != null)
		{
			onMagicUpdate();
		}
	}

	void Avocado_Enter()
	{
		if (onAvocadoEnter != null)
		{
			onAvocadoEnter();
		}
	}

	void Avocado_Update()
	{
		if (onAvocadoUpdate != null)
		{
			onAvocadoUpdate();
		}
	}
}
