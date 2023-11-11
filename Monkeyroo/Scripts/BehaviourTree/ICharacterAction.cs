namespace Character.BehaviourTree;

public interface ICharacterAction
{
    public BehaviourNode.NodeStatus Execute(Character character);
}