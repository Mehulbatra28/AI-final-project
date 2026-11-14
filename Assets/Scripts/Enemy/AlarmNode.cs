using UnityEngine;

public class AlarmNode : Node
{
    private Blackboard blackboard;
    private bool hasSpawnedReinforcements = false;

    public AlarmNode(Blackboard blackboard)
    {
        this.blackboard = blackboard;
    }
    public override NodeState Evaluate()
    {
        if (!blackboard.isRaisingAlarm)
        {
            return NodeState.FAILURE;
        }
        blackboard.agent.ResetPath();
        blackboard.isWaiting = false;
        blackboard.waitTimer = 0f;

        UpdateAnimation(5f);

        blackboard.alarmCallTimer += Time.deltaTime;
        if (!hasSpawnedReinforcements && blackboard.alarmCallTimer >= 1.5f)
        {
            hasSpawnedReinforcements = true;
            blackboard.SpawnReinforcements();
        }
        if (blackboard.alarmCallTimer >= blackboard.alarmCallDuration)
        {
            blackboard.isRaisingAlarm = false;
            blackboard.alarmCallTimer = 0f;
            hasSpawnedReinforcements = false;
            UpdateAnimation(0f);
            return NodeState.SUCCESS;
        }
        return NodeState.RUNNING;
    }
    private void UpdateAnimation(float speed)
    {
        if(blackboard.animator!=null&&!string.IsNullOrEmpty(blackboard.speedFloatParam))
        {
            blackboard.animator.SetFloat(blackboard.speedFloatParam,speed);
        }    
    }
}
