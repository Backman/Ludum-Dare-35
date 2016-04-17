using UnityEngine;
using System.Collections;
using MonsterLove.StateMachine;

public class BasicEnemy : Enemy
{
	void Hit_Enter()
	{
		BlinkManager.instance.AddBlink(gameObject, Color.white, 0.1f);
		_shapeshift.PlayCurrentHit();
	}

	void Hit_Update()
	{
	}

	void Block_Enter()
	{
		_shapeshift.PlayCurrentBlock();
	}

	void Block_Update()
	{
	}

	void Approach_Enter()
	{
	}

	void Approach_Update()
	{
		var playerPos = _player.transform.position;
		var dir = playerPos - transform.position;

		var move = Vector2.zero;

		if (dir.x > 0f)
		{
			move.x = 1f;
		}
		else if (dir.x < 0f)
		{
			move.x = -1f;
		}
		if (dir.y > 0f)
		{
			move.y = 1f;
		}
		else if (dir.y < 0f)
		{
			move.y = -1f;
		}

		if (dir.sqrMagnitude <= _config.attackRange)
		{
			_fsm.ChangeState(EnemyState.Attack);
			_movable.Move(Vector2.zero);
			return;
		}

		_movable.Move(move);

		if (_movable.isMoving)
		{
			_shapeshift.PlayCurrentMove();
		}
	}

	void Idle_Enter()
	{
		_fsm.ChangeState(EnemyState.Approach);
	}

	void Idle_Update()
	{
		_shapeshift.PlayCurrentIdle();
	}

	void Attack_Enter()
	{
		_attackTime = Time.time;
		_attackedPlayer = false;
		_shapeshift.PlayCurrentIdle();
	}

	void Attack_Update()
	{
		var playerPos = _player.transform.position;
		var length = (playerPos - transform.position).sqrMagnitude;

		if (_attackTime + _config.attackInterval <= Time.time)
		{
			_attackTime = Time.time;
			_attackedPlayer = false;
			PlayRandomBasicAttackAnimation();
		}

		if (length > _config.attackRange)
		{
			_fsm.ChangeState(EnemyState.Approach);
			return;
		}

		CheckPlayerHit();
	}

	void Attack_Finally()
	{
		_attackedPlayer = false;
	}
}
