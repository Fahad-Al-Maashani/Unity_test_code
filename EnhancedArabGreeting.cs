using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class EnhancedArabGreeting : MonoBehaviour
{
    [Header("Character and Dialogue Settings")]
    // A prefab for the characters.
    public GameObject characterPrefab;
    // Transform references for character spawn positions.
    public Transform character1Spawn;
    public Transform character2Spawn;
    // Reference to the UI text element for dialogue.
    public Text dialogueText;

    [Header("Audio Settings")]
    // Audio clips for greetings.
    public AudioClip greetingClip1;
    public AudioClip greetingClip2;
    // AudioSource to play sound effects.
    public AudioSource audioSource;

    [Header("Camera Settings")]
    // Main camera reference.
    public Camera mainCamera;
    // Camera focus points for each character.
    public Transform cameraFocus1;
    public Transform cameraFocus2;
    // Speed at which the camera moves.
    public float cameraMoveSpeed = 2f;

    // Animator references for the characters.
    private Animator animator1;
    private Animator animator2;

    // Instantiated character objects.
    private GameObject character1;
    private GameObject character2;

    void Start()
    {
        // Instantiate the characters at their designated spawn positions.
        character1 = Instantiate(characterPrefab, character1Spawn.position, Quaternion.identity);
        character2 = Instantiate(characterPrefab, character2Spawn.position, Quaternion.Euler(0, 180f, 0));

        // Get the Animator components from each character.
        animator1 = character1.GetComponent<Animator>();
        animator2 = character2.GetComponent<Animator>();

        // Start the enhanced dialogue and interaction sequence.
        StartCoroutine(GreetingSequence());
    }

    IEnumerator GreetingSequence()
    {
        // Wait for the scene to settle.
        yield return new WaitForSeconds(1f);

        // --- Part 1: Character 1 Greets ---

        // Move the camera to highlight Character 1.
        yield return MoveCameraTo(cameraFocus1.position);
        // Trigger the "Wave" animation on Character 1.
        if (animator1 != null) animator1.SetTrigger("Wave");
        // Play greeting audio for Character 1.
        if (audioSource != null && greetingClip1 != null)
            audioSource.PlayOneShot(greetingClip1);
        // Display the greeting with a typewriter effect.
        yield return DisplayDialogue("Character 1: السلام عليكم", 3f);

        yield return new WaitForSeconds(1f);

        // --- Part 2: Character 2 Responds ---
        
        // Move the camera to highlight Character 2.
        yield return MoveCameraTo(cameraFocus2.position);
        // Trigger the "Nod" animation on Character 2.
        if (animator2 != null) animator2.SetTrigger("Nod");
        // Play greeting audio for Character 2.
        if (audioSource != null && greetingClip2 != null)
            audioSource.PlayOneShot(greetingClip2);
        // Display the response.
        yield return DisplayDialogue("Character 2: وعليكم السلام", 3f);

        yield return new WaitForSeconds(1f);

        // --- Part 3: Follow-Up Conversation ---
        
        // Move back to Character 1.
        yield return MoveCameraTo(cameraFocus1.position);
        yield return DisplayDialogue("Character 1: كيف حالك؟", 3f);  // "How are you?"
        yield return new WaitForSeconds(1f);
        
        // Move to Character 2 for the reply.
        yield return MoveCameraTo(cameraFocus2.position);
        yield return DisplayDialogue("Character 2: بخير، الحمد لله", 3f);  // "I'm fine, thank God."
        
        // Clear the dialogue after the conversation ends.
        dialogueText.text = "";
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
        yield return null;
    }

    /// <summary>
    /// Displays a given message using a typewriter effect and holds it for a set duration.
    /// </summary>
    IEnumerator DisplayDialogue(string message, float displayDuration)
    {
        dialogueText.text = "";
        foreach (char letter in message)
        {
            dialogueText.text += letter;
            // Adjust the delay for typewriter effect as needed.
            yield return new WaitForSeconds(0.05f);
        }
        yield return new WaitForSeconds(displayDuration);
    }
}
