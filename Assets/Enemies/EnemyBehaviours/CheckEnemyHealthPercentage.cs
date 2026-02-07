using BehaviourTree;
public class CheckEnemyHealthPercentage : Node
{
    EnemyStats stats;
    float percentage;
    bool checkAbove;
    int originalHealth;
    public CheckEnemyHealthPercentage(EnemyStats stats, float percentage, bool checkAbove)
    {
        this.stats = stats;
        this.percentage = percentage;
        this.checkAbove = checkAbove;
    }

    public override NodeState Evaluate()
    {
        bool isAbove = (float)stats.GetHealth() / (float)stats.GetMaxHealth() > percentage;
        if (isAbove == checkAbove)
        {
            state = NodeState.SUCCESS;
        }
        else
        {
            state = NodeState.FAILURE;
        }

        return state;
    }
}
