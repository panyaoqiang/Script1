using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class singleDetection : MonoBehaviour
{
    /// <summary>
    /// ʵ�������࣬��Ӧÿһ����������⾶���
    /// </summary>
    public singleDetection()
    {

    }

    /// <summary>
    /// ���ø�ֵ
    /// </summary>
    public List<GameObject> camObj;
    List<GameObject> detectObj = new List<GameObject>();
    Vector3 photoPoint = Vector3.zero;
    Vector3 safePoint = new Vector3(200, 200, 0);
    Vector3 waitPoint = new Vector3(-200, 0, 0);
    IEnumerator getPic;
    /// <summary>
    /// �Զ�����
    /// </summary>
    Texture2D ShotRect;
    RenderTexture _ShotRect;
    List<Camera> cam = new List<Camera>();

    /// <summary>
    /// ���ñ���
    /// </summary>
    public string classifyOrDetection;
    string objName;
    public Dictionary<string, blockType> allInfo = new Dictionary<string, blockType>();

    void Start()
    {
        //���뵥�������ڰ�����������ÿ�������ӵ������б�
        for (int i = 0; i < camObj.Count; i++)
        {
            foreach (Transform item in camObj[i].transform)
            {
                cam.Add(item.gameObject.GetComponent<Camera>());
            }
        }
        getPic = takePic();

        TransformData a = new TransformData();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(getPic);
        }
    }
    void photoInitialized(int weidth, int height)
    {
        ShotRect = new Texture2D(weidth, height);
        _ShotRect = new RenderTexture(weidth, height, 32);
        _ShotRect.antiAliasing = 8;
    }

    IEnumerator takePic()
    {
        if (classifyOrDetection == "classify")
        {
            photoInitialized(960, 960);
        }
        else
        {
            print("d");
            photoInitialized(2880, 2880);
            for (int i = 0; i < cam.Count; i++)
            {
                ShotPhoto(cam[i], "test");
            }
        }
        for (int j = 0; j < detectObj.Count; j++)
        {
            detectObj[j].transform.position = photoPoint;
            for (int i = 0; i < cam.Count; i++)
            {
                ShotPhoto(cam[i], detectObj[j].name);
            }
            objName += detectObj[j].name + ".jpg\r\n";
            detectObj[j].transform.position = safePoint;
            yield return 0;
        }
        //writeTxT("G:/data/", "objName", objName);
        StopAllCoroutines();
    }

    void ShotPhoto(Camera Camera, string name)
    {
        Camera.targetTexture = _ShotRect;
        Camera.Render();
        RenderTexture.active = _ShotRect;
        ShotRect.ReadPixels(new Rect(0, 0, _ShotRect.width, _ShotRect.height), 0, 0);
        ShotRect.Apply();
        RenderTexture.active = null;
        Camera.targetTexture = null;
        byte[] b = ShotRect.EncodeToJPG();
        print("G:/data/" + name + ".jpg");
        File.WriteAllBytes("G:/data/" + name + ".jpg", b);
    }

    /// <summary>
    /// д��txt
    /// </summary>
    /// <param name="file_path">����·��</param>
    /// <param name="file_name">�ļ���</param>
    /// <param name="str_info">д����Ϣ</param>
    void writeTxT(string file_path, string file_name, string str_info)//д���ļ�
    {
        StreamWriter sw;
        if (file_path == "")
        {
            sw = File.CreateText(file_name);//����һ������д�� UTF-8 ������ı�  
        }
        else
        {
            sw = File.CreateText(file_path + "//" + file_name);//����һ������д�� UTF-8 ������ı�  
        }
        sw.WriteLine(str_info);//����Ϊ��λд���ַ���  
        sw.Close();
        sw.Dispose();//�ļ����ͷ�  
    }
}
