using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using Character.BehaviourTree;
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


    //This creates each generation and evaluates and evolves the population
    public override void _Ready()
    {
        _fitnessCalculus = new EvolutionFitnessCalculus();
        _selectionStrategy = new EvolutionSelectionStrategy(_populationSize, _percentageOfStrategiesToSelect, _tournamentSize);
        _crossoverStrategy = new EvolutionCrossoverStrategy();
        _mutationStrategy = new EvolutionMutationStrategy(_mutationRate, _maxMovementDuration);

        InitializePopulation();
        CreateNewGeneration();
    }

    private void InitializePopulation()
    {
        _kangarooStrategies = new List<Strategy>(_populationSize);
        _monkeyStrategies = new List<Strategy>(_populationSize);

        for (int i = 0; i < _populationSize; i++)
        {
            _kangarooStrategies.Add(CreateRandomStrategy());
            _monkeyStrategies.Add(CreateRandomStrategy());
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

    private Strategy CreateRandomStrategy()
    {
        List<BehaviourNode> behaviourNodesRoot = new List<BehaviourNode>();

        for (int i = 0; i < _randomStrategySequenceLenght; i++)
        {
            //Crear behaviours nuevos cada vez
            //BehaviourNode randomBehaviourNode = behaviourNodes[GD.RandRange(0, behaviourNodes.Count - 1)];
            //behaviourNodesRoot.Add(randomBehaviourNode);
        }

        BehaviourNode root = new SequenceNode(behaviourNodesRoot);

        Strategy strategy = new Strategy(root);

        return strategy;
    }
}