using Unity.VisualScripting;
using UnityEngine;

public class InvestigationNode : Node
{
    private Blackboard blackboard;

    public InvestigationNode(Blackboard blackboard)
    {
        this.blackboard=blackboard;
    }
    public override NodeState Evaluate()
    {
        if (!blackboard.isDistracted)
        {
            return NodeState.FAILURE;
        }
        if (!blackboard.hasReachedDistraction)
        {
            blackboard.agent.speed = blackboard.patrolSpeed;
            if (!blackboard.agent.hasPath || blackboard.agent.destination != blackboard.distractionpoint)
            {
                blackboard.agent.SetDestination(blackboard.distractionpoint);
            }
            UpdateAnimation(1f);

            if (!blackboard.agent.pathPending && blackboard.agent.remainingDistance <= blackboard.agent.stoppingDistance + 0.5f)
            {
                blackboard.hasReachedDistraction = true;
                blackboard.InvestigateTimer = 0f;
                blackboard.agent.ResetPath();
                UpdateAnimation(3f);
            }
            return NodeState.RUNNING;
        }

        blackboard.InvestigateTimer += Time.deltaTime;
        UpdateAnimation(3f);
        if (blackboard.InvestigateTimer >= blackboard.InvestigationDuration)
        {
            blackboard.isDistracted = false;
            blackboard.hasReachedDistraction = false;
            blackboard.InvestigateTimer = 0f;
            UpdateAnimation(0f);
            return NodeState.SUCCESS;
        }
        return NodeState.RUNNING;
    }
    public void UpdateAnimation(float speed)
    {
      if( blackboard.animator!=null&&!string.IsNullOrEmpty(blackboard.speedFloatParam))
            {
            blackboard.animator.SetFloat(blackboard.speedFloatParam,speed);
        }
    }

}
