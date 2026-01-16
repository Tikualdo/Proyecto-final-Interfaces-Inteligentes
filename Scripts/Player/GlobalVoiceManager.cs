using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;

[Serializable]
public class GroqResponse
{
    public string text;
}

public class GlobalVoiceManager : MonoBehaviour
{
    public static GlobalVoiceManager Instance { get; private set; }

    [Header("Configuración Groq")]
    public string apiKey = "";
    [TextArea(3, 5)]
    public string initialPrompt = "Lista de hechizos: Fireball, Heal, Black Hole, Lava Pool, Acid Splash, Fog.";

    [Header("Configuración Micrófono")]
    public int recordingFrequency = 44100;
    
    // OPTIMIZACIÓN 1: Bajamos a 10s. Para comandos de voz es más que suficiente.
    // Esto reduce el tamaño del buffer de 5MB a ~1.5MB.
    public int maxRecordingLength = 10; 

    // Estado interno
    private AudioClip _recordingClip;
    private string _selectedMicDevice;
    private bool _isRecording;
    private float _startRecordingTime;
    
    // OPTIMIZACIÓN 2: Buffer reutilizable para evitar el "Lag del Garbage Collector"
    private float[] _bufferCache; 

    public event Action<string> OnTranscriptionSuccess;
    public event Action<string> OnTranscriptionError;
    public event Action OnRecordingStart;
    public event Action OnRecordingStop;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeMicrophone();
            
            // Pre-reservamos la memoria UNA VEZ al iniciar.
            // Así, al soltar el botón, no hay que pedir memoria nueva al sistema.
            _bufferCache = new float[maxRecordingLength * recordingFrequency];
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeMicrophone()
    {
        if (Microphone.devices.Length > 0)
            _selectedMicDevice = Microphone.devices[0];
    }

    // --- UI HELPER (Sin cambios) ---
    public void PopulateMicrophoneDropdown(TMP_Dropdown dropdown)
    {
        if (dropdown == null) return;
        dropdown.ClearOptions();
        string[] devices = Microphone.devices;
        List<string> options = new List<string>();
        int currentSelectionIndex = 0;

        if (devices.Length == 0)
        {
            options.Add("No Microphone Detected");
            dropdown.interactable = false;
        }
        else
        {
            dropdown.interactable = true;
            for (int i = 0; i < devices.Length; i++)
            {
                options.Add(devices[i]);
                if (devices[i] == _selectedMicDevice) currentSelectionIndex = i;
            }
        }

        dropdown.AddOptions(options);
        dropdown.value = currentSelectionIndex;
        dropdown.RefreshShownValue();
        dropdown.onValueChanged.RemoveAllListeners();
        dropdown.onValueChanged.AddListener((index) => {
            if (Microphone.devices.Length > index) _selectedMicDevice = Microphone.devices[index];
        });
    }

    public void SetMicrophone(int index)
    {
        if (Microphone.devices.Length > index) _selectedMicDevice = Microphone.devices[index];
    }

    // --- GRABACIÓN ---

    public void StartListening()
    {
        if (_isRecording || string.IsNullOrEmpty(_selectedMicDevice)) return;

        // Reiniciamos el clip. Unity reutiliza la memoria interna del AudioClip si el tamaño es igual.
        _recordingClip = Microphone.Start(_selectedMicDevice, false, maxRecordingLength, recordingFrequency);
        _startRecordingTime = Time.time;
        _isRecording = true;
        OnRecordingStart?.Invoke();
    }

