using UnityEngine;

public class SpellCollisionDetector : MonoBehaviour
{
    public delegate void collisionDetected(IDamageable spellDamageable);
    public collisionDetected OnCollisionDetected;
    public collisionDetected OnTriggerDetected;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy")) {
            OnCollisionDetected(collision.gameObject.GetComponent<EnemyBase>());
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && other.gameObject.GetComponent<PlayerManager>().PlayerNotHurt())
        {
            OnTriggerDetected(other.gameObject.GetComponent<PlayerManager>());
        } else if (other.gameObject.CompareTag("Enemy") && other.gameObject.GetComponent<EnemyBase>().EnemyNotHurt()) {
            OnTriggerDetected(other.gameObject.GetComponent<EnemyBase>());
        }
    }
}
