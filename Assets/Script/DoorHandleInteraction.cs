using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using TMPro;
using System.Collections;

public class DoorHandleInteraction : MonoBehaviour
{
    [Header("Door Settings")]
    public Animator doorAnimator;               // Reference to the door's Animator component
    public string doorOpenTrigger = "DoorOpen";   // Trigger for opening the door
    public string doorCloseTrigger = "DoorClose"; // Trigger for closing the door
    private bool doorIsOpen = false;             // Tracks the current door state

    [Header("Interaction Settings")]
    public float cooldownTime = 1f;             // Cooldown time to prevent rapid re-triggering
    private bool isCooldown = false;

    [Header("Initial Instruction UI")]
    [Tooltip("Arrow image that indicates where to click to open/close the door.")]
    public GameObject instructionImage;
    [Tooltip("TextMeshPro text instruction for the task.")]
    public TMP_Text instructionText;

    [Header("Second Instruction UI")]

    [Tooltip("Second instruction arrow image (shown after door opens).")]
    public GameObject instructionImage2;
    [Tooltip("Second instruction TextMeshPro text (shown after door opens).")]
    public TMP_Text instructionText2;

    // Reference to the XR Simple Interactable component
    private XRSimpleInteractable interactable;

    void Start()
    {
        interactable = GetComponent<XRSimpleInteractable>();
        if (interactable != null)
        {
            interactable.selectEntered.AddListener(OnHandleSelected);
        }
        else
        {
            Debug.LogWarning("XRSimpleInteractable component missing on door handle.");
        }

        // Initially disable the second instruction UI
        if (instructionImage2 != null)
            instructionImage2.SetActive(false);
        if (instructionText2 != null)
            instructionText2.gameObject.SetActive(false);
    }

    void OnHandleSelected(SelectEnterEventArgs args)
    {
        if (!isCooldown)
        {
            // Hide the initial instruction UI upon clicking the door handle
            if (instructionImage != null)
                instructionImage.SetActive(false);
            if (instructionText != null)
                instructionText.gameObject.SetActive(false);

            if (!doorIsOpen)
            {
                if (doorAnimator != null)
                {
                    doorAnimator.SetTrigger(doorOpenTrigger);
                    doorIsOpen = true;
                    Debug.Log("Door is opening.");
                    // Wait for the door open animation to finish before showing second instructions
                    StartCoroutine(WaitForDoorOpenAnimation());
                }
                else
                {
                    Debug.LogWarning("Animator not assigned on door handle interaction script.");
                }
            }
            else
            {
                if (doorAnimator != null)
                {
                    doorAnimator.SetTrigger(doorCloseTrigger);
                    doorIsOpen = false;
                    Debug.Log("Door is closing.");
                    // Hide the second instruction UI when closing the door
                    if (instructionImage2 != null)
                        instructionImage2.SetActive(false);
                    if (instructionText2 != null)
                        instructionText2.gameObject.SetActive(false);
                }
                else
                {
                    Debug.LogWarning("Animator not assigned on door handle interaction script.");
                }
            }
            StartCoroutine(TriggerCooldown());
        }
    }

    // Coroutine that waits until the "DoorOpen" animation has finished, then shows the second instruction UI
    IEnumerator WaitForDoorOpenAnimation()
    {
        bool animationFinished = false;
        while (!animationFinished)
        {
            AnimatorStateInfo stateInfo = doorAnimator.GetCurrentAnimatorStateInfo(0);
            if (stateInfo.IsName("DoorOpen") && stateInfo.normalizedTime >= 1.0f)
            {
                animationFinished = true;
            }
            yield return null;
        }
        // After the door open animation finishes, show the second instruction UI
        if (instructionImage2 != null)
            instructionImage2.SetActive(true);
        if (instructionText2 != null)
            instructionText2.gameObject.SetActive(true);
        Debug.Log("Second instruction UI displayed.");
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
            interactable.selectEntered.RemoveListener(OnHandleSelected);
        }
    }
}
