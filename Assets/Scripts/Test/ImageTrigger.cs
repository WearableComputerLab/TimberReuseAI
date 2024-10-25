using UnityEngine;



public class ImageTrigger : MonoBehaviour
{
    public OpenAIImageConnector imageConnector;
    public Texture2D inputImage;
    public string prompt = "Can you use the timbers with corresponding size to desgin a furniture";

    void Start()
    {
        imageConnector.SendImageRequest(inputImage, prompt);
    }
}
