using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public interface IDamageable {
    void TakeDamage(int amount);
}

public abstract class EnemyBase : MonoBehaviour, IDamageable {
    [Header("Base Settings")]
    private int maxHealth;
    private int currentHealth;
    
    protected float invulnerabilityTime; 
    protected bool isInvulnerable = false;
    
    protected Transform playerTransform;
    protected Vector3 startPos;
    protected Renderer[] renderers;
    protected Color[] originalColors;

    protected virtual void Start() {
        GameObject player = GameObject.FindGameObjectWithTag("PlayerCore");
        if(player != null) playerTransform = player.transform;
        startPos = transform.position;

        renderers = GetComponentsInChildren<Renderer>();
    }

    public void InitializeStats(EnemyStats stats) {
        maxHealth = stats.maxHealth;
        currentHealth = stats.maxHealth;
        
        invulnerabilityTime = stats.invulnerabilityTime;

        if (renderers != null && renderers.Length > 0)
        {
            originalColors = new Color[renderers.Length];
            for (int i = 0; i < renderers.Length; i++)
            {
                if(renderers[i].material.HasProperty("_Color"))
                {
                    renderers[i].material.color = stats.skinColor;
                    originalColors[i] = stats.skinColor; 
                }
            }
        }
    }

    public bool EnemyNotHurt()
    {
        return !isInvulnerable;
    }

    public virtual void TakeDamage(int amount) {
        if (isInvulnerable) return;

        isInvulnerable = true;
        currentHealth -= amount;

        if (currentHealth <= 0) {
            Die();
        } else {
            StartCoroutine(HandleDamageRoutine());
        }
    }

    protected virtual IEnumerator HandleDamageRoutine()
    {

        FlashColor(Color.red);

        yield return new WaitForSeconds(invulnerabilityTime);

        ResetColor();
        isInvulnerable = false;
    }

    protected void FlashColor(Color color)
    {
        if (renderers == null) return;
        foreach (var r in renderers) {
            if (r.material.HasProperty("_Color")) r.material.color = color;
        }
    }

    protected void ResetColor()
    {
        if (renderers == null) return;
        for (int i = 0; i < renderers.Length; i++) {
            if (renderers[i].material.HasProperty("_Color")) 
                renderers[i].material.color = originalColors[i];
        }
    }

    protected virtual void Die() {
        Destroy(gameObject);
    }

    protected abstract void Move();

    protected Vector3 GetRandomPoint(float radius) {
        Vector3 randomDirection = Random.insideUnitSphere * radius;
        randomDirection += startPos;
        NavMeshHit hit;
        NavMesh.SamplePosition(randomDirection, out hit, radius, 1);
        return hit.position;
    }

    protected bool CanSeePlayer(float range)
    {
        if (playerTransform == null) return false;

        float eyeHeight = 1.5f * transform.localScale.y;
        float forwardOffset = 0.65f * transform.localScale.z; 

        Vector3 origin = transform.position + Vector3.up * eyeHeight + transform.forward * forwardOffset; 
        Vector3 target = playerTransform.position + Vector3.up * 1.0f; 
        
        Vector3 direction = (target - origin).normalized;
        float distanceToTarget = Vector3.Distance(origin, target);

        if (distanceToTarget > range) return false;

        //return true;

        RaycastHit hit;
        
        if (Physics.Raycast(origin, direction, out hit, range, layerMask: LayerMask.NameToLayer("IgnoreRaycast")))
        {
            if (hit.collider.CompareTag("PlayerCore"))
            {
                return true;
            }
        }
        return false;
    }
}