using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemInventory : MonoBehaviour
{
    public InventoryPanel weaponPanel;
    public InventoryPanel armorPanel;
    public InventoryPanel otherPanel;

    public InventoryButton weapon0;
    public InventoryButton weapon1;
    public InventoryButton armor;

    private PlayerProgress progress;

    private WeaponItem[] selectedWeapon;
    private Item selectedArmor;

    private void OnEnable()
    {
        selectedWeapon = new WeaponItem[2];
        selectedWeapon[0] = null;
        selectedWeapon[1] = null;
        progress = MenuController.playerProgress;
        weaponPanel.Items = progress.weapons.ConvertAll(x => (Item)x); 
        armorPanel.Items = progress.items.FindAll(x => x.itemType == ItemType.Armor);
    }

    /// <summary>
    /// Zavola se pri stisknuti tlacitka s itemem
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
                if (selectedWeapon[0] == null && item!=selectedWeapon[1])
                {
                    selectedWeapon[0] = (WeaponItem)item;
                    weapon0.CarriedItem = item;
                }
                else if (selectedWeapon[1] == null && item != selectedWeapon[0])
                {
                    selectedWeapon[1] = (WeaponItem)item;
                    weapon1.CarriedItem = item;
                }
                else if(item != selectedWeapon[0] && item != selectedWeapon[1])
                {
                    selectedWeapon[0] = (WeaponItem)item;
                    weapon0.CarriedItem = item;
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
                weapon0.CarriedItem = null;
                selectedWeapon[0] = null;
                break;
            case 1:
                weapon1.CarriedItem = null;
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
            if(selectedArmor!=null)
                MenuController.equipManager.EquipItem(selectedArmor);
            for (int i = 0; i < 2; i++)
            {
                if (selectedWeapon[i] != null) MenuController.equipManager.EquipWeapon(selectedWeapon[i]);
            }
            MenuController.PlayLevel();
        }
    }

}
