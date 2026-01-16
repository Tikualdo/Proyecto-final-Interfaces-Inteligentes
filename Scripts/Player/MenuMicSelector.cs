using UnityEngine;
using TMPro; 
using System.Collections.Generic;

public class MenuMicSelector : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown _micDropdown;

    private void Start()
    {
        // Es buena práctica comprobar si el Manager existe antes de intentar nada
        if (GlobalVoiceManager.Instance == null)
        {
            Debug.LogWarning("GlobalVoiceManager no encontrado en la escena. Asegúrate de iniciar desde el Menú.");
            return;
        }

        PopulateMicrophoneList();
    }

    private void PopulateMicrophoneList()
    {
        _micDropdown.ClearOptions();
        List<string> options = new List<string>();

        // Obtenemos los dispositivos
        string[] devices = Microphone.devices;

        foreach (var device in devices)
        {
            options.Add(device);
        }

        if (options.Count == 0)
        {
            options.Add("No Microphone Detected");
            _micDropdown.interactable = false;
        }
        else
        {
            _micDropdown.interactable = true;
        }

        _micDropdown.AddOptions(options);

        // --- CORRECCIÓN CLAVE ---
        // Limpiamos listeners anteriores para evitar duplicados si recargas el menú
        _micDropdown.onValueChanged.RemoveAllListeners(); 
        _micDropdown.onValueChanged.AddListener(OnMicSelectionChanged);

        // Seleccionamos el primero por defecto (o el que ya tenga el Manager seleccionado)
        if (options.Count > 0)
        {
            // Opcional: Podrías buscar cuál tiene seleccionado el Manager y poner ese
            _micDropdown.value = 0; 
            _micDropdown.RefreshShownValue();
            
            // Forzamos la actualización en el Manager
            OnMicSelectionChanged(0); 
        }
    }

    // El evento del Dropdown nos da un 'int index', usémoslo directamente
    private void OnMicSelectionChanged(int index)
    {
        // Verificamos que el índice sea válido (por si acaso la lista está vacía)
        if (Microphone.devices.Length <= index) return;

        // Comunicación con el Singleton
        if (GlobalVoiceManager.Instance != null)
        {
            // AHORA SÍ: Pasamos el 'int' directamente, no el string
            GlobalVoiceManager.Instance.SetMicrophone(index);
        }
    }
}