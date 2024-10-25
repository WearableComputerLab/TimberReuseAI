using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Text;
using Newtonsoft.Json.Linq;
using TMPro;

public class ChatGPTText : MonoBehaviour
{
    // Replace with your API key
    private string apiKey = "sk-svcacct-z8VX82FEAdKZFUXa9VP_0TL-FebfLtfTzdcsC28n7sW1tVsZlrGVQzuByntUqz2CjT3BlbkFJtj9rJN651CtmGSu5Sftj-stFl7lQPVRV8htxkv8nQySGKCBEn1cyy3kzmIxTKn_JgA";

    string userPrompt = "There are nine waste timbers: Cube7(15cm * 10cm * 48cm), Cube(10cm * 8cm * 153cm), Cube1(20cm * 7cm * 225cm), Cube7(15cm * 10cm * 48cm), Cube8(50cm * 12cm * 130cm), Cube9(22cm * 6cm * 35cm), Cube6(26cm * 14cm * 117cm), Cube2(18cm * 7cm * 150cm), Cube4(40cm * 18cm * 52cm), and Cube5(33cm * 10cm * 25cm). How would you like to use these waster timbers to design a furniture? Can you explain what furniture you can design based on these timbers and provide detailed steps to assemble your timbers together for the designed furniture";


    // void Start()
    // {
    //     // string userPrompt = "How would you like to use waster timber to design something?";
    //     SendChatRequest(userPrompt);
    //     print(userPrompt);
    // }

    // Public method to initiate the request
    public void SendChatRequest()
    {
        StartCoroutine(SendRequestCoroutine(userPrompt));
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
                UpdateTextMesh(reply);
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


    void UpdateTextMesh(string textResponse)
    {
        TextMeshPro textMesh = GameObject.Find("Text-Instruction").GetComponentInChildren<TextMeshPro>();
        if (textMesh != null)
        {
            textMesh.text = $"{textResponse}";
        }
        else
        {
            Debug.LogWarning("No TextMesh found in children of " + GameObject.Find("Text-Instruction").name);
        }
    }
}
