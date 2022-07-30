using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor;

public class GetPic : MonoBehaviour
{
    #region ���ø�ֵ���������ó�ʼ������
    //ֻ��rightʹ��960*960
    public Texture2D classifyShotRect;
    public RenderTexture classify_ShotRect;
    //����up��ʹ��2880*2880
    public Texture2D detectShotRect;
    public RenderTexture detect_ShotRect;
    IEnumerator pretreat;
    IEnumerator checkIterator;
    IEnumerator simShot;
    public Material oriMaterial;
    public float deltaScale = 1.5f;
    public List<GameObject> Cams;
    Dictionary<string, List<Camera>> Cameras = new Dictionary<string, List<Camera>>();
    public TransformData info;
    string not_shaftGoogLeNetPath = "G:/data/test/not_shaft/side_view/";
    string not_shaftYoloPath = "G:/data/test/not_shaft/up_view/";
    string shaftYoloPath = "G:/data/test/shaft/up_view/";//test����pic

    string shaftScalePath = "Assets/data/";
    string not_shaftScalePath = "Assets/data/"; //G:/data/test/
    #endregion

    //�����ļ�·����ȡ�ļ��������Ƶ�ָ��Ŀ¼��
    public string modelPath;

    //��ת�������ԭʼ�ļ�
    public GameObject obj2BeConvert;
    //��ȡ��������װ��ĸ����壬��������ΪԤ����
    public GameObject extractObj;
    //�����ȡ��Ϻ�������ߴ粢��ȡ�任�������в����任
    public bool start = false;
    //����������弯��
    List<GameObject> s = new List<GameObject>();
    //����ȼ�������ᣬ��¼txt�������changeSim�ĸ�ֵ
    List<GameObject> shaft = new List<GameObject>();
    //����ȼ��������ת�壬��¼txt�������changeSim�ĸ�ֵ
    List<GameObject> not_shaft = new List<GameObject>();
    Dictionary<string, sizeInfo> shaft_scale = new Dictionary<string, sizeInfo>();
    Dictionary<string, sizeInfo> not_shaft_scale = new Dictionary<string, sizeInfo>();
    Dictionary<string, List<Vector3>> oriTransform = new Dictionary<string, List<Vector3>>();
    string shaft_info;
    string not_shaft_info;

    /// <summary>
    /// start����ʹ��
    /// </summary>
    void Start()
    {
        camsAdd();
        //ֻ��rightʹ��
        classifyShotRect = new Texture2D(960, 960);
        classify_ShotRect = new RenderTexture(960, 960, 32);
        classify_ShotRect.antiAliasing = 8;
        //����upʹ��
        detectShotRect = new Texture2D(2880, 2880);
        detect_ShotRect = new RenderTexture(2880, 2880, 32);
        detect_ShotRect.antiAliasing = 8;
        pretreat = pretreatSim();
        simShot = simplesShot();
        extractObj = new GameObject();
    }

    private void Update()
    {
        if (start)
        {
            Extract3DModle();
        }
    }
    /// <summary>
    /// ���ⲿ�����ļ�
    /// </summary>
    public void getFileModel()
    {
        File.Copy(modelPath, Application.dataPath + "/Resources/data/ori3DM.fbx", true);
        AssetDatabase.Refresh();
        obj2BeConvert = Instantiate((GameObject)Resources.Load("data/ori3DM"));
        obj2BeConvert.transform.position = Vector3.zero;

        extractObj.transform.position = obj2BeConvert.transform.position;
        extractObj.transform.eulerAngles = obj2BeConvert.transform.eulerAngles;
        extractObj.name = obj2BeConvert.name.Replace("(Clone)", " ");
    }

    public void getPrefab()
    {
        extractObj.transform.position = obj2BeConvert.transform.position;
        extractObj.transform.eulerAngles = obj2BeConvert.transform.eulerAngles;
        extractObj.name = obj2BeConvert.name.Replace("(Clone)", " ");
    }



    /// <summary>
    /// ���������룬�����ʼ��
    /// </summary>
    [System.Obsolete]
    void initialization()
    {
        info.shaft_scale = shaft_scale;
        info.not_shaft_scale = not_shaft_scale;
        PrefabUtility.CreatePrefab("Assets/Resources/data/model.prefab", extractObj);
        obj2BeConvert = null;
        start = false;
        s.Clear();
        shaft.Clear();
        not_shaft.Clear();
        shaft_scale.Clear();
        not_shaft_scale.Clear();
        oriTransform.Clear();
    }

