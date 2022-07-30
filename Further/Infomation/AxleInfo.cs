using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEngine.SceneManagement;


/// <summary>
/// Kind,RightDatumFace,LeftDatumFace,FacePos,AxlePos
/// </summary>
public class AxleInfo : MonoBehaviour
{
    #region
    //每一根轴上挂载的脚本，此脚本的Dictionary储存该轴的零件信息
    //第一层索引为该轴名称，string=AxleName，对应该轴下的所有零件信息
    //第二层dictionary索引为零件类型，下拉列表选择，对应该类型下的所有同类型零件
    //第三层dictionary索引为零件名称，对应单个零件信息
    //public Dictionary<string, Dictionary<string, Dictionary
    //    <string, Dictionary<string, object>>>> ThisAxle_Part
    //    = new Dictionary<string, Dictionary<string, Dictionary
    //        <string, Dictionary<string, object>>>>();

    //Dictionary<string, Dictionary<string, Dictionary<string, object>>> Part_Kinds =
    //    new Dictionary<string, Dictionary<string, Dictionary<string, object>>>();

    //Dictionary<string, Dictionary<string, object>> ThisKind_Parts =
    //    new Dictionary<string, Dictionary<string, object>>();
    #endregion
    /// <summary>
    /// 从LoadingControl脚本出导入信息文件
    /// 
    /// 逐行解析，分类，获取数据ToArrayChar
    /// 获取基准面，基准轴的相对位置，重新实体化并添加到指定零件层
    /// 
    /// 分类信息逐个对应重载装配信息数据并装入vector3
    /// 按照对应vector3装入字段selfinfo，assemblyinfo
    /// 储存零件的信息重新载入，检索对应位置，重新赋值
    /// </summary>
    public LoadingControl Loading;
    /// <summary>
    /// 轴类型，基准面，左/右端面，基准轴
    /// </summary>
    public Dictionary<string, object> ShaftInfo;
    /// <summary>
    /// 所有在此轴上的零件
    /// </summary>
    public List<GameObject> AllParts = new List<GameObject>();

    Scene CurrentScene;

    ///识别信息后，重新装载
    /// <summary>
    /// 轴左端面
    /// </summary>
    public Vector3 LeftDatumFace;
    public Vector3 LeftDatumFaceRot;
    /// <summary>
    /// 轴右端面
    /// </summary>
    public Vector3 RightDatumFace;
    public Vector3 RightDatumFaceRot;
    /// <summary>
    /// 轴自身基准轴
    /// </summary>
    public List<Vector3> AxlePos;
    public List<Vector3> AxleRot;
    /// <summary>
    /// 轴自身基准面
    /// </summary>
    public List<Vector3> FacePos;
    public List<Vector3> FaceRot;
    /// <summary>
    /// 装配零件名
    /// </summary>
    public List<string> AllPartsName;
    /// <summary>
    /// 类型
    /// </summary>
    public string Kind;
    /// <summary>
    /// 开始PartInfo开关
    /// </summary>
    public bool ReloadPartInfo;

    /// <summary>
    /// 储存信息可读语句
    /// </summary>
    public string Date;

