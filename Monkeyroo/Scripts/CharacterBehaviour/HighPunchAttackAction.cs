using Character.BehaviourTree;

namespace Character.CharacterBehaviour;

public class HighPunchAttackAction : ICharacterAction
{
    public BehaviourNode.NodeStatus Execute(Character character)
    {
        if (character is CharacterMonkey monkey)
        {
            return monkey.AttackHighPunchAction();
        }
        
        return BehaviourNode.NodeStatus.Failure;
    }
}