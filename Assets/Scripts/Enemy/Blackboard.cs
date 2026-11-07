using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class Blackboard : MonoBehaviour
{
   public List<Transform> waypoints=new List<Transform>();
   public float patrolSpeed=2f;
   public float waitAtwaypoints=2f;
   public string speedFloatParam = "Speed";

   public NavMeshAgent agent;
   public Animator animator;

   public int currentWayPointIndex = 0;
   public float waitTimer = 0f;
   public bool isWaiting = false;

   private void Awake()
   {
    if (agent == null)
    {
        agent = GetComponent<NavMeshAgent>();
    }
    if (animator == null)
    {
        animator = GetComponentInChildren<Animator>();
    }
   }
}
