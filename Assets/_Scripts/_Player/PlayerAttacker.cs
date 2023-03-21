using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MINIGAME
{
    public class PlayerAttacker : MonoBehaviour
    {
        AnimatorHandler animatorHandler;
        InputHandler inputHandler;
        WeaponSlotManager weaponSlotManager;

        public string lastAttack;

        private void Awake()
        {
            animatorHandler = GetComponentInChildren<AnimatorHandler>();
            weaponSlotManager = GetComponentInChildren<WeaponSlotManager>();
            inputHandler = GetComponent<InputHandler>();
        }

        public void HandleWeaponCombo(WeaponItem weapon)
        {
            if (inputHandler.comboFlag)
            {
                animatorHandler.anim.SetBool("CanDoCombo", false);
                if (lastAttack == weapon.oh_Light_Attack_01)
                {
                    animatorHandler.PlayTargetAnimation(weapon.oh_Light_Attack_02, true);
                }
            }  
        }

        public void HandleLightAttack(WeaponItem weapon)
        {
            weaponSlotManager.attackingWeapon = weapon;
            animatorHandler.PlayTargetAnimation(weapon.oh_Light_Attack_01, true);
            lastAttack = weapon.oh_Light_Attack_01;
        }

        public void HandleHeavyAttack(WeaponItem weapon)
        {
            weaponSlotManager.attackingWeapon = weapon;
            animatorHandler.PlayTargetAnimation(weapon.oh_Heavy_Attack_01, true);
            lastAttack = weapon.oh_Heavy_Attack_01;
        }
    }
}
