using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using TMPro;
using System.Collections;

public class FuelCapInteraction : MonoBehaviour
{
    [Header("Fuel Cap Settings")]
    public Animator fuelCapAnimator;           // Reference to the fuel cap's Animator component
    public string fuelOpenTrigger = "FualOpen";  // Trigger for playing the fuel cap open animation
    public string fuelType = "Diesel";           // Set your fuel type here

    [Header("Animation Duration")]
    [Tooltip("Duration of the 'FualOpen' animation in seconds.")]
    public float fuelOpenAnimationDuration = 3f; // Set this to the actual length of the animation

    [Header("Instruction UI")]
    [Tooltip("First instruction image to show before clicking on fuel cap.")]
    public GameObject fuelCapFirstInstructionImage;
    [Tooltip("First instruction TextMeshPro text to show before clicking on fuel cap.")]
    public TMP_Text fuelCapFirstInstructionText;
    
    [Header("Next Instruction UI")]
    [Tooltip("Next instruction image to show after the 'FualOpen' animation finishes.")]
    public GameObject GuageLastTask;
    [Tooltip("Next instruction image to show after the 'FualOpen' animation finishes.")]
    public GameObject fuelCapNextInstructionImage;
    [Tooltip("Next instruction TextMeshPro text to show after the 'FualOpen' animation finishes.")]
    public TMP_Text fuelCapNextInstructionText;
    
    [Header("Fuel Type UI")]
    [Tooltip("TextMeshPro text to display the fuel type.")]
    public TMP_Text fuelTypeText;

    [Header("Interaction Settings")]
    [Tooltip("Additional delay after animation ends before showing the next instructions.")]
    public float textDisplayDelay = 2f;
    [Tooltip("Cooldown time to prevent rapid re-triggering.")]
    public float cooldownTime = 1f;
    private bool isCooldown = false;

    // Reference to the XR Simple Interactable component on this GameObject
    private XRSimpleInteractable interactable;

    void Start()
    {
        interactable = GetComponent<XRSimpleInteractable>();
        if (interactable != null)
        {
            interactable.selectEntered.AddListener(OnFuelCapSelected);
        }
        else
        {
            Debug.LogWarning("XRSimpleInteractable component missing on fuel cap.");
        }
        
        // Ensure the initial instruction UI is active and the subsequent UI is hidden at start.
        // if (fuelCapFirstInstructionImage != null)
        //     fuelCapFirstInstructionImage.SetActive(true);
        // if (fuelCapFirstInstructionText != null)
        //     fuelCapFirstInstructionText.gameObject.SetActive(true);
        // if (fuelCapNextInstructionImage != null)
        //     fuelCapNextInstructionImage.SetActive(false);
        // if (fuelCapNextInstructionText != null)
        //     fuelCapNextInstructionText.gameObject.SetActive(false);
        // if (fuelTypeText != null)
        //     fuelTypeText.gameObject.SetActive(false);
    }

    void OnFuelCapSelected(SelectEnterEventArgs args)
    {
        if (!isCooldown)
        {
            // Disable the initial instruction UI immediately.
            if (fuelCapFirstInstructionImage != null)
                fuelCapFirstInstructionImage.SetActive(false);
            if (fuelCapFirstInstructionText != null)
                fuelCapFirstInstructionText.gameObject.SetActive(false);

            if (fuelCapAnimator != null)
            {
                // Trigger the fuel cap open animation.
                fuelCapAnimator.SetTrigger(fuelOpenTrigger);
                Debug.Log("Fuel cap animation triggered.");
            }
            else
            {
                Debug.LogWarning("Animator not assigned on FuelCapInteraction script.");
            }
            
            // Start a coroutine that waits for the animation duration plus an additional delay,
            // then shows the next instruction UI and fuel type text.
            StartCoroutine(WaitForFuelOpenToFinish());
            StartCoroutine(TriggerCooldown());
        }
    }

    IEnumerator WaitForFuelOpenToFinish()
    {
        // Wait for the duration of the "FualOpen" animation.
        yield return new WaitForSeconds(fuelOpenAnimationDuration);
        // Wait for an additional delay.
        yield return new WaitForSeconds(textDisplayDelay);

        if (GuageLastTask != null)
            GuageLastTask.SetActive(true);

        // Enable the next instruction UI.
        if (fuelCapNextInstructionImage != null)
            fuelCapNextInstructionImage.SetActive(true);
        if (fuelCapNextInstructionText != null)
            fuelCapNextInstructionText.gameObject.SetActive(true);

        // Display the fuel type text.
        if (fuelTypeText != null)
        {
            fuelTypeText.text = "Fuel Type: " + fuelType;
            fuelTypeText.gameObject.SetActive(true);
            Debug.Log("Fuel type text displayed after delay.");
        }
    }

    IEnumerator TriggerCooldown()
    {
        isCooldown = true;
        yield return new WaitForSeconds(cooldownTime);
        isCooldown = false;
    }

    void OnDestroy()
    {
        if (interactable != null)
        {
            interactable.selectEntered.RemoveListener(OnFuelCapSelected);
        }
    }
}
