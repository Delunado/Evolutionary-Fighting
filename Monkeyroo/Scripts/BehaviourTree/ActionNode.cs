using System;
using Godot;

namespace Character.BehaviourTree;

public class ActionNode : BehaviourNode
{
    private Func<NodeStatus> _action;

    public ActionNode(Func<NodeStatus> action)
    {
        _action = action;
    }

    public override NodeStatus Evaluate()
    {
        return _action();
    }
}