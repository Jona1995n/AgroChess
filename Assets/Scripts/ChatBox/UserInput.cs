using System.Collections;
using System.Collections.Generic;
//using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.UI;

public class UserInput : MonoBehaviour
{
    public InputField inputField;
    public Text chatText;
    public int maxMessages = 12; // Maximum number of messages to display before refreshing

    private int messageCount = 0;
   


    public void SendMessage()
    {
        string message = inputField.text;
        if (!string.IsNullOrEmpty(message))
        {
            // Display the message
            DisplayMessage(message);
            inputField.text = ""; // Clear the input field after sending message

            // Check if maximum messages reached
            messageCount++;
            if (messageCount >= maxMessages)
            {
                RefreshChat();
                messageCount = 0;
            }

            // Call AI response function
            AIResponse.Instance.GenerateResponse(message);
        }
    }

    // Function to display a message
    public void DisplayMessage(string message)
    {
        chatText.text += message + "\n";
    }

    // Function to refresh the chat
    public void RefreshChat()
    {
        chatText.text = ""; // Clear all messages
    }
}
