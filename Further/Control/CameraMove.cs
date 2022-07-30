using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    public DateStore Date;

    private void Start()
    {
        if (GameObject.Find("DateSaver")!= null)
        {
            Date = GameObject.Find("DateSaver").GetComponent<DateStore>();
            gameObject.transform.position = Date.CameraPos;
            gameObject.transform.rotation = Date.CameraRot;
        }
    }
}
