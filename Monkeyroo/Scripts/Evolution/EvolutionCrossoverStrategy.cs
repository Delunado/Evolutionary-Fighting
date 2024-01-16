using System;
using System.Collections.Generic;
using System.Diagnostics;
using Character.BehaviourTree;
using Godot;

namespace Character;

public class EvolutionCrossoverStrategy
{
	private Random rng;

	public EvolutionCrossoverStrategy()
	{
		rng = new Random();
	}

	public Strategy Crossover(Strategy parent1, Strategy parent2)
	{
		BehaviourNode tree1 = parent1.TreeRoot.Clone();
		BehaviourNode tree2 = parent2.TreeRoot.Clone();

		// Select random subtree from each tree
		BehaviourNode subtree1 = SelectRandomSubtree(tree1, rng.Next(1, 4));
		BehaviourNode subtree2 = SelectRandomSubtree(tree2, rng.Next(1, 4));

		// Swap the subtrees
		DoSubtreeSwap(tree1, subtree1, subtree2);
		DoSubtreeSwap(tree2, subtree2, subtree1);

		// Return the new strategy
		return new Strategy(rng.NextDouble() < 0.5 ? tree1 : tree2);
	}

	private BehaviourNode SelectRandomSubtree(BehaviourNode root, int targetDepth)
	{
		List<BehaviourNode> nodesAtTargetDepth = new List<BehaviourNode>();

		CollectNodesAtDepth(root, targetDepth, 0, nodesAtTargetDepth);

		if (nodesAtTargetDepth.Count == 0)
		{
			throw new InvalidOperationException("No nodes found at the specified depth.");
		}

		// Randomly select a node from the list, obligating it to be a sequence node
		return nodesAtTargetDepth[rng.Next(0, nodesAtTargetDepth.Count)];
	}

	private void CollectNodesAtDepth(BehaviourNode node, int targetDepth, int currentDepth, List<BehaviourNode> nodeList)
	{
		if (node is not SequenceNode sequenceNode) return;

		if (currentDepth == targetDepth)
		{
			nodeList.Add(node);
			return; // Stop going deeper as we reached the target depth
		}

		// Traverse to the next level
		foreach (BehaviourNode child in sequenceNode.Children)
		{
			CollectNodesAtDepth(child, targetDepth, currentDepth + 1, nodeList);
		}
	}

	private void DoSubtreeSwap(BehaviourNode root, BehaviourNode subtree1, BehaviourNode subtree2)
	{
		if (root is SequenceNode sequenceNode)
		{
			try
			{
				sequenceNode.ReplaceChild(subtree1, subtree2);
			}
			catch (InvalidOperationException ex)
			{
				// Handle the case where the oldSubtree is not found
				// This might involve searching deeper in the tree
			}
		}
		else
		{
			//ASSert

			Debug.Assert(false, "Root node is not a sequence node.");
		}
	}
}
