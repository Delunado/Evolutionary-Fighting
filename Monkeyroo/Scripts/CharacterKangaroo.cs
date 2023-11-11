using System;
using Character.BehaviourTree;
using Character.CharacterBehaviour;
using Godot;

namespace Character;

public partial class CharacterKangaroo : Character
{
    [Export] private float JumpVelocity = -400.0f;
    [Export] private Area2D kickArea;

    private float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();
    private bool _isJumping = false;
    private bool _finishedJump = false;

    public CharacterKangaroo()
    {
        //MOVE THIS TO EVOLUTION MANAGER OR STRATEGY MANAGER U OTRO LADO, YA NO ES DEPENDIENTE DEL CHARACTER

        ConditionNode nearEnemyCondition = new ConditionNode(new EnemyNearCondition());
        ConditionNode farFromEnemyCondition = new ConditionNode(new EnemyFarCondition());
        ActionNode moveFrontAction = new ActionNode(new MoveFrontAction());
        ActionNode moveBackAction = new ActionNode(new MoveBackAction());
        ActionNode attackPunchAction = new ActionNode(new PunchAttackAction());
        ActionNode attackKickAction = new ActionNode(new KickAttackAction());
        ActionNode jumpAction = new ActionNode(new JumpAction());

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

        kickArea.Monitoring = false;
        kickArea.Visible = false;
        kickArea.AreaEntered += (area) =>
        {
            _sucessfulHits++;
            _damageDealt += 10;
            otherCharacter.TakeDamage(10);
        };
    }

    public BehaviourNode.NodeStatus AttackKickAction()
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

    public BehaviourNode.NodeStatus JumpAction()
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

    protected override bool CanAttack()
    {
        return !_isJumping;
    }
}