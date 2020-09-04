using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelPreaparationInventory : ItemInventory
{

    [Header("Slots")]
    public InventorySlot[] weapon;
    public InventorySlot armor;

    private PlayerProgress progress;

    private WeaponItem[] selectedWeapon;
    private Item selectedArmor;


    public override void ReloadInventory()
    {
        base.ReloadInventory();
        selectedWeapon = new WeaponItem[2];
        progress = MenuController.playerProgress;
        for (int i = 0; i < 2; i++)
        {
                selectedWeapon[i] = MenuController.equipManager.EquippedWeapons[i];
                weapon[i].CarriedItem = (Item)selectedWeapon[i];
        }
        if (MenuController.equipManager.EquippedItems.Count > 0)
        {
            selectedArmor = MenuController.equipManager.EquippedItems[0];
            armor.CarriedItem = selectedArmor;
        }
        else
        {
            selectedArmor = null;
            armor.CarriedItem = null;
        }
    }

    public override void ItemAdded(InventorySlot sender, Item item)
    {
        if (sender == armor)
        {
            selectedArmor = item;
            return;
        }
        else if (sender == weapon[0])
        {
            selectedWeapon[0] = (WeaponItem)item;
            if (selectedWeapon[1] == null || item.itemName == selectedWeapon[1].itemName)
            {
                selectedWeapon[1] = null;
                weapon[1].CarriedItem = null;
            }
            return;
        }
        else if (sender == weapon[1])
        {
            selectedWeapon[1] = (WeaponItem)item;
            if (selectedWeapon[0] == null || item.itemName == selectedWeapon[0].itemName)
            {
                selectedWeapon[0] = null;
                weapon[0].CarriedItem = null;
            }
            return;
        }
        return;
    }

    public override void ItemRemoved(InventorySlot sender, Item item)
    {
        if (sender == armor)
        {
            selectedArmor = null;
        }
        else if (sender == weapon[0])
        {
            selectedWeapon[0] = null;
        }
        else if (sender == weapon[1])
        {
            selectedWeapon[1] = null;
        }
    }

    public void PlayClick()
    {


        if (selectedWeapon[0] != null || selectedWeapon[1] != null)
        {
            if (selectedArmor != null)
            {
                MenuController.equipManager.EquipItem(selectedArmor);
            }
            for (int i = 0; i < 2; i++)
            {
               MenuController.equipManager.EquipWeapon(selectedWeapon[i],i);
            }
            MenuController.PlayLevel();
        }
    }
}
