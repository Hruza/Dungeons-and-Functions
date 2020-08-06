using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class Patrol : MonoBehaviour
{
    public Navigator nav;

    public Transform path;

    int i = 0;
    void Start()
    {
        nav.GoToTarget(path.GetChild(i).position);
    }

    void WalkEnded(Navigator.WalkingOutput output) {
        i++;
        if (i >= path.childCount) {
            i = 0;
        }
        nav.GoToTarget(path.GetChild(i).position);
    }
}
