using System;
using Godot;

namespace Character;

public partial class CharacterMonkey : Character
{
	[Export] private Sprite2D sprite;
	[Export] private Area2D punchArea;
	[Export] private Area2D highPunchArea;
	[Export] private Area2D bodyArea;
	[Export] private Node2D hitBoxDuckPosition;

	protected override event Action FinishedMovement;
	
	public override void _Ready()
	{
		InitialConfiguration();
		
		punchArea.Monitoring = false;
		punchArea.Visible = false;
		punchArea.AreaEntered += (area) =>
		{
			_sucessfulHits++;
			_damageDealt += 15;
			otherCharacter.TakeDamage(15);
		};
		
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

		MoveAndSlide();

		if (!_canDoNextMovement) return;

		MoveGene moveGene = ChooseMove();
		PerformMove(moveGene.MoveType, moveGene.Duration);
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

			case MoveType.AttackHighPunch:
				velocity.X = 0;
				Velocity = velocity;

				_totalHits++;
				highPunchArea.Monitoring = true;
				highPunchArea.Visible = true;

				GetTree().CreateTimer(0.3f).Timeout += () =>
				{
					highPunchArea.Monitoring = false;
					highPunchArea.Visible = false;
					FinishedMovement?.Invoke();
				};

				break;

			case MoveType.Duck:
				velocity.X = 0;
				Velocity = velocity;

				bodyArea.Position = hitBoxDuckPosition.Position;
				sprite.Position = hitBoxDuckPosition.Position;

				GetTree().CreateTimer(duration).Timeout += () =>
				{
					sprite.Position = Vector2.Zero;
					bodyArea.Position = Vector2.Zero;
					FinishedMovement?.Invoke();
				};

				break;

			default:
				throw new ArgumentOutOfRangeException(nameof(moveType), moveType, null);
		}
	}
}
