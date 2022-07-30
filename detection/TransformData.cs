using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

/// <summary>
/// 三个文件目录
/// 单个目录下多个txt
/// 单个txt多行
/// 单行4种数据
/// 从深度网络检测数据的txt中获取每个零件的段落信息：相对于左上角的x坐标，半径，厚度，段落种类
/// 转换得相对于图片中心坐标的数值，即ridigbody.messcenter到段落中心的向量的x坐标左负右正
/// 从GetPic中直接传输得零件采样相机orthographicSize值，该值直接替换采样相机的size
/// 从GetPic中直接传输得零件建模位姿
/// 使用该值，可求得单位像素代表的unity长度，size*2/图片分辨率960或2880，则可计算各建模参数
/// 计算相对于ridigbody.messcenter的中心可得shift
/// 计算半径可得碰撞检测小球初始化位置
/// 计算厚度得thick
/// 取值type可switch得对应碰撞检测脚本
/// </summary>
public class TransformData : MonoBehaviour
{
    detectController recreator;
    //每个文件夹目录下存放多个单零件检测数据.txt，包含单个零件所有段落信息
    Dictionary<string, string> data_Folder = new Dictionary<string, string>()
    {
        ["shaft"] = "G:/data/test/shaft_result/",
        ["gear"] = "G:/data/test/gear_result/",
        ["ring"] = "G:/data/test/ring_result/"
    };
    public Dictionary<string, sizeInfo> samplingInfo = new Dictionary<string, sizeInfo>();
    float pixid_scale = 2880;
    /// <summary>
    /// 最终数据存放点name，[list x，list r，list thick，list type]
    /// </summary>
    public Dictionary<string, blockType> allInfo = new Dictionary<string, blockType>();

    public string txtOrDir;
    /// <summary>
    /// 读取零件采样姿态欧拉角及相机采样size
    /// </summary>
    string shaft_info_path = "Assets/data/shaft_info.txt";
    string not_shaft_info_path = "Assets/data/not_shaft_info.txt";
    /// <summary>
    /// 脚本直接传输数据
    /// </summary>
    public Dictionary<string, sizeInfo> shaft_scale = new Dictionary<string, sizeInfo>();
    public Dictionary<string, sizeInfo> not_shaft_scale = new Dictionary<string, sizeInfo>();

    /// <summary>
    /// 所有采样数据存放点
    /// </summary>
    Dictionary<string, simInfo> allScaleAndAngle = new Dictionary<string, simInfo>();
    public bool start2Read = false;
    //public bool start2Trans = false;
    IEnumerator get_info;
    /// <summary>
    /// shaft、gear、ring
    /// </summary>
    Dictionary<string, List<GameObject>> allPartsType = new Dictionary<string, List<GameObject>>();

    void Start()
    {
        recreator = GetComponent<detectController>();
        get_info = getYoloData();
    }

    void Update()
    {
        if (start2Read)
        {
            getScaleData();
            start2Read = false;
        }
        //if (start2Trans && !start2Read)
        //{
        //    StartCoroutine(get_info);
        //    start2Trans = false;
        //}
    }

    #region 获取采样相机size及采样位姿
    /// <summary>
    /// 选择获取采样信息的途径
    /// </summary>
    void getScaleData()
    {
        if (txtOrDir == "txt")
        {
            getTxtScale(shaft_info_path);
            getTxtScale(not_shaft_info_path);
        }
        else
        {
            getDirScale();
        }
    }

    /// <summary>
    /// 从txt中获取GetPic中零件采样的相机size，采样姿态
    /// </summary>
    /// <param name="path"></param>
    void getTxtScale(string path)
    {
        List<string> info = read_txt(path);
        for (int i = 0; i < info.Count; i++)
        {
            string[] t = info[i].Split(new char[2] { ',', ' ' });
            List<string> t1 = new List<string>();
            for (int j = 0; j < t.Length; j++)
            {
                if (!string.IsNullOrEmpty(t[j]) && !string.IsNullOrWhiteSpace(t[j]))
                {
                    t1.Add(t[j]);
                }
            }
            simInfo a = new simInfo();
            //零件名，采样相机size，采样姿态
            a.camSize = float.Parse(t1[1]);
            //for (int k = 0; k < t1.Count; k++)
            //{
            //    print(t1[k]);
            //}
            // print(t1[2].Replace("(","") + "?" + t1[3] + "?" + t1[4].Replace(")",""));

            //(0.0与0.0与0.0)——2 3 4
            float x = float.Parse(t1[2].Replace("(", ""));
            float y = float.Parse(t1[3]);
            float z = float.Parse(t1[4].Replace(")", ""));
            a.simAngle = new Vector3(x, y, z);
            allScaleAndAngle.Add(t1[0], a);
        }
        //print("完成信息读入");
        StartCoroutine(get_info);
    }

    /// <summary>
    /// 通过脚本传输数据，直接取用其中所需部分赋值
    /// </summary>
    void getDirScale()
    {
        if (shaft_scale.Count != 0 && not_shaft_scale.Count != 0)
        {
            foreach (string key in shaft_scale.Keys)
            {
                simInfo a = new simInfo();
                a.camSize = shaft_scale[key].samScale;
                a.simAngle = shaft_scale[key].samAngle;
                allScaleAndAngle.Add(key, a);
            }
            foreach (string key in not_shaft_scale.Keys)
            {
                simInfo a = new simInfo();
                a.camSize = not_shaft_scale[key].samScale;
                a.simAngle = not_shaft_scale[key].samAngle;
                allScaleAndAngle.Add(key, a);
            }
        }
    }
    #endregion

