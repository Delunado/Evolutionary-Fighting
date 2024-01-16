using Character.BehaviourTree;

namespace Character.CharacterBehaviour;

public class EnemyHighHealthCondition : ICharacterCondition
{
    public bool Check(Character character)
    {
        return character.EnemyIsHighHealth();
    }
}