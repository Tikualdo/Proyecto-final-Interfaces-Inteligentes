using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class SlimeEnemy : EnemyBase 
{
    [Header("Datos (Scriptable Object)")]
    public SlimeStats stats;

    public enum SlimeState { Patrolling, Chasing, PreparingAttack, Attacking, Retreating, Cooldown }
    public SlimeState currentState = SlimeState.Patrolling;

    private NavMeshAgent agent;
    private Animator anim;
    private float timer;
    private float patrolTimer; 

    protected override void Start() 
    {
        base.Start(); 
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>(); 
        
        if (stats != null) {
            InitializeStats(stats);
            agent.speed = stats.moveSpeed; 
        }

        if (GetComponent<Rigidbody>() == null) {
            Rigidbody rb = gameObject.AddComponent<Rigidbody>();
            rb.isKinematic = true;
        }
    }

    void Update() 
    {
        if (anim != null) anim.SetFloat("Speed", agent.velocity.magnitude);

        if (playerTransform != null && playerTransform.gameObject.activeInHierarchy)
        {
            float dist = Vector3.Distance(transform.position, playerTransform.position);

            if (currentState != SlimeState.Attacking && currentState != SlimeState.PreparingAttack && currentState != SlimeState.Retreating && currentState != SlimeState.Cooldown)
            {
                if (dist <= stats.attackRange && CanSeePlayer(stats.attackRange))
                {
                    StartPreparing();
                }
                else if (dist <= stats.detectionRange && CanSeePlayer(stats.detectionRange))
                {
                    currentState = SlimeState.Chasing;
                }
                else
                {
                    currentState = SlimeState.Patrolling;
                }
            }
        }
        else
        {
            if (currentState != SlimeState.Cooldown && currentState != SlimeState.Retreating)
            {
                currentState = SlimeState.Patrolling;
            }
        }

        switch (currentState) 
        {
            case SlimeState.Patrolling:
                HandlePatrol();
                break;

            case SlimeState.Chasing:
                Move(); 
                if (playerTransform != null && Vector3.Distance(transform.position, playerTransform.position) > stats.detectionRange * 1.5f)
                {
                    currentState = SlimeState.Patrolling;
                }
                break;

            case SlimeState.PreparingAttack:
                PrepareAttack();
                break;

            case SlimeState.Retreating:
                HandleRetreat();
                break;

            case SlimeState.Cooldown:
                HandleCooldown();
                break;
        }
    }

    void HandlePatrol()
    {
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

    protected override void Move()
    {
        if (agent.isActiveAndEnabled && playerTransform != null) {
            agent.isStopped = false;
            agent.updateRotation = true; 
            agent.speed = stats.moveSpeed; 
            agent.SetDestination(playerTransform.position);
        }
    }

    void StartPreparing()
    {
        currentState = SlimeState.PreparingAttack;
        timer = 0;
        if (agent.enabled) {
            agent.isStopped = true;
            agent.updateRotation = false; 
            agent.velocity = Vector3.zero;
        }
        
        if (anim) anim.SetTrigger("Prepare");
    }

    void PrepareAttack() 
    {
        if (playerTransform != null) LookAtPlayer(10f); 

        timer += Time.deltaTime;
        
        if (timer >= 0.7f) { 
            currentState = SlimeState.Attacking;
            StartCoroutine(JumpAttackRoutine());
        }
    }

    IEnumerator JumpAttackRoutine()
    {
        if (anim) anim.SetTrigger("Attack");

        Vector3 startPos = transform.position;
        agent.enabled = false;
        
        float elapsed = 0;

        while (elapsed < stats.jumpDuration)
        {
            elapsed += Time.deltaTime;
            float percent = elapsed / stats.jumpDuration;

            Vector3 directionToPlayer = (playerTransform.position - startPos).normalized;
            directionToPlayer.y = 0;

            float currentDistToPlayer = Vector3.Distance(new Vector3(startPos.x, 0, startPos.z), new Vector3(playerTransform.position.x, 0, playerTransform.position.z));
            
            float targetDistance = Mathf.Max(0, currentDistToPlayer - stats.stopDistance);
            
            Vector3 dynamicTargetPos = startPos + directionToPlayer * targetDistance;
            dynamicTargetPos.y = startPos.y; 

            Vector3 currentPos = Vector3.Lerp(startPos, dynamicTargetPos, percent);
            
            float arc = Mathf.Sin(percent * Mathf.PI) * stats.jumpHeight;
            transform.position = new Vector3(currentPos.x, currentPos.y + arc, currentPos.z);
            
            yield return null;
        }

        FixPositionOnNavMesh();

        agent.enabled = true;
        currentState = SlimeState.Retreating;
        timer = 0; 
    }
    
    void HandleRetreat()
    {
        agent.updateRotation = false; 
        if (playerTransform != null) LookAtPlayer(15f); 

        Vector3 retreatPoint = transform.position;
        if (playerTransform != null)
        {
            Vector3 dirAway = (transform.position - playerTransform.position).normalized;
            dirAway.y = 0;
            retreatPoint = playerTransform.position + (dirAway * stats.retreatDistance);
        }

        if (agent.isActiveAndEnabled) {
            agent.speed = stats.retreatSpeed; 
            agent.SetDestination(retreatPoint);
        }

        float distanceToTarget = Vector3.Distance(transform.position, retreatPoint);
        timer += Time.deltaTime;

        if (distanceToTarget < 0.5f || timer > 2.0f) {
            currentState = SlimeState.Cooldown;
            timer = 0;
        }
    }

    void HandleCooldown() 
    {
        if (playerTransform != null) LookAtPlayer(5f); 
        
        timer += Time.deltaTime;
        if (timer >= stats.attackCooldown) currentState = SlimeState.Chasing;
    }

    void LookAtPlayer(float speed)
    {
        if (playerTransform == null) return;
        Vector3 dir = (playerTransform.position - transform.position).normalized;
        dir.y = 0;
        if (dir != Vector3.zero) {
            Quaternion targetRot = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * speed);
        }
    }

    void FixPositionOnNavMesh()
    {
        NavMeshHit hit;
        if (NavMesh.SamplePosition(transform.position, out hit, 2.0f, NavMesh.AllAreas))
            transform.position = hit.position;
    }

    void OnValidate()
    {
        if (stats != null)
        {
            transform.localScale = Vector3.one * stats.scale;
        }
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

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("PlayerCore"))
        {
            // LÓGICA DE DAÑO AL JUGADOR
            // Buscamos el componente del jugador (asumiendo que tiene uno llamado PlayerHealth o similar)
            IDamageable playerDamageable = collision.gameObject.GetComponentInParent<IDamageable>();
            if (playerDamageable != null)
            {
                playerDamageable.TakeDamage(stats.damage); // Usamos el daño INT heredado
            }
            // Si el jugador no usa interfaz pero tiene un método DealDamage(int):
            else 
            {
                // Intento genérico usando SendMessage o buscando el script específico de tu amigo
                // Ejemplo: collision.gameObject.SendMessage("DealDamage", stats.damage, SendMessageOptions.DontRequireReceiver);
            }
        }
    }
}