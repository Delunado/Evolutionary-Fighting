namespace Character;

public class CharacterSessionData
{
    public Strategy Strategy { get; set; }
    public float HealthNormalized { get; set; }
    public float SuccessfulHitsNormalized { get; set; }
    public float DamageDealtNormalized { get; set; }
}