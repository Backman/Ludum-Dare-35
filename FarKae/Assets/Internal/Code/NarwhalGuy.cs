using UnityEngine;
using System.Collections;
using MonsterLove.StateMachine;
using System.Collections.Generic;

public class NarwhalGuy : Enemy
{
	public enum NarwhalState
	{
		Recharge,
		ChangePowerState
	}

	[SerializeField]
	float _rechargeDuration;
	float _rechargeTime;

	Queue<Shapeshift.ShapeshiftState> _availableStates = new Queue<Shapeshift.ShapeshiftState>();

	StateMachine<NarwhalState> _narwhalFSM;

	protected override void Awake()
	{
		base.Awake();
	}

	protected override void Start()
	{
		base.Start();
		_narwhalFSM = StateMachine<NarwhalState>.Initialize(this);
		_narwhalFSM.ChangeState(NarwhalState.ChangePowerState);
	}

	void ChangeState()
	{
		if (_availableStates.Count <= 0)
		{
			FillStateQueue();
		}

		var newState = _availableStates.Dequeue();
		_shapeshift.FSM.ChangeState(newState);
	}

	void FillStateQueue()
	{
		var states = new List<Shapeshift.ShapeshiftState>()
		{
			Shapeshift.ShapeshiftState.Candy,
			Shapeshift.ShapeshiftState.Lightning,
			Shapeshift.ShapeshiftState.Avocado,
			Shapeshift.ShapeshiftState.Magic
		};

		while (states.Count > 0)
		{
			var index = Random.Range(0, states.Count);
			_availableStates.Enqueue(states[index]);
			states.RemoveAt(index);
		}
	}

	void Recharge_Enter()
	{
		_rechargeTime = Time.time;
	}

	void Recharge_Update()
	{
		if (_rechargeTime + _rechargeDuration < Time.time)
		{
			_narwhalFSM.ChangeState(NarwhalState.ChangePowerState);
		}
	}

	void ChangePowerState_Enter()
	{
		ChangeState();
		_narwhalFSM.ChangeState(NarwhalState.Recharge);
	}

	protected override void Hit_Enter()
	{
		_narwhalFSM.ChangeState(NarwhalState.ChangePowerState);
		base.Hit_Enter();
	}

	protected override void Hit_Update()
	{
		base.Hit_Update();
	}

	protected override void Block_Enter()
	{
		base.Block_Enter();
	}

	protected override void Block_Update()
	{
		base.Block_Update();
	}

	protected override void Approach_Enter()
	{
		base.Approach_Enter();
	}

	protected override void Approach_Update()
	{
		base.Approach_Update();
	}

	protected override void Attack_Enter()
	{
		base.Attack_Enter();
	}

	protected override void Attack_Finally()
	{
		base.Attack_Finally();
	}

	protected override void Attack_Update()
	{
		base.Attack_Update();
	}

	protected override void Attacking_Enter()
	{
		base.Attacking_Enter();
	}

	protected override void Attacking_Update()
	{
		base.Attacking_Update();
	}

	protected override void Idle_Enter()
	{
		base.Idle_Enter();
	}

	protected override void Idle_Update()
	{
		base.Idle_Update();
	}
}
