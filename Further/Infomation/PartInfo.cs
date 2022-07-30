using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEngine.SceneManagement;


/// <summary>
/// LeftFace,RightFace,DatumFace,DatumAxle,AssemblyPos,
/// Shaft,Order,Kind,SelfAxlePos,SelfFacePos
/// </summary>
public class PartInfo : MonoBehaviour
{
    /// <summary>
    /// 导出信息字符串
    /// </summary>
    public string Date;
    /// <summary>
    /// 材质恢复
    /// </summary>
    public Material M;
    /// <summary>
    /// 接入信息
    /// </summary>
    public LoadingControl Loading;
    /// <summary>
    /// 自带碰撞体
    /// </summary>
    public List<MeshCollider> AllCollider;


    /// <summary>
    /// 零件类型,自身基准轴,自身基准面，*左端面，*右端面
    /// </summary>
    public Dictionary<string, object> SelfInfo;
    /// <summary>
    /// 零件*装配基准轴，*装配基准面，装配顺序，*装配轴，装配位置
    /// </summary>
    public Dictionary<string, object> AssembleInfo;

    public bool Open;
    /// <summary>
    /// 重载装配信息数据
    /// </summary>
    public Vector3 LeftFace;
    public Vector3 R_LeftFace;
    public Vector3 RightFace;
    public Vector3 R_RightFace;
    public Vector3 DatumFace;
    public Vector3 R_DatumFace;
    public Vector3 DatumAxle;
    public Vector3 R_DatumAxle;
    public Vector3 AssemblyPos;
    public string Shaft;
    public string Order;
    public string Kind;
    public List<Vector3> SelfAxlePos = new List<Vector3>();
    public List<Vector3> SelfAxleRot = new List<Vector3>();
    public List<Vector3> SelfFacePos = new List<Vector3>();
    public List<Vector3> SelfFaceRot = new List<Vector3>();

