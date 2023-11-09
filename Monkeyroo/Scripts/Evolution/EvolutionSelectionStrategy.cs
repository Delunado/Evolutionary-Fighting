using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Character;

public class EvolutionSelectionStrategy
{
    Random rng;

    int _populationSize;
    int _numberOfStrategiesToSelect;
    int _tournamentSize;

    public EvolutionSelectionStrategy(int populationSize, float numberOfStrategiesToSelect, int tournamentSize)
    {
        _populationSize = populationSize;

        _numberOfStrategiesToSelect = Mathf.RoundToInt(_populationSize * numberOfStrategiesToSelect);
        _tournamentSize = tournamentSize;

        rng = new Random();
    }

    public List<Strategy> Selection(List<Strategy> selectFromStrategies)
    {
        List<Strategy> selectedStrategies = new List<Strategy>(_numberOfStrategiesToSelect);

        selectFromStrategies.Sort((a, b) => b.Fitness.CompareTo(a.Fitness));

        selectedStrategies.AddRange(selectFromStrategies.Take(_numberOfStrategiesToSelect));

        return selectedStrategies;
    }

    public List<Strategy> TournamentSelection(List<Strategy> selectFromStrategies)
    {
        List<Strategy> selectedStrategies = new List<Strategy>(_numberOfStrategiesToSelect);

        for (int i = 0; i < _numberOfStrategiesToSelect; i++)
        {
            Strategy selectedStrategy = DoTournament();
            selectedStrategies.Add(selectedStrategy);
        }

        return selectedStrategies;


        // Auxiliary function
        Strategy DoTournament()
        {
            List<Strategy> tournament = new List<Strategy>();

            // Randomly select candidates for the tournament
            for (int i = 0; i < _tournamentSize; i++)
            {
                int randomIndex = rng.Next(selectFromStrategies.Count);
                tournament.Add(selectFromStrategies[randomIndex]);
            }

            // Sort the tournament candidates by fitness and return the best one
            tournament.Sort((a, b) => b.Fitness.CompareTo(a.Fitness));
            return tournament.First();
        }
    }
}