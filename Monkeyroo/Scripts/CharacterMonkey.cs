using System;
using Character.BehaviourTree;
using Character.CharacterBehaviour;
using Godot;

namespace Character;

public partial class CharacterMonkey : Character
{
    [Export] private Sprite2D sprite;
    [Export] private Area2D highPunchArea;
    [Export] private Area2D bodyArea;
    [Export] private Node2D hitBoxDuckPosition;

    private bool _isDoingAttack = false;
    private bool _attackFinished = false;

    private bool _isDucking = false;
    private bool _finishedDuck = false;

    public CharacterMonkey()
    {
        ConditionNode nearEnemyCondition = new ConditionNode(new EnemyNearCondition());
        ConditionNode farFromEnemyCondition = new ConditionNode(new EnemyFarCondition());
        ActionNode moveFrontAction = new ActionNode(new MoveFrontAction());
        ActionNode moveBackAction = new ActionNode(new MoveBackAction());
        ActionNode attackPunchAction = new ActionNode(new PunchAttackAction());
        ActionNode attackHighPunch = new ActionNode(new HighPunchAttackAction());
        ActionNode duckAction = new ActionNode(new DuckAction());

        _behavioursPool.Add(nearEnemyCondition);
        _behavioursPool.Add(farFromEnemyCondition);
        _behavioursPool.Add(moveFrontAction);
        _behavioursPool.Add(moveBackAction);
        _behavioursPool.Add(attackPunchAction);
        _behavioursPool.Add(attackHighPunch);
        _behavioursPool.Add(duckAction);
    }

    public override void _Ready()
    {
        InitialConfiguration();

        highPunchArea.Monitoring = false;
        highPunchArea.Visible = false;
        highPunchArea.AreaEntered += (area) =>
        {
            _sucessfulHits++;
            _damageDealt += 10;
            otherCharacter.TakeDamage(10);
        };
    }

    public override void _PhysicsProcess(double delta)
    {
        if (_stopped) return;

        UpdateBehaviour();

        MoveAndSlide();
    }

    protected override bool CanAttack()
    {
        return !_isDucking;
    }

    public BehaviourNode.NodeStatus AttackHighPunchAction()
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

        highPunchArea.Monitoring = true;
        highPunchArea.Visible = true;

        _totalHits++;
        _isDoingAttack = true;

        GetTree().CreateTimer(0.3f).Timeout += () =>
        {
            highPunchArea.Monitoring = false;
            highPunchArea.Visible = false;
            _attackFinished = true;
        };

        return BehaviourNode.NodeStatus.Running;
    }

    public BehaviourNode.NodeStatus DuckAction()
    {
        if (_isDucking) return BehaviourNode.NodeStatus.Running;

        if (_finishedDuck)
        {
            _finishedDuck = false;
            return BehaviourNode.NodeStatus.Success;
        }

        Vector2 velocity = Velocity;
        velocity.X = 0;
        Velocity = velocity;

        bodyArea.Position = hitBoxDuckPosition.Position;
        sprite.Position = hitBoxDuckPosition.Position;

        _isDucking = true;
        _finishedDuck = false;

        GetTree().CreateTimer(0.4f).Timeout += () =>
        {
            sprite.Position = Vector2.Zero;
            bodyArea.Position = Vector2.Zero;

            _isDucking = false;
            _finishedDuck = true;
        };

        return BehaviourNode.NodeStatus.Running;
    }
}