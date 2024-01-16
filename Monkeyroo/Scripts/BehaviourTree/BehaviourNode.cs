namespace Character.BehaviourTree;

public abstract class BehaviourNode
{
    public enum NodeStatus
    {
        Success,
        Failure,
        Running
    }
    
    public abstract NodeStatus Evaluate(Character character);
    public abstract BehaviourNode Clone();
}