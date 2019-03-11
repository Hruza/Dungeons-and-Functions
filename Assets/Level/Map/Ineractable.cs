using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ineractable : MonoBehaviour
{
    public enum InteractableType {exit,treasure };
    public InteractableType type;
    private void OnCollisionEnter(Collision collision)
    {
        switch (type)
        {
            case InteractableType.exit:
                LevelController.LevelSuccesfulyExit();
                break;
            case InteractableType.treasure:
                break;
            default:
                break;
        }
    }
}
