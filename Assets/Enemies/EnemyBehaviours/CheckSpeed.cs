using BehaviourTree;

/// <summary>
/// Checks if the current speed matches the provided target with or without a margin of error. 
/// </summary>
internal class CheckSpeed : Node
{
    private EnemyStats stats;
    private float targetSpeed;
    private float margin;

    public CheckSpeed(EnemyStats stats, float targetSpeed, float margin)
    {
        this.stats = stats;
        this.targetSpeed = targetSpeed;
        this.margin = margin;
    }

    public CheckSpeed(EnemyStats stats, int targetSpeed)
    {
        this.stats = stats;
        this.targetSpeed = targetSpeed;
        this.margin = 0;
    }

    public override NodeState Evaluate()
    {
        if (targetSpeed - margin < stats.GetSpeed() && targetSpeed + margin > stats.GetSpeed())
        {
            state = NodeState.SUCCESS;
            return state;
        }

        state = NodeState.FAILURE;
        return state;
    }
}