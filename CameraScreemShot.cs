using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class CameraScreemShot : MonoBehaviour
{
    public int photoCount = 0;
    public List<Camera> Cameras;
    public List<GameObject> Light;
    public Texture2D ShotRect;
    public RenderTexture _ShotRect;

    // Start is called before the first frame update
    void Start()
    {
        ShotRect = new Texture2D(960, 960);
        _ShotRect = new RenderTexture(960, 960, 32);
        _ShotRect.antiAliasing = 8;

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ThreeViewShot();
        }
    }

    public void ThreeViewShot()
    {
        for (int i = 0; i < Cameras.Count; i++)
        {
            ShotPhoto(Cameras[i]);
        }
    }

    public void ShotPhoto(Camera Camera)
    {
        photoCount++;
        Camera.targetTexture = _ShotRect;
        Camera.Render();
        RenderTexture.active = _ShotRect;
        ShotRect.ReadPixels(new Rect(0, 0, _ShotRect.width, _ShotRect.height), 0, 0);
        ShotRect.Apply();
        RenderTexture.active = null;
        Camera.targetTexture = null;
        byte[] b = ShotRect.EncodeToJPG();
        File.WriteAllBytes(Application.dataPath + "/PhotoShot/" + photoCount + ".jpg", b);///Application.dataPath+"//PhotoShot" + photoCount + ".png", b
    }

    public void RandViewShot(List<GameObject> simple,Vector3 oriPoint)
    {
        for (int i = 0; i < simple.Count; i++)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                float x = Random.Range(0, 360);
                float y = Random.Range(0, 360);
                float z = Random.Range(0, 360);

                simple[i].transform.position = oriPoint;

            }

        }
    }



}