    /// <summary>
    /// ��һ��
    /// ��ȡ�������
    /// </summary>
    void Extract3DModle()
    {
        if (obj2BeConvert.GetComponentInChildren<MeshRenderer>() != null)
        {
            GameObject obj = obj2BeConvert.GetComponentInChildren<MeshRenderer>().
                gameObject;
            obj.GetComponent<MeshRenderer>().material = oriMaterial;
            obj.transform.SetParent(extractObj.transform);
            obj.name = obj.name.Replace("(Clone)", " ");
            obj.name = obj.name.Replace(" ", "");
        }
        else
        {
            start = false;
            obj2BeConvert.SetActive(false);
            StartCoroutine(pretreat);
        }

        //foreach (Transform child in obj2BeConvert.transform)
        //{
        //    GameObject copyC;
        //    if (child.GetComponent<MeshRenderer>() != null)
        //    {
        //        copyC = Instantiate(child.gameObject);
        //        copyC.transform.position = child.position;
        //        copyC.transform.eulerAngles = child.eulerAngles;
        //        copyC.transform.SetParent(extractObj.transform);
        //        copyC.name = copyC.name.Replace("(Clone)", " ");
        //        copyC.name = copyC.name.Replace(" ", "");
        //    }
        //    else if (child.GetComponentInChildren<MeshRenderer>() != null)
        //    {
        //        copyC = Instantiate(child.GetComponentInChildren<MeshRenderer>().gameObject);
        //        copyC.transform.position = child.position;
        //        copyC.transform.eulerAngles = child.eulerAngles;
        //        copyC.transform.SetParent(extractObj.transform);
        //        copyC.name = copyC.name.Replace("(Clone)", " ");
        //        copyC.name = copyC.name.Replace(" ", "");
        //    }
        //}
    }

    /// <summary>
    /// �ڶ��������������ϣ�ÿһ��������������Ƕ���ͼ���������
    /// </summary>
    /// <returns></returns>
    void camsAdd()
    {
        for (int i = 0; i < Cams.Count; i++)
        {
            List<Camera> s = new List<Camera>();
            foreach (Transform child in Cams[i].transform)
            {
                s.Add(child.transform.GetComponent<Camera>());
            }
            Cameras.Add(Cams[i].name, s);
        }
    }

    /// <summary>
    /// ������
    /// ���ģ���������б任���������Ϻ��ٽ��е���
    /// 1.��ȡģ�ͱ���
    /// 2.�����¸�����
    /// 3.�����¸�����Ե���������м��
    /// </summary>
    /// <returns></returns>
    IEnumerator pretreatSim()
    {
        foreach (Transform child in extractObj.transform)
        {
            s.Add(child.transform.gameObject);
            yield return 0;
        }
        checkIterator = changeSim();
        StartCoroutine(checkIterator);
        StopCoroutine(pretreat);
    }

    /// <summary>
    /// ���Ĳ�
    /// �����ײ���Բ����м�⣬��ȡsize_info
    /// �����Ϣ��shaftscale��notshaftscale
    /// ���������������Ϣ
    /// </summary>
    /// <returns></returns>
    IEnumerator changeSim()
    {
        //ֻ��һ�������弯��
        for (int j = 0; j < s.Count; j++)
        {
            BoxCollider b = s[j].AddComponent<BoxCollider>();
            b.isTrigger = true;
            Rigidbody r = s[j].AddComponent<Rigidbody>();
            r.useGravity = false;
            r.isKinematic = true;
            yield return new WaitForSeconds(0.1f);
            float size;
            sizeInfo box_info = facingCam(s[j]);
            //print(s[j].name + box_info.type);
            //box_info.samScale�����˸��������ȼ��ʱ���űȣ���ģ�͵�һ�μ��任��Ϊ1
            //����������Ϣ�任�󣬴�ֵ����Ϊ����������ű�
            if (box_info.type == "shaft")
            {
                size = box_info.l;
                box_info.samScale = size * box_info.samScale / 2f + deltaScale;
                shaft_info += s[j].name + ", " + box_info.samScale.ToString() + ", " +
                    (box_info.samAngle).ToString() + "\n";
                shaft.Add(s[j]);
                shaft_scale.Add(s[j].name, box_info);
            }
            else
            {
                size = box_info.r;
                box_info.samScale = size * box_info.samScale / 2f + deltaScale;
                not_shaft_info += s[j].name + ", " + box_info.samScale.ToString() + ", " +
                    (box_info.samAngle).ToString() + "\n";
                not_shaft.Add(s[j]);
                not_shaft_scale.Add(s[j].name, box_info);
            }
            yield return null;
        }
        WriteFileByLine(shaftScalePath, "shaft_info.txt", shaft_info);
        WriteFileByLine(not_shaftScalePath, "not_shaft_info.txt", not_shaft_info);
        getOriTransf();
        StartCoroutine(simShot);
        StopCoroutine(checkIterator);
    }

