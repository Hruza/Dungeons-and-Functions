using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasMessager : MonoBehaviour
{
    public GameObject messageObject;
    private static CanvasMessager instance;
    private Queue<string> messageQueue;

    public float fadeTime=1;

    private void Awake()
    {
        instance = this;
        messageQueue=new Queue<string>();
    }

    public static void ShowMessage(string text,bool priority=false) {
        if(priority){
            if(instance.routine!=null) instance.StopCoroutine(instance.routine);
            instance.routine=instance.ShowMessageOnCanvas(text);
            instance.StartCoroutine(instance.routine);
        }
        else{
            instance.messageQueue.Enqueue(text);
            if(!instance.showingMessage){
                instance.ShowNextMessage();
            }
        }
    }

    private void ShowNextMessage(){
        if(messageQueue.Count>0){
            if(routine!=null) StopCoroutine(routine);
            routine=ShowMessageOnCanvas(messageQueue.Dequeue());
            StartCoroutine(routine);
        }
    }

    bool showingMessage=false;

    private IEnumerator routine;

    private IEnumerator ShowMessageOnCanvas(string text){
            CanvasGroup gp = messageObject.GetComponent<CanvasGroup>(); 
            
            if(showingMessage){
                for (float t = gp.alpha*fadeTime; t>0 ; t-=Time.deltaTime)
                {
                    gp.alpha=t/fadeTime;
                    yield return new WaitForEndOfFrame();
                }
                gp.alpha=0;
            }
            messageObject.GetComponentInChildren<TMPro.TextMeshProUGUI>().text=text;
            showingMessage=true;
            for (float t = 0; t < fadeTime; t+=Time.deltaTime)
            {
                gp.alpha=t/fadeTime;
                yield return new WaitForEndOfFrame();
            }
            gp.alpha=1;
            yield return new WaitForSeconds(20);
            
            for (float t = fadeTime; t>0 ; t-=Time.deltaTime)
            {
                gp.alpha=t/fadeTime;
                yield return new WaitForEndOfFrame();
            }
            gp.alpha=0;
            showingMessage=false;
            yield return null;
    }
}
