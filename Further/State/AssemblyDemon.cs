using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssemblyDemon : MonoBehaviour
{
    public List<GameObject> Parts;
    public List<GameObject> P;
    public List<Vector3> pos;
    public List<Vector3> OrigionPos;
    public List<bool> Open = new List<bool>();
    public float speed = 0.3f;
    // Start is called before the first frame update
    void Start()
    {
        Open.Add(false);
        Open.Add(false);
        Open.Add(false);
        Open.Add(false);
        Open.Add(false);
        Open.Add(false);
        Open.Add(false);
        pos = new List<Vector3>();
        OrigionPos = new List<Vector3>();
        for (int i = 0; i < P.Count; i++)
        {
            pos.Add(P[i].transform.position);
        }
        for (int i = 0; i < Parts.Count; i++)
        {
            OrigionPos.Add(Parts[i].transform.position);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Open[0])
        {
            Move(Parts[0], pos[0], Open[0]);
        }
        if (Open[1])
        {
            Move(Parts[1], pos[1], Open[1]);
        }
        if (Open[2])
        {
            Move(Parts[2], pos[2], Open[2]);
        }
        if (Open[3])
        {
            Move(Parts[3], pos[3], Open[3]);
        }
        if (Open[4])
        {
            Move(Parts[4], pos[4], Open[4]);
        }
        if (Open[5])
        {
            Move(Parts[5], pos[5], Open[5]);
        }
        if (Open[6])
        {
            for (int i = 0; i < Parts.Count; i++)
            {
                Parts[i].transform.position= OrigionPos[i];
            }
        }
    }

    public void M1()
    {
        Open[0] = true;
    }

    public void M2()
    {
        Open[1] = true;
        Open[2] = true;
    }

    public void M3()
    {
        Open[3] = true;
        Open[4] = true;
    }

    public void M4()
    {
        Open[5] = true;
    }

    public void M5()
    {
        Open[6] = true;
    }

    public void Move(GameObject P, Vector3 pos, bool O)
    {
        Vector3 way = new Vector3();
        way = pos - P.transform.position;
        if (way.magnitude > 2f)
        {
            P.transform.position += way * speed * Time.deltaTime;
        }
        else
        {
            P.transform.position = pos;
            O = false;
        }

    }
}
