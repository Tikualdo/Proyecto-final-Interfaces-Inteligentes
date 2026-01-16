using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class WizardEnemy : EnemyBase
{
    [Header("Configuración")]
    public RangedStats stats;
    public Transform firePoint;

    private NavMeshAgent agent;
    private Animator anim;
    private float nextFireTime;
    
    private float patrolTimer;

    public enum State { Chasing, Attacking, Retreating, Patrolling }
    public State currentState;

    protected override void Start()
    {
        base.Start();
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();

        if (stats != null)
        {
            InitializeStats(stats);
            agent.speed = stats.moveSpeed;
            transform.localScale = Vector3.one * stats.scale;
        }
    }

    void Update()
    {
        if (anim != null) anim.SetFloat("Speed", agent.velocity.magnitude);

        if (playerTransform != null && playerTransform.gameObject.activeInHierarchy)
        {
            float dist = Vector3.Distance(transform.position, playerTransform.position);

            if (dist < stats.retreatDistance && CanSeePlayer(stats.detectionRange))
            {
                HandleRetreat();
            }
            else if (dist <= stats.attackRange * 0.9f && CanSeePlayer(stats.attackRange))
            {
                HandleAttack();
            }
            else if (dist <= stats.detectionRange && CanSeePlayer(stats.detectionRange))
            {
                HandleChasing();
            }
            else
            {
                Patrol();
            }
        }
        else
        {
            Patrol();
        }
    }

    protected override void Move() 
    {
        HandleChasing();
    }

    void Patrol()
    {
        currentState = State.Patrolling;
        agent.isStopped = false;
        agent.updateRotation = true; 
        agent.speed = stats.patrolSpeed;

        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            patrolTimer += Time.deltaTime;

            if (patrolTimer >= stats.patrolWaitTime)
            {
                Vector3 newPoint = GetRandomPoint(stats.patrolRange);
                agent.SetDestination(newPoint);
                patrolTimer = 0;
            }
        }
    }

    void HandleChasing()
    {
        currentState = State.Chasing;
        agent.isStopped = false;
        agent.updateRotation = true;
        agent.speed = stats.moveSpeed;
        agent.SetDestination(playerTransform.position);
        patrolTimer = 0;
    }

    void HandleAttack()
    {
        currentState = State.Attacking;
        agent.isStopped = true;
        LookAtPlayer(10f);

        if (Time.time >= nextFireTime)
        {
            anim.SetTrigger("Attack");
            anim.SetFloat("SpeedMultiplier", stats.attackSpeed);

            float delayBase = 0.5f;
            float delayReal = delayBase / stats.attackSpeed; 

            StartCoroutine(ShootWithDelay(delayReal));

            nextFireTime = Time.time + (1f / stats.attackSpeed);
        }
    }

    void HandleRetreat()
    {
        currentState = State.Retreating;
        agent.isStopped = false;
        agent.updateRotation = false; 
        agent.speed = stats.retreatSpeed;
        
        LookAtPlayer(20f);

        Vector3 dirAway = (transform.position - playerTransform.position).normalized;
        Vector3 retreatDest = transform.position + dirAway * 5f; 
        agent.SetDestination(retreatDest);
    }

    void ShootMagic()
    {
        if (stats.projectilePrefab != null && firePoint != null)
        {
            GameObject spell = Instantiate(stats.projectilePrefab, firePoint.position, firePoint.rotation);
            EnemyProjectile proj = spell.GetComponent<EnemyProjectile>();
            
            if (proj != null) 
            {
                Collider wizardCollider = GetComponent<Collider>();
                proj.Initialize(stats.projectileSpeed, stats.damage, wizardCollider, stats.maxProjectileDistance);
            }
        }
    }

    IEnumerator ShootWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        ShootMagic();
    }

    void LookAtPlayer(float speed)
    {
        Vector3 dir = (playerTransform.position - transform.position).normalized;
        dir.y = 0;
        if (dir != Vector3.zero)
        {
            Quaternion rot = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime * speed);
        }
    }
    
    void OnValidate()
    {
        if (stats != null) transform.localScale = Vector3.one * stats.scale;
    }

    private void OnDrawGizmosSelected()
    {
        if (stats != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, stats.attackRange);

            Gizmos.color = Color.yellow;
            Vector3 eyePos = transform.position + Vector3.up * (1.5f * transform.localScale.y); 

            float halfFOV = 110f / 2f;
            Quaternion leftRayRotation = Quaternion.AngleAxis(-halfFOV, Vector3.up);
            Quaternion rightRayRotation = Quaternion.AngleAxis(halfFOV, Vector3.up);

            Vector3 leftRayDirection = leftRayRotation * transform.forward;
            Vector3 rightRayDirection = rightRayRotation * transform.forward;

            Gizmos.DrawRay(eyePos, leftRayDirection * stats.detectionRange);
            Gizmos.DrawRay(eyePos, rightRayDirection * stats.detectionRange);
            
            Gizmos.DrawLine(eyePos + leftRayDirection * stats.detectionRange, eyePos + transform.forward * stats.detectionRange);
            Gizmos.DrawLine(eyePos + rightRayDirection * stats.detectionRange, eyePos + transform.forward * stats.detectionRange);

            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(Application.isPlaying ? startPos : transform.position, stats.patrolRange);
        }
    }
}