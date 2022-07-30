using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test2 : MonoBehaviour
{
    public GameObject c1;
    public GameObject c2;
    public Vector3 startP;
    public Vector3 endP;
    public bool startRot;
    public Vector3 center;
    public Vector3 star;
    public Vector3 end;
    public float angle = 0;
    public float r = 0.56f;
    public float v = 0;
    // Start is called before the first frame update
    void Start()
    {
        Ray ray = new Ray(new Vector3(5, r, 0), Vector3.forward);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            startP = hit.point;

            c1.transform.position = hit.point;
            //startP = hit.point;
            //Ray ray1 = new Ray(startP, Vector3.forward);
            //RaycastHit hit1;
            //if (Physics.Raycast(ray1, out hit1))
            //{
            //    c1.transform.position = hit1.point;
            //    endP = hit1.point;
            //}
        }
        center = new Vector3(5, 0, 0);
        //star = startP - center;
        //end = (endP - center).normalized * star.magnitude;
        //endP = end + center;
        //c1.transform.position = endP;
        //startRot = true;
        v = 1.5f * Mathf.Rad2Deg * Mathf.Asin(transform.localScale.x * 0.5f / r);
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        if (startRot && angle < 360)
        {
            c1.transform.RotateAround(center, Vector3.right, v);
            angle += v;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Vector3 tangent = Vector3.Cross(Vector3.right, startP - center).normalized;
            Ray ray1 = new Ray(startP, tangent);
            RaycastHit hit1;
            if (Physics.Raycast(ray1, out hit1))
            {
                c2.transform.position = hit1.point;
                startP = hit1.point;
            }
        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (startRot)
        {
            print("³ö");
            //endP = c1.transform.position;
            //startRot = false;
            //end = endP - center;
            //float d = Mathf.Acos(Vector3.Dot(end, star) / (end.magnitude * star.magnitude)) * Mathf.Rad2Deg;
            //print(360 / d);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (startRot)
        {
            print("Èë");
            //endP = c1.transform.position;
            //startRot = false;
            //end = endP - center;
            //float d = Mathf.Acos(Vector3.Dot(end, star) / (end.magnitude * star.magnitude)) * Mathf.Rad2Deg;
            //print(360 / d);
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (startRot)
        {
            print("Í£");
        }
    }
}
