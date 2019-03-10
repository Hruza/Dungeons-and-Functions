using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;

public class InvTooltip : MonoBehaviour
{
    public Text itemName;
    public Text info;
    private Item item;
    public Item Item {
        get {
            return item;
        }
        set {
            item = value;
            itemName.text= item.name;
            StringBuilder sb=new StringBuilder();
            sb.AppendFormat("{0} level {1}\n",item.itemType, item.itemLevel);
            sb.AppendLine(item.rarity.ToString());
            int lines = 2;
            switch (item.itemType)
            {
                case ItemType.Armor:

                    break;
                case ItemType.Weapon:
                    sb.AppendLine();
                    WeaponItem wp = (WeaponItem)item;
                    sb.AppendLine(wp.weaponType.ToString());
                    sb.AppendFormat("Damage: {0}-{1}\n", wp.minDamage, wp.maxDamage);
                    sb.AppendFormat("Attack speed: {0}\n", wp.attackSpeed);
                    lines += 4;
                    break;
                default:
                    break;
            }
            foreach (Stat stat in item.itemStats)
            {
                sb.AppendFormat("  {0} +{1}", stat.name, stat.value);
            }
            info.text = sb.ToString();
            GetComponent<RectTransform>().sizeDelta = new Vector2(300, 50 + (lines) * 18);
        }
    }
}
