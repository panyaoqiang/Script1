using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class collection : MonoBehaviour
{
    public GameObject obj;
    public int num = 0;
    public List<GameObject> objs = new List<GameObject>();
    bool delet = false;
    bool count = false;
    public int process = 0;
    public GameObject obj1;
    public GameObject obj2;
    public float delta = 20f;
    List<Vector3> p = new List<Vector3>();
    // Start is called before the first frame update
    void Start()
    {
        foreach (Transform child in obj.transform)
        {
            //print(child.gameObject.name);
            objs.Add(child.gameObject);
        }
        spreadOut();
        //obj1.transform.eulerAngles = new Vector3(45,45,45);
    }

    // Update is called once per frame
    void Update()
    {
        if (process == 0 && Input.GetKeyDown(KeyCode.Space))
        {
            selected(num);
            process = 1;
        }
        if (process == 1 && Input.GetKeyDown(KeyCode.LeftArrow))
        {
            resetP(num, obj1);
            process = 2;
        }
        if (process == 1 && Input.GetKeyDown(KeyCode.RightArrow))
        {
            resetP(num, obj2);
            process = 2;
        }
        if (process == 1 && Input.GetKeyDown(KeyCode.DownArrow))
        {
            process = 2;
        }
        if (process == 2 && Input.GetKeyDown(KeyCode.Space))
        {
            reselected(num);
            process = 0;
        }
    }

    public void spreadOut()
    {
        for(int i = 0; i < 20; i++)
        {
            float x = i * delta;
            for(int j = 0; j < 30; j++)
            {
                float y = j * delta;
                p.Add(new Vector3(x, y, 0));
            }
        }
        for(int k = 0; k < objs.Count; k++)
        {
            objs[k].transform.position = p[k];
        }
    }
    public void selected(int n)
    {
        objs[n].transform.position += new Vector3(0, 50, 0);
    }
    public void reselected(int n)
    {
        if (num < objs.Count)
        {
            num++;
        }
        objs[n].transform.position -= new Vector3(0, 50, 0);
    }
    public void resetP(int n, GameObject newP)
    {
        objs[n].transform.parent = newP.transform;
    }
}
