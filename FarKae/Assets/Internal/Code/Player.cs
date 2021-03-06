﻿using UnityEngine;
using System.Collections;
using MonsterLove.StateMachine;
using System.Collections.Generic;

public class Player : Entity
{
	public enum PlayerState
	{
		Normal,
		Hit,
		Attacking,
		Death
	}

	public enum RainbowState
	{
		RainbowReady,
		RainbowEnabled,
		RainbowDisabled
	}

	public enum AttackState
	{
		NoneAttack,
		BasicAttack,
		SuperAttack,
	}

	[SerializeField]
	PlayerConfig _config;

	AudioSource _audioSource;

	int _attackIndex;

	List<Collider2D> _attackedEnemies = new List<Collider2D>();

	PlayerActions _actions;

	StateMachine<PlayerState> _fsm;
	StateMachine<AttackState> _attackFSM;
	StateMachine<RainbowState> _rainbowFSM;
	Shapeshift.ShapeshiftState _shapeshiftWhenAttack;

	ScreenShake _screenShake;

	List<BetterEnemy> _rightSideAttackers = new List<BetterEnemy>();
	List<BetterEnemy> _leftSideAttackers = new List<BetterEnemy>();

	public PlayerState State
	{
		get { return _fsm.State; }
		set { _fsm.ChangeState(value); }
	}

	private float _swingTime;
	private float _attackStaggerTime;
	private float _attackCooldownTimer;
	private float _hitTime;

	[SerializeField]
	BoxCollider2D _leftAttackZone;
	[SerializeField]
	BoxCollider2D _rightAttackZone;
	private Material _originMat;
	private float _rainbowTime;
	private float _currentRainbow;

	public BoxCollider2D ClosestAttackZone(Vector3 position)
	{
		var a = (Vector2)position - ((Vector2)transform.position + _leftAttackZone.offset);
		var b = (Vector2)position - ((Vector2)transform.position + _rightAttackZone.offset);

		return a.sqrMagnitude < b.sqrMagnitude ? _leftAttackZone : _rightAttackZone;
	}

	protected override void Awake()
	{
		base.Awake();
		_originMat = GetComponent<SpriteRenderer>().material;
		_audioSource = GetComponent<AudioSource>();

		_shapeshift.Init(_config.powerStates, _config.powerStates.candyState);

		_config.powerStates.SetupFrameRates();
		_config.basicAttack.Init();
		_config.superAttack.Init();

		_fsm = StateMachine<PlayerState>.Initialize(this, PlayerState.Normal);

		_attackFSM = StateMachine<AttackState>.Initialize(this, AttackState.NoneAttack);
		_rainbowFSM = StateMachine<RainbowState>.Initialize(this, RainbowState.RainbowDisabled);
		_attackFSM.Changed += (state) =>
		{
			_attackCollider.enabled = false;
			_attackedEnemies.Clear();
			if (state != AttackState.NoneAttack)
			{
				_fsm.ChangeState(PlayerState.Attacking);
			}
			else
			{
				_attackIndex = 0;
				_fsm.ChangeState(PlayerState.Normal);
			}
		};
		Stats.Reset();
		_screenShake = Camera.main.GetComponent<ScreenShake>();
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if (_rainbowFSM.State == RainbowState.RainbowEnabled)
		{
			return;
		}

		var rainbowCoin = other.GetComponent<RainbowCoin>();
		if (rainbowCoin)
		{
			Stats.rainbowCoinsPickedUp++;
			Music.PlayClipAtPoint(_config.coinPickupSound, transform.position);
			_currentRainbow += rainbowCoin.rainbowAmount;
			if (_currentRainbow >= _config.maxRainbow)
			{
				_rainbowFSM.ChangeState(RainbowState.RainbowReady);
			}
			if (RainbowMeter.instance != null)
			{
				RainbowMeter.instance.AddRainbow(rainbowCoin.rainbowAmount);
			}
			Destroy(rainbowCoin.gameObject);
		}
	}

