using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;
using TMPro;
using System;

public class InvTooltip : MonoBehaviour
{
    public bool flipOnX = false;
    public bool flipOnY = true;

    public TextMeshProUGUI itemName;
    public TextMeshProUGUI info;
    private Item item;
    private EnemyProperties enemy;

    public static readonly string[] rarityColors = { ColorUtility.ToHtmlStringRGB(InventorySlot.commonColor) ,
                                                     ColorUtility.ToHtmlStringRGB(InventorySlot.rareColor),
                                                     ColorUtility.ToHtmlStringRGB(InventorySlot.uniqueColor),
                                                     ColorUtility.ToHtmlStringRGB(InventorySlot.legendaryColor) };

    public EnemyProperties ShownEnemy {
        get {
            return enemy;
        }
        set {
            enemy = value;
            itemName.text = enemy.name;
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("{0} enemy\n", enemy.aiType);
            sb.AppendFormat("<b>HP:</b> {0}\n", enemy.HP);
            //sb.AppendFormat("<b>Damage:</b> {0} <color=#{1}>{2}<color=black>\n", enemy.damage, ColorUtility.ToHtmlStringRGB(Damager.GetColor(enemy.damageType)),enemy.damageType);
            sb.AppendFormat("<b>Damage:</b> {0} \n", enemy.Damage);

            /*sb.Append(Weakness(enemy.weaknesses, Damager.DamageType.neutral));
            sb.Append(Weakness(enemy.weaknesses, Damager.DamageType.numeric));
            sb.Append(Weakness(enemy.weaknesses, Damager.DamageType.analytic));
            sb.Append(Weakness(enemy.weaknesses, Damager.DamageType.algebraic));*/
            sb.Append("\n");
            info.SetText(sb);
            GetComponent<RectTransform>().sizeDelta = new Vector2(210, 40 + info.preferredHeight);
            CheckConstrains();
        }
    }


    static readonly float[] nodes = { 0.5f, 0.75f, 0.95f, 1.05f, 1.25f, 1.5f,2};
    static readonly string[] intervals = { "{0} imunity\n", "{0} resistance\n", "slight {0} resistance\n" ,"","slight {0} weakness\n", "{0} weakness\n", "significant {0} weakness\n" };
    private string Weakness(Weaknesses weaknesses,Damager.DamageType damageType) {
            float mult=weaknesses.GetMultiplier(damageType);
        for(int i=0;i<nodes.Length;i++)
        {
            if (mult < nodes[i]) {
                return string.Format(intervals[i],"<color=#"+ ColorUtility.ToHtmlStringRGB(Damager.GetColor(damageType))+ ">"+damageType.ToString()+"<color=black>");
            }
        }
        return "";
    }

    public void Clear(string main, string properties)
    {
        itemName.text = main;
        info.text = properties;
        GetComponent<RectTransform>().sizeDelta = new Vector2(210, 40 + info.preferredHeight);
    }

    public Color upgrade;
    public Color neutral;
    public Color downgrade;

    string Comparer(int newVal, int oldVal,bool lowerBetter=false) {
        Color col = neutral;
        int dif = newVal - oldVal;
        if (lowerBetter? dif < 0 : dif > 0) col = upgrade;
        if (lowerBetter? dif > 0 : dif < 0) col = downgrade;
        return string.Format("<color=#{0}>({1}{2})<color=black>",ColorUtility.ToHtmlStringRGB(col),dif>0?"+":"",dif);
    }


