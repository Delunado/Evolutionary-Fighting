using System;
using System.Collections.Generic;

namespace Character;

public class EvolutionMutationStrategy
{
    Random rng;

    private float _mutationRate;
    private float _maxMovementDuration;

    public EvolutionMutationStrategy(float mutationRate, float maxMovementDuration)
    {
        _mutationRate = mutationRate;
        _maxMovementDuration = maxMovementDuration;
        rng = new Random();
    }

    public void Mutation(List<Strategy> strategies, CharacterType characterType)
    {
        foreach (Strategy strategy in strategies)
        {
            foreach (MoveGene moveGene in strategy.MovesSequence)
            {
                if (rng.NextDouble() < _mutationRate)
                {
                    if (rng.NextDouble() < 0.5)
                    {
                        MoveType mutatedMove = (MoveType) rng.Next(Enum.GetValues(typeof(MoveType)).Length);

                        switch (mutatedMove)
                        {
                            case MoveType.Jump:
                            case MoveType.Duck:
                                mutatedMove = characterType == CharacterType.Monkey ? MoveType.Duck : MoveType.Jump;
                                break;

                            case MoveType.AttackKick:
                            case MoveType.AttackHighPunch:
                                mutatedMove = characterType == CharacterType.Monkey
                                    ? MoveType.AttackHighPunch
                                    : MoveType.AttackKick;
                                break;
                        }

                        moveGene.MoveType = mutatedMove;
                    }

                    if (rng.NextDouble() < 0.5 && moveGene.MoveType != MoveType.AttackKick &&
                        moveGene.MoveType != MoveType.AttackHighPunch && moveGene.MoveType != MoveType.AttackPunch)
                    {
                        float mutatedTiming = (float) (rng.NextDouble() * _maxMovementDuration);
                        moveGene.Duration = mutatedTiming;
                    }
                }
            }
        }
    }
}