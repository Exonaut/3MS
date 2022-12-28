using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovementController : MonoBehaviour
{
    [FoldoutGroup("Dependencies", expanded: true)]
    [FoldoutGroup("Dependencies")][SerializeField, SceneObjectsOnly, Required] Camera playerCamera;
    [FoldoutGroup("Dependencies")][SerializeField, SceneObjectsOnly, Required] Logger logger;

    [FoldoutGroup("Masks", expanded: true)]
    [FoldoutGroup("Masks")] public LayerMask crouchMask;
    [FoldoutGroup("Masks")] public LayerMask slopeMask;

    [FoldoutGroup("Look Settings", expanded: true)]
    [FoldoutGroup("Look Settings")][Range(0f, 5f)] public float lookSpeed = 1.0f;
    [FoldoutGroup("Look Settings")][Range(0f, 2f)] public float mouseSensitivity = 0.6f;
    [FoldoutGroup("Look Settings")][MinMaxSlider(-90, 90, true)] public Vector2 verticalLookLimit = new Vector2(-90, 90);

    [FoldoutGroup("Ground Settings", expanded: true)]
    [FoldoutGroup("Ground Settings")][Range(0f, 50f)] public float moveSpeed = 10f;
    [FoldoutGroup("Ground Settings")][Range(0f, 50f)] public float sprintSpeed = 15f;
    [FoldoutGroup("Ground Settings")][Range(0f, 20f)] public float groundingStrength = 5f;

    [FoldoutGroup("Jump Settings", expanded: true)]
    [FoldoutGroup("Jump Settings")][Range(0f, 10f)] public float jumpStrength = 5f;
    [FoldoutGroup("Jump Settings")][Range(0f, 50f)] public float airMoveSpeed = 5f;
    [FoldoutGroup("Jump Settings")][Range(0f, 4f)] public float jumpAllowedDelay = 0.5f;
    [FoldoutGroup("Jump Settings")][Range(0f, 4f)] public float jumpBufferTime = 0.5f;
    [FoldoutGroup("Jump Settings")][Range(0f, 4f)] public float jumpGravityMultiplier = 1.0f;
    [FoldoutGroup("Jump Settings")][Range(0f, 4f)] public float fallGravityMultiplier = 1.0f;
    [FoldoutGroup("Jump Settings")][Range(0f, 5f)] public float airGroundingStrength = 1.0f;

    [FoldoutGroup("Crouch Settings", expanded: true)]
    [FoldoutGroup("Crouch Settings")][Min(.5f)] public float standingHeight = 2f;
    [FoldoutGroup("Crouch Settings")][Min(.5f)] public float crouchingHeight = 1f;
    [FoldoutGroup("Crouch Settings")][Range(0f, 50f)] public float crouchSpeed = 7f;

    InputHandler inputHandler;
    CharacterController characterController;

    bool isSprinting = false;
    bool crouchButtonPressed = false;
    bool isCrouching = false;
    bool canJump = true;
    bool sliding = false;
    bool jumpBuffer = false;
    bool holdJump = false;

    Vector3 prevSpeed = Vector3.zero;
    Vector2 prevLook = Vector2.zero;

    private void OnEnable()
    {
        // Find dependencies
        inputHandler = FindObjectOfType<InputHandler>();
        characterController = GetComponent<CharacterController>();

        logger = logger == null ? Logger.GetDefaultLogger(this) : logger;

        // Subscribe to input events
        if (inputHandler)
        {
            inputHandler.onMove += OnMove;
            inputHandler.onLook += OnLook;
            inputHandler.onJumpStart += OnJumpStart;
            inputHandler.onJumpStop += OnJumpStop;
            inputHandler.onSprint += OnSprint;
            inputHandler.onCrouch += OnCrouch;
        }
        else
        {
            logger?.LogWarning("InputHandler not found", this);
        }

        // Mouse settings
        Cursor.lockState = CursorLockMode.Locked;

        mouseSensitivity = PlayerPrefs.GetFloat(GameGlobals.key_MouseSensitivity, 0.5f);

        logger?.Log("PlayerMovementController enabled", this);
    }

    private void OnDisable()
    {
        if (inputHandler)
        {
            inputHandler.onMove -= OnMove;
            inputHandler.onLook -= OnLook;
            inputHandler.onJumpStart -= OnJumpStart;
            inputHandler.onJumpStop -= OnJumpStop;
            inputHandler.onSprint -= OnSprint;
            inputHandler.onCrouch -= OnCrouch;
        }
        else
        {
            logger?.LogWarning("InputHandler not found", this);
        }

        logger?.Log("PlayerMovementController disabled", this);
    }

    void Update()
    {
        if (!characterController) return;
        if (!playerCamera) return;

        if (characterController.isGrounded && !sliding)
        {
            canJump = true;
            StopCoroutine(JumpDelay());
            StartCoroutine(JumpDelay());
        }

        if (canJump && jumpBuffer || canJump && holdJump)
        {
            dir.y = jumpStrength;
            canJump = false;
        }

        LookPlayer();
        MovePlayer();
        CrouchPlayer();
    }

    Vector3 moveDirection = Vector3.zero;
    /// <summary>
    /// Stores the move direction of the input event
    /// </summary>
    /// <param name="direction"></param>
    void OnMove(Vector2 direction)
    {
        moveDirection = new Vector3(direction.x, 0f, direction.y);
    }

    float cameraAngle = 0f;
    /// <summary>
    /// Move camera up/down and body left/right on look input event
    /// </summary>
    /// <param name="delta">The looking delta to apply</param>
    void OnLook(Vector2 delta)
    {
        prevLook = delta;
    }

    Vector3 dir = Vector3.zero;
    /// <summary>
    /// Move the player horizontally
    /// </summary>
    void MovePlayer()
    {
        if (!characterController.isGrounded)
        {
            MoveAir();
        }
        else // Enable movement while on the ground
        {
            MoveGround();
        }

        characterController.Move(dir * Time.deltaTime); // Apply movement

        LimitVerticalSpeed();
        LimitHorizontalSpeed();

        prevSpeed = characterController.velocity;

    }

    void LookPlayer()
    {
        var delta = prevLook;
        cameraAngle -= delta.y * lookSpeed * mouseSensitivity;
        cameraAngle = Mathf.Clamp(cameraAngle, -verticalLookLimit.y, -verticalLookLimit.x);
        playerCamera.transform.localRotation = Quaternion.Euler(cameraAngle, 0, 0);
        transform.rotation *= Quaternion.Euler(0, delta.x * mouseSensitivity, 0);
    }

    void LimitVerticalSpeed()
    {
        if (characterController.isGrounded) return;

        if (characterController.velocity.y < dir.y)
        {
            dir.y = characterController.velocity.y;
        }
    }

    private void LimitHorizontalSpeed()
    {
        if (!characterController.isGrounded) return;

        // Limit horizontal speed when hitting wall
        if (Mathf.Abs(characterController.velocity.x) < Mathf.Abs(dir.x))
        {
            dir.x = characterController.velocity.x;
        }

        if (Mathf.Abs(characterController.velocity.z) < Mathf.Abs(dir.z))
        {
            dir.z = characterController.velocity.z;
        }
    }

    private void MoveGround()
    {
        Vector3 td = characterController.transform.TransformDirection(moveDirection);
        float speed = (isCrouching ? crouchSpeed : (isSprinting ? sprintSpeed : moveSpeed));
        dir.x = td.x * speed;
        dir.z = td.z * speed;

        SlopeSlip();

        ApplyGroundingForce();
    }

    void SlopeSlip()
    {
        // Slide down slopes
        sliding = false;
        float rad = characterController.radius;
        RaycastHit hit;  // Get closest surface
                         // if (Physics.SphereCast(transform.position + transform.TransformDirection(Vector3.up) * rad, rad,
                         //     transform.TransformDirection(Vector3.down), out hit, rad * 2f, slopeMask))
                         // {
        if (Physics.SphereCast(transform.position, rad,
        transform.TransformDirection(Vector3.down), out hit, characterController.height, slopeMask))
        {
            // RaycastHit pHit; // Redo raycast to remove 
            // if (Physics.Raycast(transform.position + transform.TransformDirection(Vector3.up) * rad,
            //     -((transform.position + transform.TransformDirection(Vector3.up) * rad) - hit.point),
            //     out pHit, rad * 1.5f, slopeMask))
            // {
            float angle = Vector3.Angle(hit.normal, transform.TransformDirection(Vector3.up));
            if (angle > characterController.slopeLimit && angle < 90f)
            {
                Vector3 slopeSlide = Vector3.RotateTowards(hit.normal, transform.TransformDirection(Vector3.down), Mathf.PI / 2, 100f);

                Vector3 vel = characterController.velocity;
                vel.y = 0f;
                dir = vel + slopeSlide * GameGlobals.gravity * Time.deltaTime;

                sliding = true;
            }
            // }
        }
    }

    void ApplyGroundingForce()
    {
        if (dir.y <= 0)
        {
            dir.y = -groundingStrength;
        }
    }

    private void MoveAir()
    {
        // Calcualte total gravity
        float totalGravity = GameGlobals.gravity * Time.deltaTime;
        totalGravity *= dir.y > 0 ? jumpGravityMultiplier : fallGravityMultiplier;
        totalGravity *= !holdJump ? airGroundingStrength : 1.0f;

        dir.y -= totalGravity; // Apply gravity

        // Enable air movement adjustement
        Vector3 td = characterController.transform.TransformDirection(moveDirection);
        td *= moveSpeed;
        td.y = dir.y;
        dir = Vector3.MoveTowards(dir, td, airMoveSpeed * Time.deltaTime);
    }

    void OnJumpStart(Object caller)
    {
        jumpBuffer = true;
        holdJump = true;
        StopCoroutine(JumpBuffer());
        StartCoroutine(JumpBuffer());
    }

    void OnJumpStop(Object caller)
    {
        holdJump = false;
    }

    IEnumerator JumpDelay()
    {
        yield return new WaitForSeconds(jumpAllowedDelay);
        if (!characterController.isGrounded)
        {
            canJump = false;
        }
    }

    IEnumerator JumpBuffer()
    {
        yield return new WaitForSeconds(jumpBufferTime);
        jumpBuffer = false;
    }

    void OnSprint(bool val)
    {
        isSprinting = val;
    }

    void OnCrouch(bool val)
    {
        crouchButtonPressed = val;
    }

    void CrouchPlayer()
    {
        if (crouchButtonPressed && !isCrouching)
        {
            characterController.height = crouchingHeight;
            characterController.Move(new Vector3(0, -(standingHeight - crouchingHeight) / 2, 0));
            isCrouching = true;
        }
        else if (!crouchButtonPressed && isCrouching)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.up), out hit, standingHeight + 0.25f, crouchMask))
            {
                return;
            }

            characterController.height = standingHeight;
            isCrouching = false;
        }
    }

    void OnDestroy()
    {
        logger?.Log("PlayerMovementController destroyed", this);
    }
}
