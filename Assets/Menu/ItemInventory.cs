using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemInventory : MonoBehaviour
{
    [Header("Panels")]
    public InventoryPanel weaponPanel;
    public InventoryPanel armorPanel;
    public InventoryPanel otherPanel;

    [Header("Slots")]
    public InventoryButton[] weapon;
    public InventoryButton armor;

    private PlayerProgress progress;

    private WeaponItem[] selectedWeapon;
    private Item selectedArmor;



    public void ReloadInventory(){
        defaultSize = armor.GetComponent<RectTransform>().sizeDelta;
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
            {
                selectedWeapon[i] = null;
                weapon[i].CarriedItem = null;
            }
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
        MenuController.equipManager = new EquipManager();
        weaponPanel.Items = progress.weapons.ConvertAll(x => (Item)x); 
        armorPanel.Items = progress.armors.ConvertAll(x => (Item)x);
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
                Animate(armor);
                break;
            case ItemType.Weapon:
                if ((selectedWeapon[0] == null || item.itemName == selectedWeapon[0].itemName) && (selectedWeapon[1]==null || item.itemName!=selectedWeapon[1].itemName) )
                {
                    selectedWeapon[0] = (WeaponItem)item;
                    weapon[0].CarriedItem = item;
                    Animate(weapon[0]);
                }
                else if ((selectedWeapon[1] == null || item.itemName == selectedWeapon[1].itemName ) && (selectedWeapon[0]==null || item.itemName != selectedWeapon[0].itemName))
                {
                    selectedWeapon[1] = (WeaponItem)item;
                    weapon[1].CarriedItem = item;
                    Animate(weapon[1]);
                }
                else if(selectedWeapon[0] != null && item.itemName != selectedWeapon[0].itemName && selectedWeapon[1]!=null && item.itemName != selectedWeapon[1].itemName)
                {
                    selectedWeapon[0] = (WeaponItem)item;
                    weapon[0].CarriedItem = item;
                    Animate(weapon[0]);
                }
                break;
            default:
                break;
        }
    }

    private Vector2 defaultSize;

    public void Animate(InventoryButton button) {
        RectTransform tr = button.GetComponent<RectTransform>();
        LeanTween.size(tr, defaultSize * 1.1f, 0.125f).setFrom(defaultSize).setRepeat(2).setLoopPingPong();
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
