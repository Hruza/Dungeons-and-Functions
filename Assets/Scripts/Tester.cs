using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tester : MonoBehaviour
{
    public Navigator nav;
    public GameObject dummy;
    public enum TestType { navigation, follow, dash }
    public TestType test;
    private void Start()
    {
  
    }

    void Update()
    {
        switch (test)
        {
            case TestType.navigation:
                if (Input.GetMouseButton(0))
                {
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    RaycastHit hit;
                    if (Physics.Raycast(ray, out hit))
                    {
                        nav.GoToTarget(hit.point);
                    }
                }
                break;
            case TestType.follow:
                if (Input.GetMouseButton(0))
                {
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    RaycastHit hit;
                    if (Physics.Raycast(ray, out hit))
                    {
                        dummy.transform.position = hit.point+Vector3.back;
                    }


                }
                if (Input.GetMouseButtonDown(1))
                    nav.GoToTarget(dummy,2);
                break;
            case TestType.dash:
                if (Input.GetMouseButtonDown(0))
                {
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    RaycastHit hit;
                    if (Physics.Raycast(ray, out hit))
                    {
                        nav.Dash(hit.point-nav.gameObject.transform.position,(hit.point - nav.gameObject.transform.position).magnitude);
                    }
                }
                break;
            default:
                break;
        }

    }
}