    Scene CurrentScene;
    /// <summary>
    /// 转换所有信息为可导出string格式，使之可再生
    /// 需要转换的数据：左端面，右端面，装配轴，装配基准面，装配基准轴
    /// 左端面右端面获取this，基准轴获取name，赋值后再从基准轴身上获取基准面基准轴
    /// 记录主要变量的位置参数，通过Local位置再次在star中赋值
    /// 先获取装配轴（名称），再获取剩余变量localposition
    /// </summary>
    public void TranslationInfo()
    {
        //当主体为装配轴，此脚本下的所有信息为空，故此处不编译信息
        //重新编译GameObject，转换为可导出的数据格式string
        if (SelfInfo.Count != 0 && AssembleInfo.Count != 0 && AllCollider.Count != 0)
        {
            Kind = ((string)SelfInfo["零件类型"]);
            LeftFace = ((GameObject)SelfInfo["左端面"]).transform.localPosition;
            R_LeftFace = ((GameObject)SelfInfo["左端面"]).transform.localEulerAngles;
            RightFace = ((GameObject)SelfInfo["右端面"]).transform.localPosition;
            R_RightFace = ((GameObject)SelfInfo["右端面"]).transform.localEulerAngles;
            Shaft = ((GameObject)AssembleInfo["装配轴"]).name;
            DatumAxle = ((GameObject)AssembleInfo["装配基准轴"]).transform.localPosition;
            R_DatumAxle = ((GameObject)AssembleInfo["装配基准轴"]).transform.localEulerAngles;
            DatumFace = ((GameObject)AssembleInfo["装配基准面"]).transform.localPosition;
            R_DatumFace = ((GameObject)AssembleInfo["装配基准面"]).transform.localEulerAngles;
            Order = ((string)AssembleInfo["装配顺序"]);
            //装配相对位置最后设置，统一由零件所在位置指向装配轴所在位置
            //即装配轴世界坐标减零件世界坐标得的向量，再求模长
            //再生后，只需要将此向量加上零件世界坐标即可得装配轴的世界坐标
            //求两个点的距离与阈值比较
            AssemblyPos = (Vector3)AssembleInfo["装配位置"];
            string AllSelfAxle = "";
            string AllSelfAxleRot = "";
            string AllSelfFace = "";
            string AllSelfFaceRot = "";
            //重载自身基准轴，放入string格式下的list
            foreach (Transform A in UsingFunction.Family(this.gameObject)[2].transform)
            {
                SelfAxlePos.Add(A.transform.localPosition);
                AllSelfAxle = AllSelfAxle + "\r\n" + A.transform.localPosition;
            }
            foreach (Transform A in UsingFunction.Family(this.gameObject)[2].transform)
            {
                SelfAxleRot.Add(A.transform.localEulerAngles);
                AllSelfAxleRot = AllSelfAxleRot + "\r\n" + A.transform.localEulerAngles;
            }
            //重载自身基准面,放入string格式下的list
            foreach (Transform F in UsingFunction.Family(this.gameObject)[3].transform)
            {
                SelfFacePos.Add(F.transform.localPosition);
                AllSelfFace = AllSelfFace + "\r\n" + F.transform.localPosition;
            }
            foreach (Transform F in UsingFunction.Family(this.gameObject)[3].transform)
            {
                SelfFaceRot.Add(F.transform.localEulerAngles);
                AllSelfFaceRot = AllSelfFaceRot + "\r\n" + F.transform.localEulerAngles;
            }
            //读取信息行数
            //第2，4，6，8，10，12，14,行
            //对应装配轴名称，左右端面，装配基准轴，装配基准面，装配位置，装配顺序
            Date =
            "零件的装配轴为：\r\n" + Shaft +
            "\r\n左端面坐标为：\r\n" + LeftFace +
            "\r\n左端面姿态为：\r\n" + R_LeftFace +
            "\r\n右端面坐标为：\r\n" + RightFace +
            "\r\n右端面姿态为：\r\n" + R_RightFace +
            "\r\n装配基准轴的相对坐标为：\r\n" + DatumAxle +
            "\r\n装配基准轴的相对姿态为：\r\n" + R_DatumAxle +
            "\r\n装配基准面的相对坐标为：\r\n" + DatumFace +
            "\r\n装配基准面的相对姿态为：\r\n" + R_DatumFace +
            "\r\n装配位置的相对坐标为：\r\n" + AssemblyPos +
            "\r\n零件装配顺序为从装配轴的" + Order.ToCharArray()[0] +
            "端面开始装配，从该端面数起的第" + Order.ToCharArray()[1] +
            "个零件，从外向里数起第" + Order.ToCharArray()[2] + "个零件" +

            "\r\n以下为使用数据" +
            //数据格式为
            //数据名
            //@(x,y,z)
            //数据名
            //@(x,y,z)...
            //从以下为使用数据开始
            //分别识别每一行不带@的字符串，对应数据类型
            //分别识别每一行带@的数据的第3、5、7个数据，获得所代表的向量
            "\r\n零件类型\r\n" + Kind +
            "\r\n左端面位置\r\n" + LeftFace +
            "\r\n右端面位置\r\n" + RightFace +
            "\r\n左端面位姿\r\n" + R_LeftFace +
            "\r\n右端面位姿\r\n" + R_RightFace +
            "\r\n装配轴\r\n" + Shaft +
            "\r\n装配基准轴位置\r\n" + DatumAxle +
            "\r\n装配基准面位置\r\n" + DatumFace +
            "\r\n装配基准轴位姿\r\n" + R_DatumAxle +
            "\r\n装配基准面位姿\r\n" + R_DatumFace +
            "\r\n装配顺序\r\n" + Order +
            "\r\n装配位置\r\n" + AssemblyPos +
            "\r\n自身基准轴位置" + AllSelfAxle +//k1
            "\r\n自身基准面位置" + AllSelfFace +//k2
            "\r\n自身基准轴位姿" + AllSelfAxleRot +//k1r
            "\r\n自身基准面位姿" + AllSelfFaceRot;//k2r

            WriteFileByLine("Date_outTXT", UsingFunction.Family(this.gameObject)[0].name + ".txt", Date);
        }
        //共输出10个数据结果+一个数据可视化结果
    }

