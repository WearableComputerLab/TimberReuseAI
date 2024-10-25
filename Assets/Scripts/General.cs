using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class General : MonoBehaviour
{
    public TMP_Text dimensionText;
    void Start()
    {
        Renderer renderer = GameObject.Find("Image-Display").GetComponent<Renderer>();

        // Material material = Resources.Load<Material>("Image/image");

        renderer.material.mainTexture = null;

        // You can tag your cubes with a specific tag and iterate only those
        // For example, if you tagged them as 'Timber', you can find them like this:
        GameObject[] timbers = GameObject.FindGameObjectsWithTag("Timber");

        foreach (GameObject timber in timbers)
        {

            UpdateTextMesh(timber);

            // Vector3 scale = timber.transform.localScale;
            // // Convert scale to dimensions based on your unit conversion (Unity units to centimeters in this case)
            // Vector3 dimensions = scale * 100; // Assuming 1 unit in Unity is 1 meter, and thus 100 centimeters
            // Debug.Log("Timber Dimensions " + timber.name + " :" + dimensions.x + "cm x " + dimensions.y + "cm x " + dimensions.z + "cm");
        }
    }

    void UpdateTextMesh(GameObject timber)
    {
        TextMeshPro textMesh = timber.GetComponentInChildren<TextMeshPro>();
        if (textMesh != null)
        {
            Vector3 dimensions = timber.transform.localScale * 100; // Assuming scale represents meters, convert to cm
            string cubeName = timber.name; // Get the name of the timber
            textMesh.text = $"{cubeName}: {dimensions.x:F1}cm x {dimensions.y:F1}cm x {dimensions.z:F1}cm";
        }
        else
        {
            Debug.LogWarning("No TextMesh found in children of " + timber.name);
        }
    }
}
