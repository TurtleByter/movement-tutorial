using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Variables")]
    public float playerWalkSpeed = 8;
    public float playerJumpForce = 6;

    [Header("Input Varibles")]
    public float inputSmoothTime = 0.1f;
    Vector3 movementVector;
    Vector2 inputDelta;
    [HideInInspector]
    public Vector2 currentInputDelta;
    Vector2 refrenceInputDelta;

    [Header("Gravity Varibles")]
    public LayerMask groundMask;
    public bool grounded;
    public float gravity = -12f;
    public float verticalVelocity;
    Vector3 gravityVector;

    CharacterController characterController;

    // Start is called before the first frame update
    void Start()
    {
        characterController = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        grounded = GroundCheck();

        ApplyPlayerMovement();
    }

    // Use raycast to check if player is grounded
    bool GroundCheck()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, -transform.up, out hit, 0.2f, groundMask))
            return true;
        else
            return false;
    }

    // Getting player input and calculating the movement vector
    Vector3 UpdateMovementVector()
    {
        inputDelta = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        inputDelta.Normalize();

        currentInputDelta = Vector2.SmoothDamp(currentInputDelta, inputDelta, ref refrenceInputDelta, inputSmoothTime);
        movementVector = (transform.forward * currentInputDelta.y + transform.right * currentInputDelta.x) * playerWalkSpeed;

        return movementVector;
    }

    // calculating the player gravity vector
    Vector3 UpdateGravityVector()
    {
        if (grounded)
        {
            if (Input.GetKeyDown(KeyCode.Space))
                verticalVelocity = playerJumpForce;
            else if(characterController.isGrounded)
                verticalVelocity = 0;
        }
        else
        {
            verticalVelocity += gravity * Time.deltaTime;
            verticalVelocity = Mathf.Clamp(verticalVelocity, -40, 100);
        }

        gravityVector = transform.up * verticalVelocity;
        return gravityVector;
    }

    void ApplyPlayerMovement()
    {
        Vector3 _playerVelocity = UpdateMovementVector() + UpdateGravityVector();
        characterController.Move(_playerVelocity * Time.deltaTime);
    }
}