    /// <summary>
    /// 从LoadingControl脚本出导入信息文件
    /// 
    /// 逐行解析，分类，获取数据ToArrayChar
    /// 获取基准面，基准轴的相对位置，重新实体化并添加到指定零件层
    /// 
    /// 分类信息逐个对应重载装配信息数据并装入vector3
    /// 按照对应vector3装入字段selfinfo，assemblyinfo
    /// 储存零件的信息重新载入，检索对应位置，重新赋值
    /// 
    /// </summary>
    public void ReloadInfo()
    {
        //if (LeftFace != null && RightFace != null && Shaft != null && DatumFace != null && DatumAxle != null
        //    && Order != null && Kind != null)
        //当控制脚本数据存在
        if (Loading.AllInfo.Count != 0 &&
            UsingFunction.Family(this.gameObject)[0].GetComponent<Rigidbody>() != null)
        {
            //初始化数据
            //GameObject
            SelfInfo.Clear();
            AssembleInfo.Clear();
            //string
            Kind = "";
            Order = "";
            Shaft = "";
            LeftFace = Vector3.zero;
            R_LeftFace = Vector3.zero;
            RightFace = Vector3.zero;
            R_RightFace = Vector3.zero;
            DatumAxle = Vector3.zero;
            R_DatumAxle = Vector3.zero;
            DatumFace = Vector3.zero;
            R_DatumFace = Vector3.zero;
            AssemblyPos = Vector3.zero;
            SelfAxlePos.Clear();
            SelfFacePos.Clear();
            SelfAxleRot.Clear();
            SelfFaceRot.Clear();

            Compile();
            //重载装配轴
            GameObject Target = GameObject.Find(Shaft);
            //筛选装配轴
            AssembleInfo.Add("装配轴", Target);
            AssembleInfo.Add("装配顺序", Order);
            AssembleInfo.Add("装配位置", AssemblyPos);
            SelfInfo.Add("零件类型", Kind);
            //重载装配基准轴
            List<GameObject> Axle = new List<GameObject>();
            //重载装配基准面
            List<GameObject> Face = new List<GameObject>();
            //把所有装配基准轴的自身基准轴装入axle中用作下一步筛选
            foreach (Transform A in UsingFunction.Family(Target)[2].transform)
            {
                Axle.Add(A.transform.gameObject);
            }
            //把所有装配基准轴的自身基准面装入face中用作下一步筛选
            foreach (Transform B in UsingFunction.Family(Target)[3].transform)
            {
                Face.Add(B.transform.gameObject);
            }
            //Debug.Log(Face.Count);
            //筛选装配基准轴，重新载入
            for (int i = 0; i < Axle.Count; i++)
            {
                if (Axle[i].transform.localPosition == DatumAxle &&
                    Axle[i].transform.localEulerAngles == R_DatumAxle)
                {
                    AssembleInfo.Add("装配基准轴", Axle[i]);
                }
            }
            //筛选装配基准面，重新载入
            for (int j = 0; j < Face.Count; j++)
            {
                if (Face[j].transform.localPosition == DatumFace &&
                    Face[j].transform.localEulerAngles == R_DatumFace)
                {
                    AssembleInfo.Add("装配基准面", Face[j]);
                }
            }
            //重载自身基准轴，放入
            List<GameObject> SelfAxle = new List<GameObject>();
            foreach (Transform A in UsingFunction.Family(this.gameObject)[2].transform)
            {
                SelfAxle.Add(A.transform.gameObject);
            }
            //重载自身基准面,放入
            List<GameObject> SelfFace = new List<GameObject>();
            foreach (Transform F in UsingFunction.Family(this.gameObject)[3].transform)
            {
                SelfFace.Add(F.transform.gameObject);
            }
            SelfInfo.Add("自身基准轴", SelfAxle);
            SelfInfo.Add("自身基准面", SelfFace);
            //筛选左右端面
            for (int k = 0; k < SelfFace.Count; k++)
            {
                if (SelfFace[k].transform.localPosition == LeftFace && !SelfInfo.ContainsKey("左端面")
                    && SelfFace[k].transform.localEulerAngles == R_LeftFace)
                {
                    SelfInfo.Add("左端面", SelfFace[k]);
                    //Debug.Log(UsingFunction.Family(this.gameObject)[0].name + SelfFace[k].transform.localPosition+"左");
                }
                if (SelfFace[k].transform.localPosition == RightFace && !SelfInfo.ContainsKey("右端面")
                    && SelfFace[k].transform.localEulerAngles == R_RightFace)
                {
                    SelfInfo.Add("右端面", SelfFace[k]);
                    //Debug.Log(UsingFunction.Family(this.gameObject)[0].name + SelfFace[k].transform.localPosition+"右");
                }
            }
        }
    }

