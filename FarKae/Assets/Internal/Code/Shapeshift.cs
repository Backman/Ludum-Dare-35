using UnityEngine;
using System.Collections;
using MonsterLove.StateMachine;

public class Shapeshift : MonoBehaviour
{
	public enum State
	{
		Candy = 0,
		Lightning,
		Magic,
		Avocado,
		Count
	}

	[SerializeField]
	StateConfig _candyConfig;
	[SerializeField]
	StateConfig _lightningConfig;
	[SerializeField]
	StateConfig _magicConfig;
	[SerializeField]
	StateConfig _avocadoConfig;

	public StateMachine<State> FSM;

	public System.Action onCandyEnter;
	public System.Action onCandyUpdate;
	public System.Action onLightningEnter;
	public System.Action onLightningUpdate;
	public System.Action onMagicEnter;
	public System.Action onMagicUpdate;
	public System.Action onAvocadoEnter;
	public System.Action onAvocadoUpdate;

	public State CurrentState
	{
		get { return FSM.State; }
		set { FSM.ChangeState(value); }
	}

	void Awake()
	{
		FSM = StateMachine<State>.Initialize(this);
		FSM.Changed += StateChanged;
		FSM.ChangeState(State.Candy);
	}

	void StateChanged(State state)
	{
		switch (state)
		{
			case State.Candy:
				GetComponent<SpriteRenderer>().color = _candyConfig.color;
				break;
			case State.Lightning:
				GetComponent<SpriteRenderer>().color = _lightningConfig.color;
				break;
			case State.Magic:
				GetComponent<SpriteRenderer>().color = _magicConfig.color;
				break;
			case State.Avocado:
				GetComponent<SpriteRenderer>().color = _avocadoConfig.color;
				break;
			default:
				break;
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
