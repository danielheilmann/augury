using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Mouse Camera Driver && First-Person WASD Movement
public class DebugCharController : MonoBehaviour
{
    [SerializeField] private float movementSpeed = 12f;
    [SerializeField] private float mouseSensitivity = 500f;
    private new Camera camera;
    private float xRotation;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        camera = GetComponentInChildren<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        MouseCameraDriver();
        KeyboardMovement();
    }

    // TODO: The camera rotation seems to be heavily affected by sudden (massive) framedrops, making rotation feel very jagged.
    private void MouseCameraDriver()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
        // Vector2 mouseInput = new Vector2(mouseX, mouseY);

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -80f, 80);

        camera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(transform.up * mouseX);
    }

    private void KeyboardMovement()
    {
        float inputHorizontal = Input.GetAxis("Horizontal");
        float inputVertical = Input.GetAxis("Vertical");

        Vector3 movementVector = transform.right * inputHorizontal + transform.forward * inputVertical;

        transform.position += movementVector * movementSpeed * Time.deltaTime;
    }
}
