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

	EntityConfig.PowerStates _powerStates;
	EntityConfig.PowerState _currentPowerState;

	public StateMachine<ShapeshiftState> FSM;

	GameObject _candyEffect;
	GameObject _lightningEffect;
	GameObject _magicEffect;
	GameObject _avocadoEffect;

	Movable _movable;

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

		_currentPowerState = state;
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

		_currentPowerState = powerState;
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
		if (!CheckState(_currentPowerState.hit.clip.name, 2))
		{
			_animator.Play(_currentPowerState.hit.clip.name, 2, 0f);
		}
	}

	public void PlayCurrentBlock()
	{
		//if (!CheckState(_currentPowerState.block.clip.name, 2))
		//{
		//	_animator.Play(_currentPowerState.block.clip.name, 2, 0f);
		//}
	}

	public void PlayCurrentIdle()
	{
		if (!CheckState(_currentPowerState.idle.clip.name, 0))
		{
			_animator.Play(_currentPowerState.idle.clip.name, 0, 0f);
		}
	}

	public void PlayCurrentMove()
	{
		if (!CheckState(_currentPowerState.move.clip.name, 0))
		{
			_animator.Play(_currentPowerState.move.clip.name, 0, 0f);
		}
	}
}