    /// <summary>
    /// ���岽
    /// ����shaft��notshaft��ȡԭʼ���λ��װ��oriTransform
    /// </summary>
    void getOriTransf()
    {
        for (int i = 0; i < shaft.Count; i++)
        {
            List<Vector3> o = new List<Vector3>();
            o.Add(shaft[i].transform.position);
            o.Add(shaft[i].transform.eulerAngles);
            oriTransform.Add(shaft[i].name, o);
        }
        for (int i = 0; i < not_shaft.Count; i++)
        {
            List<Vector3> o = new List<Vector3>();
            o.Add(not_shaft[i].transform.position);
            o.Add(not_shaft[i].transform.eulerAngles);
            oriTransform.Add(not_shaft[i].name, o);
        }
    }

    /// <summary>
    /// ������
    /// �Բ�ͬ���������ò�ͬ�������������
    /// </summary>
    /// <returns></returns>
    [System.Obsolete]
    IEnumerator simplesShot()
    {
        for (int j = 0; j < shaft.Count; j++)
        {
            Shot(shaft[j], "shaft");
            yield return null;
        }
        for (int j = 0; j < not_shaft.Count; j++)
        {
            Shot(not_shaft[j], "not_shaft");
            yield return null;
        }
        print("fin");
        initialization();
        StopAllCoroutines();
    }

    #region ͨ�÷���
    /// <summary>
    /// ÿһ֡����һ�����������Ƕ���ͼ
    /// </summary>
    /// <param name="sim">��ǰ֡Ҫ�������������</param>
    /// <param name="mode">����ģʽ��rl��rs��fs��fl</param>
    public void Shot(GameObject sim, string shaftOrNot)
    {
        //������������ԭ���غ�
        //sim.transform.position = - sim.GetComponent<BoxCollider>().center;
        sim.transform.parent = null;
        sim.transform.position = Vector3.zero;
        sim.transform.eulerAngles = Vector3.zero;
        if (shaftOrNot == "shaft")
        {
            for (int i = 0; i < Cameras["shaftCam"].Count; i++)
            {
                Cameras["shaftCam"][i].enabled = true;
                sim.transform.eulerAngles += shaft_scale[sim.gameObject.name].samAngle;
                sim.transform.position = -rotateMatrix
                    (shaft_scale[sim.gameObject.name].samAngle, sim.GetComponent<BoxCollider>().center);
                Cameras["shaftCam"][i].orthographicSize = shaft_scale[sim.name].samScale;
                ShotPhoto(Cameras["shaftCam"][i], sim.gameObject.name,
                    detect_ShotRect, detectShotRect, shaftYoloPath);
                Cameras["shaftCam"][i].enabled = false;
            }
        }
        else
        {
            for (int i = 0; i < Cameras["not_shaftCam"].Count; i++)
            {
                Cameras["not_shaftCam"][i].enabled = true;
                sim.transform.eulerAngles += not_shaft_scale[sim.gameObject.name].samAngle;
                sim.transform.position = -rotateMatrix
                     (not_shaft_scale[sim.gameObject.name].samAngle, sim.GetComponent<BoxCollider>().center);
                Cameras["not_shaftCam"][i].orthographicSize = not_shaft_scale[sim.name].samScale;
                if (Cameras["not_shaftCam"][i].name == "right")
                {
                    ShotPhoto(Cameras["not_shaftCam"][i], sim.gameObject.name,
                        classify_ShotRect, classifyShotRect, not_shaftGoogLeNetPath);
                }
                else
                {
                    ShotPhoto(Cameras["not_shaftCam"][i], sim.gameObject.name,
                        detect_ShotRect, detectShotRect, not_shaftYoloPath);
                }
                Cameras["not_shaftCam"][i].enabled = false;
            }
        }
        sim.transform.SetParent(extractObj.transform);
        sim.transform.position = oriTransform[sim.gameObject.name][0];
        sim.transform.eulerAngles = oriTransform[sim.gameObject.name][1];
    }

    /// <summary>
    /// ��ת����
    /// </summary>
    /// <param name="xyz">��ת��</param>
    /// <param name="rad">��ת�Ƕ�</param>
    /// <param name="oriM">����</param>
    /// <returns>��ת��ľ���</returns>
    Vector3 rotateMatrix(Vector3 scaleAngle, Vector3 oriM)
    {
        float rad = -90;
        string xyz;
        if (scaleAngle == new Vector3(0, 90, 0))
        {
            xyz = "y";
        }
        else if (scaleAngle == new Vector3(0, 0, 90))
        {
            xyz = "z";
        }
        else
        {
            return (oriM);
        }
        Vector3 r;
        switch (xyz)
        {
            case "x":
                r = new Vector3(
                    oriM.x,
                    Mathf.Cos(rad) * oriM.y - Mathf.Sin(rad) * oriM.z,
                    Mathf.Cos(rad) * oriM.z + Mathf.Sin(rad) * oriM.y
                    );
                break;
            case "y":
                r = new Vector3(
                   Mathf.Cos(rad) * oriM.x + Mathf.Sin(rad) * oriM.z,
                    oriM.y,
                    Mathf.Cos(rad) * oriM.z - Mathf.Sin(rad) * oriM.x
                    );
                break;
            default:
                r = new Vector3(
                    Mathf.Cos(rad) * oriM.x - Mathf.Sin(rad) * oriM.y,
                    Mathf.Cos(rad) * oriM.y + Mathf.Sin(rad) * oriM.x,
                    oriM.z
                    );
                break;
        }
        print(r);
        return r;
    }

