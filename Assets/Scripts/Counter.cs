using UnityEngine;
using TMPro;

public class CountdownTimer : MonoBehaviour
{
    [Header("Timer Settings")]
    public float startTimeInSeconds = 60f; // Set the countdown duration in seconds
    public TextMeshProUGUI timerText;     // Assign your TextMeshProUGUI component here

    private float currentTime;

    void Start()
    {
        // Initialize the timer with the starting time
        currentTime = startTimeInSeconds;

        // Ensure the timer display starts correctly
        UpdateTimerDisplay(currentTime);
    }

    void Update()
    {
        if (currentTime > 0)
        {
            // Decrease the current time
            currentTime -= Time.deltaTime;

            // Ensure the time doesn't go below zero
            if (currentTime < 0)
            {
                currentTime = 0;
            }

            // Update the timer text
            UpdateTimerDisplay(currentTime);
        }
        else
        {
            // Timer has reached zero
            TimerEnded();
        }
    }

    void UpdateTimerDisplay(float time)
    {
        // Convert the time into minutes and seconds
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);

        // Format the time as MM:SS
        string formattedTime = string.Format("{0:00}:{1:00}", minutes, seconds);

        // Update the TextMeshProUGUI component
        if (timerText != null)
        {
            timerText.text = formattedTime;
        }
    }

    void TimerEnded()
    {
        // This method is called when the timer reaches 00:00
        Debug.Log("Countdown Timer Ended!");

        // Add any custom logic here, such as transitioning scenes or showing a message
    }
}
