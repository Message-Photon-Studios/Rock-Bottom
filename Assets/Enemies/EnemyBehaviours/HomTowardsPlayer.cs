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
    Vector3 randomFollowPoint;
    private float randomFollowDist;
    private Quaternion currentRotation;

    public HomTowardsPlayer(EnemyStats stats, Quaternion startRotation, PlayerStats player, float speedFactor, float rotationSpeed, float randomFollowFactor)
    {
        this.stats = stats;
        this.body = stats.GetComponent<Rigidbody2D>();
        currentRotation = startRotation;
        this.player = player;
        this.speedFactor = speedFactor;
        this.rotationSpeed = rotationSpeed;
        this.randomFollowPoint = new Vector3(Random.Range(-1f,1f), Random.Range(-1f,1f), 0)*randomFollowFactor;
        this.randomFollowDist = Vector3.Magnitude(randomFollowPoint)+0.2f;
    }

    public override NodeState Evaluate()
    {
        if (stats.IsAsleep())
        {
            state = NodeState.FAILURE;
            return state;
        }
        Vector3 direction;
        if(Vector2.Distance(player.transform.position, stats.transform.position) > randomFollowDist)
            direction = (player.transform.position + randomFollowPoint) - stats.transform.position;
        else
            direction = player.transform.position - stats.transform.position;

        float playerAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        currentRotation = Quaternion.RotateTowards(currentRotation, Quaternion.Euler(new Vector3(0, 0, playerAngle)), Time.deltaTime * rotationSpeed);

        body.AddForce(currentRotation * new Vector2(stats.GetSpeed() * speedFactor, 0f) * Time.fixedDeltaTime);
        state = NodeState.SUCCESS;
        return state;
    }
}