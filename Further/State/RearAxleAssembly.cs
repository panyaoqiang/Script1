using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RearAxleAssembly : MonoBehaviour
{
    public ClickManager Click;
    /// <summary>
    /// 用作总装配或部分装配
    /// </summary>
    //public List<Button> B = new List<Button>();
    public List<Text> Name = new List<Text>();
    /// <summary>
    /// UI面板切换
    /// </summary>
    public List<GameObject> Page1 = new List<GameObject>();
    public List<GameObject> Page2 = new List<GameObject>();

    /// <summary>
    /// 口袋管理已选择数据，选择完毕后只使用口袋中的数据
    /// </summary>
    public Dictionary<string, GameObject> Pocket = new Dictionary<string, GameObject>();

    /// <summary>
    /// 临时数据
    /// </summary>
    public GameObject Body;
    public GameObject Part;
    public GameObject Body_Axle;
    public GameObject Part_Axle;
    public GameObject Body_Face;
    public GameObject Part_Face;

    Dictionary<string, Vector3> TemData = new Dictionary<string, Vector3>();
    //总开关只有1或者0
    //非装配状态或完成状态开关，所有开关为0
    //开始装配，总开关为1，其余全为0
    //单步开始装配，该开关为1
    //正在装配且已经完成该步骤，所有开关为2
    //当所有开关为2，则全部置零，结束装配状态
    //当取消装配，则全部置0，回到初始位置位姿
    public int Whole = 0;
    public int R = 0;

    public bool r = false;
    public int M1 = 0;
    public int M2 = 0;
    public int M3 = 0;
    public float Speed = 10f;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Print(Name[0], Body, "主体");
        Print(Name[1], Part, "零件");
        Print(Name[2], Body_Axle, "主体基准轴");
        Print(Name[3], Part_Axle, "零件基准轴");
        Print(Name[4], Body_Face, "主体基准面");
        Print(Name[5], Part_Face, "零件基准面");
        if (Whole == 1)
        {
            MoveAndRotate();
        }
    }


    /// <summary>
    /// 只对临时数据进行添加或替换
    /// 增，1：口袋空，直接添加。2：口袋非空，直接替换
    /// </summary>
    /// <param name="Po">装载临时数据的对应口袋，共6个</param>
    public void Add(GameObject Po, string What)
    {
        if (Click.ClickObj != null)
        {
            switch (What)
            {
                case "件":
                    if (Click.ClickObj.tag == "装配零件" || Click.ClickObj.tag == "箱体")
                    {
                        Po = Click.ClickObj;
                    }
                    ; break;
                case "轴":
                    if (Click.ClickObj.tag == "装配基准轴")
                    {
                        Po = Click.ClickObj;
                    }
                    ; break;
                case "面":
                    if (Click.ClickObj.tag == "装配基准面")
                    {
                        Po = Click.ClickObj;
                    }
                    ; break;
                default: break;
            }
        }
    }
    /// <summary>
    /// 只对临时数据进行删除
    /// 删，1：口袋空，不操作。2：口袋非空，删除数据
    /// </summary>
    /// <param name="Po">装载临时数据的对应口袋，共6个</param>
    public void Delete(GameObject Po)
    {
        if (Po != null)
        {
            Po = null;
        }
    }
    /// <summary>
    /// st1-6未完成状态下：
    /// 当口袋非空，对应Text显示口袋零件名称。处于正在选择状态，显示点击零件名称
    /// st1-6已完成状态下：
    /// 对口袋中的数据进行修改，需要撤回一切操作后，对pocket数据清空，重新选择
    /// </summary>
    public void Print(Text Pr, GameObject Po, string st)
    {
        //口袋对应位置已有零件，则长期显示该零件名称
        try
        {
            Pr.text = Pocket[st].name;
        }
        //口袋为空，处于初次或重置后的st1-6阶段，所有Text显示暂存口袋p中的数据
        catch
        {
            //口袋非空，持续显示口袋中零件的名称
            if (Po != null)
            {
                Pr.text = Po.name;
            }
            //口袋清空但已选择，显示已选择零件的名称
            if (Po == null && Click.ClickObj != null)
            {
                Pr.text = Click.ClickObj.name;
            }
            //口袋清空，未选择，显示为空
            if (Po == null && Click.ClickObj == null)
            {
                Pr.text = "";
            }
        }
    }
    ///// <summary>
    ///// 重新选择，撤回所有操作，清空pocket
    ///// </summary>
    //public void Rechoose()
    //{
    //    //解锁零件
    //    Pocket["主体"].GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
    //    //清空口袋
    //    Pocket.Clear();
    //    Body = null;
    //    Part = null;
    //    Body_Axle = null;
    //    Part_Axle = null;
    //    Body_Face = null;
    //    Part_Face = null;
    //}

    public void St1Add()
    {
        if (Body != null && Click.ClickObj.name == "差速器壳体")
        {
            Add(Body, "件");
        }
        else
        {
            //错误提醒
        }
    }
    public void St2Add()
    {
        if (Part != null)
        {
            Add(Part, "件");
        }
    }
    public void St1Del()
    {
        if (Body != null)
        {
            UsingFunction.HideAndAppear("Hide", 2, Body, null);
            Delete(Body);
            try
            {
                Pocket.Remove("主体");
            }
            catch
            {

            }
        }
    }
    public void St2Del()
    {
        if (Part != null)
        {
            UsingFunction.HideAndAppear("Hide", 2, Part, null);
            Delete(Part);
            try
            {
                Pocket.Remove("零件");
            }
            catch
            {

            }
        }
    }
    //面板1，选择零件

    /// <summary>
    /// 面板1完成
    /// 零件选择完毕后，放入pocket，再切换面板2
    /// </summary>
    public void st2fin()
    {
        UsingFunction.HideAndAppear("Appear", 2, Body, null);
        UsingFunction.HideAndAppear("Appear", 2, Part, null);
        Pocket.Add("主体", Body);
        Pocket.Add("零件", Part);
    }

    public void St3Add()
    {
        if (Body_Axle != null && UsingFunction.Family(Click.ClickObj)[0].tag == "差速器壳体")
        {
            //选择后隐藏基准轴，显示零件基准面
            Add(Body_Axle, "轴");
            UsingFunction.HideAndAppear("Hide", 2, Body, null);
        }
    }
    public void St4Add()
    {
        if (Part_Axle != null)
        {
            //选择后隐藏基准轴，显示零件基准面
            Add(Part_Axle, "轴");
            UsingFunction.HideAndAppear("Hide", 2, Part, null);
        }
    }
    public void St3Del()
    {
        if (Body_Axle != null)
        {
            //取消选择基准轴，重新显示基准轴并且隐藏基准面
            UsingFunction.HideAndAppear("Appear", 2, Body, null);
            UsingFunction.HideAndAppear("Hide", 3, Body, null);
            Delete(Body_Axle);
            try
            {
                Pocket.Remove("主体基准轴");
            }
            catch
            {

            }

        }
    }
    public void St4Del()
    {
        if (Part_Axle != null)
        {
            //取消选择基准轴，重新显示基准轴并且隐藏基准面
            UsingFunction.HideAndAppear("Appear", 2, Part, null);
            UsingFunction.HideAndAppear("Hide", 3, Part, null);
            Delete(Part_Axle);
            try
            {
                Pocket.Remove("零件基准轴");
            }
            catch
            {

            }
        }
    }
    //面板2，零件轴已经选择完毕，可以开始轴对准

    /// <summary>
    /// 面板2完成
    /// 选择完毕后切换面板，确认并开始轴对齐，并切换面板3
    /// </summary>
    public void st4fin()
    {
        Pocket.Add("主体基准轴", Body_Axle);
        Pocket.Add("零件基准轴", Part_Axle);
        DifferentialGearAssembly();
        Whole = 1;
        //DifferentialGearAssembly();//由按钮控制开始轴对齐，默认r方向
    }

    /// <summary>
    /// 完成对准后，开始选择基准面
    /// </summary>
    public void St5Add()
    {
        if (Body_Face != null)
        {
            Add(Body_Face, "面");
            UsingFunction.HideAndAppear("Hide", 3, Body, null);
        }
    }
    public void St6Add()
    {
        if (Part_Face != null)
        {
            Add(Part_Face, "面");
            UsingFunction.HideAndAppear("Hide", 3, Part, null);
        }
    }
    public void St5Del()
    {
        if (Body_Face != null)
        {
            //取消选择基准面，重新显示基准面
            UsingFunction.HideAndAppear("Appear", 3, Body, null);
            Delete(Body_Face);
            try
            {
                Pocket.Remove("主体基准面");
            }
            catch
            {

            }
        }
    }
    public void St6Del()
    {
        if (Part_Face != null)
        {
            UsingFunction.HideAndAppear("Appear", 3, Part, null);
            Delete(Part_Face);
            try
            {
                Pocket.Remove("零件基准面");
            }
            catch
            {

            }
        }
    }

    /// <summary>
    /// 面板3完成
    /// 全部完成选择，装入口袋并记录初始位置，清空临时数据，切换面板4
    /// </summary>
    public void FinChooseAll()
    {
        if (Body_Face != null && Part_Face != null)
        {
            Pocket.Add("主体基准面", Body_Face);
            Pocket.Add("零件基准面", Part_Face);

        }
    }

    /// <summary>
    /// 面板4开始，先开此方法，再打开whole=1；
    /// 差速器行星齿轮装配，首先开始此方法
    /// </summary>
    public void DifferentialGearAssembly()
    {
        //装载获取临时数据
        TemData.Add("原位置", Pocket["零件"].transform.position);
        TemData.Add("原位姿", Pocket["零件"].transform.eulerAngles);
        //无法重选基准轴
        TemData.Add("主体中心轴", Pocket["主体"].transform.right);
        TemData.Add("主体装配基准轴", Pocket["主体基准轴"].transform.eulerAngles);
        TemData.Add("零件装配基准轴", Pocket["零件基准轴"].transform.eulerAngles);
        //记录零件原位置，原位姿
        Pocket["零件"].GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        Pocket["主体"].GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        //解锁零件，锁定壳体
        R = 1;
    }

    /// <summary>
    /// 无法返回上一步，只能取消装配
    /// </summary>
    public void MoveAndRotate()
    {
        //点击按钮，开始轴对准
        if (R == 1)
        {
            //基准轴对准
            //首先旋转零件至与基准轴平行
            //传入数据作临时备用
            //计算叉乘向量确定旋转轴
            Vector3 RotAxle;
            if (!r)
            {
                RotAxle = Vector3.Cross(TemData["零件装配基准轴"].normalized, TemData["主体装配基准轴"].normalized);

            }
            else
            {
                RotAxle = Vector3.Cross(TemData["零件装配基准轴"].normalized, -TemData["主体装配基准轴"].normalized);
            }
            //计算点积角确定旋转角度大小
            float Fai = Mathf.Acos(Vector3.Dot(TemData["主体装配基准轴"].normalized, TemData["零件装配基准轴"].normalized));
            if (Fai * Mathf.Rad2Deg >= 2f)
            {
                float D = 1f;
                Pocket["零件"].GetComponent<Rigidbody>().AddTorque(RotAxle * Fai * Mathf.Rad2Deg * Speed * 10f);
                Pocket["零件"].GetComponent<Rigidbody>().angularDrag =
                    Mathf.Abs(Fai * Mathf.Rad2Deg - Fai * Mathf.Rad2Deg) + (++D) * (++D);
            }
            if (Fai * Mathf.Rad2Deg < 2f && Fai * Mathf.Rad2Deg > 0f)
            {
                Pocket["零件"].transform.eulerAngles += TemData["主体装配基准轴"] - TemData["零件装配基准轴"];
                R = 2;
                //由下一步选基准面按钮确定正反对准是否正确再开始M1 = 1;
            }

        }
        //轴对准完成后，点击按钮开始装入箱体
        if (M1 == 1 && R == 2)
        {
            //1.零件装配基准轴与壳体装配基准轴平行
            //连接两者原点，从零件指向壳体
            TemData.Add("连接向量", Pocket["主体"].transform.position - Pocket["零件"].transform.position);
            //求壳体与零件原点之间的向量与壳体中心轴的夹角cos值
            float Fai2 = Vector3.Dot(TemData["主体中心轴"].normalized, -TemData["连接向量"].normalized);
            //夹角cos乘以连接向量的模长得壳体与零件初始相对高度差
            float Hight = Fai2 * TemData["连接向量"].magnitude;
            //沿壳体x轴正向平移该高度差即可得零件与壳体中心轴接触点
            TemData.Add("中心轴对准点", Pocket["主体"].transform.position + new Vector3(Hight, 0, 0));

            Pocket["零件"].GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
            //再锁住轴本体
            Pocket["主体"].GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;

            float Dis;
            Vector3 Dir;
            //求出零件到轴端面的对准位置的距离
            Dis = (TemData["中心轴对准点"] - Pocket["零件"].transform.position).magnitude;
            if (Dis >= 2f)
            {
                Dir = (TemData["中心轴对准点"] - Pocket["零件"].transform.position);
                Pocket["零件"].GetComponent<Rigidbody>().AddForce(Dir.normalized * Dir.magnitude * Speed * 1f);
                Pocket["零件"].GetComponent<Rigidbody>().drag = 8f;
            }
            if (Dis < 2f)
            {
                Pocket["零件"].transform.position = TemData["中心轴对准点"];
                M1 = 2;
                M2 = 1;
            }
        }
        if (M2 == 1 && M1 == 2)
        {
            float Fai2 = Vector3.Dot(TemData["主体中心轴"].normalized, -TemData["连接向量"].normalized);
            //夹角cos乘以连接向量的模长得壳体与零件初始相对高度差
            float Hight = Fai2 * TemData["连接向量"].magnitude;
            float Dis;
            Vector3 Dir;
            //求出零件到轴端面的对准位置的距离
            Dis = (Pocket["主体"].transform.position - Pocket["零件"].transform.position).magnitude;
            if (Dis >= 2f)
            {
                Dir = (Pocket["主体"].transform.position - Pocket["零件"].transform.position);
                Pocket["零件"].GetComponent<Rigidbody>().AddForce(Dir.normalized * Dir.magnitude * Speed * 1f);
                Pocket["零件"].GetComponent<Rigidbody>().drag = 8f;
            }
            if (Dis < 2f)
            {
                Pocket["零件"].transform.position = Pocket["主体"].transform.position;
                M2 = 2;
                //st4完成，面板2切换，并显示零件基准面，先选择基准面再开始对准
                UsingFunction.HideAndAppear("Appear", 3, Pocket["主体"], null);
                UsingFunction.HideAndAppear("Appear", 3, Pocket["零件"], null);
            }
        }
        //插入完毕后开始面对准并安装
        if (M3 == 1 && M2 == 2)
        {
            float Dis;
            Vector3 Dir;
            //求出零件到轴端面的对准位置的距离
            Dis = (TemData["主体装配基准面"] - TemData["零件装配基准面"]).magnitude;
            if (Dis >= 2f)
            {
                Dir = (TemData["主体装配基准面"] - TemData["零件装配基准面"]);
                Pocket["零件"].GetComponent<Rigidbody>().AddForce(Dir.normalized * Dir.magnitude * Speed * 1f);
                Pocket["零件"].GetComponent<Rigidbody>().drag = 8f;
            }
            if (Dis < 2f)
            {
                Pocket["零件"].transform.position += TemData["主体装配基准面"] - TemData["零件装配基准面"];
                Pocket["零件"].GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePosition
                    | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
                M3 = 2;
            }
        }
    }
    public void Continue2Coaxial()
    {
        M1 = 1;

    }

    public void Star2Copanar()
    {
        if(!TemData.ContainsKey("零件面对齐前位置"))
        {
            TemData.Add("零件面对齐前位置", Pocket["零件"].transform.position);
            TemData.Add("零件面对齐前位姿", Pocket["零件"].transform.eulerAngles);
        }
        TemData.Add("主体装配基准面", Pocket["主体基准面"].transform.position);
        TemData.Add("零件装配基准面", Pocket["零件基准面"].transform.position);
        M3 = 1;
    }

    public void FinWhole()
    {
        Body = null;
        Part = null;
        Body_Axle = null;
        Part_Axle = null;
        Body_Face = null;
        Part_Face = null;
        Pocket["零件"].AddComponent<FixedJoint>();
        Pocket["零件"].GetComponent<FixedJoint>().connectedBody = Pocket["主体"].GetComponent<Rigidbody>();
        R = 0;
        M1 = 0;
        M2 = 0;
        M3 = 0;
        Whole = 0;
        Pocket["零件"].GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        Pocket["主体"].GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        TemData.Clear();
        Pocket.Clear();
    }

    /// <summary>
    /// 在轴对准正进行和已完成但未开始下一步状态下进行
    /// 点击则控制反转
    /// </summary>
    public void re_R1()
    {
        //当且仅当旋转开始，而对移动未开始时才能撤回，否则整体取消
        if (R != 0 && Whole == 1 && M1 == 0)
        {
            Pocket["零件"].transform.position = TemData["原位置"];
            Pocket["零件"].transform.eulerAngles = TemData["原位姿"];
            r = true;
            R = 1;
            M2 = 0;
        }
    }
    /// <summary>
    /// 完成对准后，在开始进行面对齐或完成面对齐时撤销
    /// 重新选择基准面，返回st5
    /// 位移，锁定，M3变值
    /// </summary>
    public void re_M3()
    {
        if (M2 == 2 && Whole == 1 && M3 != 0)
        {
            Pocket["零件"].transform.position = TemData["零件面对齐前位置"];
            Pocket["零件"].transform.eulerAngles = TemData["零件面对齐前位姿"];
            Pocket["零件"].GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
            TemData.Remove("主体装配基准面");
            TemData.Remove("零件装配基准面");
            Pocket.Remove("主体基准面");
            Pocket.Remove("零件基准面");
            M3 = 0;
        }
    }

    /// <summary>
    /// 点击后返回st6面板4开始
    /// </summary>
    public void re_Whole()
    {
        if (Whole == 1)
        {
            Pocket["零件"].GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
            Pocket["主体"].GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
            Pocket["零件"].transform.position = TemData["原位置"];
            Pocket["零件"].transform.eulerAngles = TemData["原位姿"];
            TemData.Clear();
            R = 0;
            M1 = 0;
            M2 = 0;
            M3 = 0;
            Whole = 0;
        }
    }

    public void PanelSwitch(List<GameObject>P,string HideAndAppear)
    {
        if (HideAndAppear == "Hide")
        {
            for(int i = P.Count; i < P.Count; i++)
            {
                P[i].SetActive(false);
            }
        }
        else
        {
            for (int i = P.Count; i < P.Count; i++)
            {
                P[i].SetActive(true);
            }
        }
    }
    /// <summary>
    /// 状态1，下一页
    /// </summary>
    public void Switch1()
    {
        PanelSwitch(Page1, "Hide");
        PanelSwitch(Page2, "Appear");
    }
    /// <summary>
    /// 状态2，上一页
    /// </summary>
    public void Switch2()
    {
        PanelSwitch(Page1, "Appear");
        PanelSwitch(Page2, "Hide");
    }
}
