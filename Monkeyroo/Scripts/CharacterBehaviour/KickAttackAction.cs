using Character.BehaviourTree;

namespace Character.CharacterBehaviour;

public class KickAttackAction : ICharacterAction
{
    public BehaviourNode.NodeStatus Execute(Character character)
    {
        if (character is CharacterKangaroo kangaroo)
        {
            return kangaroo.AttackKickAction();
        }
        
        return BehaviourNode.NodeStatus.Failure;
    }
}