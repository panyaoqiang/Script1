using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;

public class test1 : MonoBehaviour
{
    Texture2D ShotRect;
    RenderTexture _ShotRect;
    public Camera cam;
    public GameObject obj;
    public bool distinct = false;
    private void Start()
    {
        ShotRect = new Texture2D(960, 960);
        _ShotRect = new RenderTexture(960, 960, 32);
        _ShotRect.antiAliasing = 8;

        //Show3DModel(50, 10);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ShotPhoto(cam, "standard");
        }

        if (distinct)
        {
            List<Vector3> p1 = new List<Vector3>();
            for (int i = 0; i < point1.Count; i++)
            {
                if (!p1.Contains(point1[i]))
                {
                    p1.Add(point1[i]);
                    Instantiate(obj, point1[i], Quaternion.identity);

                }
            }
            distinct = false;
        }
    }

    public List<GameObject> a = new List<GameObject>();
    public List<Vector3> point = new List<Vector3>();
    public void ShotPhoto(Camera Camera, string name)
    {
        Camera.targetTexture = _ShotRect;
        Camera.Render();
        RenderTexture.active = _ShotRect;
        ShotRect.ReadPixels(new Rect(0, 0, _ShotRect.width, _ShotRect.height), 0, 0);
        ShotRect.Apply();
        RenderTexture.active = null;
        Camera.targetTexture = null;
        byte[] b = ShotRect.EncodeToJPG();
        File.WriteAllBytes("I:/data/" + name + ".jpg", b);
        //Application.dataPath + "/PhotoShot/"/Application.dataPath+"//PhotoShot" + photoCount + ".png", b
    }

    public void Show3DModel(float n, float interval)
    {
        foreach (Transform child in obj.transform)
        {
            foreach (Transform g in child)
            {
                if (g.GetComponent<BoxCollider>())
                {
                    g.GetComponent<BoxCollider>().enabled = false;
                    print("off");
                }
                a.Add(g.gameObject);
            }
        }
        for (int i = 0; i < n; i++)
        {
            float line = i * interval;
            for (int j = 0; j < n; j++)
            {
                float row = j * interval;
                Vector3 p = new Vector3(line, row, 0);
                point.Add(p);
            }
        }
        for (int i = 0; i < a.Count; i++)
        {
            a[i].transform.position = point[i];
        }
    }

    List<Vector3> point1 = new List<Vector3>();
    private void OnTriggerEnter(Collider other)
    {
        print(other.name + transform.name);
        point1.Add(other.bounds.center);
        //Instantiate(obj, other.bounds.center, Quaternion.identity);
        //obj.transform.position = other.bounds.center;
    }

}
