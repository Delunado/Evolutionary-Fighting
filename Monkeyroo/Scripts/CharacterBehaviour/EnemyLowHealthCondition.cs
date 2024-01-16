using Character.BehaviourTree;

namespace Character.CharacterBehaviour;

public class EnemyLowHealthCondition : ICharacterCondition
{
    public bool Check(Character character)
    {
        return character.EnemyIsLowHealth();
    }
}