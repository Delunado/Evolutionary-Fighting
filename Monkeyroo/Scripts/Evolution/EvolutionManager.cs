using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using Character.BehaviourTree;
using Character.CharacterBehaviour;
using Godot;

namespace Character;

public partial class EvolutionManager : Node2D
{
    [Export] private PackedScene _combatScene;

    [ExportGroup("Evolution Parameters")] [Export]
    private int _populationSize = 100;

    [Export] private int _randomStrategySequenceLenght = 3;

    [Export] private float _maxMovementDuration;

    [ExportSubgroup("Selection")] [Export] private float _percentageOfStrategiesToSelect = 0.2f;
    [Export] private int _tournamentSize = 4;

    [ExportSubgroup("Mutation")] [Export] private float _mutationRate = 0.025f;


    private List<Strategy> _kangarooStrategies = new List<Strategy>();
    private List<Strategy> _monkeyStrategies = new List<Strategy>();
    private int _currentGeneration = 0;

    private List<CombatController> _combatControllers = new List<CombatController>();
    private int _numberOfCombats = 0;

    private EvolutionFitnessCalculus _fitnessCalculus;
    private EvolutionSelectionStrategy _selectionStrategy;
    private EvolutionCrossoverStrategy _crossoverStrategy;
    private EvolutionMutationStrategy _mutationStrategy;

    private List<BehaviourNode> _behavioursPoolKangaroo = new List<BehaviourNode>();
    private List<BehaviourNode> _behavioursPoolMonkey = new List<BehaviourNode>();


    //This creates each generation and evaluates and evolves the population
    public override void _Ready()
    {
        _fitnessCalculus = new EvolutionFitnessCalculus();
        _selectionStrategy = new EvolutionSelectionStrategy(_populationSize, _percentageOfStrategiesToSelect, _tournamentSize);
        _crossoverStrategy = new EvolutionCrossoverStrategy();
        _mutationStrategy = new EvolutionMutationStrategy(_mutationRate, _maxMovementDuration);

        ConditionNode nearEnemyCondition = new ConditionNode(new EnemyNearCondition());
        ConditionNode farFromEnemyCondition = new ConditionNode(new EnemyFarCondition());
        ConditionNode enemyLowHealthCondition = new ConditionNode(new EnemyLowHealthCondition());
        ConditionNode enemyHighHealthCondition = new ConditionNode(new EnemyHighHealthCondition());
        ConditionNode characterLowHealthCondition = new ConditionNode(new CharacterLowHealthCondition());
        ConditionNode characterHighHealthCondition = new ConditionNode(new CharacterHighHealthCondition());

        ActionNode moveFrontAction = new ActionNode(new MoveFrontAction());
        ActionNode moveBackAction = new ActionNode(new MoveBackAction());
        ActionNode attackPunchAction = new ActionNode(new PunchAttackAction());

        ActionNode attackKickAction = new ActionNode(new KickAttackAction());
        ActionNode jumpAction = new ActionNode(new JumpAction());

        ActionNode attackHighPunch = new ActionNode(new HighPunchAttackAction());
        ActionNode duckAction = new ActionNode(new DuckAction());

        _behavioursPoolKangaroo.Add(nearEnemyCondition);
        _behavioursPoolKangaroo.Add(farFromEnemyCondition);
        _behavioursPoolKangaroo.Add(enemyLowHealthCondition);
        _behavioursPoolKangaroo.Add(enemyHighHealthCondition);
        _behavioursPoolKangaroo.Add(characterLowHealthCondition);
        _behavioursPoolKangaroo.Add(characterHighHealthCondition);

        _behavioursPoolKangaroo.Add(moveFrontAction);
        _behavioursPoolKangaroo.Add(moveBackAction);
        _behavioursPoolKangaroo.Add(attackPunchAction);
        _behavioursPoolKangaroo.Add(attackKickAction);
        _behavioursPoolKangaroo.Add(jumpAction);

        _behavioursPoolMonkey.Add(nearEnemyCondition);
        _behavioursPoolMonkey.Add(farFromEnemyCondition);
        _behavioursPoolMonkey.Add(enemyLowHealthCondition);
        _behavioursPoolMonkey.Add(enemyHighHealthCondition);
        _behavioursPoolMonkey.Add(characterLowHealthCondition);
        _behavioursPoolMonkey.Add(characterHighHealthCondition);

        _behavioursPoolMonkey.Add(moveFrontAction);
        _behavioursPoolMonkey.Add(moveBackAction);
        _behavioursPoolMonkey.Add(attackPunchAction);
        _behavioursPoolMonkey.Add(attackHighPunch);
        _behavioursPoolMonkey.Add(duckAction);

        InitializePopulation();
        CreateNewGeneration();
    }

    private void InitializePopulation()
    {
        _kangarooStrategies = new List<Strategy>(_populationSize);
        _monkeyStrategies = new List<Strategy>(_populationSize);

        for (int i = 0; i < _populationSize; i++)
        {
            _kangarooStrategies.Add(CreateRandomStrategy(CharacterType.Kangaroo));
            _monkeyStrategies.Add(CreateRandomStrategy(CharacterType.Monkey));
        }
    }

