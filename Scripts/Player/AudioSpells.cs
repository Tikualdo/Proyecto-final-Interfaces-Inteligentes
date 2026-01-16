using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Whisper.Samples;

[RequireComponent(typeof(PlayerSpellManager))]
public class AudioSpells : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private List<GameObject> prefabs;
    public Transform spawn;
    public Transform cameraTransform;
    [SerializeField] TMP_Text cooldownText;
    [SerializeField] GameObject cooldownObject;
    float spellSummoned = -1f;
    //Regex collect = new Regex(@"(\w*)\s*(collect)", RegexOptions.IgnoreCase);
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        prefabs = GetComponent<PlayerSpellManager>().prefabs;
        // Conexión al sistema global persistente
        if (GlobalVoiceManager.Instance!= null)
        {
            Debug.Log("Jugador conectado al servicio de voz.");
            GlobalVoiceManager.Instance.OnTranscriptionReceived += CheckText;
        }
        else
        {
            Debug.LogError("FATAL: No se encontró GlobalVoiceManager. ¿Iniciaste desde el Menú?");
            // Opcional: Instanciar un prefab de respaldo para debug en editor
        }
    }
    private void OnDestroy()
    {
        // ¡CRÍTICO! Desuscribirse para evitar memory leaks y errores de referencia nula.
        if (GlobalVoiceManager.Instance!= null)
        {
            GlobalVoiceManager.Instance.OnTranscriptionReceived -= CheckText;
        }
    }

    void FixedUpdate()
    {
        if (spellSummoned > 0f)
        {
            spellSummoned -= Time.fixedDeltaTime;
            cooldownText.text = Math.Round(spellSummoned, 1).ToString() + "s";
            if (!cooldownObject.activeSelf) { cooldownObject.SetActive(true); }
            if (spellSummoned <= 0f) { cooldownObject.SetActive(false); }
        }
    }

    void CheckText(string text)
    {
        if (spellSummoned > 0f)
        {
            return;
        }
        foreach (GameObject spell in prefabs)
        {
            SpellData spellData = spell.GetComponent<Spell>().spellData;
            if (spellData.SpellRegex.Match(text).Success)
            {
                SpawnSpell(spell);
                return;
            }
        }
    }

    void SpawnSpell(GameObject bulletPrefab)
    {
        if (spellSummoned > 0f)
        {
            return;
        }
        GameObject bullet = Instantiate(bulletPrefab, spawn.position, cameraTransform.rotation);
        SpellData spell = bullet.GetComponent<Spell>().spellData;
        bullet.transform.localScale = spell.Scale;
        Rigidbody bulletRB = bullet.GetComponent<Rigidbody>();
        bulletRB.AddForce(bullet.transform.forward * spell.SpellSpeed, ForceMode.Impulse);
        spellSummoned = spell.Cooldown;
    }

    public void StartRecord()
    {
        if (GlobalVoiceManager.Instance!= null)
        {
            GlobalVoiceManager.Instance.StartRecord();
        }
    }
    public void StopRecord()
    {
        if (GlobalVoiceManager.Instance!= null)
        {
            GlobalVoiceManager.Instance.StopRecord();
        }
    }
}
