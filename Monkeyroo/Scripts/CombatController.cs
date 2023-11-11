using System;
using System.Collections.Generic;
using Character.BehaviourTree;
using Godot;

namespace Character;

public partial class CombatController : Node2D
{
    [Export] private float _combatDuration = 30;
    [Export] private Timer _combatTimer;

    [Export] private Character _kangarooCharacter;
    [Export] private Character _monkeyCharacter;
    [Export] private Label _timerLabel;
    [Export] private Label _generationLabel;

    public List<BehaviourNode> KangarooBehaviourNodes => _kangarooCharacter.BehavioursPool;
    public List<BehaviourNode> MonkeyBehaviourNodes => _monkeyCharacter.BehavioursPool;

    public event Action<SessionData> CombatEnded;

    public override void _Ready()
    {
        _kangarooCharacter.Died += EndCombat;

        _monkeyCharacter.Died += EndCombat;

        _combatTimer.WaitTime = _combatDuration;
        _combatTimer.Start();
        _combatTimer.Timeout += EndCombat;
    }

    public void Config(Strategy getKangarooStrategy, Strategy getMonkeyStrategy, int currentGeneration)
    {
        _kangarooCharacter.InjectStrategy(getKangarooStrategy);
        _monkeyCharacter.InjectStrategy(getMonkeyStrategy);

        _generationLabel.Text = $"Gen: {currentGeneration}";
    }

    public override void _Process(double delta)
    {
        _timerLabel.Text = _combatTimer.TimeLeft.ToString("0.00");
    }

    private void EndCombat()
    {
        _combatTimer.Stop();

        _kangarooCharacter.Stop();
        _monkeyCharacter.Stop();

        SessionData sessionData = new SessionData
        {
            KangarooData = _kangarooCharacter.GetSessionData(),
            MonkeyData = _monkeyCharacter.GetSessionData(),
            CombatDurationNormalized = (float) (_combatTimer.TimeLeft) / _combatDuration
        };

        //Determine the winner
        if (sessionData.KangarooData.HealthNormalized > sessionData.MonkeyData.HealthNormalized)
        {
            sessionData.CharacterWinner = CharacterWinnerType.Kangaroo;
        }
        else if (sessionData.KangarooData.HealthNormalized < sessionData.MonkeyData.HealthNormalized)
        {
            sessionData.CharacterWinner = CharacterWinnerType.Monkey;
        }
        else
        {
            GD.Print("Health Kangaroo: " + sessionData.KangarooData.HealthNormalized + " Health Monkey: " +
                     sessionData.MonkeyData.HealthNormalized);
            sessionData.CharacterWinner = CharacterWinnerType.Draw;
        }

        CombatEnded?.Invoke(sessionData);
    }
}