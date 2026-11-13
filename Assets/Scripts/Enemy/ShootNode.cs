using Unity.VisualScripting;
using UnityEngine;

public class ShootNode : Node
{
    private Blackboard blackboard;
    private bool hasTriggeredLose = false;

    public ShootNode(Blackboard blackboard)
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

        if (!canSee)
        {
            blackboard.shootTimer = 0f;
            return NodeState.FAILURE;

        }
        float distanceToPlayer = Vector3.Distance(blackboard.transform.position, blackboard.player.position);

        if (distanceToPlayer <= blackboard.shootDistanceThreshold)
        {
            blackboard.shootTimer = 0f;
            return NodeState.FAILURE;
        }
        blackboard.LoseSightTimer = blackboard.LoseSightCooldown;

        blackboard.agent.ResetPath();
        blackboard.isWaiting = false;
        blackboard.waitTimer = 0f;
        blackboard.isDistracted = false;
        blackboard.hasReachedDistraction = false;

        Vector3 lookDir = (blackboard.player.position - blackboard.transform.position).normalized;
        lookDir.y = 0;
        if (lookDir != Vector3.zero)
        {
            blackboard.transform.rotation = Quaternion.Slerp(
                blackboard.transform.rotation,
                Quaternion.LookRotation(lookDir),
                Time.deltaTime * 10F);
        }
        UpdateAnimation(4f);
        blackboard.shootTimer += Time.deltaTime;
        if (!hasTriggeredLose && blackboard.shootTimer >= blackboard.shootDelay)
        {
            hasTriggeredLose = true;
            if (GameUIManager.instance != null)
            {
                GameUIManager.instance.ShowLosescreen();
            }
        }
        return NodeState.RUNNING;
    }
    private bool CanSeePlayer()
    {
        Vector3 guardEye = blackboard.transform.position + Vector3.up * 1.6f;
        Vector3 playerTarget = blackboard.player.position + Vector3.up * 1.0f;
        Vector3 dirToPlayer = playerTarget - guardEye;
        float distance = dirToPlayer.magnitude;

        if (distance > blackboard.viewDistance)
            return false;

        Vector3 forward = blackboard.transform.forward;
        float angle = Vector3.Angle(forward, dirToPlayer.normalized);
        if (angle > blackboard.viewangle / 2f)
            return false;

        if (Physics.Raycast(guardEye, dirToPlayer.normalized, out RaycastHit hit, distance ,blackboard.obstacleMask))
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
            blackboard.animator.SetFloat(blackboard.speedFloatParam, speed); 
        }
    }




}
