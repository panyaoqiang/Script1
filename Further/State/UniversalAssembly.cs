using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UniversalAssembly : MonoBehaviour
{
    public ClickManager Click;
    /// <summary>
    /// 用作总装配或部分装配
    /// </summary>
    //public List<Button> B = new List<Button>();
    public List<Text> Name = new List<Text>();
    public Text LineForR;
    public Text FaceForR;
    /// <summary>
    /// 口袋管理已选择数据，选择完毕后只使用口袋中的数据
    /// </summary>
    public Dictionary<string, GameObject> Pocket = new Dictionary<string, GameObject>();
    public GameObject Body;
    public GameObject Part;
    public GameObject Body_Axle;
    public GameObject Part_Axle;
    public GameObject Body_Face;
    public GameObject Part_Face;

    /// <summary>
    /// 状态器，常态0，正在操作为2，选取状态分1.1取零件，1.2取基准轴，1.3取基准面，完成选取+0.01f，
    /// </summary>
    public float StateControl = 0;
    public int MandRCounter = 0;
    public float Speed;
    public bool LineFowOrRev = true;
    public bool FaceFowOrRev = true;
    public bool _Begin2Aline = false;
    public bool _Begin2Coaxial = false;
    public bool _Begin2Aplane = false;
    public bool _Begin2Coplanar = false;

    //Vector3 BodyEngle;
    //Vector3 PartEngle;
    //float Fai;
    //Vector3 RotAxle;
    // Start is called before the first frame update
    void Start()
    {
        Initialized();
    }

    public void Initialized()
    {
        Body = null;
        Part = null;
        Body_Axle = null;
        Part_Axle = null;
        Body_Face = null;
        Part_Face = null;
        Pocket.Clear();
        StateControl = 0;
        MandRCounter = 0;
        Speed = 5f;
        LineFowOrRev = true;
        FaceFowOrRev = true;
        _Begin2Aline = false;
        _Begin2Coaxial = false;
        _Begin2Aplane = false;
        _Begin2Coplanar = false;
        PrintInitialized();
    }

    // Update is called once per frame
    void Update()
    {
        Print(Name[0], Body, "主体",false,"主体");
        Print(Name[1], Part, "零件", false, "零件");
        Print(Name[2], Body_Axle, "主体基准轴", true, "装配基准轴");
        Print(Name[3], Part_Axle, "零件基准轴", true, "装配基准轴");
        Print(Name[4], Body_Face, "主体基准面", true, "装配基准面");
        Print(Name[5], Part_Face, "零件基准面", true, "装配基准面");
        //if (Pocket["主体"] != null && Pocket["零件"] != null
        //    && Pocket["主体基准轴"] != null && Pocket["零件基准轴"] != null)
        try
        {
            if (_Begin2Aline)
            {
                AxisAlignment();
            }
            if (_Begin2Coaxial)
            {
                Coaxial();
            }

        }
        catch { }
        //if (Pocket["主体"] != null && Pocket["零件"] != null
        //    && Pocket["主体基准面"] != null && Pocket["零件基准面"] != null)
        try
        {
            if (_Begin2Aplane)
            {
                FaceAlignment();
            }
            if (_Begin2Coplanar)
            {
                Coplanar();
            }

        }
        catch { }
    }
    /// <summary>
    /// 只对临时数据进行添加或替换
    /// 增，1：口袋空，直接添加。2：口袋非空，直接替换
    /// </summary>
    /// <param name="Po">装载临时数据的对应口袋，共6个</param>
    public void Add(GameObject Po)
    {
        Po = Click.ClickObj;
        if (Click.ClickObj != null)
        {

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
    public void Print(Text Pr, GameObject Po, string st, bool T, string Type)
    {
        //口袋对应位置已有零件，则长期显示该零件名称
        try
        {
            Pr.text = Pocket[st].name;
        }
        //口袋为空，处于初次或重置后的st1-6阶段，所有Text显示暂存口袋p中的数据
        catch
        {
            if (T)
            {
                //暂存非空，持续显示零件的名称
                if (Po != null)
                {
                    Pr.text = Po.name;
                }
                //暂存清空但已选择，显示已选择零件的名称
                if (Po == null && Click.ClickObj != null && Click.ClickObj.tag == Type)
                {
                    Pr.text = Click.ClickObj.name;
                }
                //暂存清空，未选择，显示为空
                if (Po == null && Click.ClickObj == null)
                {
                    Pr.text = "";
                }
            }
            else
            {
                //暂存非空，持续显示零件的名称
                if (Po != null)
                {
                    Pr.text = Po.name;
                }
                //暂存清空但已选择，显示已选择零件的名称
                if (Po == null && Click.ClickObj != null)
                {
                    Pr.text = Click.ClickObj.name;
                }
                //暂存清空，未选择，显示为空
                if (Po == null && Click.ClickObj == null)
                {
                    Pr.text = "";
                }
            }
        }
    }
    public void PrintInitialized()
    {
        for(int i=0;i< Name.Count; i++)
        {
            Name[i].text = "";
        }
    }
    public void St1Add()
    {
        if (StateControl == 0 || StateControl == 1.1f && (Click.ClickObj != null))
        {
            Body = Click.ClickObj;
            StateControl = 1.1f;
        }
    }
    public void St2Add()
    {
        if (StateControl == 0 || StateControl == 1.1f && (Click.ClickObj != null))
        {
            Part = Click.ClickObj;
            StateControl = 1.1f;
        }
    }
    public void St1Del()
    {
        if (Body != null && StateControl == 1.1f)
        {
            Delete(Body);
        }
    }
    public void St2Del()
    {
        if (Part != null && StateControl == 1.1f)
        {
            Delete(Part);
        }
    }
    public void ConfirmChoosePartAndBody()
    {
        if (StateControl == 1.1f && Body != null && Part != null)
        {
            StateControl = 1.11f;
            Pocket.Add("主体", Body);
            Pocket.Add("零件", Part);
        }
    }
    public void Star2ChooseAxis()
    {
        if (StateControl == 1.11f || StateControl == 1.31f || StateControl > 2.2f)
        {
            UsingFunction.HideAndAppear("Appear", 2, Pocket["主体"], null);
            UsingFunction.HideAndAppear("Appear", 2, Pocket["零件"], null);
            StateControl = 1.2f;
        }
    }
    public void CancelStarChooseAxis()
    {
        if (StateControl == 1.2f)
        {
            UsingFunction.HideAndAppear("Hide", 2, Pocket["主体"], null);
            UsingFunction.HideAndAppear("Hide", 2, Pocket["零件"], null);
            StateControl = 1.11f;
        }
    }
    public void St3Add()
    {
        if (StateControl == 1.2f && Click.ClickObj != null && Click.ClickObj.tag == "装配基准轴")
        {
            //选择后隐藏基准轴，显示零件基准面
            Body_Axle = Click.ClickObj;
            UsingFunction.HideAndAppear("Hide", 2, Pocket["主体"], null);
        }
    }
    public void St4Add()
    {
        if (StateControl == 1.2f && Click.ClickObj != null && Click.ClickObj.tag == "装配基准轴")
        {
            //选择后隐藏基准轴，显示零件基准面
            Part_Axle = Click.ClickObj;
            UsingFunction.HideAndAppear("Hide", 2, Pocket["零件"], null);
        }
    }
    public void St3Del()
    {
        if (Body_Axle != null && StateControl == 1.2f)
        {
            //取消选择基准轴，重新显示基准轴并且隐藏基准面
            UsingFunction.HideAndAppear("Appear", 2, Body, null);
            //UsingFunction.HideAndAppear("Hide", 3, Body, null);
            Delete(Body_Axle);
        }
    }
    public void St4Del()
    {
        if (Part_Axle != null && StateControl == 1.2f)
        {
            //取消选择基准轴，重新显示基准轴并且隐藏基准面
            UsingFunction.HideAndAppear("Appear", 2, Part, null);
            //UsingFunction.HideAndAppear("Hide", 3, Part, null);
            Delete(Part_Axle);
        }
    }
    public void Confirm2ChooseAxis()
    {
        if (StateControl == 1.2f)
        {
            Pocket.Add("主体基准轴", Body_Axle);
            Pocket.Add("零件基准轴", Part_Axle);
            StateControl = 1.21f;
        }
    }
    public void Cancel2ChooseAxis()
    {
        if (StateControl == 1.21f)
        {
            Pocket.Remove("主体基准轴");
            Pocket.Remove("零件基准轴");
            Body_Axle = null;
            Part_Axle = null;
            _Begin2Aline = false;
            StateControl = 1.11f;
        }
    }
    public void Star2ChooseFace()
    {
        if (StateControl == 1.21f || StateControl == 1.11f || StateControl > 2.2f)
        {
            UsingFunction.HideAndAppear("Appear", 3, Pocket["主体"], null);
            UsingFunction.HideAndAppear("Appear", 3, Pocket["零件"], null);
            StateControl = 1.3f;
        }
    }
    public void CancelStar2ChooseFace()
    {
        if (StateControl == 1.3f)
        {
            UsingFunction.HideAndAppear("Hide", 3, Pocket["主体"], null);
            UsingFunction.HideAndAppear("Hide", 3, Pocket["零件"], null);
            StateControl = 1.11f;
        }
    }
    public void St5Add()
    {
        if (StateControl == 1.3f && Click.ClickObj != null && Click.ClickObj.tag == "装配基准面")
        {
            Body_Face = Click.ClickObj;
            UsingFunction.HideAndAppear("Hide", 3, Body, null);
        }
    }
    public void St6Add()
    {
        if (StateControl == 1.3f && Click.ClickObj != null && Click.ClickObj.tag == "装配基准面")
        {
            Part_Face = Click.ClickObj;
            UsingFunction.HideAndAppear("Hide", 3, Part, null);
        }
    }
    public void St5Del()
    {
        if (Body_Face != null && StateControl == 1.3f)
        {
            //取消选择基准面，重新显示基准面
            Delete(Body_Face);
            UsingFunction.HideAndAppear("Appear", 3, Body, null);
        }
    }
    public void St6Del()
    {
        if (Part_Face != null && StateControl == 1.3f)
        {
            Delete(Part_Face);
            UsingFunction.HideAndAppear("Appear", 3, Part, null);
        }
    }
    public void Confirm2ChooseFace()
    {
        if (StateControl == 1.3f)
        {
            Pocket.Add("主体基准面", Body_Face);
            Pocket.Add("零件基准面", Part_Face);
            StateControl = 1.31f;
        }
    }
    public void Cancel2ChooseFace()
    {
        if (StateControl == 1.31f)
        {
            Pocket.Remove("主体基准面");
            Pocket.Remove("零件基准面");
            Body_Face = null;
            Part_Face = null;
            _Begin2Aplane = false;
            StateControl = 1.11f;
        }
    }
    /// <summary>
    /// 轴对齐,打印正反
    /// </summary>
    public void AxisAlignment()
    {
        Vector3 BodyEngle;
        Vector3 PartEngle;
        float Fai;
        Vector3 RotAxle;
        Pocket["主体"].GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        if (LineFowOrRev)
        {
            BodyEngle = Pocket["主体基准轴"].transform.right;
        }
        else
        {
            BodyEngle = -Pocket["主体基准轴"].transform.right;
        }
        //零件轴线的方向
        PartEngle = Pocket["零件基准轴"].transform.right;
        //计算两个向量之间的夹角弧度值
        Fai = Mathf.Acos(Vector3.Dot(BodyEngle.normalized, PartEngle.normalized));
        //计算旋转轴
        if (Fai == Mathf.PI)
        {
            RotAxle = Pocket["零件"].transform.forward;
        }
        else
        {
            RotAxle = Vector3.Cross(PartEngle, BodyEngle);
        }
        if (Fai * Mathf.Rad2Deg >= 2f)
        {
            float D = 1f;
            Pocket["零件"].GetComponent<Rigidbody>().AddTorque(RotAxle * Speed * 10f * Fai * Mathf.Rad2Deg);
            Pocket["零件"].GetComponent<Rigidbody>().angularDrag =
            Mathf.Abs(Fai * Mathf.Rad2Deg - Fai * Mathf.Rad2Deg) + (++D) * (++D) * 0.07f;
        }
        else
        {
            Pocket["零件"].GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
            Pocket["零件"].transform.right += BodyEngle - PartEngle;
            Pocket["零件"].GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
            Pocket["主体"].GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
            Pocket["零件"].GetComponent<Rigidbody>().drag = 0;
            Pocket["零件"].GetComponent<Rigidbody>().angularDrag = 0.05f;
            _Begin2Aline = false;
            StateControl = 2.21f;
        }
    }
    public void LineFowardOrReverse()
    {
        LineFowOrRev = !LineFowOrRev;
        if (LineFowOrRev)
        {
            LineForR.text = "正向对齐";
        }
        else
        {
            LineForR.text = "反向对齐";
        }
    }
    public void Begin2Aline()
    {
        _Begin2Aline = true;
    }
    /// <summary>
    /// 共轴
    /// </summary>
    public void Coaxial()
    {
        ///共轴前建议先进行轴对齐
        if (Pocket["主体基准轴"] != null && Pocket["零件基准轴"] != null
            && Pocket["零件"] != null)
        {
            Pocket["主体"].GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
            Vector3 Connection = Pocket["主体基准轴"].transform.position
                - Pocket["零件基准轴"].transform.position;
            //计算两个向量之间的夹角弧度值
            float Fai = Mathf.Acos(Vector3.Dot(Connection.normalized,
                Pocket["零件基准轴"].transform.right.normalized));
            //计算分量模长
            float Long = Connection.magnitude * Mathf.Cos(Fai);
            //求差得移动方向的向量
            Vector3 Way = Connection - Pocket["零件基准轴"].transform.right.normalized * Long;
            if (Way.magnitude >= 2f)
            {
                float D = 1f;
                Pocket["零件"].GetComponent<Rigidbody>().AddForce(Way * Speed * 0.5f);
                Part.GetComponent<Rigidbody>().drag = 1f + (++D);
            }
            if (Way.magnitude < 2f)
            {
                Pocket["零件"].GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
                Pocket["零件"].transform.position += Way;
                _Begin2Coaxial = false;
                Pocket["零件"].GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
                Pocket["主体"].GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
                Pocket["零件"].GetComponent<Rigidbody>().drag = 0;
                Pocket["零件"].GetComponent<Rigidbody>().angularDrag = 0.05f;
                StateControl = 2.22f;
            }
        }
    }
    public void Begin2Coaxial()
    {
        _Begin2Coaxial = true;
    }
    /// <summary>
    /// 面对齐
    /// </summary>
    public void FaceAlignment()
    {
        Vector3 BodyEngle;
        Vector3 PartEngle;
        float Fai;
        Vector3 RotAxle;
        Pocket["主体"].GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        if (FaceFowOrRev)
        {
            BodyEngle = Pocket["主体基准面"].transform.right;
        }
        else
        {
            BodyEngle = -Pocket["主体基准面"].transform.right;
            #region
            ////零件轴线的方向
            //Vector3 PartEngle = Pocket["零件基准面"].transform.right;
            ////计算两个向量之间的夹角弧度值
            //float Fai = Mathf.Acos(Vector3.Dot(BodyEngle.normalized, PartEngle.normalized));
            ////计算旋转轴
            //Vector3 RotAxle = -1f * Vector3.Cross(PartEngle.normalized, BodyEngle.normalized);
            ////旋转零件
            //if (Fai * Mathf.Rad2Deg >= 1f)
            //{
            //    Debug.Log(Fai);
            //    float D = 1f;
            //    Pocket["零件"].GetComponent<Rigidbody>().AddTorque(RotAxle * Fai * Mathf.Rad2Deg * Speed);
            //    Pocket["零件"].GetComponent<Rigidbody>().angularDrag =
            //        Mathf.Abs(Fai * Mathf.Rad2Deg - Fai * Mathf.Rad2Deg) + (++D) * (++D) * 0.07f;
            //}
            //if (Fai * Mathf.Rad2Deg < 1f)
            //{
            //    Pocket["零件"].GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
            //    //Pocket["零件"].transform.eulerAngles -= Pocket["主体基准面"].transform.eulerAngles
            //    //    - Pocket["零件基准面"].transform.eulerAngles;
            //    _Begin2Aplane = false;
            //    Pocket["零件"].GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
            //    Pocket["零件"].GetComponent<Rigidbody>().drag = 0;
            //    Pocket["零件"].GetComponent<Rigidbody>().angularDrag = 0.05f;
            //    MandRCounter = 0;
            //}
            #endregion
        }
        //零件轴线的方向
        PartEngle = Pocket["零件基准面"].transform.right;
        //计算两个向量之间的夹角弧度值
        Fai = Mathf.Acos(Vector3.Dot(BodyEngle.normalized, PartEngle.normalized));
        //计算旋转轴
        if (Fai == Mathf.PI)
        {
            RotAxle = Pocket["零件"].transform.forward;
            Debug.Log("零件反向180°");
        }
        else
        {
            RotAxle = Vector3.Cross(PartEngle, BodyEngle);
        }
        if (Fai * Mathf.Rad2Deg >= 2f)
        {
            float D = 1f;
            Pocket["零件"].GetComponent<Rigidbody>().AddTorque(RotAxle * Fai * 10f * Mathf.Rad2Deg * Speed);
            Pocket["零件"].GetComponent<Rigidbody>().angularDrag =
                Mathf.Abs(Fai * Mathf.Rad2Deg - Fai * Mathf.Rad2Deg) + (++D) * (++D) * 0.07f;
        }
        if (Fai * Mathf.Rad2Deg < 2f)
        {
            Pocket["零件"].GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
            Pocket["零件"].transform.right += BodyEngle - PartEngle;
            Pocket["零件"].GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
            Pocket["主体"].GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
            Pocket["零件"].GetComponent<Rigidbody>().drag = 0;
            Pocket["零件"].GetComponent<Rigidbody>().angularDrag = 0.05f;
            _Begin2Aplane = false;
            StateControl = 2.31f;
        }
    }
    public void FaceFowardOrReverse()
    {
        FaceFowOrRev = !FaceFowOrRev;
        if (FaceFowOrRev)
        {
            FaceForR.text = "正向对齐";
        }
        else
        {
            FaceForR.text = "反向对齐";
        }
    }
    public void Begin2Aplane()
    {
        _Begin2Aplane = true;
    }
    /// <summary>
    /// 共面
    /// </summary>
    public void Coplanar()
    {
        ///共面前建议先面对齐
        if (Pocket["主体基准面"] != null && Pocket["零件基准面"] != null
            && Pocket["零件"] != null)
        {
            Pocket["主体"].GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
            Vector3 Connection = Pocket["主体基准面"].transform.position
                - Pocket["零件基准面"].transform.position;
            //计算两个向量之间的夹角弧度值
            float Fai = Mathf.Acos(Vector3.Dot(Connection.normalized,
                Pocket["零件基准面"].transform.right.normalized));
            //计算分量模长
            float Long = Connection.magnitude * Mathf.Cos(Fai);
            //求差得移动方向的向量
            Vector3 Way = Pocket["零件基准面"].transform.right.normalized * Long;
            if (Way.magnitude >= 2f)
            {
                float D = 1f;
                Pocket["零件"].GetComponent<Rigidbody>().AddForce(Way * Speed * 0.5f);
                Part.GetComponent<Rigidbody>().drag = (++D);
            }
            if (Way.magnitude < 2f)
            {
                Pocket["零件"].GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
                Pocket["零件"].transform.position += Way;
                _Begin2Coplanar = false;
                Pocket["零件"].GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
                Pocket["主体"].GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
                Pocket["零件"].GetComponent<Rigidbody>().drag = 0;
                Pocket["零件"].GetComponent<Rigidbody>().angularDrag = 0.05f;
                StateControl = 2.32f;
            }
        }
    }
    public void Begin2Coplanar()
    {
        _Begin2Coplanar = true;
    }
    public void FinAssembly()
    {
        if (StateControl > 2.2f)
        {
            Pocket["零件"].AddComponent<FixedJoint>();
            Pocket["零件"].GetComponent<FixedJoint>().connectedBody
                = Pocket["主体"].GetComponent<Rigidbody>();
            Pocket["主体"].GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
            Initialized();
            Debug.Log("完成装配");
        }
    }
    public void Disassembly()
    {
        if (Click.ClickObj != null)
        {
            //Debug.Log(Click.ClickObj.name);
            if (Click.ClickObj.GetComponent<FixedJoint>() != null)
            {
                Destroy(Click.ClickObj.GetComponent<FixedJoint>());
                Debug.Log("??");
            }
            else
            {
                ///此零件没有装配关系
            }
        }
    }

}
