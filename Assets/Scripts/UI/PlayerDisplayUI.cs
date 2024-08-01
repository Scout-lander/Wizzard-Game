using UnityEngine;
using UnityEngine.UI;

public class PlayerDisplayUI : MonoBehaviour
{
    public GameObject playerPrefab; // Reference to the player prefab
    public RenderTexture renderTexture; // Reference to the render texture
    public RawImage playerImage; // Reference to the Raw Image UI element

    private GameObject playerClone;
    private Camera renderCamera;

    void Start()
    {
        // Create a clone of the player model
        playerClone = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);

        // Create a new camera for rendering the player model
        GameObject cameraObject = new GameObject("RenderCamera");
        renderCamera = cameraObject.AddComponent<Camera>();

        // Set the render camera to use the render texture
        renderCamera.targetTexture = renderTexture;

        // Position the camera to focus on the player clone
        renderCamera.transform.position = new Vector3(0, 2, -5);
        renderCamera.transform.LookAt(playerClone.transform);

        // Set the Raw Image texture to the render texture
        playerImage.texture = renderTexture;
    }

    void OnDestroy()
    {
        // Clean up the clone and camera when the UI is destroyed
        if (playerClone != null)
        {
            Destroy(playerClone);
        }
        if (renderCamera != null)
        {
            Destroy(renderCamera.gameObject);
        }
    }
}
