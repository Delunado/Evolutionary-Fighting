using Character.BehaviourTree;

namespace Character.CharacterBehaviour;

public class PunchAttackAction : ICharacterAction
{
    public BehaviourNode.NodeStatus Execute(Character character)
    {
        return character.AttackPunchAction();
    }
}