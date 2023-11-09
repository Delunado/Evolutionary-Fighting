namespace Character;

public class SessionData
{
    public CharacterSessionData KangarooData { get; set; }
    public CharacterSessionData MonkeyData { get; set; }

    public float CombatDurationNormalized { get; set; }
    public CharacterWinnerType CharacterWinner { get; set; }
}