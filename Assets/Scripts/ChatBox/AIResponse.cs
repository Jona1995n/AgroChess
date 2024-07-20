using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AIResponse : MonoBehaviour
{
    public static AIResponse Instance;

    public Text chatText;
    public string[] aiMessages;
    public int maxMessages = 10; // Maximum number of AI messages to display before refreshing

    private int messageCount = 0;

    private void Awake()
    {
        Instance = this;
    }

    public void GenerateResponse(string userMessage)
    {
        string aiMessage =  aiMessages[Random.Range(0, aiMessages.Length)];
        DisplayMessage(aiMessage);
    }

    void DisplayMessage(string message)
    { // Increment message count
        messageCount++;

        // Check if the message count exceeds the maximum
        if (messageCount > maxMessages)
        {
            // If exceeded, refresh the chat by clearing it and resetting the count
            chatText.text = "";
            messageCount = 0;
        }

        // Display the message
        chatText.text += message + "\n";
    
}
}
