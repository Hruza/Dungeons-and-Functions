using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemInventory : MonoBehaviour
{
    public InventoryPanel weaponPanel;
    public InventoryPanel armorPanel;
    public InventoryPanel otherPanel;

    public InventoryButton[] weapon;
    public InventoryButton armor;

    private PlayerProgress progress;

    private WeaponItem[] selectedWeapon;
    private Item selectedArmor;

    private void OnEnable()
    {
        selectedWeapon = new WeaponItem[2];
        progress = MenuController.playerProgress;
        for (int i = 0; i <2; i++)
        {
            if (MenuController.equipManager.EquippedWeapons.Count > i)
            {
                selectedWeapon[i] = MenuController.equipManager.EquippedWeapons[i];
                weapon[i].CarriedItem = (Item)selectedWeapon[i];
            }
            else
                selectedWeapon[i] = null;
        }
        if (MenuController.equipManager.EquippedItems.Count > 0)
        {
            selectedArmor = MenuController.equipManager.EquippedItems[0];
            armor.CarriedItem = selectedArmor;
        }
        else
            selectedArmor = null;
        MenuController.equipManager = new EquipManager();
        weaponPanel.Items = progress.weapons.ConvertAll(x => (Item)x); 
        armorPanel.Items = progress.items.FindAll(x => x.itemType == ItemType.Armor);
    }

    /// <summary>
    /// Zavola se pri stisknuti tlacitka s itemem, item bude vybrany pro hru
    /// </summary>
    /// <param name="item">item v tlacitku</param>
    public void ButtonClick(Item item) {
        switch (item.itemType)
        {
            case ItemType.Armor:
                selectedArmor = item;
                armor.CarriedItem = item;
                break;
            case ItemType.Weapon:
                if (selectedWeapon[0] == null && (selectedWeapon[1]==null || item.itemName!=selectedWeapon[1].itemName))
                {
                    selectedWeapon[0] = (WeaponItem)item;
                    weapon[0].CarriedItem = item;
                }
                else if (selectedWeapon[1] == null && (selectedWeapon[0]==null || item.itemName != selectedWeapon[0].itemName))
                {
                    selectedWeapon[1] = (WeaponItem)item;
                    weapon[1].CarriedItem = item;
                }
                else if(selectedWeapon[0] != null && item.itemName != selectedWeapon[0].itemName && selectedWeapon[1]!=null && item.itemName != selectedWeapon[1].itemName)
                {
                    selectedWeapon[0] = (WeaponItem)item;
                    weapon[0].CarriedItem = item;
                }
                break;
            default:
                break;
        }
    }

    public void DeequipClick(int index) {
        switch (index)
        {
            case 0:
                weapon[0].CarriedItem = null;
                selectedWeapon[0] = null;
                break;
            case 1:
                weapon[1].CarriedItem = null;
                selectedWeapon[1] = null;
                break;
            case 3:
                armor.CarriedItem = null;
                selectedArmor = null;
                break;
            default:
                break;
        }
    }

    public void PlayClick() {


        if (selectedWeapon[0] != null || selectedWeapon[1] != null)
        {
            if (selectedArmor != null)
            {
                MenuController.equipManager.EquipItem(selectedArmor);
            }
            for (int i = 0; i < 2; i++)
            {
                if (selectedWeapon[i] != null) MenuController.equipManager.EquipWeapon(selectedWeapon[i]);
            }
            MenuController.PlayLevel();
        }
    }

}
