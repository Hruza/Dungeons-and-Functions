using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopInventory : ItemInventory
{
    [Header("Shop")]
    public InventorySlot upgradeSlot;
    public InventorySlot sacrificeSlot;
    public InventorySlot upgradedSlot;

    public InvTooltip fromTooltip;
    public InvTooltip toTooltip;

    public CanvasGroup upgradeGroup;

    public TextMeshProUGUI messageText;

    public GameObject upgradeButton;
    public Image scrollArrow;

    public Button backButton;
    public override void ItemAdded(InventorySlot sender, Item item)
    {
        if (sender == upgradeSlot && item == sacrificeSlot.CarriedItem) {
            sacrificeSlot.CarriedItem = null;
        }
        else if (sender == sacrificeSlot && item == upgradeSlot.CarriedItem)
        {
            upgradeSlot.CarriedItem = null;
        }

        if (sender == upgradeSlot)
        {
            fromTooltip.ShowItem(item);
            if (item.IsUpgradable)
            {
                Item newItem=new Item();
                if (item.quality == Quality.Basic)
                {
                    newItem = item.CopyItem();
                    newItem.quality = Quality.C;
                }
                if (item.quality == Quality.C) 
                {
                    newItem = item.Upgrade();
                }
                toTooltip.ShowItem(newItem,item);
                upgradedSlot.CarriedItem = newItem;
            }
        }
    }


    public override void ItemRemoved(InventorySlot sender, Item item)
    {
        if (sender == upgradeSlot)
        {
            fromTooltip.Clear("Item to Upgrade","");
            toTooltip.Clear("Upgraded Item","");
            upgradedSlot.CarriedItem = null;
        }
    }

    public void Upgrade()
    {
        Item upgrading = upgradeSlot.CarriedItem;
        Item sacrificing = sacrificeSlot.CarriedItem;

        if (upgrading == null)
        {
            ShowMessage("Choose item to upgrade");
            return;
        }
        if (sacrificing == null)
        {
            ShowMessage("You have to sacrifice some item");
            return;
        }
        if (upgrading.itemLevel >= sacrificing.itemLevel)
        {
            ShowMessage("Sacrifice item of higher level than upgraded item");
            return;
        }
        if (!upgrading.IsUpgradable)
        {
            ShowMessage("This item can not be further upgraded");
            return;
        }

        if (upgrading.quality == Quality.Basic)
        {
            upgrading.quality = Quality.C;
        }
        else if (upgrading.quality == Quality.C)
        {
            MenuController.playerProgress.AddItem(upgrading.Upgrade());
            MenuController.playerProgress.DestroyItem(upgrading);
        }
        
        MenuController.playerProgress.DestroyItem(sacrificing);

        MenuController.SaveProgress();

        StartCoroutine(UpgradeSequence());
    }

    private float upgradeTime = 1f;

    private IEnumerator UpgradeSequence() {
        backButton.interactable = false;
        upgradeGroup.interactable = false;
        upgradeGroup.blocksRaycasts = false;
        LeanTween.value(0, 1, upgradeTime).setOnUpdate((float flt) =>
        {
            scrollArrow.fillAmount = flt;
        });

        upgradeSlot.CarriedItem = null;
        upgradeSlot.ClickEffect();
        
        yield return new WaitForSeconds(upgradeTime/2);

        sacrificeSlot.CarriedItem = null;
        sacrificeSlot.ClickEffect();

        yield return new WaitForSeconds(upgradeTime / 2);


        upgradedSlot.ClickEffect();

        yield return new WaitForSeconds(upgradeTime);

        upgradedSlot.CarriedItem = null;
        upgradedSlot.ClickEffect();
        scrollArrow.fillAmount = 0;

        fromTooltip.Clear("Item to Upgrade", "");
        toTooltip.Clear("Upgraded Item", "");

        upgradeGroup.interactable = true;
        upgradeGroup.blocksRaycasts = true;
        
        ReloadInventory();
        backButton.interactable = true;
    }
 
    private void ShowMessage(string msg) {
        messageText.text = msg;
        LeanTween.alphaCanvas(messageText.GetComponent<CanvasGroup>(), 0, 3).setFrom(1).setEaseInCubic();
    }
}
