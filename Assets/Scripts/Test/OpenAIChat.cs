using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Text;
using UnityEngine.UI; // For UI elements
using SimpleJSON; // Include SimpleJSON.cs in your project

public class OpenAIChat : MonoBehaviour
{
    private string apiKey = "YOUR_OPENAI_API_KEY";
    private string apiUrl = "https://api.openai.com/v1/chat/completions";

    public InputField userInput;
    public Text chatOutput;

    public void OnSendButtonClicked()
    {
        string message = userInput.text;
        if (!string.IsNullOrEmpty(message))
        {
            SendMessageToChatGPT(message);
            chatOutput.text += "\nYou: " + message;
            userInput.text = "";
        }
    }

    public void SendMessageToChatGPT(string userMessage)
    {
        StartCoroutine(SendRequest(userMessage));
    }

    IEnumerator SendRequest(string prompt)
    {
        // Create the JSON request body
        JSONObject jsonObject = new JSONObject();
        jsonObject["model"] = "gpt-3.5-turbo";
        JSONArray messagesArray = new JSONArray();
        JSONObject messageObject = new JSONObject();
        messageObject["role"] = "user";
        messageObject["content"] = prompt;
        messagesArray.Add(messageObject);
        jsonObject["messages"] = messagesArray;

        string json = jsonObject.ToString();

        byte[] postData = Encoding.UTF8.GetBytes(json);

        using (UnityWebRequest request = new UnityWebRequest(apiUrl, "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(postData);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", "Bearer " + apiKey);

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string responseText = request.downloadHandler.text;
                Debug.Log("Response: " + responseText);

                // Parse the JSON response
                var jsonResponse = JSON.Parse(responseText);
                string reply = jsonResponse["choices"][0]["message"]["content"];
                Debug.Log("ChatGPT Reply: " + reply);

                chatOutput.text += "\nChatGPT: " + reply;
            }
            else
            {
                Debug.LogError("Error: " + request.error);
                chatOutput.text += "\nError: " + request.error;
            }
        }
    }
}
