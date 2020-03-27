using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Messager : MonoBehaviour
{
    public GameObject messageObject;
    private static Messager msg;

    private void Start()
    {
        msg = GetComponent<Messager>();
    }

    public static void ShowMessage(string text,Vector3 position,float textSize=1) {
        GameObject createdMessage=(GameObject)Instantiate(msg.messageObject, position, msg.transform.rotation);
        createdMessage.GetComponent<Message>().MessageText = text;
        if (textSize != 1) createdMessage.GetComponent<Message>().Size = textSize;
    }
    public static void ShowMessage(string text, Vector3 position, Color color, float textSize = 1)
    {
        GameObject createdMessage = (GameObject)Instantiate(msg.messageObject, position, msg.transform.rotation);
        Message cmsg = createdMessage.GetComponent<Message>();
        cmsg.MessageText = text;
        if (textSize != 1) cmsg.Size = textSize;
        cmsg.TextColor = color;
    }
    public static void ShowMessage(string text, Vector3 position,Color color,Color outlineColor,float textSize=1)
    {
        GameObject createdMessage = (GameObject)Instantiate(msg.messageObject, position, msg.transform.rotation);
        Message cmsg = createdMessage.GetComponent<Message>();
        cmsg.MessageText = text;
        if (textSize != 1) cmsg.Size = textSize;
        cmsg.TextColor = color;
        if(outlineColor!=null) cmsg.OutlineColor = outlineColor;
    }

    public static void ShowMessage(string text, Vector3 position, Color color, Damager.DamageType damageType, float textSize = 1)
    {
        GameObject createdMessage = (GameObject)Instantiate(msg.messageObject, position, msg.transform.rotation);
        Message cmsg = createdMessage.GetComponent<Message>();
        cmsg.MessageText = text;
        if (textSize != 1) cmsg.Size = textSize;
        cmsg.TextColor = color;
        if (damageType != Damager.DamageType.neutral) cmsg.OutlineColor = Damager.GetColor(damageType);
    }
}
