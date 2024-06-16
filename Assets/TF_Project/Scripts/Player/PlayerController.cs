using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    enum PlayerState
    {
        Gameplay,
        UI,
        Dead
    }
    [Header("References")]
    private CharacterController _characterController;
    private Animator _animator;
    private PlayerControls _controls;

    private void HandleMovement()
    {

    }
}
