using UnityEngine;

public class PatrolNode : Node
{
    private Blackboard blackboard;

    public PatrolNode(Blackboard blackboard)
    {
        this.blackboard = blackboard;
    }

    public override NodeState Evaluate()
    {
        if (blackboard.waypoints == null || blackboard.waypoints.Count == 0)
        {
            UpdateAnimation(0f);
            return NodeState.FAILURE;
        }

        Transform targetWaypoint = blackboard.waypoints[blackboard.currentWayPointIndex];
        if (targetWaypoint == null)
        {
            UpdateAnimation(0f);
            return NodeState.FAILURE;
        }

        if (blackboard.isWaiting)
        {
            blackboard.waitTimer += Time.deltaTime;
            UpdateAnimation(0f);

            if (blackboard.waitTimer >= blackboard.waitAtwaypoints)
            {
                blackboard.isWaiting = false;
                blackboard.waitTimer = 0f;
                blackboard.currentWayPointIndex = (blackboard.currentWayPointIndex + 1) % blackboard.waypoints.Count;

                if (blackboard.waypoints[blackboard.currentWayPointIndex] != null)
                {
                    blackboard.agent.SetDestination(blackboard.waypoints[blackboard.currentWayPointIndex].position);
                }
            }

            return NodeState.RUNNING;
        }

        blackboard.agent.speed = blackboard.patrolSpeed;

        if (!blackboard.agent.hasPath || blackboard.agent.destination != targetWaypoint.position)
        {
            blackboard.agent.SetDestination(targetWaypoint.position);
        }

        if (!blackboard.agent.pathPending && blackboard.agent.remainingDistance <= blackboard.agent.stoppingDistance)
        {
            blackboard.isWaiting = true;
            blackboard.waitTimer = 0f;
            blackboard.agent.ResetPath();
            UpdateAnimation(0f);
            return NodeState.RUNNING;
        }

        float normalizedSpeed = blackboard.agent.velocity.magnitude / Mathf.Max(blackboard.patrolSpeed, 0.01f);
        UpdateAnimation(Mathf.Clamp01(normalizedSpeed));
        return NodeState.RUNNING;
    }

    private void UpdateAnimation(float speed)
    {
        if (blackboard.animator != null && !string.IsNullOrEmpty(blackboard.speedFloatParam))
        {
            blackboard.animator.SetFloat(blackboard.speedFloatParam, speed);
        }
    }
}