    /// <summary>
    /// ʹ����������x�ᣬ
    /// </summary>
    /// <param name="box">�����������ȡ�����boxcollider</param>
    /// <returns>�������ǳߴ��µĻ�ת�뾶r�;��򳤶�l������ȼ����</returns>
    sizeInfo facingCam(GameObject box)
    {
        float r;
        float l;
        string type;
        float scale;
        Vector3 rotAngle;
        float x = box.GetComponent<BoxCollider>().size.x;
        float y = box.GetComponent<BoxCollider>().size.y;
        float z = box.GetComponent<BoxCollider>().size.z;
        float xy = Mathf.Abs(y - x);
        float yz = Mathf.Abs(y - z);
        float xz = Mathf.Abs(x - z);
        float min = xy;
        if (yz < min)
        {
            min = yz;
        }
        if (xz < min)
        {
            min = xz;
        }
        //z��Ϊ���棬��y����ת90��
        if (min == xy)
        {
            //box.transform.eulerAngles += new Vector3(0, 90, 0);
            rotAngle = new Vector3(0, 90, 0);
            r = x;
            l = z;
            scale = box.transform.localScale.z;
        }
        //y��Ϊ����,��z����ת90��
        else if (min == xz)
        {
            //box.transform.eulerAngles += new Vector3(0, 0, 90);
            rotAngle = new Vector3(0, 0, 90);
            r = x;
            l = y;
            scale = box.transform.localScale.y;
        }
        //x��Ϊ���治��ת��
        else
        {
            rotAngle = Vector3.zero;
            r = y;
            l = x;
            scale = box.transform.localScale.x;
        }

        if (l / r >= 1.2)
        {
            type = "shaft";
        }
        else
        {
            type = "not_shaft";
        }
        sizeInfo re = new sizeInfo();
        re.r = r;
        re.l = l;
        re.type = type;
        re.samScale = scale;
        re.samAngle = rotAngle;
        return (re);
    }

    /// <summary>
    /// дtxt
    /// </summary>
    /// <param name="file_path"></param>
    /// <param name="file_name"></param>
    /// <param name="str_info"></param>
    void WriteFileByLine(string file_path, string file_name, string str_info)
    {
        StreamWriter sw;
        //if (!File.Exists(file_path + "//" + file_name))
        //{

        if (file_path == "")
        {
            sw = File.CreateText(file_name);//����һ������д�� UTF-8 ������ı�  
        }
        else
        {
            sw = File.CreateText(file_path + "//" + file_name);//����һ������д�� UTF-8 ������ı�  
        }
        //Debug.Log("�ļ������ɹ���");
        //}
        //else
        //{
        //    if (file_path == "")
        //    {
        //        sw = File.AppendText(file_name);//������ UTF-8 �����ı��ļ��Խ��ж�ȡ  
        //    }
        //    else
        //    {
        //        sw = File.AppendText(file_path + "//" + file_name);//������ UTF-8 �����ı��ļ��Խ��ж�ȡ  
        //    }
        //}
        sw.WriteLine(str_info);//����Ϊ��λд���ַ���  
        sw.Close();
        sw.Dispose();//�ļ����ͷ�  
    }

    /// <summary>
    /// �������
    /// </summary>
    /// <param name="Camera">�ô�����������������</param>
    /// <param name="name">�����������</param>
    public void ShotPhoto(Camera Camera, string name, RenderTexture _ShotRect, Texture2D ShotRect, string path)
    {
        Camera.targetTexture = _ShotRect;
        Camera.Render();
        RenderTexture.active = _ShotRect;
        ShotRect.ReadPixels(new Rect(0, 0, _ShotRect.width, _ShotRect.height), 0, 0);
        ShotRect.Apply();
        RenderTexture.active = null;
        Camera.targetTexture = null;
        byte[] b = ShotRect.EncodeToJPG();
        File.WriteAllBytes(path + name + ".jpg", b);//Application.dataPath + "/PhotoShot/"/Application.dataPath+"//PhotoShot" + photoCount + ".png", b
    }
    #endregion
}
/// <summary>
/// ���������ߴ�����Ϣ
/// ���β���������scale��Ϊ1
/// </summary>
public struct sizeInfo
{
    public float r;
    public float l;
    public string type;
    public float samScale;
    public Vector3 samAngle;
}
