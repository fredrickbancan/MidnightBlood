using UnityEngine;

public class MouseViewAlpha : MonoBehaviour
{

    public float mouseSensitivity = 100f;

    public Transform playerBody;

    float xRotation = 0f;
    float yRotation = 0f;
    float prevMouseX;

    // Start is called before the first frame update
    void Start()
    {
        // To lock the mouse cursor to the gamescene, stopping it from travelling out of the scene.
        Cursor.lockState = CursorLockMode.Confined;
    }

    // Update is called once per frame
    void Update()
    {
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        xRotation -= mouseY;
        // Clamp the camera to limit it from being able to look to far down, ending up looking behind the player
        // So we say you can look as far as straight up or straight down.
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        if (!GameManagerAlpha.instance.paused)
        {
            Cursor.visible = false;
            transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;

            float deltaMouseX = mouseX - prevMouseX;
            yRotation += deltaMouseX;
            playerBody.Rotate(Vector3.up * yRotation);
            prevMouseX = mouseX;
        }
    }
}
