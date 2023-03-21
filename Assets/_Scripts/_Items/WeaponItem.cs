using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MINIGAME
{
    [CreateAssetMenu(menuName = "Items/Weapon Item")]
    public class WeaponItem : Items
    {
        public GameObject modelPrefab;
        public bool isUnarmed;

        [Header("Idle Animation")]
        public string right_arm_idle;
        public string left_arm_idle;

        [Header("One Handed Attack Animation")]
        public string oh_Light_Attack_01;
        public string oh_Light_Attack_02;
        public string oh_Heavy_Attack_01;

        [Header("Stamina Costs")]
        public int baseStamina;
        public float lightAttackMultiplier;
        public float heavyAttackMultiplier;
         
    }
}