    public bool Open = true;
    /// <summary>
    /// 当信息导入完成，零件具备碰撞体
    /// </summary>
    public void ReloadInfo()
    {
        if (Loading.AllInfo.Count != 0 &&
            UsingFunction.Family(this.gameObject)[0].GetComponent<Rigidbody>() != null)
        {
            //初始化数据
            //GameObject
            ShaftInfo.Clear();
            AllParts.Clear();

            //vector3
            AllPartsName.Clear();
            AxlePos.Clear();
            AxleRot.Clear();
            FacePos.Clear();
            FaceRot.Clear();
            LeftDatumFace = Vector3.zero;
            LeftDatumFaceRot = Vector3.zero;
            RightDatumFace = Vector3.zero;
            RightDatumFaceRot = Vector3.zero;
            Kind = "";

            Compile();

            //重载自身基准轴，自身基准面
            List<GameObject> Axle = new List<GameObject>();
            List<GameObject> Face = new List<GameObject>();
            foreach (Transform A in UsingFunction.Family(this.gameObject)[2].transform)
            {
                Axle.Add(A.gameObject);
            }
            foreach (Transform F in UsingFunction.Family(this.gameObject)[3].transform)
            {
                Face.Add(F.gameObject);
                if (F.transform.localPosition == LeftDatumFace)
                {
                    ShaftInfo.Add("左端面", F.gameObject);
                }
                else if (F.transform.localPosition == RightDatumFace)
                {
                    ShaftInfo.Add("右端面", F.gameObject);
                }
            }
            ShaftInfo.Add("自身基准轴", Axle);
            ShaftInfo.Add("自身基准面", Face);
            ShaftInfo.Add("零件类型", Kind);

            for (int i = 0; i < AllParts.Count; i++)
            {
                if (GameObject.Find(AllParts[i].name) != null)
                {
                    AllParts.Add(GameObject.Find(AllParts[i].name));
                }
            }
        }
    }
    /// <summary>
    /// 零件装配信息识别，按照零件的装配信息格式识别信息，并将AllInfo信息中的无分类信息装载入PartInfo内
    /// 按照所在零件的tag分类，只在tag为装配零件的时候才运行重载方法
    /// 已导入数据文件，识别数据，分类数据，对应重载入重载信息字段
    /// </summary>
    public void Compile()
    {
        if (UsingFunction.Family(this.gameObject)[0].tag == "装配轴" ||
            UsingFunction.Family(this.gameObject)[0].tag == "拨叉轴" ||
            UsingFunction.Family(this.gameObject)[0].tag == "壳体")
        {
            int Begin2Read = 0;
            string[] R = Loading.AllInfo[UsingFunction.Family(this.gameObject)[0].gameObject.name];
            for (int i = 0; i < R.Length; i++)
            {
                if (R[i].Contains("以下为使用数据"))
                {
                    Begin2Read = i;
                    //Debug.Log(i + "    " + R.Length);
                    break;
                }
            }
            ///获取有用数据，装入新列表
            List<string> _R = new List<string>();
            for (int j = Begin2Read + 1; j < R.Length; j++)
            {
                _R.Add(R[j]);
            }
            for (int k = 0; k < _R.Count; k++)
            {
                //Debug.Log(_R[k]);
            }
                Distinguish(_R);
        }
    }

    /// <summary>
    /// 输入任意一个包含三维向量的字符及任意插入空格，包含小数点等字符串
    /// </summary>
    /// <param name="V">传入字符串</param>
    /// <returns></returns>
    public Vector3 String2Vector3(string V)
    {
        Vector3 B;
        char[] c = V.ToCharArray();
        int D1 = 0;
        int D2 = 0;
        for (int i = 0; i < c.Length; i++)
        {
            switch (c[i])
            {
                case (','):
                    if (D1 == 0 && D2 == 0) { D1 = i; }
                    else if (D2 == 0 && D1 != 0) { D2 = i; }; break;
            }
        }
        string jb = "";
        for (int i = 1; i < D1; i++)
        {
            jb = jb + c[i];
        }
        B.x = float.Parse(jb);
        jb = "";
        for (int i = D1 + 1; i < D2; i++)
        {
            jb = jb + c[i];
        }
        B.y = float.Parse(jb);
        jb = "";
        for (int i = D2 + 1; i < c.Length - 1; i++)
        {
            jb = jb + c[i];
        }
        B.z = float.Parse(jb);
        return B;
    }

