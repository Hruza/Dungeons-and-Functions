using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damager
{
    public enum DamageType { neutral, numeric, analytic, algebraic }

    public int value;
    public DamageType type;
    public Vector2 direction;

    static public Color GetColor(DamageType type) {
        switch (type)
        {
            case DamageType.neutral:
                return Color.white;
            case DamageType.numeric:
                return Color.green;
            case DamageType.analytic:
                return Color.cyan;
            case DamageType.algebraic:
                return new Color(1,0.5f,0);
            default:
                return Color.black;
        }
    }

    public Damager(int damageValue, Vector2 direction,DamageType type = DamageType.neutral)
    {
        this.value = damageValue;
        this.type = type;
        this.direction = direction;
    }

    public Damager(float damageValue, Vector2 direction, DamageType type = DamageType.neutral)
    {
        this.value = Mathf.RoundToInt(damageValue);
        this.type = type;
        this.direction = direction;
    }

    static public void InflictDamage(GameObject damaged, float damageValue, Vector2 direction ,DamageType type = DamageType.neutral)
    {
        InflictDamage(damaged, Mathf.RoundToInt(damageValue),direction, type);
    }

    static public void InflictDamage(GameObject damaged, int damageValue, Vector2 direction , DamageType type = DamageType.neutral)
    {
        Damager dmg = new Damager(damageValue, direction,type);
        damaged.SendMessage("GetDamage", dmg, SendMessageOptions.DontRequireReceiver);
    }

    public int EvaluateDamage(Weaknesses weaknesses)
    {
        return Mathf.RoundToInt(value * weaknesses.GetMultiplier(type));
    }
}

[System.Serializable]
public class Weaknesses {

    public float neutralMult;

    public float AnlMult;

    public float NumMult;

    public float AlgMult;

    public float GetMultiplier(Damager.DamageType type){
        switch (type)
        {
            case Damager.DamageType.neutral:
                return neutralMult; 
            case Damager.DamageType.numeric:
                return NumMult;
            case Damager.DamageType.analytic:
                return AnlMult;
            case Damager.DamageType.algebraic:
                return AlgMult;
            default:
                return neutralMult;
        }
    }

    public Weaknesses() {
        neutralMult = 1;
        AlgMult = 1;
        AnlMult = 1;
        NumMult = 1;
    }

    public Weaknesses(float anlMult, float numMult,float algMult)
    {
        neutralMult = 1;
        AlgMult = algMult;
        AnlMult = anlMult;
        NumMult = numMult;
    }

    public readonly static Weaknesses nullWeaknesses = new Weaknesses()
    {
        neutralMult = 100,
        AlgMult = -1,
        AnlMult = -1,
        NumMult = -1,
    };
}
