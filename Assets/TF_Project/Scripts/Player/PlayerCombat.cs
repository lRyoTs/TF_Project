using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCombat : MonoBehaviour
{
    public List<AttackSO> combo;
    private float lastClickedTime;
    private float lastComboEnd;
    private int comboCounter;
    private bool canAttack;
    [Header("References")]
    [SerializeField] private Animator animator;
    [SerializeField] private Weapon weapon;
    [SerializeField] private PlayerStats _playerStats;

    // Start is called before the first frame update
    void Start()
    {
        comboCounter = 0;
        canAttack = true;
    }

    // Update is called once per frame
    void Update()
    {
        ExitAttack();
    }

    private void Attack()
    {
        if(Time.time - lastComboEnd > 0.2f && comboCounter <= combo.Count)
        {
            StartCoroutine(AttackCooldown());
            CancelInvoke("EndCombo");

            if(Time.time - lastClickedTime >= 0.3f)
            {
                animator.runtimeAnimatorController = combo[comboCounter].animatorOV;
                animator.SetTrigger("Attack");
                weapon.damage = combo[comboCounter].multiplier;
                comboCounter++;
                lastClickedTime = Time.time;

                if(comboCounter >= combo.Count)
                {
                    comboCounter = 0;
                }
            }
        }
    }

    private void ExitAttack()
    {
        if(animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.9f && animator.GetCurrentAnimatorStateInfo(0).IsTag("Attack"))
        {
            Invoke("EndCombo", 1);
        }
    }

    private void EndCombo() 
    {
        comboCounter = 0;
        lastClickedTime = Time.time;
    }

    private IEnumerator AttackCooldown()
    {
        canAttack = false;
        InputActionsManager.DisableActionMap();
        yield return new WaitForSeconds(1f);
        InputActionsManager.ToggleActionMap(InputActionsManager.inputActions.General);
        canAttack = true;
    }
    private void OnEnable()
    {
        InputActionsManager.inputActions.General.BasicAttack.started += DoAttack;
    }

    private void DoAttack(InputAction.CallbackContext context)
    {
        if (canAttack)
        {
            Attack();
        }
        
    }

    private void OnDisable()
    {
        InputActionsManager.inputActions.General.BasicAttack.started -= DoAttack;
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }
}
