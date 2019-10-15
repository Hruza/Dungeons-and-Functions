using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour
{
    public GameObject barrier;
    private bool active;

    private Transform playerTransform;
    private void Start()
    {
        playerTransform = Player.player.transform;
    }

    public void Activate()
    {
        if (!active)
        {
            barrier.transform.rotation = playerTransform.rotation;
            barrier.SetActive(true);
            active = true;
        }
    }

    private void Update()
    {
        if (active) {
            barrier.transform.rotation = playerTransform.rotation;
        }
    }

    public void Deactivate() {
        if (active)
        {
            barrier.SetActive(false);
            active = false;
        }
    }
}