    private void CreateNewGeneration()
    {
        _numberOfCombats = _populationSize;

        for (int i = 0; i < _populationSize; i++)
        {
            CombatController combatSceneInstance = _combatScene.Instantiate<CombatController>();
            AddChild(combatSceneInstance);

            combatSceneInstance.GlobalPosition = new Vector2(0, 1920 * i);
            combatSceneInstance.Visible = i == 0;

            combatSceneInstance.Config(_kangarooStrategies[i], _monkeyStrategies[i], _currentGeneration + 1);

            combatSceneInstance.CombatEnded += OnCombatEnded;

            _combatControllers.Add(combatSceneInstance);
        }

        _kangarooStrategies.Clear();
        _monkeyStrategies.Clear();
    }

    private void OnCombatEnded(SessionData sessionData)
    {
        GD.Print("Combat ended: " + sessionData.CharacterWinner);

        // Update the fitness of each strategy inside the session data, etc.
        // Then, save the strategy to the list of strategies
        sessionData.KangarooData.Strategy.Fitness = _fitnessCalculus.CalculateFitness(sessionData, CharacterWinnerType.Kangaroo);
        sessionData.MonkeyData.Strategy.Fitness = _fitnessCalculus.CalculateFitness(sessionData, CharacterWinnerType.Monkey);

        _kangarooStrategies.Add(sessionData.KangarooData.Strategy);
        _monkeyStrategies.Add(sessionData.MonkeyData.Strategy);

        // Check if all combats have ended
        _numberOfCombats--;

        CheckEndGeneration();
    }

    private void CheckEndGeneration()
    {
        if (_numberOfCombats != 0) return;

        // Clean up
        _combatControllers.ForEach(combatController => { combatController.QueueFree(); });
        _combatControllers.Clear();

        // Evolution
        EvolvePopulation();

        // New generation
        GD.Print("Generation " + _currentGeneration + " ended");
        _currentGeneration++;

        CreateNewGeneration();
    }

    private void EvolvePopulation()
    {
        List<Strategy> newKangarooStrategies = new List<Strategy>(_populationSize);
        List<Strategy> newMonkeyStrategies = new List<Strategy>(_populationSize);

        //1. Selection
        List<Strategy> selectedKangarooStrategies = _selectionStrategy.TournamentSelection(_kangarooStrategies);
        List<Strategy> selectedMonkeyStrategies = _selectionStrategy.TournamentSelection(_monkeyStrategies);

        //2. Crossover
        for (int i = 0; i < _populationSize; i++)
        {
            Strategy parent1Kangaroo = selectedKangarooStrategies[GD.RandRange(0, selectedKangarooStrategies.Count - 1)];
            Strategy parent2Kangaroo = selectedKangarooStrategies[GD.RandRange(0, selectedKangarooStrategies.Count - 1)];
            Strategy crossoverKangaroo = _crossoverStrategy.Crossover(parent1Kangaroo, parent2Kangaroo);

            Strategy parent1Monkey = selectedMonkeyStrategies[GD.RandRange(0, selectedMonkeyStrategies.Count - 1)];
            Strategy parent2Monkey = selectedMonkeyStrategies[GD.RandRange(0, selectedMonkeyStrategies.Count - 1)];
            Strategy crossoverMonkey = _crossoverStrategy.Crossover(parent1Monkey, parent2Monkey);

            newKangarooStrategies.Add(crossoverKangaroo);
            newMonkeyStrategies.Add(crossoverMonkey);
        }

        //3. Mutation
        _mutationStrategy.Mutation(newKangarooStrategies, CharacterType.Kangaroo);
        _mutationStrategy.Mutation(newMonkeyStrategies, CharacterType.Monkey);

        //4. Set the new population
        _kangarooStrategies = newKangarooStrategies;
        _monkeyStrategies = newMonkeyStrategies;
    }


    // Strategy generation
    private Strategy CreateRandomStrategy(CharacterType characterType)
    {
        return new Strategy(CreateRandomBehaviourTree(characterType, 0));
    }

    private BehaviourNode CreateRandomBehaviourTree(CharacterType characterType, int currentDepth)
    {
        int maxDepth = 4;

        if (currentDepth >= maxDepth)
        {
            return GetRandomLeafNode(characterType);
        }

        List<BehaviourNode> childNodes = new List<BehaviourNode>();

        int numberOfChildren = GD.RandRange(1, 3);

        for (int i = 0; i < numberOfChildren; i++)
        {
            childNodes.Add(CreateRandomBehaviourTree(characterType, currentDepth + 1));
        }

        return new SequenceNode(childNodes);
    }

    private BehaviourNode GetRandomLeafNode(CharacterType characterType)
    {
        // Select a random action or condition node based on character type
        return characterType == CharacterType.Kangaroo
            ? _behavioursPoolKangaroo[GD.RandRange(0, _behavioursPoolKangaroo.Count - 1)]
            : _behavioursPoolMonkey[GD.RandRange(0, _behavioursPoolMonkey.Count - 1)];
    }
}