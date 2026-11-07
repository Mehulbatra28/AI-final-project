using UnityEngine;
using System.Collections.Generic;
public class Sequence : Node
{
   protected List<Node> nodes=new List<Node>();
   public Sequence(){}
   public Sequence(List<Node>nodes)
   {
    this.nodes=nodes;
   }
   public override NodeState Evaluate()
   {
    foreach(var node in nodes)
    {
        switch(node.Evaluate())
        {
            case NodeState.RUNNING:
            return NodeState.RUNNING;
            case NodeState.FAILURE:
            return NodeState.FAILURE;
            case NodeState.SUCCESS:
            continue;
        }
    }
    return NodeState.SUCCESS;
   }
}
