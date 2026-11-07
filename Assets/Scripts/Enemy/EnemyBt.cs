using UnityEngine;
using System.Collections.Generic;
public class EnemyBt : MonoBehaviour
{
    private Blackboard blackboard;
    private Node rootNode;

    private void Awake()
    {
        blackboard=GetComponent<Blackboard>();
    }
    private void Start()
    {
        BuildTree();
    }
    private void Update()
    {
        if (rootNode != null)
        {
            rootNode.Evaluate();
        }
    }
    private void BuildTree()
    {
        PatrolNode patrolNode=new PatrolNode(blackboard);

        rootNode=new Selector(new List<Node>
        {
            patrolNode
        });
    }
}
