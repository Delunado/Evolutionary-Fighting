using System;

namespace Character;

public class EvolutionFitnessCalculus
{
    public float CalculateFitness(SessionData sessionData, CharacterWinnerType characterWinnerType)
    {
        float healthWeight = 1.2f;
        float winWeight = 1.75f;
        float hitsWeight = 1.3f;
        float damageWeight = 1.2f;
        float survivalWeight = 1.75f;

        float healthFitness = sessionData.KangarooData.HealthNormalized;
        float damageFitness = sessionData.KangarooData.DamageDealtNormalized;
        float successfulHitsFitness = sessionData.KangarooData.SuccessfulHitsNormalized;
        float survivalFitness = sessionData.CombatDurationNormalized;

        float winFitness = sessionData.CharacterWinner == characterWinnerType ? winWeight : 0.0f;

        float fitness = healthFitness * healthWeight
                        + damageFitness * damageWeight
                        + successfulHitsFitness * hitsWeight
                        + survivalFitness * survivalWeight
                        + winFitness;

        float maxPossibleFitness = 1.0f * healthWeight
                                   + 1.0f * damageWeight
                                   + 1.0f * hitsWeight
                                   + 1.0f * survivalWeight
                                   + winWeight;

        float normalizedFitness = fitness / maxPossibleFitness;
        normalizedFitness = Math.Clamp(normalizedFitness, 0.0f, 1.0f);

        return normalizedFitness;
    }
}