using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerMovement), typeof(CharacterController))]
public class PlayerCamera : MonoBehaviour
{
    [Header("Transform Varibles")]
    public Transform playerCamera;
    public Transform cameraHandle;
    float handlePitch;

    [Header("Input Varibles")]
    public float mouseSensitivity = 3f;
    public float mouseSmoothTime = 0.01f;
    Vector2 mouseInputDelta;
    Vector2 currentMouseDelta;
    Vector2 refrenceMouseDelta;

    [Header("Effects Varibles")]
    public bool hasMovementTilt = true;
    public float tiltMultiplyer = 1.5f;
    public bool hasCameraBob = true;
    public float cameraBobAmplitude = 0.1f;

    PlayerMovement playerMovement;
    CharacterController characterController;

    // Start is called before the first frame update
    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        characterController = GetComponent<CharacterController>();

        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateCameraLook();
    }

    // Get mouse and movement input, then apply camera transformations
    void UpdateCameraLook()
    {
        mouseInputDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        currentMouseDelta = Vector2.SmoothDamp(currentMouseDelta, mouseInputDelta, ref refrenceMouseDelta, mouseSmoothTime);

        handlePitch -= currentMouseDelta.y * mouseSensitivity;
        handlePitch = Mathf.Clamp(handlePitch, -90, 90);

        cameraHandle.localEulerAngles = Vector3.right * handlePitch;
        transform.Rotate(Vector3.up * currentMouseDelta.x * mouseSensitivity);

        if (hasMovementTilt)
            playerCamera.localEulerAngles = Vector3.forward * (-playerMovement.currentInputDelta.x * tiltMultiplyer);

        if (hasCameraBob)
            CameraBob(playerMovement.playerWalkSpeed);
    }

    // Apply camera bob transformations
    void CameraBob(float _playerSpeed)
    {
        if (playerMovement.currentInputDelta.magnitude >= 0.1f)
            playerCamera.localPosition = Vector3.Lerp(playerCamera.localPosition, FootStepMotion(_playerSpeed), Time.deltaTime * 10);
        else
            playerCamera.localPosition = Vector3.Lerp(playerCamera.localPosition, Vector3.zero, Time.deltaTime * 10);
    }

    // Calculates camera local position
    Vector3 FootStepMotion(float _frequency)
    {
        Vector3 _position = Vector3.zero;
        _position.y += Mathf.Sin(Time.time * _frequency) * cameraBobAmplitude * playerMovement.currentInputDelta.magnitude;
        _position.x += Mathf.Cos(Time.time * _frequency / 2) * cameraBobAmplitude * 2 * playerMovement.currentInputDelta.magnitude;
        return _position;
    }
}
