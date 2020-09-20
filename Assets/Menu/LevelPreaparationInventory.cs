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

    static public LevelPreaparationInventory instance;

    public override void ReloadInventory()
    {
        instance = this;
        base.ReloadInventory();
        selectedWeapon = new WeaponItem[2];
        progress = MenuController.playerProgress;
        for (int i = 0; i < 2; i++)
        {
            if (MenuController.equipManager.EquippedWeapons[i] == null || MenuController.playerProgress.weapons.Contains(MenuController.equipManager.EquippedWeapons[i]))
            {
                selectedWeapon[i] = MenuController.equipManager.EquippedWeapons[i];
                weapon[i].CarriedItem = (Item)selectedWeapon[i];
            }
            else {
                weapon[i].CarriedItem = null;
            }
        }
        if (MenuController.equipManager.EquippedItems.Count > 0)
        {
            if (MenuController.equipManager.EquippedItems[0] == null || (MenuController.playerProgress.armors.Contains((ArmorItem)MenuController.equipManager.EquippedItems[0])))
            {
                selectedArmor = MenuController.equipManager.EquippedItems[0];
                armor.CarriedItem = selectedArmor;
            }
            else {
                armor.CarriedItem = null;
            }
        }
        else
        {
            selectedArmor = null;
            armor.CarriedItem = null;
        }
    }

    public void EquipItem(Item item) {
        switch (item.itemType)
        {
            case ItemType.Armor:
                armor.SetItem(item);
                break;
            case ItemType.Weapon:
                if (selectedWeapon[0] != null || (selectedWeapon[0] != null  && selectedWeapon[1] != null))
                {
                    weapon[1].SetItem(item);
                }
                else
                {
                    weapon[0].SetItem(item);
                }
                break;
            case ItemType.none:
                break;
            default:
                break;
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
