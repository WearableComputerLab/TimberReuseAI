using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.Networking;

public class ChatGPTImage : MonoBehaviour
{
    // Replace with your API key
    private string apiKey = "sk-svcacct-z8VX82FEAdKZFUXa9VP_0TL-FebfLtfTzdcsC28n7sW1tVsZlrGVQzuByntUqz2CjT3BlbkFJtj9rJN651CtmGSu5Sftj-stFl7lQPVRV8htxkv8nQySGKCBEn1cyy3kzmIxTKn_JgA";
    // private string apiURL = "https://api.openai.com/v1/chat/completions";
    private string apiURL = "https://api.openai.com/v1/images/generations";


    public Texture2D inputImage;

    private Texture2D generatedImage;


    // // Start is called before the first frame update
    // void Start()
    // {
    //     // Load an image from your resources (make sure to place the image in the Resources folder)
    //     // Texture2D image = Resources.Load<Texture2D>("path_to_your_image");
    //     StartCoroutine(SendImageToChatGPT(inputImage));

    //     // StartCoroutine(DisplayGenerativeImage());
    // }

    public void SendImageRequest()
    {
        StartCoroutine(SendImageToChatGPT(inputImage));
    }


    IEnumerator SendImageToChatGPT(Texture2D image)
    {
        string base64Image = Convert.ToBase64String(image.EncodeToJPG());
        string jsonPayload = CreateJsonPayload(base64Image);

        // Create UnityWebRequest
        UnityWebRequest request = new UnityWebRequest(apiURL, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonPayload);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();

        // Set request headers
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", $"Bearer {apiKey}");

        yield return request.SendWebRequest();

        // Handle the response
        try
        {
            if (request.result == UnityWebRequest.Result.Success)
            {
                string responseText = request.downloadHandler.text;
                Debug.Log("Response Received: " + responseText);

                // Parse the response to get the image URL
                JObject jsonResponse = JObject.Parse(responseText);

                string imageUrl = jsonResponse["data"][0]["b64_json"].ToString();

                Debug.Log("Image URL: " + imageUrl);

                generatedImage = ConvertBase64ToTexture(imageUrl);

                SaveTextureAsPNG(generatedImage);

                DisplayGenerativeImage();

                // yield return StartCoroutine(DownloadImage(imageUrl));

                // if (jsonResponse["data"][0].Contains("url"))
                // {
                //     string imageUrl = jsonResponse["data"][0]["url"].ToString();
                //     Debug.Log("Image URL: " + imageUrl);
                //     yield return StartCoroutine(DownloadImage(imageUrl));
                // }
                // else
                // {
                //     string reply = jsonResponse["data"][0]["content"].ToString();
                //     Debug.Log("Image URL: " + reply);
                // }

                // Download the generated image
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


    void DisplayGenerativeImage()
    {
        Renderer renderer = GameObject.Find("Image-Display").GetComponent<Renderer>();

        // Material material = Resources.Load<Material>("Image/image");

        renderer.material.mainTexture = generatedImage;

        // renderer.material = material;

        // yield return null; // Ensures the coroutine actually yields
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
            SaveTextureAsPNG(generatedImage);
        }
        else
        {
            Debug.LogError($"Error Downloading Image: {request.error}");
        }
    }

    public void SaveTextureAsPNG(Texture2D texture)
    {
        string path = Application.dataPath + "/Image/SavedImage.png";

        byte[] bytes = texture.EncodeToPNG(); // Encode texture into PNG
        File.WriteAllBytes(path, bytes); // Write to a file
        Debug.Log("Saved Image to: " + path);
    }

    Texture2D ConvertBase64ToTexture(string base64)
    {
        byte[] imageData = System.Convert.FromBase64String(base64);

        Texture2D texture = new Texture2D(2, 2); // Create a new "empty" texture
        if (texture.LoadImage(imageData)) // Load the image data into the texture (size will be updated automatically)
        {
            return texture;
        }
        else
        {
            Debug.LogError("Could not decode image from base64.");
            return null;
        }
    }


    string CreateJsonPayload(string base64Image)
    {
        JObject payload = new JObject
        {
            ["model"] = "dall-e-3",
            // ["prompt"] = "Generate a white cat",
            ["prompt"] = "There are nine waste timbers in the image: Cube7(15cm * 10cm * 48cm), Cube(10cm * 8cm * 153cm), Cube1(20cm * 7cm * 225cm), Cube7(15cm * 10cm * 48cm), Cube8(50cm * 12cm * 130cm), Cube9(22cm * 6cm * 35cm), Cube6(26cm * 14cm * 117cm), Cube2(18cm * 7cm * 150cm), Cube4(40cm * 18cm * 52cm), and Cube5(33cm * 10cm * 25cm). Can you design furniture based on these timber cubes and generate an image of your designed furniture with assembling instructions",
            ["size"] = "1024x1024",
            ["quality"] = "standard",
            ["response_format"] = "b64_json",
            ["n"] = 1
        };
        return payload.ToString();
    }



    // string CreateJsonPayload(string base64Image)
    // {
    //     JObject payload = new JObject
    //     {
    //         ["model"] = "gpt-4o-mini",
    //         ["messages"] = new JArray
    //         {
    //             new JObject
    //             {
    //                 ["role"] = "user",
    //                 ["content"] = 
    //                 // "There are nine waste timbers: Cube7(15cm * 10cm * 48cm), Cube(10cm * 8cm * 153cm), Cube1(20cm * 7cm * 225cm), Cube7(15cm * 10cm * 48cm), Cube8(50cm * 12cm * 130cm), Cube9(22cm * 6cm * 35cm), Cube6(26cm * 14cm * 117cm), Cube2(18cm * 7cm * 150cm), Cube4(40cm * 18cm * 52cm), and Cube5(33cm * 10cm * 25cm). How would you like to use these waster timbers to design a furniture? Can you explain what furniture you can design based on these timbers and provide detailed steps to assemble your timbers together for the designed furniture"
    //                 new JArray
    //                 {
    //                     new JObject
    //                     {
    //                         ["type"] = "text",
    //                         // ["text"] = "These are timbers. Can you design furniture based on these timbers and give me the design image?"
    //                         ["text"] = "There are nine waste timbers in the image: Cube7(15cm * 10cm * 48cm), Cube(10cm * 8cm * 153cm), Cube1(20cm * 7cm * 225cm), Cube7(15cm * 10cm * 48cm), Cube8(50cm * 12cm * 130cm), Cube9(22cm * 6cm * 35cm), Cube6(26cm * 14cm * 117cm), Cube2(18cm * 7cm * 150cm), Cube4(40cm * 18cm * 52cm), and Cube5(33cm * 10cm * 25cm). How would you like to use these waster timbers to design a furniture? Can you explain what furniture you can design based on these timbers and provide detailed steps to assemble your timbers together for the designed furniture"
    //                     },

    //                     new JObject
    //                     {
    //                         ["type"] = "image_url",
    //                         ["image_url"] = new JObject
    //                         {
    //                             ["url"] = $"data:image/jpeg;base64,{base64Image}"
    //                         }
    //                     }
    //                 }
    //             }
    //         }
    //     };
    //     return payload.ToString();
    // }

}

