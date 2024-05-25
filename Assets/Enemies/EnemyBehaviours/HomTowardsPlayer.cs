using BehaviourTree;
using UnityEngine;

/// <summary>
/// Moves the object towards the target. Does not roate the sprite. 
/// </summary>
internal class HomTowardsPlayer : Node
{
    private EnemyStats stats;
    private Rigidbody2D body;
    private PlayerStats player;
    private float speedFactor;
    private float rotationSpeed;
    private Quaternion currentRotation;

    public HomTowardsPlayer(EnemyStats stats, PlayerStats player, float speedFactor, float rotationSpeed)
    {
        this.stats = stats;
        this.body = stats.GetComponent<Rigidbody2D>();
        currentRotation = stats.transform.rotation;
        this.player = player;
        this.speedFactor = speedFactor;
        this.rotationSpeed = rotationSpeed;
    }

    public override NodeState Evaluate()
    {
        if (stats.IsAsleep())
        {
            state = NodeState.FAILURE;
            return state;
        }

        Vector3 direction = player.transform.position - stats.transform.position;
        float playerAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        currentRotation = Quaternion.RotateTowards(currentRotation, Quaternion.Euler(new Vector3(0, 0, playerAngle)), Time.deltaTime * rotationSpeed);

        body.AddForce(currentRotation * new Vector2(stats.GetSpeed() * speedFactor, 0f) * Time.fixedDeltaTime);
        state = NodeState.SUCCESS;
        return state;
    }
}