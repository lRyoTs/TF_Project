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

    private PlayerState _playerState = PlayerState.Gameplay;


    private void Update()
    {
        switch (_playerState)
        {
            case PlayerState.Gameplay:
                break;
            case PlayerState.Dead:
                break;
            case PlayerState.UI:
                break;

        }    

    }
    //Gameplay
    #region Gameplay
    private void HandleMovement()
    {

    }
    #endregion

    //UI
    #region UI
    private void HandleUINavigation()
    {

    }
    #endregion

    #region Input Related Functions

    #endregion
}
