using UnityEngine;
using System.Collections;

public class PlayerMove : MonoBehaviour
{
    [Header("Player Movement")]
    public float playerSpeed = 18f;
    public float sprintSpeed = 28f;
    public float crouchSpeed = 3f;
    public float jumpHeight = 3f;
    public float gravity = -40f;

    [Header("Crouching")]
    public float crouchHeight = 0.8f;
    private float standingHeight;

    [Header("Sliding")]
    public float slideSpeed = 35f;
    private Vector3 slideDirection;

    [Header("Dashing")]
    public float dashSpeed = 40f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 0.3f;
    private float dashCooldownTimer = 0f;

    [Header("Wall Mechanics")]
    public LayerMask whatIsWall;
    public float wallCheckDistance = 0.5f;
    public float wallSlideSpeed = 2f;
    [Space]
    public float wallJumpUpForce = 7f;
    public float wallJumpSideForce = 5f;
    [Space]
    public float wallRunSpeed = 15f;
    public float wallClimbSpeed = 3f;
    public float wallRunCameraTilt = 15f;
    public float maxWallRunTime = 1.5f;
    private float wallRunTimer;


    [Header("Camera Effects")]
    public Camera playerCameraComponent;
    public float cameraChangeSpeed = 8f;
    [Space]
    public bool useHeadbob = true;
    public float headbobFrequency = 10f;
    public float headbobAmplitude = 0.1f;
    [Space]
    public float dashFOV = 90f;
    private float normalFOV;
    private float headbobTimer = 0f;

    [Header("Mouse Look")]
    public Transform playerCamera;
    public float mouseSensitivity = 150f;
    private float xRotation = 0f;

    // --- Private Variables ---
    private CharacterController playercc;
    private Vector3 playerVelocity;
    private bool isGrounded;
    private bool isPlayerWalking;
    private bool isCrouching = false;
    private bool isSliding = false;
    private bool isDashing = false;
    private bool isWallSliding = false;
    private bool isWallRunning = false;
    private RaycastHit leftWallHit;
    private RaycastHit rightWallHit;
    private bool wallLeft;
    private bool wallRight;
    private float moveX;
    private float moveZ;
    private Vector3 standingCameraPos;
    private Vector3 crouchCameraPos;

    void Start()
    {
        playercc = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;

        if (playerCameraComponent != null)
            normalFOV = playerCameraComponent.fieldOfView;

        standingHeight = playercc.height;
        standingCameraPos = playerCamera.localPosition;
        crouchCameraPos = new Vector3(standingCameraPos.x, standingCameraPos.y - (standingHeight - crouchHeight), standingCameraPos.z);

        // Initialize wall run timer
        wallRunTimer = maxWallRunTime;
    }

    void Update()
    {
        HandleCameraEffects();
        CheckForWall();
        HandleWallSlidingState();

        if (dashCooldownTimer > 0) dashCooldownTimer -= Time.deltaTime;
        if (Input.GetKeyDown(KeyCode.Q) && dashCooldownTimer <= 0) StartCoroutine(Dash());

        moveX = Input.GetAxis("Horizontal");
        moveZ = Input.GetAxis("Vertical");

        HandleMovement();
        HandleMouseLook();
        HandleCrouchAndSlide();
    }

    private void HandleCameraEffects()
    {
        float targetFOV = isDashing ? dashFOV : normalFOV;
        if (playerCameraComponent != null)
            playerCameraComponent.fieldOfView = Mathf.Lerp(playerCameraComponent.fieldOfView, targetFOV, Time.deltaTime * cameraChangeSpeed);

        Vector3 targetCameraPos = isCrouching ? crouchCameraPos : standingCameraPos;

        if (useHeadbob && isPlayerWalking && isGrounded && !isSliding)
        {
            float currentSpeed = new Vector2(playercc.velocity.x, playercc.velocity.z).magnitude;
            headbobTimer += Time.deltaTime * currentSpeed;
            targetCameraPos.y += Mathf.Sin(headbobTimer * headbobFrequency) * headbobAmplitude;
            targetCameraPos.x += Mathf.Cos(headbobTimer * headbobFrequency / 2) * headbobAmplitude / 2;
        }

        playerCamera.localPosition = Vector3.Lerp(playerCamera.localPosition, targetCameraPos, Time.deltaTime * cameraChangeSpeed);
    }

    private void CheckForWall()
    {
        wallRight = Physics.Raycast(transform.position, transform.right, out rightWallHit, wallCheckDistance, whatIsWall);
        wallLeft = Physics.Raycast(transform.position, -transform.right, out leftWallHit, wallCheckDistance, whatIsWall);
    }

    private void HandleWallSlidingState()
    {
        isWallSliding = !isGrounded && (wallLeft || wallRight) && !isWallRunning;
    }

