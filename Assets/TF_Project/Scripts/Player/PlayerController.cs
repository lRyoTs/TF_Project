using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    #region Variables
    enum PlayerState
    {
        Gameplay,
        UI,
        Dead
    }
    [Header("References")]
    [SerializeField]
    private CharacterController _characterController;
    [SerializeField]
    private Animator _animator;

    [SerializeField]
    private Transform cameraTransform;

    [Header("Movement")]
    [SerializeField]
    private float playerSpeed = 5.0f;
    [SerializeField]
    private float jumpHeight = 1.0f;
    [SerializeField]
    private float gravityValue = -9.81f;
    [SerializeField]
    private Vector3 verticalVelocity;
    [SerializeField]
    [Range(0.0f, 0.3f)]
    private float RotationSmoothTime = 0.12f;

    private Vector2 movementDirection;
    private Vector2 lookDirection;
    private bool groundedPlayer;
    private bool isWalking;

    [Header("Dash")]
    private bool runPressed = true;
    private bool canDash = true;
    [SerializeField] private float dashSpeed;
    [SerializeField] private float dashTime;

    [Header("Stamina")]
    private int jumpStaminaCost = 2;
    private int dashStaminaCost = 5;
    private int shootStaminaCost = 5;

    [Header("Cinemachine")]
    [Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
    public GameObject CinemachineCameraTarget;

    [Tooltip("How far in degrees can you move the camera up")]
    public float TopClamp = 70.0f;

    [Tooltip("How far in degrees can you move the camera down")]
    public float BottomClamp = -30.0f;

    [Tooltip("Additional degress to override the camera. Useful for fine tuning camera position when locked")]
    public float CameraAngleOverride = 0.0f;

    [Tooltip("For locking the camera position on all axis")]
    public bool LockCameraPosition = false;
    private float sensitivity = 1f;
    // cinemachine
    private float _cinemachineTargetYaw;
    private float _cinemachineTargetPitch;

    private const float _threshold = 0.01f;

    [Header("Shoot")]
    [SerializeField] private Transform ProjectileSpawnPosition;
    [SerializeField] private ParticleSystem BulletPrefab;
   
    private PlayerState _playerState = PlayerState.Gameplay;

    //Animations parameters
    private float velocityZ = .0f;
    private float velocityX = .0f;
    private const float MAXIMUN_WALK_VELOCITY = 0.5f;
    private const float MAXIMUN_RUN_VELOCITY = 2f;

    //player
    private float rotationVelocity;
    #endregion


    private void Start()
    {
        InputActionsManager.ToggleActionMap(InputActionsManager.inputActions.General);
        runPressed = false;
    }

    private void Update()
    {
        switch (_playerState)
        {
            case PlayerState.Gameplay:
                ApplyGravity();
                HandleMovement();
                break;
            case PlayerState.Dead:
            case PlayerState.UI:
                HandleUINavigation();
                break;
        }    
    }

    private void LateUpdate()
    {
        switch (_playerState)
        {
            case PlayerState.Gameplay:
                UpdateAnimations();
                Look();
                break;
            case PlayerState.Dead:
                break;
            case PlayerState.UI:
                break;
        }

    }

    //Gameplay
    #region Gameplay
    private void ApplyGravity()
    {
        verticalVelocity.y += gravityValue * Time.deltaTime;
        //Stop velocity for dropping infinitely when grounded 
        if (verticalVelocity.y < 0.0f)
        {
            verticalVelocity.y = -2f;
        }
    }

    private Vector3 GetMoveDirection()
    {

        Vector3 direction = new Vector3(movementDirection.x, 0, movementDirection.y).normalized;
        return direction;
    }

    private void ChangeVelocity(float directionZ,float directionX, float currentMaxVelocity)
    {
        if(directionZ > 0f && velocityZ < currentMaxVelocity) //Forward
        {
            velocityZ += Time.deltaTime * 2f;
        }
        if(directionZ < 0f && velocityZ > -currentMaxVelocity) //Backward
        {
            velocityZ -= Time.deltaTime * 2f;
        }
        if(directionX < 0f && velocityX > -currentMaxVelocity) //Left
        {
            velocityX -= Time.deltaTime * 2f;
        }
        if(directionX > 0f && velocityX < currentMaxVelocity) //Right
        {
            velocityX += Time.deltaTime * 2f;
        }

        //decrease VelocityZ
        if(directionZ == 0)
        {
            velocityZ += velocityZ > 0f ? Time.deltaTime * -2f : Time.deltaTime * 2f;
            if (velocityZ < 0.1f || velocityZ > -0.1f) velocityZ = 0f;
        }
        if(directionX == 0)
        {
            velocityX += velocityX > 0f ? Time.deltaTime * -2f : Time.deltaTime * 2f;
            if (velocityX < 0.1f || velocityX > -0.1f) velocityX = 0f;
        }
    }

    private void HandleMovement()
    {
        float targetSpeed;
        float currentMaxVelocity = !runPressed ? 0.5f : 2f;
        if(movementDirection != Vector2.zero)
        {
            targetSpeed = !runPressed ? playerSpeed/2 : playerSpeed;
        }
        else
        {
            targetSpeed = 0;
        }

        Vector3 direction = GetMoveDirection();

        //direction = direction.x * cameraTransform.right.normalized + direction.z * cameraTransform.forward.normalized;

        ChangeVelocity(direction.z, direction.x, currentMaxVelocity);

        //Rotate towards camera
        float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;
        float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle,ref rotationVelocity, RotationSmoothTime);

        transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);

        Vector3 targetDirection = Quaternion.Euler(0.0f, targetAngle, 0.0f) * Vector3.forward;

        //Move player
        _characterController.Move((targetDirection.normalized * targetSpeed * Time.deltaTime) + verticalVelocity * Time.deltaTime);
    }

    /// <summary>
    /// Move virtual camera to aling to the desired camera angle
    /// </summary>
    private void Look()
    {
        // if there is an input and camera position is not fixed
        if (lookDirection.sqrMagnitude >= _threshold && !LockCameraPosition)
        {
            _cinemachineTargetYaw += lookDirection.x * Time.deltaTime * sensitivity;
            _cinemachineTargetPitch += lookDirection.y * Time.deltaTime * sensitivity;
        }

        // clamp our rotations so our values are limited 360 degrees
        _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
        _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

        // Cinemachine will follow this target
        CinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch + CameraAngleOverride,
            _cinemachineTargetYaw, 0.0f);
        //transform.transform.rotation = Quaternion.Euler(0.0f,CinemachineCameraTarget.transform.rotation.y,0.0f);
    }

    /// <summary>
    /// Limits camera angle
    /// </summary>
    /// <param name="lfAngle">CurrentAngle</param>
    /// <param name="lfMin">MinAngle</param>
    /// <param name="lfMax">MaxAngle</param>
    /// <returns></returns>
    private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle < -360f) lfAngle += 360f;
        if (lfAngle > 360f) lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }

    public void SetSensitivity(float newSensitivity)
    {
        sensitivity = newSensitivity;
    }

    private void UpdateAnimations()
    {
        //Movement Animation
        _animator.SetFloat("Velocity Z", velocityZ);
        _animator.SetFloat("Velocity X", velocityX);
    }

    public void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
    }
    #endregion

    //UI
    #region UI
    private void HandleUINavigation()
    {

    }
    #endregion

    #region Input Related Functions
    private void DoMove(InputAction.CallbackContext obj)
    {
        movementDirection = obj.ReadValue<Vector2>();
    }

    private void DoLook(InputAction.CallbackContext obj)
    {
        lookDirection = obj.ReadValue<Vector2>();
        Debug.Log(lookDirection);
    }

    private void DoJump(InputAction.CallbackContext obj)
    {
        groundedPlayer = _characterController.isGrounded;
        if (groundedPlayer)
        {
            verticalVelocity.y = Mathf.Sqrt(jumpHeight * -2 * gravityValue); //velocity to reach desired height
            _animator.SetTrigger("Jump");
        }
    }

    private void DoPause(InputAction.CallbackContext obj)
    {
        //InputActionsManager.ToggleActionMap(InputActionsManager.inputActions.UI);
    }

    private void DoRun(InputAction.CallbackContext obj)
    {
        if (obj.canceled)
        {
            runPressed = false;
        }
        else
        {
            runPressed = true;
        }
    }
    #endregion

    private void OnEnable()
    {
        //General Subscribe
        InputActionsManager.inputActions.General.Jump.started += DoJump;
        InputActionsManager.inputActions.General.Pause.started += DoPause;

        InputActionsManager.inputActions.General.Run.started += DoRun;
        InputActionsManager.inputActions.General.Run.performed += DoRun;
        InputActionsManager.inputActions.General.Run.canceled += DoRun;

        InputActionsManager.inputActions.General.Movement.started += DoMove;
        InputActionsManager.inputActions.General.Movement.performed += DoMove;
        InputActionsManager.inputActions.General.Movement.canceled += DoMove;
        
        InputActionsManager.inputActions.General.Look.started += DoLook;
        InputActionsManager.inputActions.General.Look.performed += DoLook;
        InputActionsManager.inputActions.General.Look.canceled += DoLook;
    }

    private void OnDisable()
    {
        //General Unsubcribe
        InputActionsManager.inputActions.General.Jump.started -= DoJump;
        InputActionsManager.inputActions.General.Pause.started -= DoPause;

        InputActionsManager.inputActions.General.Run.started -= DoRun;
        InputActionsManager.inputActions.General.Run.performed -= DoRun;
        InputActionsManager.inputActions.General.Run.canceled -= DoRun;

        InputActionsManager.inputActions.General.Movement.started -= DoMove;
        InputActionsManager.inputActions.General.Movement.performed -= DoMove;
        InputActionsManager.inputActions.General.Movement.canceled -= DoMove;

        InputActionsManager.inputActions.General.Look.started -= DoLook;
        InputActionsManager.inputActions.General.Look.performed -= DoLook;
        InputActionsManager.inputActions.General.Look.canceled -= DoLook;
    }
}
