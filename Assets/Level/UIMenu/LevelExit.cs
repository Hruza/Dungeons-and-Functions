using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelExit : MonoBehaviour
{
    public TextMeshProUGUI levelName;
    public GameObject completed;
    public GameObject failed;
    public GameObject backButton;
    public Rewards rewards;


    public InventoryPanel panel;

    const int minRoomCountForBonusLoot=5;

    public bool debug = false;
    public Level debugLevel;

    void Update() {
        if(Input.GetKeyDown(KeyCode.E))
            LevelEnded( new LevelResults(true, 5, 5, 30,new List<SecretRoom>()));
    }


    public void LevelEnded(LevelResults result) {
        StartCoroutine(LevelEndedSeq(result));
    }

    public IEnumerator LevelEndedSeq(LevelResults result) {

        levelName.GetComponent<CanvasGroup>().alpha = 0;
        completed.GetComponent<CanvasGroup>().alpha = 0;
        failed.GetComponent<CanvasGroup>().alpha = 0;
        backButton.GetComponent<CanvasGroup>().alpha = 0;

        Level level;
        if (!debug)
            level = MenuController.selectedLevel;
        else
            level = debugLevel ;
        levelName.text = level.levelName;
        LeanTween.alphaCanvas(levelName.GetComponent<CanvasGroup>(), 1, 1f).setFrom(0);
        LeanTween.moveY(levelName.GetComponent<RectTransform>(), levelName.GetComponent<RectTransform>().localPosition.y, 1f).setFrom(levelName.GetComponent<RectTransform>().localPosition.y+70).setEaseOutQuad();
        yield return new WaitForSeconds(0.5f);
        if (result.completd)
        {
            LeanTween.alphaCanvas(completed.GetComponent<CanvasGroup>(), 1, 1f).setFrom(0);
            LeanTween.moveY(completed.GetComponent<RectTransform>(), completed.GetComponent<RectTransform>().localPosition.y, 1f).setFrom(completed.GetComponent<RectTransform>().localPosition.y + 70).setEaseOutQuad();
            yield return new WaitForSeconds(2f);

            LeanTween.alphaCanvas(completed.GetComponent<CanvasGroup>(), 0, 0.6f).setFrom(1);
            LeanTween.moveY(completed.GetComponent<RectTransform>(), completed.GetComponent<RectTransform>().localPosition.y + 140, 1f).setFrom(completed.GetComponent<RectTransform>().localPosition.y).setEaseInQuad();


            StartCoroutine(rewards.CreateReward(level,result));
        }
        else
        {
            LeanTween.alphaCanvas(failed.GetComponent<CanvasGroup>(), 1, 1f).setFrom(0);
            LeanTween.moveY(failed.GetComponent<RectTransform>(), failed.GetComponent<RectTransform>().localPosition.y, 1f).setFrom(failed.GetComponent<RectTransform>().localPosition.y + 70).setEaseInOutQuad();
            yield return new WaitForSeconds(0.5f);
            LeanTween.alphaCanvas(backButton.GetComponent<CanvasGroup>(), 1, 1f).setFrom(0);
            backButton.GetComponent<CanvasGroup>().interactable = true;
            LeanTween.moveY(backButton.GetComponent<RectTransform>(), backButton.GetComponent<RectTransform>().anchoredPosition.y, 1f).setFrom(backButton.GetComponent<RectTransform>().anchoredPosition.y + 70).setEaseInOutQuad();
            MenuController.SaveProgress();
        }
        yield return null;
    }
}
