using Character.BehaviourTree;

namespace Character.CharacterBehaviour;

public class EnemyNearCondition : ICharacterCondition
{
    public bool Check(Character character)
    {
        return character.IsEnemyNear();
    }
}