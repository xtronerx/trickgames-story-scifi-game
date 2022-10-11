using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerMovement : MonoBehaviour
{
    #region Public Variables
    [Header("Movement Variables")]
    [Range(1,15)]
    [SerializeField] private float walkSpeed;
    [Space(3)]
    [Range(1,30)]
    [SerializeField] private float sprintSpeed;
    [Space(3)]
    [Range(0.5f,5)]
    [SerializeField] private float groundDrag;
    [Space(3)]
    [Range(1,30)]
    [SerializeField] private float gravityForce;

    [Space(15)]

    [Header("Jumping Variables")]
    [Range(1, 30)]
    [SerializeField] private float jumpForce;
    [Space(3)]
    [Range(0, 5)]
    [SerializeField] private float jumpCooldown;
    [Space(3)]
    [Range(0, 5)]
    [Tooltip("How fast the player can move with contols while in the air")]
    [SerializeField] private float airMultiplier;

    [Space(15)]

    [Header("Crouching Variables")]
    [Range(1, 10)]
    [SerializeField] private float crouchSpeed;
    [Space(3)]
    [Range(0.5f, 3)]
    [Tooltip("'1' means there will be no change in height when the player crouches. G" +
        "reater that '1' means the colider will increase in size. " +
        "Less that '1' means that the colider will decrease in size.")]
    [SerializeField] private float crouchYScale;

    [Space(15)]

    [Header("Input Variables")]
    [SerializeField] private KeyCode jumpKey = KeyCode.Space;
    [Space(3)]
    [SerializeField] private KeyCode sprintKey = KeyCode.LeftShift;
    [Space(3)]
    [SerializeField] private KeyCode crouchKey = KeyCode.LeftControl;

    [Space(15)]

    [Header("Ground Check Variables")]
    [Range(1, 3)]
    [SerializeField] private float playerHeight;
    [Space(3)]
    [SerializeField] private LayerMask whatIsGround;

    [Space(15)]

    [Header("Slope Handling Variables")]
    [Range(0, 80)]
    [SerializeField] private float maxSlopeAngle;

    [Space(15)]

    [Header("Player Face-Direction Variables")]
    [Tooltip("The player will move in the forward direction by wherever the blue arrow points on the object in this field")]
    [SerializeField] private Transform playerOrientation;
    #endregion

    #region Private Variables
    float horizontalInput;
    float verticalInput;
    float moveSpeed;
    Vector3 moveDirection;
    Rigidbody rb;
    RaycastHit slopeHit;
    bool exitingSlope;
    float startYScale;
    bool grounded;
    bool readyToJump;
    MovementState state;
    #endregion

    public enum MovementState
    {
        walking,
        sprinting,
        crouching,
        air
    }

    #region Standard Functions
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
       
        readyToJump = true;

        startYScale = transform.localScale.y;
    }
    private void Update()
    {
        // ground check
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);

        MyInput();
        SpeedControl();
        StateHandler();
        

        // handle drag
        if (grounded)
            rb.drag = groundDrag;
        else
            rb.drag = 0;

        Gravity();
    }
    private void FixedUpdate()
    {
        MovePlayer();
    }
    #endregion

    #region Custom Functions
    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        // when to jump
        if (Input.GetKey(jumpKey) && readyToJump && grounded)
        {
            readyToJump = false;

            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
        }

        // start crouch
        if (Input.GetKeyDown(crouchKey))
        {
            transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
            rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
        }

        // stop crouch
        if (Input.GetKeyUp(crouchKey))
        {
            transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);
        }
    }
    private void StateHandler()
    {
        // Mode - Crouching
        if (Input.GetKey(crouchKey))
        {
            state = MovementState.crouching;
            moveSpeed = crouchSpeed;
        }

        // Mode - Sprinting
        else if (grounded && Input.GetKey(sprintKey))
        {
            state = MovementState.sprinting;
            moveSpeed = sprintSpeed;
        }

        // Mode - Walking
        else if (grounded)
        {
            state = MovementState.walking;
            moveSpeed = walkSpeed;
        }

        // Mode - Air
        else
        {
            state = MovementState.air;
        }
    }
    private void MovePlayer()
    {      
        // calculate movement direction
        moveDirection = playerOrientation.forward * verticalInput + playerOrientation.right * horizontalInput;

        // on slope
        if (OnSlope() && !exitingSlope)
        {
            rb.AddForce(GetSlopeMoveDirection() * moveSpeed * 20f, ForceMode.Force);

            if (rb.velocity.y > 0)
                rb.AddForce(Vector3.down * 80f, ForceMode.Force);
        }

        // on ground
        else if (grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);

        // in air
        else if (!grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);

        // turn gravity off while on slope
        rb.useGravity = !OnSlope();
    }
    private void SpeedControl()
    {
        // limiting speed on slope
        if (OnSlope() && !exitingSlope)
        {
            if (rb.velocity.magnitude > moveSpeed)
                rb.velocity = rb.velocity.normalized * moveSpeed;
        }

        // limiting speed on ground or in air
        else
        {
            Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

            // limit velocity if needed
            if (flatVel.magnitude > moveSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * moveSpeed;
                rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
            }
        }
    }
    private void Jump()
    {
        exitingSlope = true;

        // reset y velocity
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }
    private void ResetJump()
    {
        readyToJump = true;

        exitingSlope = false;
    }
    private bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.3f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }

        return false;
    }
    private Vector3 GetSlopeMoveDirection()
    {
        return Vector3.ProjectOnPlane(moveDirection, slopeHit.normal).normalized;
    }
    private void Gravity()
    {
        if (grounded) return;

        float gravity = 0f;
        gravity = rb.velocity.y;
        gravity += -gravityForce * Time.deltaTime;
        rb.velocity = new Vector3(rb.velocity.x, gravity, rb.velocity.z);
    }
    #endregion
}
