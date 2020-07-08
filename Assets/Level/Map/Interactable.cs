using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    public enum InteractableType {exit,treasure };
    public InteractableType type;
    public GameObject onDestroyParticles;
    public SecretRoom Secret {
        get {
            return secret;
        }
        set {
            type = InteractableType.treasure;
            secret = value;
        }
    }
    private SecretRoom secret;

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
                LevelController.secrets.Add(Secret);
                switch (Secret.type)
                {
                    case SecretRoomType.extraRandomItem:
                        Messager.ShowMessage("+ bonus loot", transform.position, Color.yellow);
                        break;
                    case SecretRoomType.unlockLevel:
                        Messager.ShowMessage("Secret level unlocked", transform.position, Color.yellow);
                        break;
                    case SecretRoomType.extraItem:
                        Messager.ShowMessage("+ special loot", transform.position, Color.yellow);
                        break;
                    default:
                        break;
                }
                if(onDestroyParticles!=null)
                    Destroy(Instantiate(onDestroyParticles, transform.position,transform.rotation),5f);
                Destroy(this.gameObject);
                break;
            default:
                break;
        }
    }
}
