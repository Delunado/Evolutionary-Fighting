using Character.BehaviourTree;

namespace Character.CharacterBehaviour;

public class MoveBackAction : ICharacterAction
{
    public BehaviourNode.NodeStatus Execute(Character character)
    {
        character.MoveBack();

        return BehaviourNode.NodeStatus.Success;
    }
}