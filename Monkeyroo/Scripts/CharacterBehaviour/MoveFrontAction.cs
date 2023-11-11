using Character.BehaviourTree;

namespace Character.CharacterBehaviour;

public class MoveFrontAction : ICharacterAction
{
    public BehaviourNode.NodeStatus Execute(Character character)
    {
        character.MoveFront();
        
        return BehaviourNode.NodeStatus.Success;
    }
}