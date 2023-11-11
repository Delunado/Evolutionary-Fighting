using System;
using Godot;

namespace Character.BehaviourTree;

public class ActionNode : BehaviourNode
{
    private ICharacterAction _action;

    public ActionNode(ICharacterAction action)
    {
        _action = action;
    }

    public override NodeStatus Evaluate(Character character)
    {
        return _action.Execute(character);
    }
}