    public void ShowItem(Item value, Item comparedTo = null)
    {
        item = value;
        itemName.text = item.itemName;
        StringBuilder sb = new StringBuilder();
        sb.AppendFormat("{0} level {1} (score {2})\n", item.itemType, item.itemLevel, item.EvaluateScore());
        sb.AppendLine("<color=#" + rarityColors[item.rarity.GetHashCode()] + ">" + item.rarity.ToString() + "<color=black>");
        int lines = 2;
        switch (item.itemType)
        {
            case ItemType.Armor:
                ArmorItem am = (ArmorItem)item;
                sb.AppendLine();
                if (comparedTo != null)
                {
                    sb.AppendFormat("<b>Armor:</b> {0} {1}\n", am.Armor, Comparer(am.Armor, ((ArmorItem)comparedTo).Armor));
                    sb.AppendFormat("<b>Speed reduction:</b> {0} {1}\n", am.movementSpeedReduction, Comparer(am.movementSpeedReduction, ((ArmorItem)comparedTo).movementSpeedReduction, true));
                    sb.AppendFormat("<b>Additional HP:</b> {0} {1}\n", am.AdditionalHP, Comparer(am.AdditionalHP, ((ArmorItem)comparedTo).AdditionalHP));
                }
                else
                {
                    sb.AppendFormat("<b>Armor:</b> {0}\n", am.Armor);
                    sb.AppendFormat("<b>Speed reduction:</b> {0}\n", am.movementSpeedReduction);
                    sb.AppendFormat("<b>Additional HP:</b> {0}\n", am.AdditionalHP);
                }
                lines += 3;
                break;
            case ItemType.Weapon:
                sb.AppendLine();
                WeaponItem wp = (WeaponItem)item;
                sb.AppendLine(wp.weaponType.ToString());
                if (comparedTo != null)
                {
                    sb.AppendFormat("<b>Damage:</b> {0} {2}-{1} {3} \n", wp.MinDamage, wp.MaxDamage, Comparer(wp.MinDamage,((WeaponItem)comparedTo).MinDamage ), Comparer(wp.MaxDamage,((WeaponItem)comparedTo).MaxDamage));
                    sb.AppendFormat("<b>Attack speed:</b> {0} {1}\n", wp.attackSpeed, Comparer(wp.attackSpeed,((WeaponItem)comparedTo).attackSpeed) );
                }
                else
                {
                    sb.AppendFormat("<b>Damage:</b> {0}-{1} \n", wp.MinDamage, wp.MaxDamage);
                    sb.AppendFormat("<b>Attack speed:</b> {0}\n", wp.attackSpeed);
                }
                lines += 4;
                break;
            default:
                break;
        }
        if (item.itemStats.Length > 0)
        {
            sb.Append("\n");
            lines++;
            foreach (Stat stat in item.itemStats)
            {
                sb.AppendFormat(" {0} +{1}\n", stat.name, stat.value);
                lines++;
            }
        }
        if (item.ItemComment != "")
        {
            sb.Append("\n");
            sb.Append("<i>" + item.ItemComment + "</i>");
            lines += 2;
        }
        sb.Append("\n ");
        info.SetText(sb);
        GetComponent<RectTransform>().sizeDelta = new Vector2(210, 40 + info.preferredHeight);
        if (flipOnX || flipOnY) CheckConstrains();
    }

    private void CheckConstrains()
    {
        RectTransform rect = GetComponent<RectTransform>();
        Vector3[] corners = new Vector3[4];
        rect.GetWorldCorners(corners);
        if (flipOnX && corners[3].x > Screen.width)
        {
            rect.pivot = new Vector2(1.1f, rect.pivot.y);
        }
        else
        {
            rect.pivot = new Vector2(-0.1f, rect.pivot.y);
        }
        if (flipOnY && corners[3].y < 0)
        {
            rect.pivot = new Vector2(rect.pivot.x, -0.05f);
        }
        else
        {
            rect.pivot = new Vector2(rect.pivot.x, 1.05f);
        }


    }

    private void Update()
    {

        //  Debug.LogFormat("posx: {0} posy:{1}    width: {2} height: {3}      localpos: {4}    A:{5}    B{6}  corner:{7}", rect.position.x, rect.position.y, Screen.width, Screen.height, rect.localPosition, rect.position.x + rect.rect.width, rect.position.y - rect.rect.height,corners);

    }
}
