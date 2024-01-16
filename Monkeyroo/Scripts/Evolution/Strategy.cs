using System.Collections.Generic;
using Character.BehaviourTree;

namespace Character;

public class Strategy
{
    private BehaviourNode _behaviourTreeRoot;
    public BehaviourNode TreeRoot => _behaviourTreeRoot;

    public float Fitness { get; set; }
    
    public Strategy(BehaviourNode root)
    {
        _behaviourTreeRoot = root;
    }

    public BehaviourNode.NodeStatus Execute(Character character)
    {
        return _behaviourTreeRoot.Evaluate(character);
    }
}