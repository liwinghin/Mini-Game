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

        [Header("One Handed Attack Animation")]
        public string oh_Light_Attack_01;
        public string oh_Light_Attack_02;
        public string oh_Heavy_Attack_01;
        public string oh_Heavy_Attack_02;

    }
}
