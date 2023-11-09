using System;
using Godot;

namespace Character;

public partial class CharacterKangaroo : Character
{
    [Export] private float JumpVelocity = -400.0f;
    [Export] private Area2D punchArea;
    [Export] private Area2D kickArea;

    private float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();
    private bool _isJumping = false;
    private float _lastJumpDuration = 0.0f;

    protected override event Action FinishedMovement;

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

    protected override void PerformMove(MoveType moveType, float duration)
    {
        _canDoNextMovement = false;
        Vector2 velocity = Velocity;
        velocity.X = 0;

        switch (moveType)
        {
            case MoveType.MoveFront:
                velocity.X = Speed;
                Velocity = velocity;

                GetTree().CreateTimer(duration).Timeout += () => { FinishedMovement?.Invoke(); };

                break;

            case MoveType.MoveBack:
                velocity.X = -Speed;
                Velocity = velocity;

                GetTree().CreateTimer(duration).Timeout += () => { FinishedMovement?.Invoke(); };

                break;

            case MoveType.AttackPunch:
                velocity.X = 0;
                Velocity = velocity;

                _totalHits++;
                punchArea.Monitoring = true;
                punchArea.Visible = true;

                GetTree().CreateTimer(0.2f).Timeout += () =>
                {
                    punchArea.Monitoring = false;
                    punchArea.Visible = false;
                    FinishedMovement?.Invoke();
                };

                break;

            case MoveType.AttackKick:
                velocity.X = 0;
                Velocity = velocity;

                _totalHits++;
                kickArea.Monitoring = true;
                kickArea.Visible = true;

                GetTree().CreateTimer(0.3f).Timeout += () =>
                {
                    kickArea.Monitoring = false;
                    kickArea.Visible = false;
                    FinishedMovement?.Invoke();
                };

                break;

            case MoveType.Jump:
                //GD.Print("Jump");

                velocity.Y = JumpVelocity;
                Velocity = velocity;

                _isJumping = true;

                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(moveType), moveType, null);
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        if (_stopped) return;

        MoveAndSlide();

        if (_isJumping)
        {
            if (IsOnFloor())
            {
                _isJumping = false;

                GetTree().CreateTimer(_lastJumpDuration).Timeout += () => { FinishedMovement?.Invoke(); };
            }
            else
            {
                Vector2 velocity = Velocity;
                velocity.Y += gravity * (float) delta;
                Velocity = velocity;
            }
        }

        if (!_canDoNextMovement) return;

        MoveGene moveGene = ChooseMove();
        PerformMove(moveGene.MoveType, moveGene.Duration);
    }
}