    /// <summary>
    /// 识别装配信息，读取装配信息，重载入
    /// </summary>
    /// <param name="_R">可识别数据列表</param>
    /// <returns></returns>
    public void Distinguish(List<string> _R)
    {
        int k0 = 0;
        //基准轴的localpos
        int k1 = 0;
        int k1r = 0;
        //基准面的localpos
        int k2 = 0;
        int k2r = 0;
        for (int k = 0; k < _R.Count; k++)
        {
            switch (_R[k])
            {
                //"\r\n零件类型\r\n@" + Kind +
                //"\r\n左端面\r\n@" + LeftDatumFace +
                //"\r\n右端面\r\n@" + RightDatumFace +
                //"\r\n装配零件\r\n" + Parts +
                //"\r\n自身基准轴\r\n" + AP +
                //"\r\n自身基准面\r\n" + FP;
                case ("零件类型"): Kind = _R[k + 1]; break;
                case ("装配零件"): k0 = k; break;
                case ("自身基准轴位置"): k1 = k; break;
                case ("自身基准面位置"): k2 = k; break;
                case ("自身基准轴位姿"): k1r = k; break;
                case ("自身基准面位姿"): k2r = k; break;
                case ("左端面位置"):
                    LeftDatumFace = String2Vector3(_R[k + 1]);
                    ; break;
                case ("右端面位置"):
                    RightDatumFace = String2Vector3(_R[k + 1]);
                    ; break;
                case ("左端面位姿"):
                    LeftDatumFaceRot = String2Vector3(_R[k + 1]);
                    ; break;
                case ("右端面位姿"):
                    RightDatumFaceRot = String2Vector3(_R[k + 1]);
                    ; break;
                default: break;
            }
        }
        //读取所有装配零件
        for (int i = k0 + 1; i < k1; i++)
        {
            AllPartsName.Add(_R[i]);
        }
        //读取自身基准轴位置
        for (int i = k1 + 1; i < k2; i++)
        {
            AxlePos.Add(String2Vector3(_R[i]));
        }
        //读取自身基准面位置
        for (int j = k2 + 2; j < k1r; j++)
        {
            FacePos.Add(String2Vector3(_R[j]));
        }
        //读取自身基准轴位姿
        for (int i = k1r + 1; i < k2r; i++)
        {
            AxleRot.Add(String2Vector3(_R[i]));
        }
        //读取自身基准面位姿
        for (int j = k2r + 2; j < _R.Count; j++)
        {
            FaceRot.Add(String2Vector3(_R[j]));
        }
        //实例化自身基准轴
        for (int i = 0; i < AxlePos.Count; i++)
        {
            GameObject a = ((GameObject)Instantiate(Resources.Load("_Prefabs/axle"),
                UsingFunction.Family(this.gameObject)[2].transform));
            a.transform.localPosition = AxlePos[i];
            a.transform.localEulerAngles = AxleRot[i];
        }
        //实例化自身基准面
        for (int i = 0; i < FacePos.Count; i++)
        {
            GameObject b = ((GameObject)Instantiate(Resources.Load("_Prefabs/face"),
                UsingFunction.Family(this.gameObject)[3].transform));
            b.transform.localPosition = FacePos[i];
            b.transform.localEulerAngles = FaceRot[i];
        }
    }

