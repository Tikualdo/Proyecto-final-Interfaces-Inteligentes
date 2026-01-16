using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class EnemyProjectile : MonoBehaviour
{
    private int damage;
    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null) Debug.LogError("¡ERROR! No encuentro el Rigidbody en la bola.");
    }

    public void Initialize(float shootForce, int _damage, Collider shooterCollider, float maxDistance)
    {
        damage = _damage;

        float lifetime = 5f;
        if (shootForce > 0)
        {
            lifetime = maxDistance / shootForce;
        }

        Destroy(gameObject, lifetime); 

        Collider myCollider = GetComponent<Collider>();
        if (myCollider != null && shooterCollider != null)
        {
            Physics.IgnoreCollision(myCollider, shooterCollider);
        }

        if (rb != null)
        {
            rb.isKinematic = false; 
            rb.linearVelocity = Vector3.zero; 
            rb.AddForce(transform.forward * shootForce, ForceMode.Impulse);
        }
    }
    
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("PlayerCore"))
        {
            IDamageable playerDamageable = collision.gameObject.GetComponentInParent<IDamageable>();
            if (playerDamageable != null)
            {
                playerDamageable.TakeDamage(damage); // Usamos el daño INT heredado
            }
            Destroy(gameObject);
        }
        else if (!collision.gameObject.CompareTag("Enemy") && !collision.gameObject.CompareTag("Projectile"))
        {
            Destroy(gameObject);
        }
    }
}