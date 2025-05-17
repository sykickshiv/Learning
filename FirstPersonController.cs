using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class FirstPersonController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 3.0f;
    public float sprintSpeed = 6.0f;

    [Header("Mouse Look Settings")]
    public float mouseSensitivity = 2.0f;
    public Transform playerCamera;

    [Header("Head Bob Settings")]
    public float bobFrequency = 2.4f;
    public float bobHorizontalAmplitude = 0.1f;
    public float bobVerticalAmplitude = 0.12f;

    [Header("Gravity Settings")]
    public float gravity = -9.81f;
    private float verticalVelocity = 0f;

    private CharacterController controller;
    private float pitch = 0f;
    private Vector3 originalCameraLocalPos;
    private float bobTimer = 0f;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;

        // Store original camera local position
        if (playerCamera != null)
            originalCameraLocalPos = playerCamera.localPosition;
    }

    void Update()
    {
        HandleLook();
        HandleMovement();
        HandleHeadBob();
    }

    void HandleLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, -75f, 75f);

        playerCamera.localRotation = Quaternion.Euler(pitch, 0, 0);
        transform.Rotate(Vector3.up * mouseX);
    }

    void HandleMovement()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 move = transform.right * moveX + transform.forward * moveZ;
        float speed = Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : walkSpeed;

        // Gravity handling
        if (controller.isGrounded)
        {
            verticalVelocity = -2f; // Keeps the character grounded
        }
        else
        {
            verticalVelocity += gravity * Time.deltaTime;
        }

        move.y = verticalVelocity; // Apply gravity to vertical movement
        controller.Move(move * speed * Time.deltaTime);
    }

    void HandleHeadBob()
    {
        if (controller.isGrounded && controller.velocity.magnitude > 0.1f)
        {
            // Determine if sprinting or walking
            bool isSprinting = Input.GetKey(KeyCode.LeftShift);
            float currentFrequency = isSprinting ? bobFrequency * 1.8f : bobFrequency;
            float currentHorizontalAmp = isSprinting ? bobHorizontalAmplitude * 1.5f : bobHorizontalAmplitude;
            float currentVerticalAmp = isSprinting ? bobVerticalAmplitude * 1.5f : bobVerticalAmplitude;

            bobTimer += Time.deltaTime * currentFrequency;
            float bobX = Mathf.Sin(bobTimer) * currentHorizontalAmp;
            float bobY = Mathf.Cos(bobTimer * 2f) * currentVerticalAmp;

            playerCamera.localPosition = originalCameraLocalPos + new Vector3(bobX, bobY, 0f);
        }
        else
        {
            // Smooth reset when idle
            playerCamera.localPosition = Vector3.Lerp(playerCamera.localPosition, originalCameraLocalPos, Time.deltaTime * 5f);
            bobTimer = 0f;
        }
    }
}
