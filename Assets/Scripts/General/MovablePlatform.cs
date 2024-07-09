using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovablePlatform : MonoBehaviour
{
    public Transform[] waypoints;
    public float speed = 2f;
    private int currentWaypointIndex = 0;
    private Vector2 moveDistance;

    private void Awake() {       
        if (waypoints == null || waypoints.Length == 0) {
            Debug.LogError("Waypoints array is empty or null");
            return;
        }
        transform.position = waypoints[0].position;
    }

    void FixedUpdate()
    {
        if (waypoints.Length == 0) return;

        Transform targetWaypoint = waypoints[currentWaypointIndex];
        Vector2 newPos = Vector2.MoveTowards(transform.position, targetWaypoint.position, speed * Time.fixedDeltaTime);
        moveDistance = (newPos - (Vector2)transform.position);
        transform.position = newPos;

        if (Vector2.Distance(transform.position, targetWaypoint.position) < 0.1f)
        {
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
        }
    }

    //保证player与平台一起移动
    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            other.transform.position += (Vector3)moveDistance;
        }
    }

    void OnDrawGizmos()
    {
        if (waypoints == null || waypoints.Length < 2) return;

        Gizmos.color = Color.red;
        for (int i = 0; i < waypoints.Length; i++)
        {
            Gizmos.DrawSphere(waypoints[i].position, 0.2f);
            if (i < waypoints.Length - 1)
            {
                Gizmos.DrawLine(waypoints[i].position, waypoints[i + 1].position);
            }
            else
            {
                Gizmos.DrawLine(waypoints[i].position, waypoints[0].position);
            }
        }
    }
}
