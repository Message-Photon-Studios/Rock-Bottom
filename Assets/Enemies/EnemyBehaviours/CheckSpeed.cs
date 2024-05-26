using BehaviourTree;

internal class CheckSpeed : Node
{
    private EnemyStats stats;
    private int targetSpeed;
    private int margin;

    public CheckSpeed(EnemyStats stats, int targetSpeed, int margin)
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
        if (stats.GetSpeed() == targetSpeed + margin || stats.GetSpeed() == targetSpeed - margin)
        {
            state = NodeState.SUCCESS;
            return state;
        }

        state = NodeState.FAILURE;
        return state;
    }
}