	protected override void Start()
	{
		base.Start();
		_shapeshift.FSM.Changed += ShapeshiftStateChanged;
		_shapeshift.FSM.ChangeState(Shapeshift.ShapeshiftState.Candy);

		if (RainbowMeter.instance)
		{
			RainbowMeter.instance.maxRainbow = _config.maxRainbow;
		}
	}

	private void OnEnable()
	{
		_actions = PlayerActions.CreateWithDefaultBindings();
	}

	private void OnDisable()
	{
		_actions.Destroy();
	}

	public override void Damage(float amount)
	{
		if (_rainbowFSM.State == RainbowState.RainbowEnabled)
		{
			return;
		}

		health -= amount;
		if (health > 0f)
		{
			OnDamage(amount);
		}
		else
		{
			health = 0f;
			if (!isDead)
			{
				isDead = true;
				OnDeath();
			}
		}
	}

	protected override void OnDamage(float amount)
	{
		if (_rainbowFSM.State == RainbowState.RainbowEnabled)
		{
			health += amount;
		}
		else
		{
			Music.PlayClipAtPoint(_config.hitSound, transform.position);
			State = PlayerState.Hit;
		}
	}

	protected override void OnDeath()
	{
		State = PlayerState.Death;
		Music.PlayClipAtPoint(_config.deathSound, transform.position);
		StartCoroutine(DeathFlicker());
	}

	void Death_Enter()
	{
		
	}

	IEnumerator DeathFlicker()
	{
		_shapeshift.PlayCurrentDeath();
		var start = Time.time;
		var renderer = GetComponent<SpriteRenderer>();
		renderer.material = _config.rainbowRaveMaterial;
		GameManager.instance.GameOver(_config.deathSound.audioClip.length);
		while (true)
		{
			renderer.enabled = false;
			yield return new WaitForSeconds(_config.deathFlickerInterval);
			renderer.enabled = true;
			yield return new WaitForSeconds(_config.deathFlickerInterval);
		}
	}

	private void ShapeshiftStateChanged(Shapeshift.ShapeshiftState state)
	{
		_screenShake.Shake();
	}

	public void PlayRandomBasicAttackAnimation()
	{
		var state = _shapeshift.CurrentState;
		var rand = Random.Range(0, _config.basicAttack.animations[state].Length);
		_animator.Play(_config.basicAttack.animations[state][rand].clip.name, 1, 0);
	}

	public void PlayRandomSuperAttackAnimation()
	{
		var state = _shapeshift.CurrentState;
		var rand = Random.Range(0, _config.superAttack.animations[state].Length);
		_animator.Play(_config.superAttack.animations[state][rand].clip.name, 1, 0);
	}

	public bool RequestRightSideAttack(BetterEnemy enemy)
	{
		_rightSideAttackers.RemoveAll(e => e == null);
		if (_rightSideAttackers.Count < _config.simultaneousAttackers)
		{
			if (!_rightSideAttackers.Contains(enemy))
			{
				_rightSideAttackers.Add(enemy);
			}
			return true;
		}
		return false;
	}

	public bool RequestLeftSideAttack(BetterEnemy enemy)
	{
		_rightSideAttackers.RemoveAll(e => e == null);
		if (_rightSideAttackers.Count < _config.simultaneousAttackers)
		{
			if (!_rightSideAttackers.Contains(enemy))
			{
				_rightSideAttackers.Add(enemy);
			}
			return true;
		}
		return false;
	}

	public bool RequestAttack(BetterEnemy enemy)
	{
		return RequestRightSideAttack(enemy);
	}

	public void RemoveAttacker(BetterEnemy enemy)
	{
		if (_rightSideAttackers.Contains(enemy))
		{
			_rightSideAttackers.Remove(enemy);
		}
		else if (_leftSideAttackers.Contains(enemy))
		{
			_leftSideAttackers.Remove(enemy);
		}
	}

	void NewShapeShiftState(Shapeshift.ShapeshiftState state)
	{
		if (_shapeshiftWhenAttack != state)
		{
			_shapeshiftWhenAttack = state;
			_attackIndex = 0;
			_shapeshift.FSM.ChangeState(state, StateTransition.Overwrite);
		}
	}

