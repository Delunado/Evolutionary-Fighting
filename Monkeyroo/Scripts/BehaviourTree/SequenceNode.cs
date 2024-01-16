using System;
using System.Collections.Generic;

namespace Character.BehaviourTree;

public class SequenceNode : BehaviourNode
{
    private List<BehaviourNode> _children;
    public List<BehaviourNode> Children => _children;

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

    public override BehaviourNode Clone()
    {
        List<BehaviourNode> children = new List<BehaviourNode>();

        foreach (BehaviourNode child in _children)
        {
            children.Add(child.Clone());
        }

        return new SequenceNode(children);
    }

    public void ReplaceChild(BehaviourNode oldChild, BehaviourNode newChild)
    {
        int childIndex = _children.IndexOf(oldChild);

        if (childIndex != -1)
        {
            _children[childIndex] = newChild;
        }
        else
        {
            throw new InvalidOperationException("Child node not found in this sequence.");
        }
    }
}