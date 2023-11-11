using System;
using Godot;

namespace Character.BehaviourTree;

public class ConditionNode : BehaviourNode
{
    private ICharacterCondition _condition;

    public ConditionNode(ICharacterCondition condition)
    {
        _condition = condition;
    }

    public override NodeStatus Evaluate(Character character)
    {
        return _condition.Check(character) ? NodeStatus.Success : NodeStatus.Failure;
    }
}