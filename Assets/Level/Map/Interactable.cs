using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    public enum InteractableType {exit,treasure };
    public InteractableType type;

    static public Interactable exit;

    public void SetInteractable(bool interactable=true) {
        GetComponent<Collider2D>().enabled = interactable;
        switch (type)
        {
            case InteractableType.exit:
                GetComponentInChildren<SpriteRenderer>().color = Color.white;
                break;
            case InteractableType.treasure:
                break;
            default:
                break;
        }
    }

    public void Start()
    {
        if (type == InteractableType.exit) exit = this;
    }

    private void OnTriggerEnter2D(Collider2D collision)
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
