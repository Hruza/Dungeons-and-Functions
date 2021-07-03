using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TextTooltip :TooltipButton
{
    public string text;

    public override void OnPointerEnter(PointerEventData data)
    {
        ShowTooltip(text);
    }

}
