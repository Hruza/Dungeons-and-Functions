using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class TooltipButton : MonoBehaviour, IPointerEnterHandler,IPointerExitHandler
{
    private GameObject tooltip;
    public GameObject tooltipPrefab;

    protected void ShowTooltip(Item item,ItemInventory itemInventory=null)
    {
        if (item != null && tooltip == null)
        {
            if (itemInventory != null) tooltip = (GameObject)Instantiate(tooltipPrefab, Input.mousePosition, tooltipPrefab.transform.rotation, itemInventory.transform);
            else tooltip = (GameObject)Instantiate(tooltipPrefab, Input.mousePosition, tooltipPrefab.transform.rotation, transform.parent.parent);
            tooltip.GetComponent<InvTooltip>().Item = item;
            StartCoroutine(MoveTooltip());
        }
    }

    protected void ShowTooltip(EnemyProperties enemy)
    {
        if ( tooltip == null)
        {
            tooltip = (GameObject)Instantiate(tooltipPrefab, Input.mousePosition, tooltipPrefab.transform.rotation, transform.parent.parent);
            tooltip.GetComponent<InvTooltip>().ShownEnemy = enemy;
            StartCoroutine(MoveTooltip());
        }
    }

    public IEnumerator MoveTooltip()
    {
        while (tooltip != null)
        {
            tooltip.transform.position = Input.mousePosition;
            yield return new WaitForEndOfFrame();
        }
    }

    public void OnPointerExit(PointerEventData data)
    {
        if (tooltip != null)
            Destroy(tooltip);
    }
    public abstract void OnPointerEnter(PointerEventData data);
}
