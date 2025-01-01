using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class TriviaGame : MonoBehaviour
{
    [Header("UI References")]
    public Canvas triviaCanvas;               // The canvas containing the trivia UI
    public Image option1Image;                 // The image for option 1
    public Image option2Image;                 // The image for option 2
    public TextMeshProUGUI scoreText;          // The score UI text
    public TextMeshProUGUI questionText;       // The question UI text

    [Header("Trivia Data")]
    public Sprite[] option1Sprites;            // Sprites for option 1
    public Sprite[] option2Sprites;            // Sprites for option 2
    public string[] questions;                // The trivia questions
    public int[] correctAnswers;              // Index of correct answers (0 for option 1, 1 for option 2)

    [Header("Canvas Movement")]
    public Transform startPosition;           // Start position for canvas movement
    public Transform endPosition;             // End position for canvas movement
    public float movementDuration = 1f;       // Duration of the movement

    private int currentQuestionIndex = 0;     // Index to track the current question
    private int score = 0;                     // Player's score

    void Start()
    {
        // Initialize first question
        LoadQuestion();
    }

    // Load the current question
    void LoadQuestion()
    {
        // Set the question text
        questionText.text = questions[currentQuestionIndex];

        // Set the images for the options
        option1Image.sprite = option1Sprites[currentQuestionIndex];
        option2Image.sprite = option2Sprites[currentQuestionIndex];

        // Move the canvas to the starting position
        triviaCanvas.transform.position = startPosition.position;

        // Animate the canvas moving to the end position
        StartCoroutine(MoveCanvas());
    }

    // Move the canvas with floating effect
    IEnumerator MoveCanvas()
    {
        float timeElapsed = 0f;

        while (timeElapsed < movementDuration)
        {
            float lerpFactor = Mathf.SmoothStep(0f, 1f, timeElapsed / movementDuration);
            triviaCanvas.transform.position = Vector3.Lerp(startPosition.position, endPosition.position, lerpFactor);

            // Add floating effect (a slight vertical movement when the canvas reaches the destination)
            if (lerpFactor > 0.95f)
            {
                float floatFactor = Mathf.Sin(Time.time * 2f) * 5f; // Floating effect
                triviaCanvas.transform.position = new Vector3(triviaCanvas.transform.position.x, triviaCanvas.transform.position.y + floatFactor, triviaCanvas.transform.position.z);
            }

            timeElapsed += Time.deltaTime;
            yield return null;
        }

        triviaCanvas.transform.position = endPosition.position;  // Ensure it reaches the exact end position
    }

    // Called when the user clicks on the first option
    public void OnOption1Clicked()
    {
        CheckAnswer(0);  // Option 1 is answer 0
    }

    // Called when the user clicks on the second option
    public void OnOption2Clicked()
    {
        CheckAnswer(1);  // Option 2 is answer 1
    }

    // Check if the selected answer is correct
    void CheckAnswer(int selectedOption)
    {
        if (selectedOption == correctAnswers[currentQuestionIndex])
        {
            score++;  // Increment the score if the answer is correct
            scoreText.text = "Score: " + score;
        }

        // Move to the next question
        NextQuestion();
    }

    // Load the next question or reset if we've reached the end
    void NextQuestion()
    {
        currentQuestionIndex++;

        if (currentQuestionIndex >= questions.Length)
        {
            currentQuestionIndex = 0;  // Reset to the first question (or end the game)
            score = 0;  // Reset score (optional)
            scoreText.text = "Score: " + score;
        }

        // Load the new question
        LoadQuestion();
    }
}
