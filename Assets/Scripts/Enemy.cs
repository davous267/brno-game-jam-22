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
    private float startPursuitDistance = 8.0f;

    [SerializeField]
    private float attackRadiusDistance = 5.0f;

    [SerializeField]
    private float fireDistance = 4.0f;

    [Header("Shooting settings")]
    [SerializeField]
    private Transform bulletSpawnLocation;

    [SerializeField]
    private GameObject bulletPrefab;

    [SerializeField]
    private float fireInterval = 3.0f;

    private Renderer enemyRenderer;
    private EnemyState state;
    private Player player;
    private Vector3 currentDestination;
    private float lastFireTime;
    float dissolveStartTime;

    private void Start()
    {
        state = EnemyState.WANDERING;
        player = GameManager.Instance.Player;
        currentDestination = transform.position;
        enemyRenderer = GetComponent<Renderer>();
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
            enemyRenderer.material.SetFloat("StartTime", dissolveStartTime);
            dissolveStartTime -= Time.deltaTime;
            if(dissolveStartTime < 0)
            {
                Destroy(gameObject);
            }
            // When the enemy will not be instantly destroyed on hit,
            // we can handle the dying animation/dissolve here ...
        }
    }

    public void TakeHit()
    {
        state = EnemyState.DEAD;
        dissolveStartTime = enemyRenderer.material.GetFloat("StartTime");
        
    }

    private bool CheckStateTransitions()
    {
        // TODO Just temporary based only on distance (not raycast) and random constants

        if (state == EnemyState.WANDERING)
        {
            // TODO Add some "distance to player" function etc.
            if (Vector3.Distance(player.transform.position, transform.position) < startPursuitDistance)
            {
                state = EnemyState.PURSUING;
                return true;
            }
        }
        else if (state == EnemyState.PURSUING)
        {
            if (Vector3.Distance(player.transform.position, transform.position) < attackRadiusDistance)
            {
                state = EnemyState.ATTACKING;
                return true;
            }
            else if (Vector3.Distance(player.transform.position, transform.position) >= startPursuitDistance)
            {
                state = EnemyState.WANDERING;
                return true;
            }
        }
        else if (state == EnemyState.ATTACKING)
        {
            if (Vector3.Distance(player.transform.position, transform.position) >= attackRadiusDistance)
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
                agent.isStopped = false;
                currentDestination = GetRandomNavmeshLocation(16.0f);
                agent.SetDestination(currentDestination);
            }

            Debug.DrawLine(transform.position, currentDestination, Color.green);
        }
        else if (state == EnemyState.PURSUING)
        {
            agent.isStopped = false;
            currentDestination = GetPlayerNavmeshLocation();
            agent.SetDestination(currentDestination);
            Debug.DrawLine(transform.position, currentDestination, Color.yellow);
        }
        else if (state == EnemyState.ATTACKING)
        {
            if (Vector3.Distance(player.transform.position, transform.position) > fireDistance)
            {
                agent.isStopped = false;
                currentDestination = GetPlayerNavmeshLocation();
                agent.SetDestination(currentDestination);
                Debug.DrawLine(transform.position, currentDestination, Color.red);
            }
            else
            {
                agent.isStopped = true;
                bool playerHit = false;

                if (SeesPlayer())
                {
                    TryToFire();
                    playerHit = true;
                }
                
                if(!playerHit)
                {
                    transform.forward = Vector3.RotateTowards(Vector3.ProjectOnPlane(transform.forward, Vector3.up),
                        Vector3.ProjectOnPlane(player.transform.position - transform.position, Vector3.up), Mathf.Deg2Rad * agent.angularSpeed * Time.deltaTime, 0.0f).normalized;
                }
                    
                
            }
        }
    }

    private void TryToFire()
    {
        if(Time.time - lastFireTime >= fireInterval)
        {
            var newBullet = Instantiate(bulletPrefab, bulletSpawnLocation.transform.position, bulletSpawnLocation.transform.rotation);
            newBullet.GetComponent<Bullet>().enemyThatFired = gameObject;
            newBullet.GetComponent<Rigidbody>().AddForce(transform.forward * 15.0f, ForceMode.Impulse);
            

            lastFireTime = Time.time;
        }
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

    private bool SeesPlayer(out float distance)
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

    private bool SeesPlayer()
    {
        return SeesPlayer(out _);
    }
}