    #region 读取深度学习网络识别数据的txt
    /// <summary>
    /// 读取txt传输数据
    /// 迭代完毕后，所有零件信息按照[name,blockType]形式储存
    /// </summary>
    /// <returns></returns>
    IEnumerator getYoloData()
    {
        foreach (string key in data_Folder.Keys)
        {
            //文件夹管理类，读取目录下文件信息
            DirectoryInfo listdir = new DirectoryInfo(data_Folder[key]);
            //文件信息类，返回文件完整路径数组
            FileInfo[] allName = listdir.GetFiles("*", SearchOption.AllDirectories);
            List<GameObject> oneType = new List<GameObject>();
            for (int i = 0; i < allName.Length; i++)
            {
                string[] a = allName[i].ToString().Split('\\');
                //取路径最后一位即文件名.txt，去除.txt读取第一位
                string name = a[a.Length - 1].Split('.')[0];
                //单个零件所有信息
                List<string> r = read_txt(allName[i].ToString());
                //print(name);
                //返回单个零件所有段落信息，所有段落的x坐标，所有段落的半径，所有段落的厚度，所有段落的类型
                allInfo.Add(name, getInfo(r, name, key));
                oneType.Add(GameObject.Find(name));
            }
            allPartsType.Add(key, oneType);
            yield return (0);
        }
        recreator.allInfo = allInfo;
        recreator.start2Rebuild = true;
        recreator.allPartsType = allPartsType;
        print("完成detectController赋值");
        StopAllCoroutines();
    }

    /// <summary>
    /// 读取txt
    /// </summary>
    /// <param name="path">传入txt文件地址</param>
    /// <returns>每一行一个元素的string列表</returns>
    List<string> read_txt(string path)
    {
        List<string> result = new List<string>();
        string[] txt = File.ReadAllLines(path);
        for (int i = 0; i < txt.Length; i++)
        {
            if (txt[i] != "")
            {
                result.Add(txt[i]);
            }
        }
        return (result);
    }

    /// <summary>
    /// 传入单个零件所有段落信息
    /// </summary>
    /// <param name="info">根据传参返回对应类型的段落信息</param>
    blockType getInfo(List<string> info, string name, string partType)
    {
        blockType blockInfo = new blockType();
        blockInfo.partsType = partType;
        blockInfo.x = new List<float>();
        blockInfo.r = new List<float>();
        blockInfo.thick = new List<float>();
        blockInfo.type = new List<string>();
        //info储存单个零件所有检测信息txt文件
        for (int i = 0; i < info.Count; i++)
        {
            string[] t = info[i].Split(new char[2] { ',', ' ' });
            List<string> a = new List<string>();
            for (int j = 0; j < t.Length; j++)
            {
                if (t[j] != string.Empty)
                {
                    a.Add(t[j]);
                    //print(t[j]);
                }
            }
            float camSize = allScaleAndAngle[name].camSize;
            //所有数据返回均为像素值
            blockInfo.x.Add(transformer(float.Parse(a[0]), camSize, "x"));
            blockInfo.r.Add(transformer(float.Parse(a[1]), camSize, "r"));
            blockInfo.thick.Add(transformer(float.Parse(a[2]), camSize, "thick"));
            blockInfo.type.Add(a[3]);
            //print(blockInfo.x);
        }
        return (blockInfo);
    }
    #endregion

    /// <summary>
    /// 转换段落中心x绝对坐标为相对图像中心x相对坐标，即建模距离左负右正
    /// 图像分辨率单位长度与设置的rect大小有关
    /// 相机size大小决定物体在相机视野画面中的占比，size=1余cube=2对应
    /// 设置rect=960*960，size=6，则此时截图长宽各代表unity内的12个单位长度
    /// 此时960个像素代表12个单位长度
    /// 单个像素代表  12/960 （unit/个像素）
    /// game界面显示分辨率大小与整体无关
    /// </summary>
    /// <param name="info">传入单个数据，绝对坐标零点为图片左上角</param>
    /// <param name="scale">采样图片对应的cam的size</param>
    /// <returns></returns>
    public float transformer(float info, float scale, string s)
    {
        if (s == "x")
        {
            //计算单位像素所对应的unity长度值
            float perPixidLong = scale * 2 / pixid_scale;
            float centerX = pixid_scale / 2;
            //获取该段落距离rigidbody.meshcenter的相对坐标位置，左负右正
            info -= centerX;
            info *= perPixidLong;
            return (info);
        }
        else
        {
            //计算单位像素所对应的unity长度值
            float perPixidLong = scale * 2 / pixid_scale;
            //获取该段落距离rigidbody.meshcenter的相对坐标位置，左负右正
            info *= perPixidLong;
            return (info);
        }
    }


    //void t()
    //{
    //    List<float> a = allInfo["NONE-DC_Shell113"].x;
    //    List<string> t = allInfo["NONE-DC_Shell113"].type;
    //    for (int i = 0; i < a.Count; i++)
    //    {
    //        Vector3 np = new Vector3(a[i], 0, 0);
    //        GameObject instance = (GameObject)Instantiate(GameObject.Find("c1"), np, new Quaternion(0, 0, 0, 0));
    //        if (t[i] != "Ring")
    //        {
    //            instance.GetComponent<MeshRenderer>().material = null;
    //        }
    //    }
    //}
}