	#region State Functions

	void RainbowReady_Enter()
	{
	}

	void RainbowReady_Update()
	{
		if (_actions.RainbowPower())
		{
			_rainbowFSM.ChangeState(RainbowState.RainbowEnabled);
		}
	}

	void RainbowEnabled_Enter()
	{
		GetComponent<SpriteRenderer>().material = _config.rainbowRaveMaterial;
		Music.PlayClipAtPoint(_config.rainbowSound, transform.position);
		if (RainbowMeter.instance)
		{
			RainbowMeter.instance.StartDecay(_config.rainbowRaveDuration);
		}
		_rainbowTime = Time.time;
	}

	void RainbowEnabled_Update()
	{
		if (_rainbowTime + _config.rainbowRaveDuration < Time.time)
		{
			_rainbowFSM.ChangeState(RainbowState.RainbowDisabled);
			return;
		}

		Stats.secondsRainbow += Time.deltaTime;
	}

	void RainbowDisabled_Enter()
	{
		GetComponent<SpriteRenderer>().material = _originMat;
		_currentRainbow = 0f;
	}

	void RainbowDisabled_Update()
	{
		if (_rainbowTime + _config.rainbowRaveDuration < Time.time)
		{
		}
	}

	void BasicAttack_Enter()
	{
		_swingTime = Time.time;

		_attackIndex++;
		_swingTime = Time.time + _config.attackCooldown;
		_attackCooldownTimer = Time.time;
		PlayRandomBasicAttackAnimation();

		Music.PlayClipAtPoint(_config.basicAttack.punchSound, transform.position);

		_attackedEnemies.Clear();
	}

	void BasicAttack_Update()
	{
		if (_swingTime + _config.attackSwingDuration < Time.time)
		{
			_attackFSM.ChangeState(AttackState.NoneAttack);
			return;
		}

		if (_shapeshiftWhenAttack != _shapeshift.CurrentState)
		{
			_attackIndex = 0;
		}

		if (_attackCooldownTimer + _config.attackCooldown < Time.time
			&& _actions.StateActions[_shapeshift.CurrentState].WasPressed)
		{
			if (++_attackIndex >= _config.basicAttackCount)
			{
				_attackFSM.ChangeState(AttackState.SuperAttack);
			}
			else
			{
				PlayRandomBasicAttackAnimation();
				Music.PlayClipAtPoint(_config.basicAttack.punchSound, transform.position);
				_swingTime = Time.time + _config.attackCooldown;
				_attackCooldownTimer = Time.time;
				_attackedEnemies.Clear();
			}
			return;
		}

		CheckEnemyHit(_config.basicAttack);
	}

	private void SuperAttack_Enter()
	{
		PlayRandomSuperAttackAnimation();

		Music.PlayClipAtPoint(_config.superAttack.punchSound, transform.position);

		_attackStaggerTime = Time.time;
		_attackedEnemies.Clear();
	}

	private void SuperAttack_Update()
	{
		if (_attackStaggerTime + _config.attackStaggerDuration < Time.time)
		{
			_attackFSM.ChangeState(AttackState.NoneAttack);
			_fsm.ChangeState(PlayerState.Normal);
			return;
		}

		if (CheckEnemyHit(_config.superAttack))
		{
			StartCoroutine(_actions.Vibrate(1f, 0.3f));
		}
	}

	private void Hit_Enter()
	{
		BlinkManager.instance.AddBlink(gameObject, Color.white, 0.1f);
		_screenShake.Shake();
		_shapeshift.PlayCurrentHit();
		_hitTime = Time.time;
	}

	private void Hit_Update()
	{
		if (_hitTime + _config.hitStaggerDuration <= Time.time)
		{
			State = PlayerState.Normal;
		}
	}

	private void Hit_Finally()
	{
		_animator.Play("None", _stateLayers.hitLayer, 0f);
	}

