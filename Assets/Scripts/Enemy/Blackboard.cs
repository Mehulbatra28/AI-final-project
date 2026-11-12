using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using UnityEngine.Rendering;

public class Blackboard : MonoBehaviour
{
    public List<Transform> waypoints = new List<Transform>();
    public float patrolSpeed=2f;
   public float waitAtwaypoints= 2f;
    public string speedFloatParam = "Speed";

    public Transform player;
    public float chaseSpeed = 5f;
    public float viewDistance = 12f;

    public float viewangle = 90f;
    public float catchDistance = 1.5f;
    public LayerMask obstacleMask = ~0;
    public float LoseSightCooldown = 2f;
    public float LoseSightTimer = 0f;

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
        if(player==null)
        {
            GameObject playerObj = GameObject.FindWithTag("Player");
            if(playerObj!=null)
            {
                player = playerObj.transform;
            }
        }
    }
}
