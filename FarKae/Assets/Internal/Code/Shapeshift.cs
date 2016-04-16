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
	[SerializeField]
	StateConfig _candyConfig;
	[SerializeField]
	StateConfig _lightningConfig;
	[SerializeField]
	StateConfig _magicConfig;
	[SerializeField]
	StateConfig _avocadoConfig;

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

	public StateConfig currentStateConfig;
	public string currentBasicAttackName;

	public AnimationClipPlayable[] playables;

	public ShapeshiftState CurrentState
	{
		get { return FSM.State; }
		set { FSM.ChangeState(value); }
	}

	void Awake()
	{
		FSM = StateMachine<ShapeshiftState>.Initialize(this);
		FSM.Changed += StateChanged;
		FSM.ChangeState(ShapeshiftState.Candy);

		CreateStateEffect(_candyConfig.effect, out _candyEffect);
		CreateStateEffect(_lightningConfig.effect, out _lightningEffect);
		CreateStateEffect(_magicConfig.effect, out _magicEffect);
		CreateStateEffect(_avocadoConfig.effect, out _avocadoEffect);

		currentStateConfig = _candyConfig;
		currentBasicAttackName = _candyConfig.basicAttacks[0].clip.name;
		ApplyAnimations(currentStateConfig);
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

	void ChangeState(StateConfig config, GameObject effect)
	{
		PlayStateEffect(effect);
		if (applyColors)
		{
			GetComponent<SpriteRenderer>().color = config.color;
		}
		else
		{
			if (currentStateConfig)
			{
				ApplyAnimations(config);
			}
		}

		currentStateConfig = config;
	}

	void ApplyAnimations(StateConfig stateConfig)
	{
		var animator = GetComponent<Animator>();
		if (!animator)
		{
			return;
		}
		var controller = new AnimatorOverrideController();
		controller.runtimeAnimatorController = animator.runtimeAnimatorController;

		controller[currentStateConfig.idle.clip.name] = stateConfig.idle.clip;
		controller[currentStateConfig.move.clip.name] = stateConfig.move.clip;
		controller[currentStateConfig.superAttack.clip.name] = stateConfig.superAttack.clip;

		animator.runtimeAnimatorController = controller;
	}

	void StateChanged(ShapeshiftState state)
	{
		switch (state)
		{
			case ShapeshiftState.Candy:
				ChangeState(_candyConfig, _candyEffect);
				break;
			case ShapeshiftState.Lightning:
				ChangeState(_lightningConfig, _lightningEffect);
				break;
			case ShapeshiftState.Magic:
				ChangeState(_magicConfig, _magicEffect);
				break;
			case ShapeshiftState.Avocado:
				ChangeState(_avocadoConfig, _avocadoEffect);
				break;
			default:
				break;
		}
	}

	public void SetRandomBasicAttackAnimation()
	{
		var rand = Random.Range(0, currentStateConfig.basicAttacks.Length);

		var animator = GetComponent<Animator>();
		var controller = new AnimatorOverrideController();
		controller.runtimeAnimatorController = animator.runtimeAnimatorController;

		var newClip = currentStateConfig.basicAttacks[rand].clip;

		controller[currentBasicAttackName] = newClip;

		currentBasicAttackName = newClip.name;

		animator.runtimeAnimatorController = controller;
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
