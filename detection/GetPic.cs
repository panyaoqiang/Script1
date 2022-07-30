using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor;

public class GetPic : MonoBehaviour
{
    #region 永久赋值，迭代后不用初始化重置
    //只有right使用960*960
    public Texture2D classifyShotRect;
    public RenderTexture classify_ShotRect;
    //所有up均使用2880*2880
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
    string shaftYoloPath = "G:/data/test/shaft/up_view/";//test换成pic

    string shaftScalePath = "Assets/data/";
    string not_shaftScalePath = "Assets/data/"; //G:/data/test/
    #endregion

    //输入文件路径读取文件，并复制到指定目录下
    public string modelPath;

    //待转换零件，原始文件
    public GameObject obj2BeConvert;
    //提取零件本体后装入的父物体，将被制作为预制体
    public GameObject extractObj;
    //零件提取完毕后检测零件尺寸并获取变换参数进行采样变换
    public bool start = false;
    //采样检测物体集合
    List<GameObject> s = new List<GameObject>();
    //长宽比检测结果，轴，记录txt及对象池changeSim的赋值
    List<GameObject> shaft = new List<GameObject>();
    //长宽比检测结果，回转体，记录txt及对象池changeSim的赋值
    List<GameObject> not_shaft = new List<GameObject>();
    Dictionary<string, sizeInfo> shaft_scale = new Dictionary<string, sizeInfo>();
    Dictionary<string, sizeInfo> not_shaft_scale = new Dictionary<string, sizeInfo>();
    Dictionary<string, List<Vector3>> oriTransform = new Dictionary<string, List<Vector3>>();
    string shaft_info;
    string not_shaft_info;

    /// <summary>
    /// start永久使用
    /// </summary>
    void Start()
    {
        camsAdd();
        //只有right使用
        classifyShotRect = new Texture2D(960, 960);
        classify_ShotRect = new RenderTexture(960, 960, 32);
        classify_ShotRect.antiAliasing = 8;
        //所有up使用
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
    /// 从外部导入文件
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
    /// 多个零件导入，检测后初始化
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
    /// 第一步
    /// 提取零件本体
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
    /// 第二步，添加相机集合，每一个个体包含各个角度视图的相机个体
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
    /// 第三步
    /// 添加模型样本进行变换，当添加完毕后再进行迭代
    /// 1.提取模型本体
    /// 2.放入新父物体
    /// 3.遍历新父物体对单个零件进行检测
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
    /// 第四步
    /// 添加碰撞属性并进行检测，获取size_info
    /// 添加信息至shaftscale，notshaftscale
    /// 添加轴与非轴分类信息
    /// </summary>
    /// <returns></returns>
    IEnumerator changeSim()
    {
        //只有一个父物体集合
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
            //box_info.samScale储存了该零件长宽比检测时缩放比，新模型第一次检测变换常为1
            //经过下列信息变换后，此值将变为相机采样缩放比
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
    /// 第五步
    /// 根据shaft与notshaft获取原始零件位姿装进oriTransform
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
    /// 第六步
    /// 对不同的样本采用不同的相机进行拍摄
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

    #region 通用方法
    /// <summary>
    /// 每一帧拍摄一个零件的任意角度视图
    /// </summary>
    /// <param name="sim">当前帧要拍摄样本的零件</param>
    /// <param name="mode">拍摄模式：rl，rs，fs，fl</param>
    public void Shot(GameObject sim, string shaftOrNot)
    {
        //是网格中心与原点重合
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
    /// 旋转矩阵
    /// </summary>
    /// <param name="xyz">旋转轴</param>
    /// <param name="rad">旋转角度</param>
    /// <param name="oriM">矩阵</param>
    /// <returns>旋转后的矩阵</returns>
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
    /// 使零件面向相机x轴，
    /// </summary>
    /// <param name="box">传入零件并获取零件的boxcollider</param>
    /// <returns>返回真是尺寸下的回转半径r和径向长度l及长宽比检测结果</returns>
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
        //z轴为正面，绕y轴旋转90°
        if (min == xy)
        {
            //box.transform.eulerAngles += new Vector3(0, 90, 0);
            rotAngle = new Vector3(0, 90, 0);
            r = x;
            l = z;
            scale = box.transform.localScale.z;
        }
        //y轴为正面,绕z轴旋转90°
        else if (min == xz)
        {
            //box.transform.eulerAngles += new Vector3(0, 0, 90);
            rotAngle = new Vector3(0, 0, 90);
            r = x;
            l = y;
            scale = box.transform.localScale.y;
        }
        //x轴为正面不用转动
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
    /// 写txt
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
            sw = File.CreateText(file_name);//创建一个用于写入 UTF-8 编码的文本  
        }
        else
        {
            sw = File.CreateText(file_path + "//" + file_name);//创建一个用于写入 UTF-8 编码的文本  
        }
        //Debug.Log("文件创建成功！");
        //}
        //else
        //{
        //    if (file_path == "")
        //    {
        //        sw = File.AppendText(file_name);//打开现有 UTF-8 编码文本文件以进行读取  
        //    }
        //    else
        //    {
        //        sw = File.AppendText(file_path + "//" + file_name);//打开现有 UTF-8 编码文本文件以进行读取  
        //    }
        //}
        sw.WriteLine(str_info);//以行为单位写入字符串  
        sw.Close();
        sw.Dispose();//文件流释放  
    }

    /// <summary>
    /// 拍摄程序
    /// </summary>
    /// <param name="Camera">用传入相机个体进行拍摄</param>
    /// <param name="name">保存零件名称</param>
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
/// 返回样本尺寸检测信息
/// 初次采样，返回scale常为1
/// </summary>
public struct sizeInfo
{
    public float r;
    public float l;
    public string type;
    public float samScale;
    public Vector3 samAngle;
}
