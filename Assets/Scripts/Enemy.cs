using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyState
{
    WANDERING,  // Walks freely
    PURSUING,   // Follows the player
    ATTACKING,  // Attacks the player
    DEAD        // Enemy is dead
}

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private NavMeshAgent agent;

    [Header("State settings")]
    [SerializeField]
    [Tooltip("Distance to start pursuit if player visible")]
    private float startPursuitDistance = 8.0f;

    [SerializeField]
    [Tooltip("Distance to always start pursuit, even if player not in fov")]
    private float startPursuitIfNotInFieldOfView = 2.0f;

    [SerializeField]
    private float attackRadiusDistance = 5.0f;

    [SerializeField]
    private float fireDistance = 4.0f;

    [SerializeField]
    private float wanderingFieldOfViewDeg = 45.0f;

    [SerializeField]
    private float pursuingFieldOfViewDeg = 180.0f;

    [Header("Shooting settings")]
    [SerializeField]
    private Transform bulletSpawnLocation;

    [SerializeField]
    private GameObject bulletPrefab;

    [SerializeField]
    private float fireInterval = 3.0f;

    private EnemyState state;
    private Player player;
    private Vector3 currentDestination;
    private float lastFireTime;

    private float FieldOfView => state == EnemyState.PURSUING || state == EnemyState.ATTACKING ? pursuingFieldOfViewDeg : wanderingFieldOfViewDeg;

    private void Start()
    {
        state = EnemyState.WANDERING;
        player = GameManager.Instance.Player;
        currentDestination = transform.position;
    }

    private void Update()
    {
        if (state != EnemyState.DEAD)
        {
            if (CheckStateTransitions())
            {
                Debug.Log("Enemy state changed: " + state.ToString());
            }
            Behave();  
        }
        else
        {
            // When the enemy will not be instantly destroyed on hit,
            // we can handle the dying animation/dissolve here ...
        }
    }

    public void TakeHit()
    {
        state = EnemyState.DEAD;
        Destroy(gameObject);
    }

    private bool CheckStateTransitions()
    {
        float dist;

        if (state == EnemyState.WANDERING)
        {
            if ((SeesPlayer(out dist) && dist <= startPursuitDistance) ||
                (SeesPlayer(out dist, true) && dist <= startPursuitIfNotInFieldOfView))
            {
                state = EnemyState.PURSUING;
                return true;
            }
        }
        else if (state == EnemyState.PURSUING)
        {
            if (SeesPlayer(out dist) && dist <= attackRadiusDistance)
            {
                state = EnemyState.ATTACKING;
                return true;
            }
            else if (!SeesPlayer(out dist) || dist > startPursuitDistance)
            {
                state = EnemyState.WANDERING;
                return true;
            }
        }
        else if (state == EnemyState.ATTACKING)
        {
            if (!SeesPlayer(out dist) || dist > attackRadiusDistance)
            {
                state = EnemyState.PURSUING;
                return true;
            }
        }

        return false;
    }

    private void Behave()
    {
        if (state == EnemyState.WANDERING)
        {
            if (DestinationReached())
            {
                SetAgentDestination(GetRandomNavmeshLocation(16.0f));
            }

            Debug.DrawLine(transform.position, currentDestination, Color.green);
        }
        else if (state == EnemyState.PURSUING)
        {
            SetAgentDestination(GetPlayerNavmeshLocation());
            Debug.DrawLine(transform.position, currentDestination, Color.yellow);
        }
        else if (state == EnemyState.ATTACKING)
        {
            if (Vector3.Distance(player.transform.position, transform.position) > fireDistance)
            {
                SetAgentDestination(GetPlayerNavmeshLocation());
                Debug.DrawLine(transform.position, currentDestination, Color.red);
            }
            else
            {
                agent.isStopped = true;
                bool playerInLineOfSight = IsPlayerInLineOfSight();

                if (!playerInLineOfSight)
                {
                    transform.forward = Vector3.RotateTowards(Vector3.ProjectOnPlane(transform.forward, Vector3.up),
                        Vector3.ProjectOnPlane(player.transform.position - transform.position, Vector3.up),
                        Mathf.Deg2Rad * agent.angularSpeed * Time.deltaTime, 0.0f).normalized;
                }
            }

            if (IsPlayerInLineOfSight())
            {
                TryToFire();
            }
        }
    }

    private void TryToFire()
    {
        if (Time.time - lastFireTime >= fireInterval)
        {
            var newBullet = Instantiate(bulletPrefab, bulletSpawnLocation.transform.position, bulletSpawnLocation.transform.rotation);
            newBullet.GetComponent<Bullet>().enemyThatFired = gameObject;
            newBullet.GetComponent<Rigidbody>().AddForce(transform.forward * 15.0f, ForceMode.Impulse);


            lastFireTime = Time.time;
        }
    }

    private void SetAgentDestination(Vector3 vec)
    {
        agent.isStopped = false;
        currentDestination = vec;
        agent.SetDestination(currentDestination);
    }

    private bool DestinationReached()
    {
        return !agent.pathPending &&
            agent.remainingDistance <= agent.stoppingDistance &&
            (!agent.hasPath ||
            agent.velocity.sqrMagnitude == 0f);
    }

    // Based on http://answers.unity.com/answers/1426690/view.html
    private Vector3 GetRandomNavmeshLocation(float radius)
    {
        Vector3 randomDirection = Random.insideUnitSphere * radius;
        randomDirection += transform.position;

        NavMeshHit hit;
        Vector3 finalPosition = Vector3.zero;
        if (NavMesh.SamplePosition(randomDirection, out hit, radius, 1))
        {
            finalPosition = hit.position;
        }
        return finalPosition;
    }

    private Vector3 GetPlayerNavmeshLocation()
    {
        NavMeshHit hit;
        Vector3 finalPosition = Vector3.zero;
        if (NavMesh.SamplePosition(player.transform.position, out hit, 2.0f, 1))
        {
            finalPosition = hit.position;
        }
        return finalPosition;
    }

    // Returns true if player is directly in line of sight of the gun
    private bool IsPlayerInLineOfSight(out float distance)
    {
        RaycastHit hit;
        if (Physics.Raycast(bulletSpawnLocation.transform.position, bulletSpawnLocation.transform.forward, out hit))
        {
            distance = hit.distance;
            return hit.collider.gameObject.tag == "Player";
        }

        distance = -1.0f;
        return false;
    }

    private bool IsPlayerInLineOfSight()
    {
        return IsPlayerInLineOfSight(out _);
    }

    // Returns true if the player is visible for this agent
    private bool SeesPlayer(out float distance, bool ignoreFov = false)
    {
        RaycastHit hit;

        if ((ignoreFov || (!ignoreFov && Vector3.Angle(
                Vector3.ProjectOnPlane(transform.forward, Vector3.up),
                Vector3.ProjectOnPlane(player.BarrelPosition - bulletSpawnLocation.transform.position, Vector3.up)) <= FieldOfView))
            && Physics.Linecast(bulletSpawnLocation.transform.position, player.transform.position, out hit, ~(1 << LayerMask.NameToLayer("Enemy"))))
        {
            distance = hit.distance;
            return hit.collider.gameObject.CompareTag("Player");
        }

        distance = -1.0f;
        return false;
    }

    private bool SeesPlayer(bool ignoreFov = false)
    {
        return SeesPlayer(out _, ignoreFov);
    }
}
