using Character.BehaviourTree;

namespace Character.CharacterBehaviour;

public class EnemyFarCondition : ICharacterCondition
{
    public bool Check(Character character)
    {
        return character.IsEnemyFar();
    }
}