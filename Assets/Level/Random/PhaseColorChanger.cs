using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhaseColorChanger : MonoBehaviour
{
    public SpriteRenderer sprite;

    public bool useDamageTypes;
    public Damager.DamageType[] damageTypes;
    public Color[] colors;

    void OnPhaseStart(int index) {
        if (useDamageTypes) {
            if (damageTypes[index] == Damager.DamageType.neutral) {
                sprite.color = Color.clear;
            }
            else
                sprite.color = Damager.GetColor(damageTypes[index]);
        }
        else
        {
            sprite.color = colors[index];
        }
    }
}
