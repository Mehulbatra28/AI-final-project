using UnityEngine;
using UnityEngine.Rendering;
using UnityPipeline.Microsoft.CodeAnalysis.FlowAnalysis;

public class ChaseNode : Node
{
    private Blackboard blackboard;
    private bool hasTriggerLose = false;

    public ChaseNode(Blackboard blackboard)
    {
        this.blackboard = blackboard;
    }
    public override NodeState Evaluate()
    {
        if (blackboard.player == null)
        {
            return NodeState.FAILURE;
        }
        bool canSee = CanSeePlayer();

        if (canSee)
        {
            blackboard.LoseSightTimer = blackboard.LoseSightCooldown;
        }
        else if (blackboard.LoseSightTimer > 0f)
        {
            blackboard.LoseSightTimer -= Time.deltaTime;
        }

        if (!canSee && blackboard.LoseSightTimer <=0f)
        {
            return NodeState.FAILURE;
        }

        blackboard.isWaiting = false;
        blackboard.waitTimer = 0f;
        blackboard.isDistracted = false;
        blackboard.hasReachedDistraction = false;
        blackboard.agent.speed = blackboard.chaseSpeed;
        blackboard.agent.SetDestination(blackboard.player.position);
        blackboard.agent.speed = blackboard.chaseSpeed;
        blackboard.agent.SetDestination(blackboard.player.position);

        UpdateAnimation(2f);

        float distanceToPlayer = Vector3.Distance(blackboard.transform.position, blackboard.player.position);
        if (!hasTriggerLose && distanceToPlayer <= blackboard.catchDistance)
        {
            hasTriggerLose = true;
            GameUIManager.instance.ShowLosescreen();
        }
        return NodeState.RUNNING;
    }
    private bool CanSeePlayer()
    {
        Vector3 guardEye = blackboard.transform.position +Vector3.up * 1.6f;
        Vector3 playerTarget = blackboard.player.position + Vector3.up * 1.0f;
        Vector3 dirToPlayer = playerTarget - guardEye;
        float distance = dirToPlayer.magnitude;

        if (distance > blackboard.viewDistance)
            return false;

        Vector3 forward = blackboard.transform.forward;
        float angle = Vector3.Angle(forward, dirToPlayer.normalized);
        if (angle > blackboard.viewangle / 2f)
            return false;

        if (Physics.Raycast(guardEye, dirToPlayer.normalized, out RaycastHit hit, distance, blackboard.obstacleMask))
        {
            if (hit.transform != blackboard.player && !hit.transform.CompareTag("Player"))
            {
                return false;
            }
        }
        return true;
    }
    private void UpdateAnimation(float speed)
    {
        if(blackboard.animator!=null&&!string.IsNullOrEmpty(blackboard.speedFloatParam))
        {
            blackboard.animator.SetFloat(blackboard.speedFloatParam,speed);
        }
    }
}
