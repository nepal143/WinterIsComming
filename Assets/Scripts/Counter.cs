using UnityEngine;
using TMPro;

public class CountdownTimer : MonoBehaviour
{
    [Header("Timer Settings")]
    public float startTimeInSeconds = 60f;  // Set the starting time in seconds
    public TextMeshProUGUI timerText;       // Drag and drop your TextMeshProUGUI here

    private float currentTime;
    private bool isTimerRunning = false;

    void Start()
    {
        // Initialize the timer
        currentTime = startTimeInSeconds;

        // Start the timer automatically (optional)
        StartTimer();
    }

    void Update()
    {
        if (isTimerRunning && currentTime > 0)
        {
            // Decrease the current time
            currentTime -= Time.deltaTime;

            // Ensure time doesn't go below zero
            if (currentTime < 0)
            {
                currentTime = 0;
                isTimerRunning = false;
                TimerEnded();
            }

            // Update the timer text
            UpdateTimerDisplay(currentTime);
        }
    }

    /// <summary>
    /// Starts the timer.
    /// </summary>
    public void StartTimer()
    {
        if (timerText == null)
        {
            Debug.LogError("Timer TextMeshProUGUI is not assigned!");
            return;
        }

        isTimerRunning = true;

        // Initialize the timer display
        UpdateTimerDisplay(currentTime);
    }

    /// <summary>
    /// Updates the timer display with the formatted time.
    /// </summary>
    /// <param name="time">The time to display (in seconds).</param>
    private void UpdateTimerDisplay(float time)
    {
        // Convert the time into minutes and seconds
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);

        // Format the time as MM:SS
        string formattedTime = string.Format("{0:00}:{1:00}", minutes, seconds);

        // Update the TextMeshProUGUI component
        timerText.text = formattedTime;
    }

    /// <summary>
    /// Called when the timer reaches zero.
    /// </summary>
    private void TimerEnded()
    {
        Debug.Log("Countdown Timer Ended!");
        // Add custom behavior here, such as triggering an event or stopping the game.
    }
}
