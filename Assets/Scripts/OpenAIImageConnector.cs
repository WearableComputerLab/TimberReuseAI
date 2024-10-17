using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;


public class OpenAIImageConnector : MonoBehaviour
{
    // Replace with your API key
    private string apiKey = "sk-svcacct-z8VX82FEAdKZFUXa9VP_0TL-FebfLtfTzdcsC28n7sW1tVsZlrGVQzuByntUqz2CjT3BlbkFJtj9rJN651CtmGSu5Sftj-stFl7lQPVRV8htxkv8nQySGKCBEn1cyy3kzmIxTKn_JgA";

    // Public method to initiate the image request
    public void SendImageRequest(Texture2D image, string prompt)
    {
        StartCoroutine(SendImageCoroutine(image, prompt));
    }

    // Coroutine to handle asynchronous request
    private IEnumerator SendImageCoroutine(Texture2D image, string prompt)
    {
        string url = "https://api.openai.com/v1/images/edits"; // or /variations

        // Encode the image to PNG
        byte[] imageBytes = image.EncodeToPNG();

        // Create a temporary file to store the image
        string tempImagePath = Path.Combine(Application.persistentDataPath, "temp_image.png");
        File.WriteAllBytes(tempImagePath, imageBytes);

        // Create form data
        List<IMultipartFormSection> form = new List<IMultipartFormSection>();

        // Add the image file
        form.Add(new MultipartFormFileSection("image", imageBytes, "temp_image.png", "image/png"));

        // If using Image Editing, you can add a mask image (optional)
        // form.Add(new MultipartFormFileSection("mask", maskBytes, "mask.png", "image/png"));

        // Add the prompt
        form.Add(new MultipartFormDataSection("prompt", prompt));

        // Specify the number of images to generate (optional)
        form.Add(new MultipartFormDataSection("n", "1"));

        // Specify the size of the generated image (optional)
        form.Add(new MultipartFormDataSection("size", "512x512"));

        // Create UnityWebRequest
        UnityWebRequest request = UnityWebRequest.Post(url, form);

        // Set request headers
        request.SetRequestHeader("Authorization", $"Bearer {apiKey}");

        // Send the request
        yield return request.SendWebRequest();

        // Handle the response
        if (request.result == UnityWebRequest.Result.Success)
        {
            string responseText = request.downloadHandler.text;
            Debug.Log("Response Received: " + responseText);

            // Parse the response to get the image URL
            JObject jsonResponse = JObject.Parse(responseText);
            string imageUrl = jsonResponse["data"][0]["url"].ToString();

            // Download the generated image
            yield return StartCoroutine(DownloadImage(imageUrl));
        }
        else
        {
            Debug.LogError($"Error: {request.error}");
            Debug.LogError($"Response: {request.downloadHandler.text}");
        }

        // Clean up temporary image file
        if (File.Exists(tempImagePath))
            File.Delete(tempImagePath);
    }

    // Coroutine to download the generated image
    private IEnumerator DownloadImage(string imageUrl)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(imageUrl);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Texture2D generatedImage = DownloadHandlerTexture.GetContent(request);
            Debug.Log("Generated Image Downloaded");

            // Use the generated image (e.g., apply it to a material)
            // Example: ApplyToMaterial(generatedImage);
        }
        else
        {
            Debug.LogError($"Error Downloading Image: {request.error}");
        }
    }

    // Example method to apply the image to a material
    private void ApplyToMaterial(Texture2D texture)
    {
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material.mainTexture = texture;
        }
    }
}
