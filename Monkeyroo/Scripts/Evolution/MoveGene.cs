namespace Character;

public class MoveGene
{
    public MoveType MoveType;
    public float Duration; // Time to wait before executing the next move

    public MoveGene(MoveType moveType, float duration)
    {
        MoveType = moveType;
        Duration = duration;
    }
}