    void Start()
    {
        AllCollider = new List<MeshCollider>();
        SelfInfo = new Dictionary<string, object>();
        AssembleInfo = new Dictionary<string, object>();
        M = GameObject.Find("Controler").GetComponent<ClickManager>().M;
        MaterialRecover();
        Open = true;
        CurrentScene = SceneManager.GetActiveScene();
        if (CurrentScene.name == "Assembly")
        {
            //首先获取控制脚本
            Loading = GameObject.Find("LoadingControl").GetComponent<LoadingControl>();
        }
    }
    /// <summary>
    /// 零件装配信息识别，按照零件的装配信息格式识别信息，并将AllInfo信息中的无分类信息装载入PartInfo内
    /// 按照所在零件的tag分类，只在tag为装配零件的时候才运行重载方法
    /// 已导入数据文件，识别数据，分类数据，对应重载入重载信息字段
    /// </summary>
    public void Compile()
    {
        if (UsingFunction.Family(this.gameObject)[0].tag == "装配零件" ||
            UsingFunction.Family(this.gameObject)[0].tag == "拨叉")
        {
            int Begin2Read = 0;
            string[] R = Loading.AllInfo[UsingFunction.Family(this.gameObject)[0].gameObject.name];
            for (int i = 0; i < R.Length; i++)
            {
                if (R[i].Contains("以下为使用数据"))
                {
                    //记录使用段数据的位置
                    Begin2Read = i;
                    break;
                }
            }
            //Debug.Log(Begin2Read);
            //Debug.Log(R.Length);
            //获取有用数据，装入新列表
            //从使用段数据位置开始读取
            List<string> _R = new List<string>();
            for (int j = Begin2Read; j < R.Length; j++)
            {
                _R.Add(R[j]);
            }
            Distinguish(_R);
            //Debug.Log("实例化完成");
        }
    }

    /// <summary>
    /// 识别装配信息，读取装配信息，重载入
    /// </summary>
    /// <param name="_R">可识别数据列表</param>
    /// <returns></returns>
    public void Distinguish(List<string> _R)
    {
        //基准轴的localpos
        int k1 = 0;
        int k1r = 0;
        //基准面的localpos
        int k2 = 0;
        int k2r = 0;
        //抽取数据节点，装入对应数据
        for (int k = 0; k < _R.Count; k++)
        {
            switch (_R[k])
            {
                case ("零件类型"):
                    Kind = _R[k + 1];
                    ; break;
                case ("装配轴"):
                    Shaft = _R[k + 1];
                    ; break;
                case ("装配顺序"):
                    Order = _R[k + 1];
                    ; break;
                case ("自身基准轴位置"): k1 = k; break;
                case ("自身基准轴位姿"): k1r = k; break;
                case ("自身基准面位置"): k2 = k; break;
                case ("自身基准面位姿"): k2r = k; break;
                case ("左端面位置"):
                    //LeftFace = (new Vector3(_R[k + 1].ToCharArray()[3],
                    //    _R[k + 1].ToCharArray()[5], _R[k + 1].ToCharArray()[7]));
                    LeftFace = String2Vector3(_R[k + 1]);
                    ; break;
                case ("左端面位姿"):
                    //LeftFace = (new Vector3(_R[k + 1].ToCharArray()[3],
                    //    _R[k + 1].ToCharArray()[5], _R[k + 1].ToCharArray()[7]));
                    R_LeftFace = String2Vector3(_R[k + 1]);
                    ; break;
                case ("右端面位置"):
                    //RightFace = (new Vector3(_R[k + 1].ToCharArray()[3],
                    //    _R[k + 1].ToCharArray()[5], _R[k + 1].ToCharArray()[7]));
                    RightFace = String2Vector3(_R[k + 1]);
                    ; break;
                case ("右端面位姿"):
                    //RightFace = (new Vector3(_R[k + 1].ToCharArray()[3],
                    //    _R[k + 1].ToCharArray()[5], _R[k + 1].ToCharArray()[7]));
                    R_RightFace = String2Vector3(_R[k + 1]);
                    ; break;
                case ("装配基准轴位置"):
                    DatumAxle = String2Vector3(_R[k + 1]);
                    ; break;
                case ("装配基准轴位姿"):
                    R_DatumAxle = String2Vector3(_R[k + 1]);
                    ; break;
                case ("装配基准面位置"):
                    DatumFace = String2Vector3(_R[k + 1]);
                    ; break;
                case ("装配基准面位姿"):
                    R_DatumFace = String2Vector3(_R[k + 1]);
                    ; break;
                case ("装配位置"):
                    AssemblyPos = String2Vector3(_R[k + 1]);
                    ; break;
                default: break;
            }
        }
        //读取自身基准轴位置
        for (int i = k1 + 1; i < k2; i++)
        {
            SelfAxlePos.Add(String2Vector3(_R[i]));
        }
        //Debug.Log(SelfAxlePos.Count);
        //读取自身基准面位置
        for (int j = k2 + 1; j < k1r; j++)
        {
            SelfFacePos.Add(String2Vector3(_R[j]));
        }
        //Debug.Log(SelfFacePos.Count+UsingFunction.Family(this.gameObject)[0].name);
        for (int i = k1r + 1; i < k2r; i++)
        {
            SelfAxleRot.Add(String2Vector3(_R[i]));
        }
        for (int j = k2r + 1; j < _R.Count; j++)
        {
            SelfFaceRot.Add(String2Vector3(_R[j]));
        }
        //Debug.Log(SelfFaceRot.Count + UsingFunction.Family(this.gameObject)[0].name);
        //实例化自身基准轴
        for (int i = 0; i < SelfAxlePos.Count; i++)
        {
            GameObject a = ((GameObject)Instantiate(Resources.Load("_Prefabs/axle"),
                UsingFunction.Family(this.gameObject)[2].transform));
            a.transform.localPosition = SelfAxlePos[i];
            a.transform.localEulerAngles = SelfAxleRot[i];
        }
        //实例化自身基准面
        for (int i = 0; i < SelfFacePos.Count; i++)
        {
            GameObject b = ((GameObject)Instantiate(Resources.Load("_Prefabs/face"),
                UsingFunction.Family(this.gameObject)[3].transform));
            b.transform.localPosition = SelfFacePos[i];
            b.transform.localEulerAngles = SelfFaceRot[i];
            //Debug.Log(i + UsingFunction.Family(this.gameObject)[0].name);
        }
    }

