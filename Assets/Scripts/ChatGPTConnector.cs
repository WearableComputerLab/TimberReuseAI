using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Text;
using Newtonsoft.Json.Linq;

public class ChatGPTConnector : MonoBehaviour
{
    // Replace with your API key
    private string apiKey = "sk-svcacct-z8VX82FEAdKZFUXa9VP_0TL-FebfLtfTzdcsC28n7sW1tVsZlrGVQzuByntUqz2CjT3BlbkFJtj9rJN651CtmGSu5Sftj-stFl7lQPVRV8htxkv8nQySGKCBEn1cyy3kzmIxTKn_JgA";

    // Public method to initiate the request
    public void SendChatRequest(string prompt)
    {
        StartCoroutine(SendRequestCoroutine(prompt));
    }

    // Coroutine to handle asynchronous request
    private IEnumerator SendRequestCoroutine(string prompt)
    {
        string url = "https://api.openai.com/v1/chat/completions";

        // Create the request payload
        JObject payload = new JObject
        {
            ["model"] = "gpt-3.5-turbo",
            ["messages"] = new JArray
            {
                new JObject
                {
                    ["role"] = "user",
                    ["content"] = prompt
                }
            }
        };

        // Convert payload to JSON string
        string jsonString = payload.ToString();

        // Create UnityWebRequest
        UnityWebRequest request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonString);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();

        // Set request headers
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", $"Bearer {apiKey}");

        // Send the request
        yield return request.SendWebRequest();

        // Handle the response inside a try-finally block to ensure disposal
        try
        {
            if (request.result == UnityWebRequest.Result.Success)
            {
                string responseText = request.downloadHandler.text;
                Debug.Log("Response Received: " + responseText);

                // Parse the response
                JObject jsonResponse = JObject.Parse(responseText);
                string reply = jsonResponse["choices"][0]["message"]["content"].ToString();

                // Output the reply
                Debug.Log("ChatGPT Reply: " + reply);
            }
            else
            {
                Debug.LogError($"Error: {request.error}");
                Debug.LogError($"Response: {request.downloadHandler.text}");
            }
        }
        finally
        {
            // Dispose of the UnityWebRequest to prevent memory leaks
            request.Dispose();
        }
    }
}
