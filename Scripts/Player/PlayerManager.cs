using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Haptics;

public class PlayerManager : MonoBehaviour, IDamageable
{
    [Range(0,1)]
    [SerializeField] float hurtIntensity;
    [SerializeField] float hurtDuration;
    [SerializeField] float deathDuration;
    [SerializeField] int maxHP;
    [SerializeField] GameObject inventory;
    [SerializeField] HapticImpulsePlayer leftController;
    [SerializeField] HapticImpulsePlayer rightController;
    [SerializeField] TMP_Text hpText;
    [SerializeField] float damageCooldown = 5f;
    bool playerAlreadyHurt = false;
    bool playerAlreadyHealed = false;
    int actualHP;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        actualHP = maxHP;
        hpText.text = actualHP.ToString();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void TakeDamage(int damage)
    {
        if (damage > 0) {
            if (playerAlreadyHurt)
            {
                return;
            }
            playerAlreadyHurt = true;
            actualHP -= damage;
            hpText.text = actualHP.ToString();
            if (actualHP <= 0)
            {
                StartCoroutine(DeathAnimation());
            } else
            {
                StartCoroutine(PlayerHurt());
            }
        } else if (damage < 0) {
            if (playerAlreadyHealed)
            {
                return;
            }
            playerAlreadyHealed = true;
            actualHP -= damage;
            if (actualHP > maxHP)
            {
                actualHP = maxHP;
            }
            hpText.text = actualHP.ToString();
            StartCoroutine(Heal());
        }
    }

    public bool PlayerNotHurt()
    {
        return !playerAlreadyHurt;
    }
    public bool PlayerNotHealed()
    {
        return !playerAlreadyHurt;
    }

    void TriggerHaptic(float intensity, float duration)
    {
        leftController.SendHapticImpulse(intensity, duration);
        rightController.SendHapticImpulse(intensity, duration);
    }

    public void EnableInventory()
    {
        inventory.SetActive(!inventory.activeSelf);
    }

    IEnumerator DeathAnimation()
    {
        TriggerHaptic(1f, deathDuration);
        yield return new WaitForSeconds(deathDuration + 1);
        SceneManager.LoadScene(0);
    }

    IEnumerator PlayerHurt()
    {
        TriggerHaptic(hurtIntensity, hurtDuration);
        hpText.color = Color.red;
        yield return new WaitForSeconds(.5f);
        hpText.color = Color.black;
        yield return new WaitForSeconds(damageCooldown - .5f);
        playerAlreadyHurt = false;
    }

    IEnumerator Heal()
    {
        yield return new WaitForSeconds(1f);
        playerAlreadyHealed = false;
    }
}