	private void Normal_Update()
	{
		if (_stateLayers.CheckState("None", _stateLayers.attackLayer))
		{
			var move = _actions.Move.Value;

			_movable.Move(move);

			if (_movable.isMoving)
			{
				_shapeshift.PlayCurrentMove();
			}
			else
			{
				_shapeshift.PlayCurrentIdle();
			}
		}
		else
		{
			_shapeshift.PlayCurrentIdle();
		}

		var candy = _actions.Candy.WasPressed;
		var lightning = _actions.Lightning.WasPressed;
		var magic = _actions.Magic.WasPressed;
		var avocado = _actions.Avocado.WasPressed;

		if (candy)
		{
			NewShapeShiftState(Shapeshift.ShapeshiftState.Candy);
		}
		else if (lightning)
		{
			NewShapeShiftState(Shapeshift.ShapeshiftState.Lightning);
		}
		else if (magic)
		{
			NewShapeShiftState(Shapeshift.ShapeshiftState.Magic);
		}
		else if (avocado)
		{
			NewShapeShiftState(Shapeshift.ShapeshiftState.Avocado);
		}
	}

	void NoneAttack_Update()
	{
		bool attack = _actions.StateActions[_shapeshift.CurrentState].WasPressed;

		if (attack)
		{
			_attackFSM.ChangeState(AttackState.BasicAttack);
		}
	}

	#endregion

	private bool CheckEnemyHit(PlayerConfig.PlayerAttack attack)
	{
		if (!_attackCollider || !_attackCollider.enabled)
		{
			return false;
		}

		var pointA = _attackCollider.offset;
		pointA.x *= _movable.GetDirection();
		pointA = (pointA + (Vector2)transform.position) - _attackCollider.size / 2f;
		var pointB = _attackCollider.offset;
		pointB.x *= _movable.GetDirection();
		pointB = (pointB + (Vector2)transform.position) + _attackCollider.size / 2f;

		Collider2D[] colliders = new Collider2D[8];

		if (Physics2D.OverlapAreaNonAlloc(pointA, pointB, colliders, (1 << _hitboxColliderLayer.value) | (1 << _attackColliderLayer.value)) <= 0)
		{
			return false;
		}

		bool hit = false;
		for (int i = 0; i < colliders.Length; i++)
		{
			var otherCollider = colliders[i];
			if (otherCollider && otherCollider.gameObject != gameObject)
			{
				var enemy = otherCollider.GetComponentInParent<BetterEnemy>();
				if (!enemy || _attackedEnemies.Contains(otherCollider))
				{
					continue;
				}

				Camera.main.GetComponent<ScreenShake>().Shake();

				hit = true;
				if (_rainbowFSM.State == RainbowState.RainbowEnabled)
				{
					if (Combo.Instance != null)
					{
						Combo.Instance.AddCombo(attack.damage);
					}
					Music.PlayClipAtPoint(attack.hitSound, transform.position);
					enemy.GetComponent<Movable>().Push(_movable.GetDirection(), 1, 0.2f);
					Stats.damageDone += enemy.health;
					enemy.Damage(enemy.health);
				}
				else
				{
					if (_shapeshift.CurrentState == enemy.ShapeshiftState)
					{
						if (Combo.Instance != null)
						{
							Combo.Instance.AddCombo(attack.damage);
						}
						Stats.damageDone += attack.damage;
						enemy.Damage(attack.damage);
						if (attack == _config.superAttack)
						{
							Music.PlayClipAtPoint(attack.hitSound, transform.position);
							enemy.GetComponent<Movable>().Push(_movable.GetDirection(), 1, 0.2f);
						}
						else
						{
							Music.PlayClipAtPoint(attack.hitSound, transform.position);
							enemy.GetComponent<Movable>().Push(_movable.GetDirection(), 0.1f, 0.05f);
						}
					}
					else
					{
						if (_attackedEnemies.Count == 0)
						{
							Music.PlayClipAtPoint(attack.blockSound, transform.position);
						}
						enemy.Block();
					}
				}
				_attackedEnemies.Add(otherCollider);
			}
		}

		if (hit)
		{
			FreezeFrameController.instance.DoFreeze(attack.freezeDuration, attack.freezeValue);
		}
		return hit;
	}
}