    private void StartWallRun()
    {
        isWallRunning = true;
        playerVelocity = new Vector3(playercc.velocity.x, 0f, playercc.velocity.z);
    }

    private void StopWallRun()
    {
        isWallRunning = false;
    }

    private void WallRunningMovement()
    {
        wallRunTimer -= Time.deltaTime;
        if (wallRunTimer <= 0)
        {
            StopWallRun();
            return;
        }

        playerVelocity.y = 0; // Prevent sliding down initially

        Vector3 wallNormal = wallRight ? rightWallHit.normal : leftWallHit.normal;
        Vector3 wallForward = Vector3.Cross(wallNormal, transform.up);

        if (Vector3.Dot(transform.forward, wallForward) < 0)
        {
            wallForward = -wallForward;
        }

        playercc.Move(wallForward * wallRunSpeed * Time.deltaTime);

        if (Input.GetKey(KeyCode.W))
        {
            playercc.Move(transform.up * wallClimbSpeed * Time.deltaTime);
        }
    }

    void HandleMovement()
    {
        isGrounded = playercc.isGrounded;
        if (isGrounded && playerVelocity.y < 0)
        {
            playerVelocity.y = -2f;
            if (wallRunTimer < maxWallRunTime) wallRunTimer = maxWallRunTime;
        }

        if (isWallRunning)
        {
            WallRunningMovement();
        }

        if (isDashing) return;

        if (!isWallRunning)
        {
            bool isSprinting = Input.GetKey(KeyCode.LeftShift) && !isCrouching && moveZ > 0;
            float currentSpeed = isCrouching ? crouchSpeed : (isSprinting ? sprintSpeed : playerSpeed);
            Vector3 moveDirection = transform.right * moveX + transform.forward * moveZ;

            if (isSliding) playercc.Move(slideDirection * slideSpeed * Time.deltaTime);
            else playercc.Move(moveDirection * currentSpeed * Time.deltaTime);
        }

        isPlayerWalking = new Vector2(playercc.velocity.x, playercc.velocity.z).magnitude >= 0.1f;

        if (Input.GetButtonDown("Jump"))
        {
            if (isWallRunning) WallJump();
            else if ((wallLeft || wallRight) && !isGrounded && wallRunTimer > 0) StartWallRun();
            else if (isSliding)
            {
                playerVelocity = slideDirection * slideSpeed;
                playerVelocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }
            else if (isGrounded && !isCrouching)
            {
                playerVelocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }
        }

        if (isWallRunning && !wallLeft && !wallRight) StopWallRun();

        if (isWallSliding) playerVelocity.y = Mathf.Clamp(playerVelocity.y, -wallSlideSpeed, float.MaxValue);
        else if (!isWallRunning) playerVelocity.y += gravity * Time.deltaTime;

        if (!isWallRunning) playercc.Move(playerVelocity * Time.deltaTime);
    }

    private void WallJump()
    {
        StopWallRun();

        Vector3 wallNormal = wallRight ? rightWallHit.normal : leftWallHit.normal;
        Vector3 forceToApply = transform.up * wallJumpUpForce + wallNormal * wallJumpSideForce;
        playerVelocity.y = 0;
        playerVelocity += forceToApply;
    }

    void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        transform.Rotate(Vector3.up * mouseX);

        float targetTilt = isWallRunning ? (wallRight ? wallRunCameraTilt : -wallRunCameraTilt) : 0f;
        float currentTilt = Mathf.LerpAngle(playerCamera.localRotation.eulerAngles.z, targetTilt, Time.deltaTime * cameraChangeSpeed);
        playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, currentTilt);
    }

    void HandleCrouchAndSlide()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl) && isPlayerWalking && isGrounded) StartSlide();
        else if (Input.GetKeyUp(KeyCode.LeftControl)) StopSlide();

        float targetHeight = isCrouching ? crouchHeight : standingHeight;
        playercc.height = Mathf.Lerp(playercc.height, targetHeight, Time.deltaTime * cameraChangeSpeed);
    }

    private void StartSlide()
    {
        isSliding = true; isCrouching = true;
        slideDirection = (transform.forward * moveZ + transform.right * moveX).normalized;
    }

    private void StopSlide()
    {
        isSliding = false; isCrouching = false;
    }

    private IEnumerator Dash()
    {
        isDashing = true;
        dashCooldownTimer = dashCooldown;
        float originalGravity = gravity;
        gravity = 0f;
        Vector3 dashDirection = (transform.forward * moveZ + transform.right * moveX);
        if (dashDirection.magnitude < 0.1f) dashDirection = transform.forward;
        float timer = 0f;
        while (timer < dashDuration)
        {
            playercc.Move(dashDirection.normalized * dashSpeed * Time.deltaTime);
            timer += Time.deltaTime;
            yield return null;
        }
        gravity = originalGravity;
        isDashing = false;
    }
}