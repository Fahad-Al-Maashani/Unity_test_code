using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class DialogueChoice
{
    [Tooltip("Text to display for this choice.")]
    public string choiceText;
    
    [Tooltip("The dialogue node index to jump to when this choice is selected.")]
    public int nextNodeIndex;
}

[System.Serializable]
public class DialogueNode
{
    [Tooltip("Name of the speaker (e.g., Character 1 or Character 2).")]
    public string speakerText;
    
    [TextArea(3, 5)]
    [Tooltip("The dialogue message to display.")]
    public string dialogue;
    
    [Tooltip("Optional: The audio clip to play as the dialogue is shown.")]
    public AudioClip dialogueClip;
    
    [Tooltip("Optional: The transform where the camera should move during this dialogue.")]
    public Transform cameraFocus;
    
    [Tooltip("Optional: Animation trigger to fire on the speaking character.")]
    public string animationTrigger;
    
    [Tooltip("Dialogue choices to present for branching. Leave empty for auto-advance.")]
    public List<DialogueChoice> choices;
    
    [Tooltip("If there are no choices, and auto-advance is desired, wait for this many seconds.")]
    public float autoAdvanceDelay = 2f;
}

public class ComplexArabGreeting : MonoBehaviour
{
    [Header("Characters")]
    public GameObject characterPrefab;
    public Transform character1Spawn;
    public Transform character2Spawn;
    public Animator character1Animator;
    public Animator character2Animator;

    [Header("Dialogue Settings")]
    public List<DialogueNode> dialogueNodes = new List<DialogueNode>();
    public int startingNodeIndex = 0;

    [Header("UI Settings")]
    public Text dialogueText;
    public Text speakerText;
    public GameObject choicesPanel;           // Panel to hold choice buttons.
    public GameObject choiceButtonPrefab;       // Prefab of a UI Button for choices.

    [Header("Camera Settings")]
    public Camera mainCamera;
    public float cameraMoveSpeed = 2f;

    [Header("Audio Settings")]
    public AudioSource audioSource;

    // Internal dialogue state.
    private int currentNodeIndex;
    private bool choiceSelected = false;
    private int selectedNextNode = -1;

    // Instantiated character GameObjects.
    private GameObject character1;
    private GameObject character2;

    void Start()
    {
        // Instantiate the two characters.
        character1 = Instantiate(characterPrefab, character1Spawn.position, Quaternion.identity);
        character2 = Instantiate(characterPrefab, character2Spawn.position, Quaternion.Euler(0, 180f, 0));
        
        // If animators aren’t already set in the Inspector, try to get them from the spawned objects.
        if (character1Animator == null && character1 != null)
            character1Animator = character1.GetComponent<Animator>();
        if (character2Animator == null && character2 != null)
            character2Animator = character2.GetComponent<Animator>();

        currentNodeIndex = startingNodeIndex;
        StartCoroutine(RunDialogueSequence());
    }

    IEnumerator RunDialogueSequence()
    {
        // Loop through the dialogue nodes until we run out.
        while (currentNodeIndex < dialogueNodes.Count)
        {
            DialogueNode node = dialogueNodes[currentNodeIndex];
            
            // Camera transition: Move camera to the focus point designated for this dialogue.
            if (node.cameraFocus != null && mainCamera != null)
            {
                yield return StartCoroutine(MoveCameraTo(node.cameraFocus.position));
            }
            
            // Display the speaker's name.
            if (speakerText != null)
                speakerText.text = node.speakerText;

            // Trigger an animation if one is specified.
            if (!string.IsNullOrEmpty(node.animationTrigger))
            {
                if (node.speakerText.Contains("Character 1") && character1Animator != null)
                    character1Animator.SetTrigger(node.animationTrigger);
                else if (node.speakerText.Contains("Character 2") && character2Animator != null)
                    character2Animator.SetTrigger(node.animationTrigger);
            }

            // Play any dialogue audio associated with this node.
            if (node.dialogueClip != null && audioSource != null)
                audioSource.PlayOneShot(node.dialogueClip);

            // Display the dialogue using a typewriter text effect.
            yield return StartCoroutine(TypewriterEffect(node.dialogue));

            // Check: Does this dialogue node present choices?
            if (node.choices != null && node.choices.Count > 0)
            {
                // Present choices and wait for user input.
                yield return StartCoroutine(ShowChoices(node.choices));
                currentNodeIndex = selectedNextNode;
            }
            else
            {
                // No choices means auto-advance after a short delay.
                yield return new WaitForSeconds(node.autoAdvanceDelay);
                currentNodeIndex++;
            }
        }

        // Dialogue has ended; optionally clear UI.
        if (dialogueText != null)
            dialogueText.text = "";
        if (speakerText != null)
            speakerText.text = "";
    }

    /// <summary>
    /// Smoothly moves the main camera to a target position.
    /// </summary>
    IEnumerator MoveCameraTo(Vector3 targetPosition)
    {
        float elapsedTime = 0f;
        Vector3 startingPos = mainCamera.transform.position;
        while (Vector3.Distance(mainCamera.transform.position, targetPosition) > 0.1f)
        {
            elapsedTime += Time.deltaTime * cameraMoveSpeed;
            mainCamera.transform.position = Vector3.Lerp(startingPos, targetPosition, elapsedTime);
            yield return null;
        }
    }

    /// <summary>
    /// Displays the given message with a typewriter effect.
    /// </summary>
    IEnumerator TypewriterEffect(string message)
    {
        dialogueText.text = "";
        foreach (char letter in message)
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(0.05f);
        }
    }

    /// <summary>
    /// Creates choice buttons on the UI panel for each dialogue choice and waits until one is selected.
    /// </summary>
    IEnumerator ShowChoices(List<DialogueChoice> choices)
    {
        // Clean up any pre-existing choices.
        foreach (Transform child in choicesPanel.transform)
        {
            Destroy(child.gameObject);
        }
        choicesPanel.SetActive(true);
        choiceSelected = false;
        selectedNextNode = -1;

        // Create a button for each choice.
        foreach (DialogueChoice choice in choices)
        {
            GameObject buttonObj = Instantiate(choiceButtonPrefab, choicesPanel.transform);
            Button button = buttonObj.GetComponent<Button>();
            Text buttonText = buttonObj.GetComponentInChildren<Text>();
            buttonText.text = choice.choiceText;
            button.onClick.AddListener(() => OnChoiceSelected(choice.nextNodeIndex));
        }

        // Wait until the player clicks a button.
        while (!choiceSelected)
        {
            yield return null;
        }

        // Remove the choice buttons.
        foreach (Transform child in choicesPanel.transform)
        {
            Destroy(child.gameObject);
        }
        choicesPanel.SetActive(false);
    }

    /// <summary>
    /// Called when a dialogue choice is selected.
    /// </summary>
    /// <param name="nextNodeIndex">The index of the next dialogue node.</param>
    void OnChoiceSelected(int nextNodeIndex)
    {
        selectedNextNode = nextNodeIndex;
        choiceSelected = true;
    }
}
