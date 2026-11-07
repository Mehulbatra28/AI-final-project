using UnityEngine;
using System.Collections.Generic;

public class Selector : Node
{
    protected List<Node> nodes = new List<Node>();
    public Selector()
    {

    }
    public Selector(List<Node> nodes)
    {
        this.nodes = nodes;
    }
    public override NodeState Evaluate()
    {
        foreach(var node in nodes)
        {
            switch(node.Evaluate())
            {
                case NodeState.RUNNING:
                return NodeState.RUNNING;
                case NodeState.SUCCESS:
                return NodeState.SUCCESS;
                case NodeState.FAILURE:
                continue;
            }
        }
        return NodeState.FAILURE;
    }

}
