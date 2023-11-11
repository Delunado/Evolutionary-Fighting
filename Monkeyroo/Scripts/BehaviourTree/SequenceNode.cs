using System.Collections.Generic;

namespace Character.BehaviourTree;

public class SequenceNode : BehaviourNode
{
    private List<BehaviourNode> _children;

    public SequenceNode(List<BehaviourNode> children)
    {
        _children = children;
    }

    public override NodeStatus Evaluate(Character character)
    {
        foreach (BehaviourNode child in _children)
        {
            NodeStatus status = child.Evaluate(character);

            if (status != NodeStatus.Success)
            {
                return status;
            }
        }

        return NodeStatus.Success;
    }
}