    //当主体为零件时，此脚本下所有信息为空， 此处不编译
    public void TranslateInfo()
    {
        string AP = "";
        string AR = "";
        string FP = "";
        string FR = "";
        string Parts = "";
        if (ShaftInfo.Count != 0)
        {
            LeftDatumFace = ((GameObject)ShaftInfo["左端面"]).transform.localPosition;
            LeftDatumFaceRot = ((GameObject)ShaftInfo["左端面"]).transform.localEulerAngles;
            RightDatumFace = ((GameObject)ShaftInfo["右端面"]).transform.localPosition;
            RightDatumFaceRot = ((GameObject)ShaftInfo["右端面"]).transform.localEulerAngles;
            Kind = ((string)ShaftInfo["零件类型"]);
        }
        //编译基准轴所在位置
        foreach (Transform A in UsingFunction.Family(this.gameObject)[2].transform)
        {
            AxlePos.Add(A.localPosition);
            AP = AP + "\r\n" + A.localPosition;
        }
        foreach (Transform A in UsingFunction.Family(this.gameObject)[2].transform)
        {
            AxleRot.Add(A.localEulerAngles);
            AR = AR + "\r\n" + A.localEulerAngles;
        }
        //编译基准面所在位置
        foreach (Transform F in UsingFunction.Family(this.gameObject)[3].transform)
        {
            FacePos.Add(F.localPosition);
            FP = FP + "\r\n" + F.localPosition;
        }
        foreach (Transform F in UsingFunction.Family(this.gameObject)[3].transform)
        {
            FaceRot.Add(F.localEulerAngles);
            FR = FR + "\r\n" + F.localEulerAngles;
        }
        for (int i = 0; i < AllParts.Count; i++)
        {
            AllPartsName.Add(AllParts[i].name);
            Parts = Parts + "\r\n" + AllParts[i].name;
        }
        //装配信息文件需要读取的部分
        //第2，4，6行，后识别文字
        Date =
        "轴的左端面的相对位置为：\r\n" + LeftDatumFace +
        "\r\n左端面的相对位姿为：\r\n" + LeftDatumFaceRot +
        "\r\n右端面的相对位置为：\r\n" + RightDatumFace +
        "\r\n右端面的相对位姿为：\r\n" + RightDatumFaceRot +
        "\r\n所有基准轴对于此轴的相对位置分别为：" + AP +
        "\r\n所有基准轴对于此轴的相对位姿分别为：" + AR +
        "\r\n所有基准面对于此轴的相对位置分别为：" + FP +
        "\r\n所有基准面对于此轴的相对位姿分别为：" + FR +
        "\r\n与此轴装配的零件有：" + Parts +

        "\r\n以下为使用数据" +
        //数据格式为
        //数据名
        //(x,y,z)
        //数据名
        //(x,y,z)...
        //从以下为使用数据开始
        //分别识别每一行不带@的字符串，对应数据类型
        //分别识别每一行带@的数据的第3、5、7个数据，获得所代表的向量
        "\r\n零件类型\r\n" + Kind +
        "\r\n左端面位置\r\n" + LeftDatumFace +
        "\r\n右端面位置\r\n" + RightDatumFace +
        "\r\n左端面位姿\r\n" + LeftDatumFaceRot +
        "\r\n右端面位姿\r\n" + RightDatumFaceRot +
        "\r\n装配零件" + Parts +
        "\r\n自身基准轴位置" + AP +
        "\r\n自身基准面位置" + FP +
        "\r\n自身基准轴位姿" + AR +
        "\r\n自身基准面位姿" + FR;
        WriteFileByLine("Date_outTXT", UsingFunction.Family(this.gameObject)[0].name + ".txt", Date);
        //输出6个结果，分别对应自身基准面，自身基准轴，左右端面，装配零件集合，类型
    }

    void Start()
    {
        CurrentScene = SceneManager.GetActiveScene();
        ShaftInfo = new Dictionary<string, object>();
        if (CurrentScene.name == "Assembly")
        {
            Loading = GameObject.Find("LoadingControl").GetComponent<LoadingControl>();
        }
    }
    

    private void FixedUpdate()
    {
        if (Loading != null && CurrentScene.name == "Assembly")
        {
            if (Loading.LoadInfo && Open&&
                (UsingFunction.Family(this.gameObject)[0].tag=="装配轴" ||
                UsingFunction.Family(this.gameObject)[0].tag=="壳体"))
            {
                ReloadInfo();
                Open=false;
                Loading.LoadedNum++;
            }
        }
    }

    public void Initialization()
    {
        for (int i = 0; i < AllParts.Count; i++)
        {
            UsingFunction.Hide_Translucent_Appear("Appear", AllParts[i]);
        }
        ShaftInfo.Clear();
        AllParts.Clear();
        if (this.transform.parent.transform.Find("Axis").transform.childCount != 0)
        {
            foreach (Transform Child in this.transform.parent.transform.Find("Axis").transform)
            {
                if (Child != null)
                {
                    Destroy(Child.gameObject);
                }
            }
        }
        if (this.transform.parent.transform.Find("Surface").transform.childCount != 0)
        {
            foreach (Transform Child in this.transform.parent.transform.Find("Surface").transform)
            {
                if (Child != null)
                {
                    Destroy(Child.gameObject);
                }
            }
        }
    }

    void WriteFileByLine(string file_path, string file_name, string str_info)//写入文件
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
