using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damager
{
    public enum DamageType { neutral, numeric, analytic, algebraic }

    public int value;
    public DamageType type;

    public Damager(int damageValue, DamageType type = DamageType.neutral)
    {
        this.value = damageValue;
        this.type = type;
    }

    public Damager(float damageValue, DamageType type = DamageType.neutral)
    {
        this.value = Mathf.RoundToInt(damageValue);
        this.type = type;
    }

    static public void InflictDamage(GameObject damaged, float damageValue, DamageType type = DamageType.neutral)
    {
        InflictDamage(damaged, Mathf.RoundToInt(damageValue), type);
    }

    static public void InflictDamage(GameObject damaged, int damageValue, DamageType type = DamageType.neutral)
    {
        Damager dmg = new Damager(damageValue, type);
        damaged.SendMessage("GetDamage", dmg, SendMessageOptions.DontRequireReceiver);
    }
}

[System.Serializable]
public class Weaknesses {
    private Dictionary<Damager.DamageType,float> multiplier;

    public float AnlMult {
        get {
            return multiplier[Damager.DamageType.analytic];
        }
        set {
            multiplier[Damager.DamageType.analytic] = value;
        }
    }

    public float NumMult
    {
        get
        {
            return multiplier[Damager.DamageType.numeric];
        }
        set
        {
            multiplier[Damager.DamageType.numeric] = value;
        }
    }

    public float AlgMult
    {
        get
        {
            return multiplier[Damager.DamageType.algebraic];
        }
        set
        {
            multiplier[Damager.DamageType.algebraic] = value;
        }
    }

    public float GetMultiplier(Damager.DamageType type){
        return multiplier[type];
    }

    public void SetMultiplier(Damager.DamageType type,float value)
    {
        multiplier[type] = value;
    }

    public Weaknesses() {
        multiplier = new Dictionary<Damager.DamageType, float>();
        multiplier[Damager.DamageType.neutral] = 1;
        multiplier[Damager.DamageType.analytic] = 1;
        multiplier[Damager.DamageType.numeric] = 1;
        multiplier[Damager.DamageType.algebraic] = 1;
    }

    public Weaknesses(float anlMult, float numMult,float algMult)
    {
        multiplier[Damager.DamageType.neutral] = 1;
        multiplier[Damager.DamageType.analytic] = anlMult;
        multiplier[Damager.DamageType.numeric] = numMult;
        multiplier[Damager.DamageType.algebraic] = algMult;
    }
}
