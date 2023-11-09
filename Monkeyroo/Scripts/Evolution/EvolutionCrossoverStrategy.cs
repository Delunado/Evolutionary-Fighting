using System;
using System.Collections.Generic;

namespace Character;

public class EvolutionCrossoverStrategy
{
    private Random rng;

    public EvolutionCrossoverStrategy()
    {
        rng = new Random();
    }

    public Strategy Crossover(Strategy parent1, Strategy parent2)
    {
        List<MoveGene> moves = new List<MoveGene>();

        int minLength = Math.Min(parent1.MovesSequence.Count, parent2.MovesSequence.Count);
        int maxLength = Math.Max(parent1.MovesSequence.Count, parent2.MovesSequence.Count);

        // Loop through the genes up to the minimum length, randomly selecting from each parent
        for (int i = 0; i < minLength; i++)
        {
            moves.Add(rng.NextDouble() < 0.5 ? parent1.MovesSequence[i] : parent2.MovesSequence[i]);
        }

        // If the parents have different lengths, go through the remaining length and randomly select if enters, and from which parent
        for (int i = minLength; i < maxLength; i++)
        {
            if (rng.NextDouble() < 0.5) continue;

            Strategy chosenParent = rng.NextDouble() < 0.5 ? parent1 : parent2;
            moves.Add(chosenParent.MovesSequence[i % chosenParent.MovesSequence.Count]);
        }

        Strategy offspringStrategy = new Strategy(moves);
        return offspringStrategy;
    }
}