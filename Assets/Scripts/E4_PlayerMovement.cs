using UnityEngine;
using UnityEngine.InputSystem;

public class E4_PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;

    public float mouseSensitivity = 120f;
    public Transform cameraTransform;

    public Transform fpsPoint;
    public Transform tpsPoint;

    Rigidbody rb;
    E4_InputAction input;

    Vector2 moveInput;
    float xRotation;
    bool isFPS = true;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        input = new E4_InputAction();
    }

    void OnEnable()
    {
        input.Player.Enable();
        input.Player.SwitchView.performed += OnSwitchView;
    }

    void OnDisable()
    {
        input.Player.SwitchView.performed -= OnSwitchView;
        input.Player.Disable();
    }

    void Start()
    {
        SetCameraView();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        moveInput = input.Player.Move.ReadValue<Vector2>();
        HandleLook();
    }

    //void FixedUpdate()
    //{
    //    HandleMove();
    //}

    //void HandleMove()
    //{
    //    // Calculate movement direction based on input
    //    Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;

    //    // Apply movement with Rigidbody
    //    Vector3 velocity = move * moveSpeed;
    //    velocity.y = rb.linearVelocity.y; // keep existing Y velocity (gravity)
    //    rb.linearVelocity = velocity;
    //}


    void HandleLook()
    {
        Vector2 lookInput = input.Player.Look.ReadValue<Vector2>();

        float mouseX = lookInput.x * mouseSensitivity * Time.deltaTime;
        float mouseY = lookInput.y * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -60f, 80f);

        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        rb.MoveRotation(rb.rotation * Quaternion.Euler(0f, mouseX, 0f));
    }

    void OnSwitchView(InputAction.CallbackContext ctx)
    {
        isFPS = !isFPS;
        SetCameraView();
    }

    void SetCameraView()
    {
        Transform target = isFPS ? fpsPoint : tpsPoint;
        cameraTransform.localPosition = target.localPosition;
        cameraTransform.localRotation = Quaternion.identity;
    }
}
