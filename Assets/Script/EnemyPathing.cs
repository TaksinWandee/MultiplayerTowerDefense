using UnityEngine;
using Unity.Netcode;

public class EnemyPathing : NetworkBehaviour
{
    public Transform[] waypoints;
    public float speed = 2f;
    private int waypointIndex = 0;

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return; // Only the server moves the enemy
    }

    void Update()
    {
        if (!IsServer) return;

        // Move the enemy towards the next waypoint
        if (waypointIndex < waypoints.Length)
        {
            transform.position = Vector2.MoveTowards(transform.position, waypoints[waypointIndex].position, speed * Time.deltaTime);

            // Check if the enemy reached the current waypoint
            if (Vector2.Distance(transform.position, waypoints[waypointIndex].position) < 0.1f)
            {
                waypointIndex++;
            }
        }
        else
        {
            // Enemy has reached the goal
            ReachGoal();
            Destroy(gameObject);
        }
    }

    private void ReachGoal()
    {
        // Call the GameManager to handle the game over process
        GameManager.Instance.EnemyReachedGoal();
    }
}
