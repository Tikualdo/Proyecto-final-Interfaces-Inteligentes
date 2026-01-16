using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.EventSystems; // Necesario para detectar el click

// Este script ya no necesita IPointerClickHandler ni referencias a manos externas
public class InventoryInteractableSlot : MonoBehaviour
{
    [Header("Referencia al Objeto Real")]
    public GameObject sceneItem;

    // Esta referencia es opcional si la asignas en el inspector,
    // pero el script puede encontrarla dinámicamente.
    private XRSimpleInteractable _simpleInteractable;

    void Awake()
    {
        _simpleInteractable = GetComponent<XRSimpleInteractable>();
    }

    void OnEnable()
    {
        _simpleInteractable.selectEntered.AddListener(OnSlotSelected);
    }

    void OnDisable()
    {
        _simpleInteractable.selectEntered.RemoveListener(OnSlotSelected);
    }

    // Este método salta AUTOMÁTICAMENTE cuando el XR Simple Interactable es activado
    private void OnSlotSelected(SelectEnterEventArgs args)
    {
        // ¡MAGIA! args.interactorObject ES la mano que hizo click (NearFarInteractor)
        XRBaseInteractor handThatClicked = args.interactorObject as XRBaseInteractor;

        if (handThatClicked != null)
        {
            // Como el inventario no se "agarra" (solo se clicka), el interactor sigue seleccionando el botón.
            // Necesitamos forzar que el botón se deseleccione inmediatamente para poder volver a pulsarlo luego.
            StartCoroutine(ResetButtonSelection(handThatClicked));

            // Ejecutamos la lógica de equipar en esa mano
            StartCoroutine(SwapAndEquip(handThatClicked));
        }
    }

    private IEnumerator ResetButtonSelection(XRBaseInteractor hand)
    {
        // Esperamos un frame
        yield return null;
        // Le pedimos al manager que libere el botón para que deje de estar "pulsado"
        if (hand.interactionManager != null)
        {
            hand.interactionManager.SelectExit(hand as IXRSelectInteractor, _simpleInteractable);
        }
    }

    // --- LÓGICA DE INTERCAMBIO (Idéntica a la anterior) ---

    private IEnumerator SwapAndEquip(XRBaseInteractor hand)
    {
        XRInteractionManager manager = hand.interactionManager;

        // 1. Si la mano ya tiene ESTE objeto, no hacemos nada
        if (hand.hasSelection && hand.interactablesSelected[0].transform.gameObject == sceneItem)
        {
            yield break;
        }

        // 2. SOLTAR LO QUE TENGA
        if (hand.hasSelection)
        {
            IXRSelectInteractable currentHeldItem = hand.interactablesSelected[0];
            GameObject currentObj = currentHeldItem.transform.gameObject;
            
            // Importante: No intentamos soltar el propio botón (ya lo hicimos en ResetButtonSelection)
            if (currentObj != this.gameObject) 
            {
                manager.SelectExit(hand as IXRSelectInteractor, currentHeldItem);
                
                if (currentObj.GetComponent<InventoryItem>() != null)
                {
                    yield return null;
                    currentObj.SetActive(false);
                }
            }
        }

        // 3. PREPARAR NUEVO OBJETO
        if (sceneItem.activeInHierarchy)
        {
            var interactable = sceneItem.GetComponent<XRGrabInteractable>();
            if (interactable.isSelected) manager.SelectExit(interactable.interactorsSelecting[0], interactable);
        }

        sceneItem.SetActive(true);
        sceneItem.transform.position = hand.transform.position;
        sceneItem.transform.rotation = hand.transform.rotation;
        
        Rigidbody rb = sceneItem.GetComponent<Rigidbody>();
        if (rb != null) { rb.linearVelocity = Vector3.zero; rb.angularVelocity = Vector3.zero; }

        yield return null;

        // 4. AGARRAR
        XRGrabInteractable targetInteractable = sceneItem.GetComponent<XRGrabInteractable>();
        if (targetInteractable != null)
        {
            manager.SelectEnter(hand as IXRSelectInteractor, targetInteractable as IXRSelectInteractable);
        }
    }
}