using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyButton : TooltipButton
{
    public EnemyProperties enemy;

    public Image image;

    public EnemyProperties CarriedEnemy
    {
        get {
            return enemy;
        }
        set {
            image.sprite = value.sprite;
            enemy = value;
        }
    }

    public void MouseEnter()
    {
        ShowTooltip(enemy);
    }
}

