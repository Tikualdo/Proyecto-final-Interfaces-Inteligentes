using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class ItemUnlocker : MonoBehaviour
{
    [Header("Conexión con el Inventario")]
    public GameObject inventoryButton; // El botón UI que corresponde a este objeto

    private XRGrabInteractable _grabInteractable;
    private bool _isUnlocked = false;

    void Awake()
    {
        _grabInteractable = GetComponent<XRGrabInteractable>();

        // 1. Ocultamos el botón al iniciar el juego
        if (inventoryButton != null)
        {
            inventoryButton.SetActive(false);
        }
    }

    void OnEnable()
    {
        // Nos suscribimos al evento de agarre
        _grabInteractable.selectEntered.AddListener(OnGrabbed);
    }

    void OnDisable()
    {
        _grabInteractable.selectEntered.RemoveListener(OnGrabbed);
    }

    private void OnGrabbed(SelectEnterEventArgs args)
    {
        // 2. Si es la primera vez que lo agarramos, desbloqueamos
        if (!_isUnlocked)
        {
            _isUnlocked = true;
            if (inventoryButton != null)
            {
                inventoryButton.SetActive(true);
                Debug.Log($"Ítem {name} desbloqueado en el inventario.");
            }
        }
    }
}