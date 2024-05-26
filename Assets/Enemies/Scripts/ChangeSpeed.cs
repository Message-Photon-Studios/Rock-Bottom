using BehaviourTree;

internal class ChangeSpeed : Node
{
    private EnemyStats stats;
    private float targetSpeed;
    private float changeRate;

    public ChangeSpeed(EnemyStats stats, float targetSpeed, float changeRate)
    {
        this.stats = stats;
        this.targetSpeed = targetSpeed;
        this.changeRate = changeRate;
    }

    public ChangeSpeed(EnemyStats stats, float targetSpeed)
    {
        this.stats = stats;
        this.targetSpeed = targetSpeed;
        this.changeRate = 0;
    }

    public override NodeState Evaluate()
    {
        if (stats.IsAsleep())
        {
            state = NodeState.FAILURE;
            return state;
        }

        if (changeRate == 0)
        {
            //= targetSpeed
            state = NodeState.SUCCESS;
            return state;
        }

        if (stats.GetSpeed() > targetSpeed)
        {
            if (stats.GetSpeed() - targetSpeed < changeRate)
            {
                //= targetSpeed;
            } else
            {
                //= stats.GetSpeed() - changeRate;
            }
        } else if (targetSpeed > stats.GetSpeed())
        {
            if (targetSpeed - stats.GetSpeed() < changeRate)
            {
                //= targetSpeed;
            } else
            {
                //= stats.GetSpeed() + changeRate;
            }
        }

        state = NodeState.SUCCESS;
        return state;
    }
}