    public async void StopAndProcess()
    {
        if (!_isRecording) return;

        // 1. Parar micrófono (Esto suele ser rápido, pero en algunos Android viejos puede tardar 2ms)
        int lastPos = Microphone.GetPosition(_selectedMicDevice);
        Microphone.End(_selectedMicDevice);
        _isRecording = false;
        OnRecordingStop?.Invoke();

        // Calcular duración real
        float recordingDuration = Time.time - _startRecordingTime;
        if (recordingDuration < 0.3f) return; // Ignorar toques accidentales muy cortos

        // 2. EXTRACCIÓN OPTIMIZADA (ZERO ALLOCATION)
        // Calculamos cuántas muestras necesitamos realmente
        int samplesNeeded = lastPos > 0 ? lastPos : (int)(recordingDuration * recordingFrequency);
        
        // Seguridad: Clamp para no salirnos del array
        samplesNeeded = Mathf.Clamp(samplesNeeded, 0, _bufferCache.Length);

        // AQUÍ ESTABA EL LAG: Antes hacíamos "new float[]". Ahora usamos el cache.
        // GetData sigue siendo Main Thread, pero ahora no genera basura para el GC.
        _recordingClip.GetData(_bufferCache, 0);

        // Guardamos datos locales para el hilo
        int channels = _recordingClip.channels;
        int freq = _recordingClip.frequency;

        // Clonamos SOLO la parte necesaria para el hilo (Array.Copy es ultra rápido en memoria, mucho más que 'new')
        // Necesitamos clonar porque si el usuario vuelve a grabar inmediatamente, sobrescribiría _bufferCache
        // mientras el hilo secundario aún lo está leyendo.
        float[] threadSafeData = new float[samplesNeeded];
        Array.Copy(_bufferCache, threadSafeData, samplesNeeded);

        Debug.Log($"[VoiceManager] Enviando {recordingDuration:F2}s a segundo plano...");

        // 3. PROCESAMIENTO PARALELO
        // Todo el trabajo pesado de codificación WAV ocurre aquí
        byte[] wavData = await Task.Run(() => EncodeToWAV(threadSafeData, channels, freq));

        StartCoroutine(SendToGroq(wavData));
    }

    // --- RED ---

    private IEnumerator SendToGroq(byte[] wavData)
    {
        WWWForm form = new WWWForm();
        form.AddBinaryData("file", wavData, "audio.wav", "audio/wav");
        form.AddField("model", "whisper-large-v3");
        form.AddField("language", "es");
        
        if (!string.IsNullOrEmpty(initialPrompt)) form.AddField("prompt", initialPrompt);

        using (UnityWebRequest request = UnityWebRequest.Post("https://api.groq.com/openai/v1/audio/transcriptions", form))
        {
            request.SetRequestHeader("Authorization", "Bearer " + apiKey);
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Error Groq: {request.error}");
                OnTranscriptionError?.Invoke(request.error);
            }
            else
            {
                try 
                {
                    var response = JsonUtility.FromJson<GroqResponse>(request.downloadHandler.text);
                    string finalText = response.text.Trim();
                    // Limpiamos signos de puntuación extra que a veces mete Whisper
                    finalText = finalText.TrimEnd('.', ',', '!', '?');
                    
                    Debug.Log($"[VoiceManager] Resultado: {finalText}");
                    OnTranscriptionSuccess?.Invoke(finalText);
                }
                catch (Exception e) { OnTranscriptionError?.Invoke("JSON Error: " + e.Message); }
            }
        }
    }

    // --- ENCODER (THREAD SAFE) ---
    private byte[] EncodeToWAV(float[] samples, int channels, int frequency)
    {
        using (MemoryStream stream = new MemoryStream())
        {
            BinaryWriter writer = new BinaryWriter(stream);
            writer.Write(Encoding.ASCII.GetBytes("RIFF")); writer.Write(0);
            writer.Write(Encoding.ASCII.GetBytes("WAVE")); writer.Write(Encoding.ASCII.GetBytes("fmt "));
            writer.Write(16); writer.Write((short)1); writer.Write((short)channels);
            writer.Write(frequency); writer.Write(frequency * channels * 2);
            writer.Write((short)(channels * 2)); writer.Write((short)16);
            writer.Write(Encoding.ASCII.GetBytes("data")); writer.Write(0);

            // Conversión directa
            short[] intData = new short[samples.Length];
            // Optimizamos el bucle para velocidad
            for (int i = 0; i < samples.Length; i++)
            {
                float s = samples[i];
                // Clamp manual rápido
                if (s > 1f) s = 1f;
                else if (s < -1f) s = -1f;
                intData[i] = (short)(s * 32767f);
            }

            byte[] byteData = new byte[intData.Length * 2];
            Buffer.BlockCopy(intData, 0, byteData, 0, byteData.Length);
            writer.Write(byteData);
            
            writer.Seek(4, SeekOrigin.Begin); writer.Write((int)(stream.Length - 8));
            writer.Seek(40, SeekOrigin.Begin); writer.Write((int)(stream.Length - 44));
            return stream.ToArray();
        }
    }
}