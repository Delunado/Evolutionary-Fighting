using Character.BehaviourTree;

namespace Character.CharacterBehaviour;

public class CharacterHighHealthCondition : ICharacterCondition
{
    public bool Check(Character character)
    {
        return character.IsHighHealth();
    }
}