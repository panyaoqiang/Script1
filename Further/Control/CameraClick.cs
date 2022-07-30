using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraClick : MonoBehaviour
{

    GameObject ClickObj = null;
    Vector3 OrigionalPosition;
    Vector3 OrigionalRotation;
    Vector3 LockPosition;
    Vector3 LockRotation;
    //Text Reset;
    public float speed;
    public int OrigionalCount = 0;
    public int LockCount = 1;
    bool Catch = false;

    private void Start()
    {

    }
    void Update()
    {
        speed = 10f;
        RayCatch(ClickObj,Catch);
        if (Catch)
        {
            GetOrigional(ClickObj, OrigionalCount);
            ClickToMove(ClickObj, speed);
            ClickToRotate(ClickObj, speed);
        }

        ResetTransform(ClickObj, LockCount);
        if (Input.GetKeyDown(KeyCode.L))
        {
            LockCount = 0;
            LockAll(ClickObj, LockCount);
        }
        if (LockCount == 0 && Input.GetKeyDown(KeyCode.Space))
        {
            ObjClear(ClickObj);
            CountClear(OrigionalCount, LockCount);
        }


    }
    public void RayCatch(GameObject ClickObj,bool Catch)
    {
        try
        {

            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                Physics.Raycast(ray, out hit);
                ClickObj = hit.transform.gameObject;
                Catch = true;
            }


        }
        catch
        {

        }

    }

    public void GetOrigional(GameObject ClickObj, int OrigionalCount)
    {
        if (OrigionalCount == 0)
        {
            OrigionalPosition = ClickObj.transform.position;
            OrigionalRotation = ClickObj.transform.eulerAngles;
            OrigionalCount++;
        }


    }

    public void ClickToMove(GameObject ClickObj, float speed)
    {
        if (Input.GetKeyDown(KeyCode.A))
        //X轴+
        {
            ClickObj.transform.position += new Vector3(speed * Time.deltaTime, 0f, 0f);
        }
        if (Input.GetKeyDown(KeyCode.D))
        //X轴-
        {
            ClickObj.transform.position -= new Vector3(speed * Time.deltaTime, 0f, 0f);
        }

        //Y轴+
        if (Input.GetKeyDown(KeyCode.W))
        {
            ClickObj.transform.position += new Vector3(0f, speed * Time.deltaTime, 0f);
        }
        //Y轴-
        if (Input.GetKeyDown(KeyCode.S))
        {
            ClickObj.transform.position -= new Vector3(0f, speed * Time.deltaTime, 0f);
        }

        //Z轴+
        if (Input.GetKeyDown(KeyCode.Q))
        {
            ClickObj.transform.position -= new Vector3(0f, 0f, speed * Time.deltaTime);
        }
        //Z轴-
        if (Input.GetKeyDown(KeyCode.E))
        {
            ClickObj.transform.position += new Vector3(0f, 0f, speed * Time.deltaTime);
        }

    }

    public void ClickToRotate(GameObject ClickObj, float speed)
    {
        if (Input.GetKeyDown(KeyCode.Keypad2))
        //X轴+
        {
            ClickObj.transform.eulerAngles += new Vector3(speed * Time.deltaTime, 0f, 0f);
        }
        if (Input.GetKeyDown(KeyCode.Keypad5))
        //X轴-
        {
            ClickObj.transform.eulerAngles -= new Vector3(speed * Time.deltaTime, 0f, 0f);
        }
        if (Input.GetKeyDown(KeyCode.Keypad3))
        //Z轴+
        {
            ClickObj.transform.eulerAngles += new Vector3(0f, 0f, speed * Time.deltaTime);
        }
        if (Input.GetKeyDown(KeyCode.Keypad1))
        //Z轴-
        {
            ClickObj.transform.eulerAngles -= new Vector3(0f, 0f, speed * Time.deltaTime);
        }
        if (Input.GetKeyDown(KeyCode.Keypad6))
        //Y轴+
        {
            ClickObj.transform.eulerAngles += new Vector3(0f, speed * Time.deltaTime, 0f);
        }
        if (Input.GetKeyDown(KeyCode.Keypad4))
        //Y轴-
        {
            ClickObj.transform.eulerAngles -= new Vector3(0f, speed * Time.deltaTime, 0f);
        }

    }

    public void ResetTransform(GameObject ClickObj, int LockCount)
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            LockCount = 1;
            ClickObj.transform.position = OrigionalPosition;
            ClickObj.transform.eulerAngles = OrigionalRotation;
            //Reset.text = "物体已重置";
        }

    }

    public void LockAll(GameObject ClickObj, int LockCount)
    {
        if (LockCount == 0)
        {
            LockPosition = ClickObj.transform.position;
            LockRotation = ClickObj.transform.eulerAngles;
            ClickObj.transform.position = LockPosition;
            ClickObj.transform.eulerAngles = LockRotation;

        }

    }

    public GameObject ObjClear(GameObject ClickObj)
    {
        ClickObj = null;
        return ClickObj;
    }

    public void CountClear(int OrigionalCount, int LockCount)
    {
        OrigionalCount--;
        LockCount--;
    }

}
