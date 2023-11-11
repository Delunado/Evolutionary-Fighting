using Godot;
using System;
using System.Collections.Generic;
using Character.BehaviourTree;

namespace Character;

public abstract partial class Character : CharacterBody2D
{
	[Export] private CharacterType _characterType;

	[Export] private int health;
	private int _maxHealth;
	[Export] private ProgressBar _healthBar;

	[Export] private Area2D punchArea;
	protected bool _isDoingAttack = false;
	protected bool _attackFinished = false;

	[Export] protected Character otherCharacter;

	[Export] protected float Speed = 300.0f;

	protected Strategy _strategy;
	protected int _totalHits = 0;
	protected int _sucessfulHits = 0;
	protected int _damageDealt = 0;

	protected bool _stopped = false;

	protected List<BehaviourNode> _behavioursPool = new List<BehaviourNode>();
	public List<BehaviourNode> BehavioursPool => _behavioursPool;

	public event Action Died;

	protected void InitialConfiguration()
	{
		_maxHealth = health;

		_healthBar.MaxValue = health;
		_healthBar.Value = health;

		punchArea.Monitoring = false;
		punchArea.Visible = false;
		punchArea.AreaEntered += (area) =>
		{
			_sucessfulHits++;
			_damageDealt += 15;
			otherCharacter.TakeDamage(15);
		};
	}

	public void InjectStrategy(Strategy strategy)
	{
		_strategy = strategy;
	}

	protected void UpdateBehaviour()
	{
		_strategy.Execute(this);
	}

	public void Stop()
	{
		_stopped = true;
	}

	public void TakeDamage(int damage)
	{
		health -= damage;
		health = Mathf.Max(health, 0);
		_healthBar.Value = health;

		if (health == 0)
		{
			Died?.Invoke();
		}
	}

	public CharacterSessionData GetSessionData()
	{
		CharacterSessionData sessionData = new CharacterSessionData
		{
			Strategy = _strategy,
			HealthNormalized = (float) health / _maxHealth,
			DamageDealtNormalized = (float) _damageDealt / _maxHealth,
			SuccessfulHitsNormalized = _totalHits > 0 ? ((float) _sucessfulHits / _totalHits) : 0.0f
		};

		return sessionData;
	}

	public void MoveFront()
	{
		Vector2 velocity = Velocity;
		velocity.X = Speed;
		Velocity = velocity;
	}

	public void MoveBack()
	{
		Vector2 velocity = Velocity;
		velocity.X = -Speed;
		Velocity = velocity;
	}

	public bool IsEnemyNear()
	{
		return Mathf.Abs(otherCharacter.Position.X - Position.X) < 175.0f;
	}

	public bool IsEnemyFar()
	{
		return Mathf.Abs(otherCharacter.Position.X - Position.X) > 175.0f;
	}

	protected abstract bool CanAttack();

	public BehaviourNode.NodeStatus AttackPunchAction()
	{
		if (!CanAttack()) return BehaviourNode.NodeStatus.Failure;
		if (_isDoingAttack) return BehaviourNode.NodeStatus.Running;

		if (_attackFinished)
		{
			_attackFinished = false;
			_isDoingAttack = false;
			return BehaviourNode.NodeStatus.Success;
		}

		Vector2 velocity = Velocity;
		velocity.X = 0;
		Velocity = velocity;

		punchArea.Monitoring = true;
		punchArea.Visible = true;

		_totalHits++;
		_isDoingAttack = true;

		GetTree().CreateTimer(0.2f).Timeout += () =>
		{
			punchArea.Monitoring = false;
			punchArea.Visible = false;
			_isDoingAttack = false;
			_attackFinished = true;
		};

		return BehaviourNode.NodeStatus.Running;
	}
}
