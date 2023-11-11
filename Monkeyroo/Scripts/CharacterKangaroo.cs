using System;
using Character.BehaviourTree;
using Godot;

namespace Character;

public partial class CharacterKangaroo : Character
{
    [Export] private float JumpVelocity = -400.0f;
    [Export] private Area2D punchArea;
    [Export] private Area2D kickArea;

    private float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();
    private bool _isJumping = false;
    private bool _finishedJump = false;

    private bool _isDoingAttack = false;
    private bool _attackFinished = false;

    public CharacterKangaroo()
    {
        ConditionNode nearEnemyCondition = new ConditionNode(IsNearEnemyCondition);
        ConditionNode farFromEnemyCondition = new ConditionNode(IsFarFromEnemyCondition);
        ActionNode moveFrontAction = new ActionNode(MoveFrontAction);
        ActionNode moveBackAction = new ActionNode(MoveBackAction);
        ActionNode attackPunchAction = new ActionNode(AttackPunchAction);
        ActionNode attackKickAction = new ActionNode(AttackKickAction);
        ActionNode jumpAction = new ActionNode(JumpAction);

        _behavioursPool.Add(nearEnemyCondition);
        _behavioursPool.Add(farFromEnemyCondition);
        _behavioursPool.Add(moveFrontAction);
        _behavioursPool.Add(moveBackAction);
        _behavioursPool.Add(attackPunchAction);
        _behavioursPool.Add(attackKickAction);
        _behavioursPool.Add(jumpAction);
    }

    public override void _Ready()
    {
        InitialConfiguration();

        //Disable the punch area
        punchArea.Monitoring = false;
        punchArea.Visible = false;
        punchArea.AreaEntered += (area) =>
        {
            _sucessfulHits++;
            _damageDealt += 15;
            otherCharacter.TakeDamage(15);
        };

        kickArea.Monitoring = false;
        kickArea.Visible = false;
        kickArea.AreaEntered += (area) =>
        {
            _sucessfulHits++;
            _damageDealt += 10;
            otherCharacter.TakeDamage(10);
        };
    }

    private BehaviourNode.NodeStatus AttackPunchAction()
    {
        if (_isJumping) return BehaviourNode.NodeStatus.Failure;
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

    private BehaviourNode.NodeStatus AttackKickAction()
    {
        if (_isJumping) return BehaviourNode.NodeStatus.Failure;
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

        kickArea.Monitoring = true;
        kickArea.Visible = true;

        _totalHits++;
        _isDoingAttack = true;

        GetTree().CreateTimer(0.3f).Timeout += () =>
        {
            kickArea.Monitoring = false;
            kickArea.Visible = false;
            _isDoingAttack = false;
            _attackFinished = true;
        };

        return BehaviourNode.NodeStatus.Running;
    }

    private BehaviourNode.NodeStatus JumpAction()
    {
        if (_isJumping) return BehaviourNode.NodeStatus.Running;

        if (_finishedJump)
        {
            _finishedJump = false;
            return BehaviourNode.NodeStatus.Success;
        }

        Vector2 velocity = Velocity;
        velocity.X = 0;
        velocity.Y = JumpVelocity;
        Velocity = velocity;

        _isJumping = true;
        _finishedJump = false;

        return BehaviourNode.NodeStatus.Running;
    }

    public override void _PhysicsProcess(double delta)
    {
        if (_stopped) return;

        if (_isJumping)
        {
            if (IsOnFloor())
            {
                _isJumping = false;
                _finishedJump = true;
            }
            else
            {
                Vector2 velocity = Velocity;
                velocity.Y += gravity * (float) delta;
                Velocity = velocity;
            }
        }

        UpdateBehaviour();

        MoveAndSlide();
    }
}