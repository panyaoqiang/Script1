using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor;

/// <summary>
/// 第一个运行的脚本，执行完毕后调用GetPic
/// 提取零件本体并另存为预制体
/// 检测并记录零件最佳采样尺寸及变换角度
/// 数据存储为集合并导出为txt
/// </summary>
public class ChangeSize : MonoBehaviour
{
    #region 永久赋值，迭代后不用初始化重置
    IEnumerator pretreat;
    IEnumerator checkIterator;
    public float deltaScale = 2;
    GetPic photo;
    #endregion

    //待转换零件，原始文件
    public GameObject obj2BeConvert;
    //提取零件本体后装入的父物体，将被制作为预制体
    public GameObject extractObj;
    //零件提取完毕后检测零件尺寸并获取变换参数进行采样变换
    public bool start2CheckSize = false;

    //采样检测物体集合
    List<GameObject> s = new List<GameObject>();
    //长宽比检测结果，轴，记录txt及对象池
    string shaft_info;
    List<GameObject> shaft = new List<GameObject>();
    Dictionary<string, sizeInfo> shaft_transf = new Dictionary<string, sizeInfo>();
    //长宽比检测结果，回转体，记录txt及对象池
    string not_shaft_info;
    List<GameObject> not_shaft = new List<GameObject>();
    Dictionary<string, sizeInfo> not_shaft_transf = new Dictionary<string, sizeInfo>();


    void Start()
    {
        Extract3DModle();
        photo = this.GetComponent<GetPic>();
        pretreat = pretreatSim();
    }

    private void Update()
    {
        if (start2CheckSize)
        {
            StartCoroutine(pretreat);
            start2CheckSize = false;
        }
    }

    /// <summary>
    /// 多个零件导入，完成单个零件后初始化
    /// </summary>
    [System.Obsolete]
    void initialized()
    {
        GameObject changedObj = PrefabUtility.CreatePrefab("Assets/data/" + extractObj.name + "copy.prefab", extractObj);
        extractObj = new GameObject();
        extractObj.transform.position = Vector3.zero;
        extractObj.transform.eulerAngles = Vector3.zero;
        obj2BeConvert = null;
        start2CheckSize = false;
        s.Clear();
        shaft_info = "";
        not_shaft_info = "";
        shaft.Clear();
        not_shaft.Clear();
        shaft_transf.Clear();
        not_shaft_transf.Clear();
    }

    /// <summary>
    /// 第一步
    /// 提取零件本体
    /// </summary>
    void Extract3DModle()
    {
        extractObj = new GameObject();
        extractObj.transform.position = Vector3.zero;
        extractObj.transform.eulerAngles = Vector3.zero;
        extractObj.name = obj2BeConvert.name + "c";
        foreach (Transform child in obj2BeConvert.transform)
        {
            if (child.GetComponentInChildren<MeshRenderer>() != null)
            {
                child.GetComponentInChildren<MeshRenderer>().
                    gameObject.transform.SetParent(extractObj.transform);
            }
        }
    }

    /// <summary>
    /// 第二步
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
    /// 第三步
    /// 添加碰撞属性并进行检测，获取size_info
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
            //box_info.samScale储存了该零件长宽比检测时缩放比，新模型第一次检测变换常为1
            //经过下列信息变换后，此值将变为相机采样缩放比
            if (box_info.type == "shaft")
            {
                size = box_info.l;
                box_info.samScale = size * box_info.samScale / 2f + deltaScale;
                //一个零件一行：零件名，采样相机size，采样姿态
                shaft_info += s[j].name + ", " +
                    (size * box_info.samScale / 2f + deltaScale).ToString() + ", " +
                    (box_info.samAngle).ToString() + "\n";
                shaft.Add(s[j]);
                shaft_transf.Add(s[j].name, box_info);
            }
            else
            {
                size = box_info.r;
                box_info.samScale = size * box_info.samScale / 2f + deltaScale;
                not_shaft_info += s[j].name + ", " +
                    (size * box_info.samScale / 2f + deltaScale).ToString() + ", " +
                    (box_info.samAngle).ToString() + "\n";
                not_shaft.Add(s[j]);
                not_shaft_transf.Add(s[j].name, box_info);
            }
            yield return null;
        }
        WriteFileByLine("Assets/data", "shaft_info.txt", shaft_info);
        WriteFileByLine("Assets/data", "not_shaft_info.txt", not_shaft_info);
        print("fin");
        StopCoroutine(checkIterator);
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
    /// 通用方法
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

}
///// <summary>
///// 返回样本尺寸检测信息
///// 初次采样，返回scale常为1
///// </summary>
//public struct sizeInfo
//{
//    public float r;
//    public float l;
//    public string type;
//    public float samScale;
//    public Vector3 samAngle;
//}
