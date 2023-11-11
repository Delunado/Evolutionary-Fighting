using Character.BehaviourTree;

namespace Character.CharacterBehaviour;

public class DuckAction : ICharacterAction
{
    public BehaviourNode.NodeStatus Execute(Character character)
    {
        if (character is CharacterMonkey monkey)
        {
            return monkey.DuckAction();
        }

        return BehaviourNode.NodeStatus.Failure;
    }
}