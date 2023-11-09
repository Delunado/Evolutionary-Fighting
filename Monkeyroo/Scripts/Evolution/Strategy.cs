using System.Collections.Generic;

namespace Character;

public class Strategy
{
    public List<MoveGene> MovesSequence { get; private set; }
    public float Fitness { get; set; }

    
    int _currentMoveIndex = 0;
    
    public Strategy(List<MoveGene> movesSequence)
    {
        MovesSequence = movesSequence;
    }
    
    public MoveGene GetNextMove()
    {
        if (_currentMoveIndex >= MovesSequence.Count)
        {
            _currentMoveIndex = 0;
        }
        
        return MovesSequence[_currentMoveIndex++];
    }
}