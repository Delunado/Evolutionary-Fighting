using System;
using Godot;

namespace Character.BehaviourTree;

public class ConditionNode : BehaviourNode
{
    private Func<bool> _condition;

    public ConditionNode(Func<bool> condition)
    {
        _condition = condition;
    }

    public override NodeStatus Evaluate()
    {
        return _condition() ? NodeStatus.Success : NodeStatus.Failure;
    }
}