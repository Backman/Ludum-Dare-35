using UnityEngine;
using System.Collections;
using MonsterLove.StateMachine;

public class BasicEnemy : Enemy
{
	protected override void Hit_Enter()
	{
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
