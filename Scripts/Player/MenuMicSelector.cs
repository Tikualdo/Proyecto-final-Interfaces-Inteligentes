using UnityEngine;
using TMPro; // Asumiendo TextMeshPro para la UI
using System.Collections.Generic;

public class MenuMicSelector : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown _micDropdown;

    private void Start()
    {
        PopulateMicrophoneList();
    }

    private void PopulateMicrophoneList()
    {
        _micDropdown.ClearOptions();
        List<string> options = new List<string>();

        // Acceder a la API nativa de Unity para obtener dispositivos 
        foreach (var device in Microphone.devices)
        {
            options.Add(device);
        }

        if (options.Count == 0)
        {
            options.Add("No Microphone Detected");
            _micDropdown.interactable = false;
        }

        _micDropdown.AddOptions(options);
        
        // Configurar listener para cambios
        _micDropdown.onValueChanged.AddListener(OnMicSelectionChanged);
        
        // Seleccionar el primero por defecto si existe
        if (options.Count > 0)
        {
            OnMicSelectionChanged(0);
        }
    }

    private void OnMicSelectionChanged(int index)
    {
        string selectedMic = _micDropdown.options[index].text;
        
        // Comunicación con el Singleton
        if (GlobalVoiceManager.Instance!= null)
        {
            GlobalVoiceManager.Instance.SetMicrophone(selectedMic);
        }
    }
}