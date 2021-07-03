using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialController : MonoBehaviour
{
    static private TutorialController instance;

    public string spawn;
    public string clear1;
    public string clear2;
    public string clear3;
    public string entered;

    private int clearedCount=0;

    private bool active=false;

    private void Awake() {
        instance= this;
    }

    private void Start() {
        if(MenuController.selectedLevel.levelName=="The Introductory Lecture"){
            active=true; 
            Invoke("Spawned",1);
        }    
        else active=false;
        clearedCount=0;
        
    }

    private void Spawned(){
        InstTriggerEvent("spawned");
    }

    static public void TriggerEvent(string eventName){
        if(instance.active) instance.InstTriggerEvent(eventName);
    }

    private void InstTriggerEvent(string eventName){
        switch(eventName){
            case "spawned":
                CanvasMessager.ShowMessage(spawn,true);
                return;
            case "entered":
                if(clearedCount==0){
                    CanvasMessager.ShowMessage(entered,true);
                }
                return;
            case "cleared":
                if(clearedCount==0){
                    CanvasMessager.ShowMessage(clear1,true);
                }
                else if(clearedCount==1){
                    CanvasMessager.ShowMessage(clear2,true);
                }
                else{
                    CanvasMessager.ShowMessage(clear3,true);
                }
                clearedCount++;
                return;
            case "leftSpawn":
                return;
            default:
                Debug.LogWarning("Trigerring non-existing tutorial event");
                return;
        }
    }
}
/*
<b>Theorem 1.1</b> Let P be the player, then P can be rotated using the mouse. Furthermore, P can be moved usingthe set {<b>[W],[A],[S],[D]</b>} of keyboard keys.
<b>Theorem 1.3</b> Let R be the set of all rooms, the lecture can be finished only if <b>at least half</b> (rounded up) of the rooms in R are cleared. Clearing more rooms than neccesary will result in higher score.
<b>Theorem 1.4</b> Lecture is finished by reching the exit with the player P. To reach the exit, a map opened with the key <b>[M]</b>, an be used. 
<b>Corollary 1.5</b> All rooms are clear, the lecture is finished by entering the exit.
<b>Definition 1.2</b> The enemies are solved using the <b>right mouse button</b>, the room is considered clear if all enemies are solved.
*/