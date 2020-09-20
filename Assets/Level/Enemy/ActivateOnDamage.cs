using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateOnDamage : MonoBehaviour
{
    public MonoBehaviour behaviour;

    bool started = false;
    public void GetDamage(Damager dmg)
    {
        if (!started)
        {
            started = true;
            behaviour.enabled = true;
            behaviour.SendMessage("Start");
            this.enabled = false;
        }
    }
}
