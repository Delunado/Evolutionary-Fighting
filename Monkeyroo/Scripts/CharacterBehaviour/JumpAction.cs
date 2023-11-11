using Character.BehaviourTree;

namespace Character.CharacterBehaviour;

public class JumpAction : ICharacterAction
{
    public BehaviourNode.NodeStatus Execute(Character character)
    {
        if (character is CharacterKangaroo kangaroo)
        {
            return kangaroo.JumpAction();
        }
        
        return BehaviourNode.NodeStatus.Failure;
    }
}