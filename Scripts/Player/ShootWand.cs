using UnityEngine;

public class ShootWand : MonoBehaviour
{
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] Transform spawn;
    float shootFired = -1f;
    void FixedUpdate()
    {
        if (shootFired > 0f)
        {
            shootFired -= Time.fixedDeltaTime;
        }
    }

    public void ShootBullet()
    {
        if (shootFired > 0f)
        {
            return;
        }
        GameObject bullet = Instantiate(bulletPrefab, spawn.position, transform.rotation);
        SpellData spell = bullet.GetComponent<Spell>().spellData;
        bullet.transform.localScale = spell.Scale;
        Rigidbody bulletRB = bullet.GetComponent<Rigidbody>();
        bulletRB.AddForce(bullet.transform.up * spell.SpellSpeed, ForceMode.Impulse);
        shootFired = spell.Cooldown;
    }
}
