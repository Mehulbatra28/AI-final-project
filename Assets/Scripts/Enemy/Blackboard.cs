using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using UnityEngine.Rendering;
using UnityPipeline.Microsoft.CodeAnalysis.CSharp.Syntax;
using Unity.AppUI.Editor;

public class Blackboard : MonoBehaviour
{
    public List<Transform> waypoints = new List<Transform>();
    public float patrolSpeed = 2f;
    public float waitAtwaypoints = 2f;
    public string speedFloatParam = "Speed";

    public Transform player;
    public float chaseSpeed = 5f;
    public float viewDistance = 12f;

    public float viewangle = 90f;
    public float catchDistance = 1.5f;
    public LayerMask obstacleMask = ~0;
    public float LoseSightCooldown = 0.1f;
    public float LoseSightTimer = 0f;

    public NavMeshAgent agent;
    public Animator animator;


    public int currentWayPointIndex = 0;
    public float waitTimer = 0f;
    public bool isWaiting = false;


    public bool isDistracted = false;
    public Vector3 distractionpoint;
    public float InvestigationDuration = 3.9f;
    public float InvestigateTimer = 0f;
    public bool hasReachedDistraction = false;

    public float shootDistanceThreshold = 6f;
    public float shootDelay = 1.5f;
    public float shootTimer = 0f;

    public static bool isAlarmActive = false;
    public bool isRaisingAlarm = false;
    public float alarmCallDuration = 10f;
    public float alarmCallTimer = 0f;
    public GameObject guardPrefab;
    public Transform[] GuardSpawnPoints;
    public bool wasChasingPlayer = false;


    private void OnDestroy()
    {
        isAlarmActive = false;
    }

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
        if (player == null)
        {
            GameObject playerObj = GameObject.FindWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
            }
        }
    }
    public void TriggerDistraction(Vector3 position)
    {
        if (NavMesh.SamplePosition(position, out NavMeshHit hit, 5f, NavMesh.AllAreas))
        {
            distractionpoint = hit.position;
        }
        else
        {
            distractionpoint = position;
        }

        isDistracted = true;
        hasReachedDistraction = false;
        InvestigateTimer = 0f;
    }

    public void RaiseAlarm()
    {
        if (isAlarmActive) return;
        isAlarmActive = true;
        isRaisingAlarm = true;
        alarmCallTimer = 0f;
    }

    public void SpawnReinforcements()
    {
        for (int i = 0; i < 2; i++)
        {
            Vector3 spawnPos;
            spawnPos = GuardSpawnPoints[i].position;
            if (NavMesh.SamplePosition(spawnPos, out NavMeshHit hit, 5f, NavMesh.AllAreas))
            {
                spawnPos = hit.position;
            }
            GameObject prefabToSpawn = guardPrefab != null ? guardPrefab : gameObject;
            GameObject newGuard = Instantiate(prefabToSpawn, spawnPos, transform.rotation);

            Blackboard newBb = newGuard.GetComponent<Blackboard>();
            newBb.isWaiting = false;
            newBb.waitTimer = 0f;
            newBb.TriggerDistraction(player.position);
        }
    }
}
