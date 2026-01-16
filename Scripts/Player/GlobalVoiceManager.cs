using UnityEngine;
using Whisper;
using Whisper.Utils;

/// <summary>
/// Gestor centralizado que persiste entre escenas.
/// Mantiene la conexión con el hardware de audio y la instancia de Whisper.
/// </summary>
public class GlobalVoiceManager : MonoBehaviour
{
    // Instancia estática para acceso global (Patrón Singleton)
    public static GlobalVoiceManager Instance { get; private set; }

    [Header("Componentes Internos")]
    private WhisperManager _whisperManager;
    private MicrophoneRecord _microphoneRecord;
   
    private string _currentMicDevice;
    public bool IsInitialized { get; private set; } = false;

    private void Awake()
    {
        ManageSingleton();
    }

    private void ManageSingleton()
    {
        // Si no existe instancia, esta se convierte en la oficial y persiste.
        if (Instance == null)
        {
            Instance = this;
            transform.SetParent(null); // Asegurar que es objeto raíz
            DontDestroyOnLoad(gameObject);
            
            InitializeWhisper();
        }
        else if (Instance!= this)
        {
            // Si ya existe una instancia (ej. volvimos al menú), destruimos esta copia nueva.
            // Esto es crucial para evitar duplicados y conflictos de hardware.
            Debug.Log("[GlobalVoiceManager] Instancia duplicada detectada y destruida.");
            Destroy(gameObject);
            return;
        }
    }

    private void InitializeWhisper()
    {
        _whisperManager = gameObject.GetComponent<WhisperManager>();
        _microphoneRecord = gameObject.AddComponent<MicrophoneRecord>();
        _microphoneRecord.echo = false;
        _microphoneRecord.OnRecordStop += OnRecordStop;
        // Inicialización opcional. WhisperManager suele inicializarse en su propio Start/Awake.
        // Aquí podemos suscribirnos a eventos globales del sistema.
        _whisperManager.OnNewSegment += HandleNewTranscription;
        _whisperManager.language = "English";
        IsInitialized = true;
    }

    /// <summary>
    /// Método público para cambiar el micrófono desde la UI del Menú.
    /// Reinicia el subsistema de grabación de forma segura.
    /// </summary>
    public void SetMicrophone(string deviceName)
    {
        if (_currentMicDevice == deviceName && _microphoneRecord.IsRecording) return;

        Debug.Log($"[GlobalVoiceManager] Cambiando micrófono a: {deviceName}");
        
        // Detener grabación actual si existe

        _currentMicDevice = deviceName;
        _microphoneRecord.SelectedMicDevice = _currentMicDevice;

        // Iniciar grabación con el nuevo dispositivo.
        // NOTA: Es fundamental revisar que MicrophoneRecord acepte el dispositivo como parámetro.
        // Si usas la versión estándar de Macoron/whisper.unity, puede requerir modificaciones 
        // o reiniciar el componente.[8, 12]
    }

    public void StartRecord()
    {
        if (!_microphoneRecord.IsRecording)
        {
            _microphoneRecord.StartRecord(); 
        }
    }

    public void StopRecord()
    {
        if (_microphoneRecord.IsRecording)
        {
            _microphoneRecord.StopRecord();
        }
    }

    // Evento C# para desacoplar la lógica. Los observadores (UI, Jugador) se suscriben aquí.
    public event System.Action<string> OnTranscriptionReceived;

    private void HandleNewTranscription(WhisperSegment segment)
    {
        Debug.Log(segment.Text);
        // Retransmitir el texto a quien esté escuchando (Jugador o UI de la escena actual)
        //OnTranscriptionReceived?.Invoke(segment.Text);
    }

    private async void OnRecordStop(AudioChunk recordedAudio)
    {   
        var res = await _whisperManager.GetTextAsync(recordedAudio.Data, recordedAudio.Frequency, recordedAudio.Channels);
        OnTranscriptionReceived?.Invoke(res.Result);
    }
}