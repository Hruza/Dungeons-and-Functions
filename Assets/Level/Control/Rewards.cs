using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class Rewards : MonoBehaviour
{
    public int lootForCompletion=1;
    public int lootForCleared=1;


    public GameObject rouletteButton;

    public GameObject button;

    public GameObject nextButton;

    public GameObject confirmButton;

    public GameObject score;
    public GameObject scoreText;
    public GameObject rewardText;
    public GameObject chooseOne;

    List<Item> extras;
    int rewardCount;
    LevelResults result;
    Level level;

    public IEnumerator CreateReward(Level level, LevelResults result) {
        extras = new List<Item>();
        this.level = level;
        this.result = result;
        rewardCount = 0;
        if (level.lootAfterFinish)
        {
            rewardCount+=lootForCompletion;
            if (result.ClearedAll) rewardCount+=lootForCleared;
            rewardCount += result.additionalLoot;
        }
        foreach (SecretRoom secret in result.secrets)
        {
            switch (secret.type)
            {
                case SecretRoomType.extraRandomItem:
                    rewardCount++;
                    break;
                case SecretRoomType.unlockLevel:
                    MenuController.playerProgress.unlockedLevels.Add(secret.unlockedLevel);
                    break;
                case SecretRoomType.extraItem:
                    foreach (ItemPattern pattern in secret.loot)
                    {
                        extras.Add(Item.Generate(pattern));
                    }
                    break;
                default:
                    break;
            }
        }


        foreach (ItemPattern pattern in level.loot)
        {
            extras.Add(Item.Generate(pattern));
        }


        if (extras.Count > 0)
        {
            StartCoroutine(ShowExtras(extras));
            if (MenuController.playerProgress != null)
            {
                MenuController.playerProgress.armors.AddRange(extras.FindAll(x => x.itemType == ItemType.Armor).ConvertAll(x => (ArmorItem)x));
                MenuController.playerProgress.weapons.AddRange(extras.FindAll(x => x.itemType == ItemType.Weapon).ConvertAll(x => (WeaponItem)x));
            }
        }
        else {
            StartCoroutine(ShowLoot());
        }
        yield return null;
    }

    private IEnumerator ShowExtras(List<Item> extras) {
        LeanTween.moveY(rewardText.GetComponent<RectTransform>(), rewardText.GetComponent<RectTransform>().localPosition.y + 140, 1f).setFrom(rewardText.GetComponent<RectTransform>().localPosition.y).setEaseOutQuad();

        yield return new WaitForSeconds(0.4f);

        LeanTween.alphaCanvas(rewardText.GetComponent<CanvasGroup>(), 1, 0.6f).setFrom(0);

        yield return new WaitForSeconds(0.5f);
        List<GameObject> buttons = new List<GameObject>();
        foreach (Item extra in extras)
        {
            GameObject newButton = Instantiate(button, transform);
            newButton.GetComponent<InventorySlot>().CarriedItem = extra;
            newButton.GetComponent<CanvasGroup>().alpha = 0;
            newButton.GetComponent<InventorySlot>().triggerOnClick = null;
            buttons.Add(newButton);
        }
        foreach (GameObject gameObject in buttons)
        {
            LeanTween.scale(gameObject, 1.5f * gameObject.transform.localScale, 0.5f).setEasePunch();
            LeanTween.alphaCanvas(gameObject.GetComponent<CanvasGroup>(), 1, 0.25f);
            yield return new WaitForSeconds(0.5f);
        }
        if (level.lootAfterFinish)
        {
            nextButton.GetComponent<CanvasGroup>().interactable = true;
            nextButton.GetComponent<CanvasGroup>().blocksRaycasts = true;
        }
        else { 
            GetComponentInParent<LevelExit>().backButton.GetComponent<CanvasGroup>().interactable = true;
            GetComponentInParent<LevelExit>().backButton.GetComponent<CanvasGroup>().blocksRaycasts = true;
        }
        MoveYFrom(nextButton, 70, true);
        yield return null;
    }

    private IEnumerator ShowLoot()
    {
        MoveYFrom(scoreText, -70);
        MoveYFrom(score, -70);
        yield return new WaitForSeconds(1);
        LeanTween.value(0, result.score, 3f).setOnUpdate((float flt) =>
       {
           score.GetComponent<TMPro.TextMeshProUGUI>().text = Mathf.RoundToInt(flt).ToString();
       }).setEaseOutQuint();
        yield return new WaitForSeconds(2.7f);
        LeanTween.scale(score, 1.5f * gameObject.transform.localScale, 0.5f).setEasePunch();


        List<GameObject> buttons = new List<GameObject>();
        MoveYFrom(chooseOne,70);
        yield return new WaitForSeconds(1);
        for (int i = 0; i < rewardCount; i++)
        {
            GameObject newButton = Instantiate(button, transform);
            newButton.GetComponent<CanvasGroup>().alpha = 0;
            confirmButton.GetComponent<CanvasGroup>().interactable = false;
            newButton.GetComponent<InventorySlot>().triggerOnClick = this.gameObject;
            buttons.Add(newButton);
        }
        foreach (GameObject gameObject in buttons)
        {
            StartCoroutine(Roulette(gameObject));
            LeanTween.alphaCanvas(gameObject.GetComponent<CanvasGroup>(), 1, 0.25f);
            gameObject.GetComponent<CanvasGroup>().blocksRaycasts = true;
            yield return new WaitForSeconds(0.5f);
        }

        yield return null;
    }

    InventorySlot chosenItem=null;

    public void ButtonClicked(InventorySlot button) {
        Debug.Log("Message recieved");
        if (chosenItem != null) {
            LeanTween.size(chosenItem.GetComponent<RectTransform>(), 200 * Vector2.one, 0.2f);
        }
        chosenItem=((InventorySlot)button);
        LeanTween.size(chosenItem.GetComponent<RectTransform>(), 270 * Vector2.one, 0.2f);
        confirmButton.GetComponent<CanvasGroup>().alpha = 1;
        confirmButton.GetComponent<CanvasGroup>().blocksRaycasts = true;
        confirmButton.GetComponent<CanvasGroup>().interactable = true;
    }

    IEnumerator Roulette(GameObject button) {
        button.GetComponent<CanvasGroup>().blocksRaycasts = false;
        button.GetComponent<CanvasGroup>().interactable = false;
        button.GetComponent<InventorySlot>().showTooltip = false;
        Item[] items = new Item[20];
        for (int i = 0; i < 20; i++)
        {
            items[i]=Item.Generate(level.difficulty, result.score);
        }
        LeanTween.value(0, 19, 3f).setOnUpdate((float flt )=>  button.GetComponent<InventorySlot>().CarriedItem=items[Mathf.RoundToInt(flt)]).setEaseOutQuad();
        yield return new WaitForSeconds(2.8f);
        LeanTween.scale(button.gameObject, 1.5f * gameObject.transform.localScale, 0.5f).setEasePunch();
        button.GetComponent<InventorySlot>().showTooltip = true;
        button.GetComponent<CanvasGroup>().interactable = true;
        button.GetComponent<CanvasGroup>().blocksRaycasts = true;
    }

    public void Next() {
            Hide(rewardText);
            Hide(nextButton);
        foreach (Transform tr in transform)
        {
            Destroy(tr.gameObject);
        }
        if (rewardCount > 0) {
            StartCoroutine(ShowLoot());
        }
    }

    public void Choose()
    {

    }

    public void Confirm() {
        if (MenuController.playerProgress != null)
        {
            MenuController.playerProgress.AddItem(chosenItem.CarriedItem);
        }
        MenuController.SaveProgress();
        MenuController.menuController.SetPage(3);
    }

    private void Hide(GameObject gobj) {
        gobj.GetComponent<CanvasGroup>().interactable = false;
        LeanTween.alphaCanvas(gobj.GetComponent<CanvasGroup>(), 0, 0.5f);
        gobj.GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

    private void MoveYFrom(GameObject gobj,float to,bool makeVisible=true) {
        LeanTween.alphaCanvas(gobj.GetComponent<CanvasGroup>(), makeVisible?1:0, 1f).setFrom(makeVisible ? 0 : 1);
        LeanTween.moveY(gobj.GetComponent<RectTransform>(), gobj.GetComponent<RectTransform>().localPosition.y, 1f).setFrom(gobj.GetComponent<RectTransform>().localPosition.y + to).setEaseOutQuad();
    }
}
