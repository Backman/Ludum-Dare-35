using UnityEngine;
using System.Collections;
using MonsterLove.StateMachine;

public class Enemy : Entity
{
	public enum EnemyState
	{
		Approach,
		Idle,
		Attack,
		Block,
		Hit
	}

	[SerializeField]
	protected EnemyConfig _config;

	protected bool _attackedPlayer;

	protected Player _player;

	protected StateMachine<EnemyState> _fsm;

	protected float _staggerTime;
	protected float _attackTime;

	public EnemyState State
	{
		get { return _fsm.State; }
		set { _fsm.ChangeState(value); }
	}

	public virtual void RandomizePowerState()
	{
		_shapeshift.FSM.ChangeState((Shapeshift.ShapeshiftState)Random.Range(0, (int)Shapeshift.ShapeshiftState.Count));
	}

	protected override void Awake()
	{
		base.Awake();
		_config.powerStates.SetupFrameRates();

		_fsm = StateMachine<EnemyState>.Initialize(this);
		_fsm.ChangeState(EnemyState.Approach);

		_config.basicAttack.Init();

		_shapeshift.Init(_config.powerStates, _config.powerStates.candyState);
	}

	protected override void Start()
	{
		base.Start();
		_player = FindObjectOfType<Player>();
	}

	public override void PlayRandomBasicAttackAnimation()
	{
		var state = _shapeshift.CurrentState;
		var rand = Random.Range(0, _config.basicAttack.animations[state].Length);
		_animator.Play(_config.basicAttack.animations[state][rand].clip.name, 1, 0);
	}

	protected override void OnDamage(float amount)
	{
		_fsm.ChangeState(EnemyState.Hit);
	}

	protected override void OnDeath()
	{
		WaveController.instance.RemoveEnemy(gameObject);
		Destroy(gameObject);
	}

	public void Block(float strength = 1f)
	{
		_fsm.ChangeState(EnemyState.Block);
	}

	protected void CheckPlayerHit()
	{
		if (!_attackCollider || !_attackCollider.enabled)
		{
			return;
		}

		var min = _attackCollider.offset;
		min.x *= _movable.GetDirection();
		min += (Vector2)transform.position;
		var max = min + (Vector2)_attackCollider.bounds.max;

		if (Physics2D.OverlapAreaNonAlloc(min, max, _overlappedHitColliders, (1 << _hitboxColliderLayer.value) | (1 << _attackColliderLayer.value)) <= 0)
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
				var player = otherCollider.GetComponentInParent<Player>();
				if (player && !_attackedPlayer)
				{
					_attackedPlayer = true;
					player.Damage(50f);
				}
			}
		}
	}
}
