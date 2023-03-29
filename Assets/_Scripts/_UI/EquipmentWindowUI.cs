using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MINIGAME
{
    public class EquipmentWindowUI : MonoBehaviour
    {
        public bool rightHandSlot01Selected;
        public bool rightHandSlot02Selected;

        public bool leftHandSlot01Selected;
        public bool leftHandSlot02Selected;

        HandleEquipmentSlotUI[] handleEquipmentSlotUI;

        private void Start()
        {
            handleEquipmentSlotUI = GetComponentsInChildren<HandleEquipmentSlotUI>();
        }

        public void LoadWeaponsOnEquipmentScreen(PlayerInventory playerInventory)
        {
            for (int i = 0; i < handleEquipmentSlotUI.Length; i++)
            {
                if (handleEquipmentSlotUI[i].rightHandSlot01)
                {
                    handleEquipmentSlotUI[i].AddItem(playerInventory.weaponsInRightHandSlots[0]);
                    print(playerInventory.weaponsInRightHandSlots[0].name);
                }
                else if (handleEquipmentSlotUI[i].rightHandSlot02)
                {
                    handleEquipmentSlotUI[i].AddItem(playerInventory.weaponsInRightHandSlots[1]);
                }
                else if (handleEquipmentSlotUI[i].leftHandSlot01)
                {
                    handleEquipmentSlotUI[i].AddItem(playerInventory.weaponsInLeftHandSlots[0]);
                }
                else
                {
                    handleEquipmentSlotUI[i].AddItem(playerInventory.weaponsInLeftHandSlots[1]);
                }
            }
        }

        public void SelectedRightHandSlot01()
        {
            rightHandSlot01Selected = true;
        }
        public void SelectedRightHandSlot02()
        {
            rightHandSlot02Selected = true;
        }
        public void SelectedLeftHandSlot01()
        {
            leftHandSlot01Selected = true;
        }
        public void SelectedLeftHandSlot02()
        {
            leftHandSlot02Selected = true;
        }
    }
}