using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using TMPro;
using System.Collections;

public class GaugeGrabToggleIndicator : MonoBehaviour
{
    [Header("Gauge Indicator Settings")]
    [Tooltip("A visible indicator (e.g. a transparent cube) that shows while the gauge is grabbed.")]
    public GameObject gaugeIndicator;

    [Header("Initial Instruction UI")]
    [Tooltip("Initial instruction image to disable when gauge is grabbed.")]
    public GameObject gaugeInitialInstructionImage;
    [Tooltip("Initial instruction text to disable when gauge is grabbed.")]
    public TMP_Text gaugeInitialInstructionText;
    [Tooltip("Initial instruction text to disable when gauge is grabbed.")]
    public TMP_Text InitialInstructionText;


    [Header("Next Instruction UI")]
    [Tooltip("Next instruction image to enable after gauge is grabbed.")]
    public GameObject gaugeNextInstructionImage;
    [Tooltip("Next instruction text to enable after gauge is grabbed.")]
    public TMP_Text gaugeNextInstructionText;

    // Reference to the XRGrabInteractable component on this gauge
    private XRGrabInteractable grabInteractable;
    // Reference to the Rigidbody for physics control
    private Rigidbody rb;

    void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
        rb = GetComponent<Rigidbody>();

        // Optionally hide the gauge indicator and next instruction UI at start.
        if (gaugeIndicator != null)
            gaugeIndicator.SetActive(false);
        // if (gaugeNextInstructionImage != null)
        //     gaugeNextInstructionImage.SetActive(false);
        // if (gaugeNextInstructionText != null)
        //     gaugeNextInstructionText.gameObject.SetActive(false);
    }

    void OnEnable()
    {
        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.AddListener(OnGrabbed);
            grabInteractable.selectExited.AddListener(OnReleased);
        }
    }

    void OnDisable()
    {
        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.RemoveListener(OnGrabbed);
            grabInteractable.selectExited.RemoveListener(OnReleased);
        }
    }

    private void OnGrabbed(SelectEnterEventArgs args)
    {
        // Disable the initial instruction UI when the gauge is grabbed.
        if (gaugeInitialInstructionImage != null)
            gaugeInitialInstructionImage.SetActive(false);
        if (gaugeInitialInstructionText != null)
            gaugeInitialInstructionText.gameObject.SetActive(false);
        if (InitialInstructionText != null)
            InitialInstructionText.gameObject.SetActive(false);

        // Enable the next instruction UI immediately after the gauge is grabbed.
        if (gaugeNextInstructionImage != null)
            gaugeNextInstructionImage.SetActive(true);
        if (gaugeNextInstructionText != null)
            gaugeNextInstructionText.gameObject.SetActive(true);

        // Optionally, show the gauge indicator while grabbed.
        if (gaugeIndicator != null)
            gaugeIndicator.SetActive(true);
        
        // Disable gravity and set isKinematic to true while the gauge is held.
        if (rb != null)
        {
            rb.useGravity = false;
            rb.isKinematic = true;
        }
    }

    private void OnReleased(SelectExitEventArgs args)
    {
        // When the gauge is released, hide the gauge indicator.
        if (gaugeIndicator != null)
            gaugeIndicator.SetActive(false);
        
        // Enable gravity and set isKinematic to false so physics takes over.
        if (rb != null)
        {
            rb.useGravity = true;
            rb.isKinematic = false;
        }
    }
}
