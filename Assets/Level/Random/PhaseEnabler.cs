using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhaseEnabler : MonoBehaviour
{
    public GameObject controledGameObject;
    public bool[] states;
    void OnPhaseStart(int index)
    {
        controledGameObject.SetActive(states[index]);
    }
}
