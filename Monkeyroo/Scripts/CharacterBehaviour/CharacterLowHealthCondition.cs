using Character.BehaviourTree;

namespace Character.CharacterBehaviour;

public class CharacterLowHealthCondition : ICharacterCondition
{
    public bool Check(Character character)
    {
        return character.IsLowHealth();
    }
}