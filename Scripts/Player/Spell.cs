using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class Spell : MonoBehaviour
{
    public SpellData spellData;
    public int timeToLive = 45;
    SpellCollisionDetector spellCollisionDetector;
    SpellCollisionDetector radiusSpellCollisionDetector;
    private Rigidbody rb;
    private GameObject body;
    private GameObject radius = null;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        body = transform.GetChild(0).gameObject;
        spellCollisionDetector = GetComponent<SpellCollisionDetector>();
        spellCollisionDetector.OnCollisionDetected += DirectDamage;
        rb.useGravity = spellData.HasGravity;
        if (spellData.IsRadius)
        {
            radius = transform.GetChild(2).gameObject;
            radiusSpellCollisionDetector = radius.GetComponent<SpellCollisionDetector>();
            radiusSpellCollisionDetector.OnTriggerDetected += RadiusDamage;
            radius.transform.localScale = new Vector3(spellData.Radius, spellData.IsSphere ? spellData.Radius : (0.05f * spellData.Radius/5f), spellData.Radius);
            radius.SetActive(false);
        }
        Destroy(gameObject, timeToLive);
    }

    void OnCollisionEnter(Collision collision)
    {
        rb.isKinematic = true;
        rb.rotation = Quaternion.Euler(0,0,0);
        radius?.SetActive(true);
        body.SetActive(false);
        GetComponent<Collider>().enabled = false;
        if (spellData.IsRadius)
        {
            Destroy(gameObject, spellData.RadiusTime);        
        } else
        {
            Destroy(gameObject);        
        }
    }

    void DirectDamage(IDamageable damageable)
    {
        damageable.TakeDamage(spellData.Damage);
    }
    void RadiusDamage(IDamageable damageable)
    {
        damageable.TakeDamage(spellData.RadiusDamage);
    }
}
