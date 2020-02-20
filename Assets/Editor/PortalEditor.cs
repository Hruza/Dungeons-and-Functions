using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor (typeof (Portal))]
public class PortalEditor : Editor {

	void OnSceneGUI() {
		Portal fow = (Portal)target;
        Quaternion rot = Quaternion.Euler(0,0,fow.angle);
		Handles.color = Color.blue;
        Handles.DrawLine(fow.transform.position-(rot*(Vector3.up*0.5f*fow.length)), fow.transform.position + (rot*(Vector3.up * 0.5f * fow.length)));
        Handles.color = Color.green;
        Handles.DrawLine(fow.transform.position,fow.transform.position - (rot *(2* Vector3.right)));
	}

}
