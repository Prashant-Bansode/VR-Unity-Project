using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using TMPro;
using System.Collections;

public class CharacterAudioPlayer : MonoBehaviour
{
    [Header("Audio Settings")]
    [Tooltip("Audio clip to play when the character is clicked.")]
    public AudioClip audioClip;

    [Tooltip("AudioSource component used to play the audio clip.")]
    public AudioSource audioSource;

    [Header("Initial Instruction UI")]
    [Tooltip("First instruction image (visible before audio plays).")]
    public GameObject instructionImage;
    [Tooltip("First instruction text (visible before audio plays).")]
    public TMP_Text instructionText;

    [Header("Next Instruction UI")]
    [Tooltip("Next instruction image (enabled after audio finishes).")]
    public GameObject nextTask;
    
    [Tooltip("Next instruction image (enabled after audio finishes).")]
    public GameObject nextInstructionImage;
    [Tooltip("Next instruction text (enabled after audio finishes).")]
    public TMP_Text nextInstructionText;

    // Reference to the XRSimpleInteractable component on this GameObject
    private XRSimpleInteractable interactable;

    void Start()
    {
        interactable = GetComponent<XRSimpleInteractable>();
        if (interactable != null)
        {
            interactable.selectEntered.AddListener(OnSelectEntered);
        }
        else
        {
            Debug.LogWarning("XRSimpleInteractable component missing on character.");
        }

        // Set initial UI states: show first instructions, hide next instructions
        // if (instructionImage != null)
        //     instructionImage.SetActive(true);
        // if (instructionText != null)
        //     instructionText.gameObject.SetActive(true);
        // if (nextInstructionImage != null)
        //     nextInstructionImage.SetActive(false);
        // if (nextInstructionText != null)
        //     nextInstructionText.gameObject.SetActive(false);
    }

    void OnDestroy()
    {
        if (interactable != null)
        {
            interactable.selectEntered.RemoveListener(OnSelectEntered);
        }
    }

    // Called when the character is clicked via XR input
    private void OnSelectEntered(SelectEnterEventArgs args)
    {
        if (audioSource != null && audioClip != null)
        {
            // Immediately disable the first instruction UI
            if (instructionImage != null)
                instructionImage.SetActive(false);
            if (instructionText != null)
                instructionText.gameObject.SetActive(false);

            // Stop any currently playing audio and start playing the new audio
            if (audioSource.isPlaying)
            {
                audioSource.Stop();
            }
            audioSource.clip = audioClip;
            audioSource.Play();
            Debug.Log("Character audio played.");

            // Start coroutine to wait until audio finishes, then enable next instruction UI
            StartCoroutine(WaitForAudioToEnd());
        }
    }

    // Coroutine that waits for the audio to finish playing, then enables the next instruction UI
    private IEnumerator WaitForAudioToEnd()
    {
        yield return new WaitForSeconds(audioClip.length);
        if (nextTask != null)
            nextTask.SetActive(true);
        if (nextInstructionImage != null)
            nextInstructionImage.SetActive(true);
        if (nextInstructionText != null)
            nextInstructionText.gameObject.SetActive(true);
        Debug.Log("Next instruction UI displayed after audio finished.");
    }
}