    /// <summary>
    /// 输入任意一个包含三维向量的字符及任意插入空格，包含小数点等字符串
    /// 使.txt数据每行转换成三维向量
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

    private void LateUpdate()
    {
        if (Loading != null && CurrentScene.name == "Assembly")
        {
            if (Loading.LoadInfo && Open && Loading.LoadedNum == Loading.AxleNum &&
                (this.transform.parent.gameObject.tag == "装配零件" || this.transform.parent.tag == "拨叉"))
            {
                Open = false;
                ReloadInfo();
            }
        }
    }

    public void Initialization()
    {
        SelfInfo.Clear();
        AssembleInfo.Clear();
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

    public void MaterialRecover()
    {
        //模型层里装载的模型为复合模型
        if (UsingFunction.Family(this.gameObject)[5].transform.GetChild(0).gameObject.transform.childCount != 0)
        {
            foreach (Transform Child in UsingFunction.Family(this.gameObject)[5].GetComponentInChildren<Transform>())
            {
                foreach (Transform R in Child.GetComponentInChildren<Transform>())
                {
                    if (R.GetComponent<MeshRenderer>().material != M)
                    //Resources.Load<Material>("Assets/Resources/Hard Surface Pro/Shared/Materials/TestMat_Glass_Crystal"))
                    {
                        //Debug.Log(R.GetComponent<MeshRenderer>().material);
                        R.GetComponent<MeshRenderer>().material = M;
                        Resources.Load<Material>("Assets/Resources/Hard Surface Pro/Shared/Materials/TestMat_Glass_Crystal");
                    }
                }
            }
        }
        else
        {
            if (UsingFunction.Family(this.gameObject)[5].GetComponentInChildren<MeshRenderer>().material != M)
            {
                UsingFunction.Family(this.gameObject)[5].GetComponentInChildren<MeshRenderer>().material = M;
                //Resources.Load<Material>
                //    ("Assets/Resources/Hard Surface Pro/Shared/Materials/TestMat_Glass_Crystal");
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
