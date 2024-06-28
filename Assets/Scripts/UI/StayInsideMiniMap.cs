using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StayInsideMiniMap : MonoBehaviour {

    public Transform MinimapCam;
    public SpriteRenderer spriteRenderer; // Reference to the SpriteRenderer component
    public Color normalColor = Color.white; // Default color when inside the mini-map
    public Color grayColor = Color.gray; // Color when outside the mini-map
    bool isOutsideBorder = false;

    Vector3 TempV3;

    void Update () {
        TempV3 = transform.parent.transform.position;
        TempV3.y = transform.position.y;
        transform.position = TempV3;

        // Check if the object is outside the border
        if (IsOutsideBorder()) {
            isOutsideBorder = true;
            spriteRenderer.color = grayColor; // Change the sprite color to gray
        } else {
            if (isOutsideBorder) {
                // Object just came back inside the border
                isOutsideBorder = false;
                spriteRenderer.color = normalColor; // Change the sprite color back to normal
            }
        }
    }

    void LateUpdate () {
        transform.position = new Vector3 (
            Mathf.Clamp(transform.position.x, MinimapCam.position.x - 30.5f, 30.5f + MinimapCam.position.x),
            Mathf.Clamp(transform.position.y, MinimapCam.position.y - 17.5f, 17.5f + MinimapCam.position.y)
        );
    }

    bool IsOutsideBorder() {
        // Check if the object's position is outside the border of the mini-map
        return transform.position.x <= MinimapCam.position.x - 30.5f || 
               transform.position.x >= MinimapCam.position.x + 30.5f || 
               transform.position.y <= MinimapCam.position.y - 17.5f || 
               transform.position.y >= MinimapCam.position.y + 17.5f;
    }
}
