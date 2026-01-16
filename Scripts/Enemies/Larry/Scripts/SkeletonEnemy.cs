using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class SkeletonEnemy : EnemyBase
{
    [Header("Datos")]
    public MeleeStats stats; // Al ser MeleeStats hijo de EnemyStats, tiene acceso a todo

    [Header("Audio Clips")]
    public AudioClip footstepSound;
    public AudioClip gruntSound;

    private NavMeshAgent agent;
    private Animator anim;
    private AudioSource audioSource;

    private float patrolTimer;
    private bool isBusy = false;
    
    // Timers Audio
    private float nextFootstepTime;
    private float nextGruntTime;

    protected override void Start()
    {
        base.Start();
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();
        audioSource = GetComponent<AudioSource>();

        nextGruntTime = Time.time + Random.Range(5f, 10f);

        if (stats != null)
        {
            // Inicializamos con int (del padre)
            InitializeStats(stats);
            
            // CAMBIO AQUÍ: Usamos moveSpeed (del padre) en vez de chaseSpeed
            agent.speed = stats.moveSpeed; 
            
            transform.localScale = Vector3.one * stats.scale;
        }
    }

    void Update()
    {
        HandleAudio();

        if (isBusy) return;

        if (anim != null) anim.SetFloat("Speed", agent.velocity.magnitude);

        if (playerTransform != null && playerTransform.gameObject.activeInHierarchy)
        {
            float distance = Vector3.Distance(transform.position, playerTransform.position);

            // 1. ATACAR
            if (distance <= stats.attackRange && CanSeePlayer(stats.attackRange))
            {
                StartCoroutine(PerformChargeAttack());
            }
            // 2. PERSEGUIR
            else if (distance <= stats.detectionRange && CanSeePlayer(stats.detectionRange))
            {
                agent.isStopped = false;
                // CAMBIO AQUÍ: Usamos moveSpeed
                agent.speed = stats.moveSpeed; 
                agent.SetDestination(playerTransform.position);
                patrolTimer = 0;
            }
            // 3. PATRULLAR
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

    void Patrol()
    {
        agent.isStopped = false;
        // La velocidad de patrulla SÍ es específica de MeleeStats, así que se queda igual
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

    void HandleAudio()
    {
        if (audioSource == null || stats == null) return;

        bool isMoving = agent.velocity.sqrMagnitude > 0.1f && !isBusy;

        if (isMoving)
        {
            if (Time.time >= nextFootstepTime)
            {
                if (footstepSound) audioSource.PlayOneShot(footstepSound, 0.5f);
                nextFootstepTime = Time.time + stats.footstepRate;
            }
        }
        else
        {
            nextFootstepTime = 0; 
        }

        if (Time.time >= nextGruntTime && !isBusy)
        {
            if (gruntSound) audioSource.PlayOneShot(gruntSound, 0.7f);
            nextGruntTime = Time.time + Random.Range(5f, 10f);
        }
    }

    protected override void Move() { }

    IEnumerator PerformChargeAttack()
    {
        isBusy = true;
        agent.isStopped = true;
        agent.velocity = Vector3.zero;

        Vector3 targetDir = playerTransform.position - transform.position;
        targetDir.y = 0;
        transform.rotation = Quaternion.LookRotation(targetDir);
        
        if (anim) anim.SetTrigger("Attack");
        yield return new WaitForSeconds(stats.windupTime);

        float speed = (stats.chargeSpeed > 0.1f) ? stats.chargeSpeed : 10f;
        float neededTime = stats.chargeDistance / speed;
        float timer = 0;
        
        if (anim) anim.SetBool("IsCharging", true); 

        while (timer < neededTime)
        {
            agent.Move(transform.forward * speed * Time.deltaTime);
            timer += Time.deltaTime;
            yield return null;
        }

        agent.velocity = Vector3.zero;
        if (anim) anim.SetBool("IsCharging", false);

        float targetTime = stats.recoveryTime / 2.0f;
        if (targetTime < 0.1f) targetTime = 0.5f;

        if (anim)
        {
            anim.SetBool("IsTired", true);
            yield return null;

            AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);
            float animLength = (stateInfo.length > 0) ? stateInfo.length : 1f;

            float mySpeed = (animLength > targetTime) ? (animLength / targetTime) : 1f;
            anim.SetFloat("FallSpeed", mySpeed);
        }
        
        yield return new WaitForSeconds(targetTime);

        if (anim)
        {
            anim.SetTrigger("StandUp");
            yield return null; 

            float safety = 0;
            while (!anim.GetCurrentAnimatorStateInfo(0).IsName("Tired_Up") && safety < 1f)
            {
                yield return null;
                safety += Time.deltaTime;
            }

            AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);
            float animLength = (stateInfo.length > 0) ? stateInfo.length : 1f;

            float mySpeed = (animLength > targetTime) ? (animLength / targetTime) : 1f;
            anim.SetFloat("StandSpeed", mySpeed);
        }

        yield return new WaitForSeconds(targetTime);

        if (anim)
        {
            anim.SetBool("IsTired", false);
            anim.SetFloat("FallSpeed", 1f);
            anim.SetFloat("StandSpeed", 1f);
        }
        
        isBusy = false;
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
            // Ataque
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, stats.attackRange);

            // Visión (Cono)
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

            // Patrulla
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(Application.isPlaying ? startPos : transform.position, stats.patrolRange);
        }
    }
}