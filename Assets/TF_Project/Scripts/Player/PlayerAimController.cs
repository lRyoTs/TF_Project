using Cinemachine;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerAimController : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private PlayerController _playerController;
    [SerializeField]
    private Image crosshair;

    [SerializeField] private CinemachineVirtualCamera aimVirtualCamera;
    [SerializeField] private LayerMask aimColliderLayerMask;
    [SerializeField] private float normalSensitivity = 1f;
    [SerializeField] private float aimSensitivity = 0.5f;
    [SerializeField] private GameObject debugObject;
    private Vector3 mouseWorldPosition;
    private bool isAimActive = false;

    private void Awake()
    {
        isAimActive = false;
    }
    // Update is called once per frame
    void Update()
    {
        GetAimDirection();
        SwitchToAimCamera();
    }

    private void SwitchToAimCamera()
    {
        GetAimDirection();
        if (isAimActive)
        {
            aimVirtualCamera.gameObject.SetActive(true);
            _playerController.SetSensitivity(aimSensitivity);
            crosshair.DOFade(1f, 1.5f);
        }
        else
        {
            aimVirtualCamera.gameObject.SetActive(false);
            _playerController.SetSensitivity(normalSensitivity);
            crosshair.DOFade(0f, 1.5f);
        }
    }

    private void GetAimDirection()
    {
        mouseWorldPosition = Vector3.zero;
        Vector2 screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f); // Get center of the screen
        Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);
        if (Physics.Raycast(ray, out RaycastHit raycastHit, 999f, aimColliderLayerMask))
        {
            mouseWorldPosition = raycastHit.point;
            debugObject.transform.position = raycastHit.point;
        }

        Vector3 worldAimTarget = mouseWorldPosition;
        worldAimTarget.y = transform.position.y;
        Vector3 aimDirection = (worldAimTarget - transform.position).normalized;

        //transform.forward = Vector3.Lerp(transform.forward, aimDirection, Time.deltaTime * 20f);
    }

    public Vector3 GetMouseWorldPosition()
    {
        return mouseWorldPosition;
    }

    private void OnEnable()
    {
        InputActionsManager.inputActions.General.SwitchAim.started += SwitchAim;
    }
    private void OnDisable()
    {
        InputActionsManager.inputActions.General.SwitchAim.started -= SwitchAim;
    }

    private void SwitchAim(InputAction.CallbackContext context)
    {
        isAimActive = !isAimActive;
    }
}

