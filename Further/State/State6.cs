
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class State6 : MonoBehaviour
{
    public State5 St5;
    public State7 St7;
    public AssemblyCollection Collection;
    public ClickManager ClickObj;
    public Text text;
    public WholeTips Tip;

    public Vector3 InitialPos;
    public Vector3 InitialRot;

    public float Speed = 100f;
    public float StayDis = 500f;
    //对准总开关
    public bool Star = false;
    //选择左边对齐
    public bool M_Left = false;
    //选择右边对齐
    public bool M_Right = false;

    bool StarSt6 = false;
    bool Confirm = false;

    public GameObject B;
    char LeftOrRight;
    GameObject L;
    GameObject R;

    //bool ButtonR = false;
    // Start is called before the first frame update
    //void Start()
    //{
    //    Initialization();
    //}

    // Update is called once per frame
    void Update()
    {
        St6();
        //ResetSt();
    }

    public void St6()
    {
        LeftOrRight = UsingFunction.Family(Collection.Collection[1])[4]
                .GetComponent<PartInfo>().Order.ToCharArray()[0];
        StarRotate();
        StarAline();
        //空格开始对准装配
        Aline_Rot(Collection.GearAxis, Collection.PartAxis, Speed);
        Aline_Move(Collection.GearAxis, Collection.PartAxis, Speed);

        if (Aline_Rot(Collection.GearAxis, Collection.PartAxis, Speed) &&
            Aline_Move(Collection.GearAxis, Collection.PartAxis, Speed))
        {
            //装配完毕确认效果
            if (StarSt6)
            {
                //隐藏所有轴线体
                UsingFunction.HideAndAppear("Hide", 2, Collection.Collection[0], null);
                UsingFunction.HideAndAppear("Hide", 2, Collection.Collection[1], null);
                UsingFunction.HideAndAppear("Appear", 3, Collection.Collection[0], null);
                UsingFunction.HideAndAppear("Appear", 3, Collection.Collection[1], null);
                //Collection.Collection[0].GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
                Collection.Collection[1].GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
                St7.enabled = true;
                St7.Initialization();
                this.GetComponent<State6>().enabled = false;
            }
        }
        StarSt6 = false;
        Confirm = false;
        //对准完成后，发现位置不对，需要重新对准，复位零件

        CheckFace();
    }

    /// <summary>
    /// 查看零件装配轴的左右端面
    /// </summary>
    public void CheckFace()
    {
        foreach (Transform T in UsingFunction.Family(Collection.Collection[0])[3].transform)
        {
            if (T.localPosition == UsingFunction.Family(Collection.Collection[0])[4].GetComponent<AxleInfo>()
                .RightDatumFace)
            {
                R = T.transform.gameObject;
            }
            else if (T.localPosition == UsingFunction.Family(Collection.Collection[0])[4].
                GetComponent<AxleInfo>().LeftDatumFace)
            {
                L = T.transform.gameObject;
            }
        }
        if (UsingFunction.GetUI() != null)
        {
            if (UsingFunction.GetUI().tag == "Tips" && (UsingFunction.GetUI().name == "右端面"
                || UsingFunction.GetUI().name == "左端面"))
            {

                if (UsingFunction.GetUI().name == "左端面")
                {
                    UsingFunction.Hide_Translucent_Appear("Appear", L);
                }
                if (UsingFunction.GetUI().name == "右端面")
                {
                    UsingFunction.Hide_Translucent_Appear("Appear", R);
                }
            }
        }
        if (UsingFunction.GetUI() == null || UsingFunction.GetUI().tag == "Drag" || UsingFunction.GetUI().name == "提示框"
            || UsingFunction.GetUI().name == "取消" || UsingFunction.GetUI().name == "选择左右端面" ||
            UsingFunction.GetUI().name == "选择左右端面")
        {
            UsingFunction.Hide_Translucent_Appear("Hide", L);
            UsingFunction.Hide_Translucent_Appear("Hide", R);
        }
    }

    public void ResetSt()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            ClickObj.ClickObj = null;
            ClickObj.ClickObjFamily = null;
            //对齐完毕
            //复位已经旋转的零件
            //恢复所有bool值
            //此时拾取器上显示选择的轴，零件，选择的两个轴线

            //R1重新选择左右端面
            if (Aline_Move(Collection.GearAxis, Collection.PartAxis, Speed) &&
                Aline_Rot(Collection.GearAxis, Collection.PartAxis, Speed))
            {
                //显示已经选中的轴和零件的轴线体
                Collection.GearAxis.GetComponent<MeshRenderer>().enabled = true;
                Collection.PartAxis.GetComponent<MeshRenderer>().enabled = true;
                Collection.GearAxis.GetComponent<BoxCollider>().enabled = true;
                Collection.PartAxis.GetComponent<BoxCollider>().enabled = true;
                //复位零件
                Collection.Collection[1].transform.position = InitialPos;
                Collection.Collection[1].transform.eulerAngles = InitialRot;
                M_Left = false;
                M_Right = false;
                Star = false;
            }
            //零件处于旋转中或者完全没开始旋转
            //复位
            //回退St5
            //初始化St5

            //R2回退St5重选零件对准的自身基准轴
            if (!Aline_Move(Collection.GearAxis, Collection.PartAxis, Speed)
                && !Aline_Rot(Collection.GearAxis, Collection.PartAxis, Speed))
            {
                M_Left = false;
                M_Right = false;
                Star = false;
                Collection.Collection[1].transform.position = InitialPos;
                Collection.Collection[1].transform.eulerAngles = InitialRot;
                St5.enabled = true;
                St5.Initialization();
                this.GetComponent<State6>().enabled = false;
            }
            //复位零件
            //开始旋转，直到选择左右端面的步骤

            //R3零件已经旋转完毕，但需要重新选择左右端面
            if (!Aline_Move(Collection.GearAxis, Collection.PartAxis, Speed)
                && Aline_Rot(Collection.GearAxis, Collection.PartAxis, Speed))
            {
                Collection.Collection[1].transform.position = InitialPos;
                Collection.Collection[1].transform.eulerAngles = InitialRot;
                M_Right = false;
                M_Left = false;
            }
        }
    }
    public void Initialization()
    {
        St5 = this.GetComponent<State5>();
        St7 = this.GetComponent<State7>();
        ClickObj = this.GetComponent<ClickManager>();
        Collection = this.GetComponent<AssemblyCollection>();
        Tip = GameObject.Find("UIControler").GetComponent<WholeTips>();

        //显示已经选中的轴和零件的轴线体
        Collection.GearAxis.GetComponent<MeshRenderer>().enabled = true;
        Collection.PartAxis.GetComponent<MeshRenderer>().enabled = true;
        Collection.GearAxis.GetComponent<BoxCollider>().enabled = true;
        Collection.PartAxis.GetComponent<BoxCollider>().enabled = true;

        M_Left = false;
        M_Right = false;
        Star = false;
        StarSt6 = false;
        Confirm = false;
        InitialPos = Collection.Collection[1].transform.position;
        InitialRot = Collection.Collection[1].transform.eulerAngles;

        if (ClickObj.ClickObj != null)
        {
            ClickObj.ClickObj = null;
            ClickObj.ClickObjFamily = null;
        }
        else { }
        Tip.InputTips("Tips", "零件对准装配轴线后\r\n选择从轴的左或右端面装配", 2);
    }

    public void StarRotate()
    {
        if (Confirm)
        {
            Star = true;
        }
    }

    public void StarAline()
    {
        if (Input.GetKeyDown(KeyCode.Z) && Star)
        {
            M_Left = true;
            M_Right = false;
        }
        if (Input.GetKeyDown(KeyCode.Y) && Star)
        {
            M_Right = true;
            M_Left = false;
        }
    }

    public void ChooseLeft()
    {
        if (LeftOrRight == '右')
        {
            Tip.InputTips("Caution", "建议按照装配方案，选择右端面为装配基准面\n可打开操作面板查看零件装配信息", 2);
        }
        M_Left = true;
        M_Right = false;
    }

    public void ChooseRight()
    {
        if (LeftOrRight == '左')
        {
            Tip.InputTips("Caution", "建议按照装配方案，选择左端面为装配基准面\n可打开操作面板查看零件装配信息", 2);
        }
        M_Right = true;
        M_Left = false;
    }

    //传入两个轴
    public bool Aline_Rot(GameObject Gear, GameObject Part, float Speed)
    {
        //需要获取装配轴的安装的轴的位置，角度
        //需要获取零件总体的旋转角度和位置
        //零件与安装轴线角度对齐
        //零件与安装轴线位置对准
        //重复解锁，冻结约束
        //传入轴的轴线体和零件主体
        //首先将零件的轴线体赋值给A，再替换为零件本体
        //从而获取了轴线体，零件轴线体和零件本体（需要移动的物体）

        if (Star)
        {
            Tip.InputTips("Tips", "正在对准", 2);
            //点击旋转按钮
            //装载零件轴线体到A
            //替换零件本体
            //解锁零件本体
            GameObject A = Part;
            Part = UsingFunction.Family(Part)[0];
            Part.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
            //返回值判断是否完成旋转对准
            //r前提已经开始旋转对准
            bool r = false;

            //对准轴轴线线的方向
            Vector3 GearEngle = Gear.transform.right;
            //装配零件轴线的方向
            Vector3 PartEngle = A.transform.right;
            //计算两个向量之间的夹角弧度值
            float Fai = Mathf.Acos(Vector3.Dot(GearEngle.normalized, PartEngle.normalized));
            //计算旋转轴
            Vector3 RotAxle = Vector3.Cross(PartEngle.normalized, GearEngle.normalized);
            //旋转零件
            if (Fai * Mathf.Rad2Deg >= 2f)
            {
                float D = 1f;
                Part.GetComponent<Rigidbody>().AddTorque(RotAxle * Fai * Mathf.Rad2Deg * Speed);
                Part.GetComponent<Rigidbody>().angularDrag =
                    Mathf.Abs(Fai * Mathf.Rad2Deg - Fai * Mathf.Rad2Deg) + (++D) * (++D) * 0.07f;

            }
            if (Fai * Mathf.Rad2Deg < 2f)
            {
                Part.transform.eulerAngles = Gear.transform.eulerAngles;
                r = true;
                Tip.InputTips("Tips", "对准完毕，选择左右端面进行装配", 2);
            }
            return r;
        }
        else { return false; }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="Gear">GearAxis</param>
    /// <param name="Part">PartAxis</param>
    /// <param name="Speed"></param>
    /// <returns></returns>
    public bool Aline_Move(GameObject Gear, GameObject Part, float Speed)
    {
        if (M_Left && Star && Aline_Rot(Collection.GearAxis, Collection.PartAxis, Speed))
        {

            bool M = false;
            //获取零件中心到端面的距离
            float Dis;
            Vector3 Dir;
            //装载和解锁
            GameObject A = Part;
            Part = UsingFunction.Family(Part)[0];
            Part.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
            B = L;
            Dis = (B.transform.position - Collection.Collection[1].transform.position
                - Collection.Collection[0].transform.right.normalized * StayDis).magnitude;
            //Debug.Log(Dis);
            //当距离大于阈值，执行操作
            if (Dis >= 5f)
            {
                //Dir = (Gear.gameObject.transform.position - A.gameObject.transform.position -
                //    Gear.gameObject.transform.right.normalized * StayDis);
                Dir = (B.transform.position - Collection.Collection[1].transform.position
                - Collection.Collection[0].transform.right.normalized * StayDis);
                float D = 1f;
                Part.GetComponent<Rigidbody>().AddForce(Dir.normalized * Dir.magnitude * Speed * 0.5f);
                Part.GetComponent<Rigidbody>().drag = 1f + (++D);
                //Tip.InputTips("Tips", "正在对准，请稍等", 2);
            }
            if (Dis < 5f)
            {
                //Part.gameObject.transform.position = Gear.gameObject.transform.position
                //    - Gear.gameObject.transform.right.normalized * StayDis;
                Collection.Collection[1].transform.position = B.transform.position -
                    Collection.Collection[0].transform.right.normalized * StayDis;
                M = true;
                Tip.InputTips("Tips", "对准完毕", 2);
            }
            return M;
        }
        if (M_Right && Star && Aline_Rot(Collection.GearAxis, Collection.PartAxis, Speed))
        {
            //if (LeftOrRight == '左')
            //{
            //    text.text = ("错误，应选择\n左端面为装配基准面");
            //}
            //else
            //{
            //    text.text = ("选择正确");
            //}

            bool M = false;
            //获取零件中心到端面的距离
            float Dis;
            Vector3 Dir;
            //装载和解锁
            GameObject A = Part;
            Part = UsingFunction.Family(Part)[0];
            Part.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
            //求出零件到轴左边的对准位置的距离
            //Dis = (Gear.gameObject.transform.position - A.gameObject.transform.position +
            //    Gear.gameObject.transform.right.normalized * StayDis).magnitude;

            // = ((GameObject)UsingFunction.Family(Collection.Collection[0])[4].
            //    GetComponent<AxleInfo>().ShaftInfo["右端面"]);
            //foreach (Transform k in UsingFunction.Family(Collection.Collection[0])[3].transform)
            //{
            //    if (k.localPosition == UsingFunction.Family(Collection.Collection[0])[4].GetComponent<AxleInfo>
            //        ().RightDatumFace)
            //    {
            //        B = k.transform.gameObject;
            //    }
            //}
            B = R;
            Dis = (B.transform.position - Collection.Collection[1].transform.position
                + Collection.Collection[0].transform.right.normalized * StayDis).magnitude;
            #region
            //当距离大于阈值，执行操作
            //if (Dis >= 0.1f)
            //{
            //    //Dir = (Gear.gameObject.transform.position - A.gameObject.transform.position +
            //    //    Gear.gameObject.transform.right.normalized * StayDis);
            //    Dir = (B.transform.position - Collection.Collection[1].transform.position
            //    + Collection.Collection[0].transform.right.normalized * StayDis);
            //    if (Dir.magnitude > 1f)
            //    {
            //        float D = 1f;
            //        Part.gameObject.GetComponent<Rigidbody>().AddForce(Dir.normalized * Dir.magnitude * Speed * 0.5f);
            //        Part.gameObject.GetComponent<Rigidbody>().drag = 1f + (++D);
            //    }
            //    if (Dir.magnitude <= 1f)
            //    {
            //        Collection.Collection[1].transform.position = B.transform.position +
            //        Collection.Collection[0].transform.right.normalized * StayDis;
            //        M = true;
            //    }
            //}
            //if (Dis < 0.1f)
            //{
            //    Part.gameObject.transform.position = Gear.gameObject.transform.position +
            //        Gear.gameObject.transform.right.normalized * StayDis;
            //    M = true;
            //}
            #endregion
            if (Dis >= 5f)
            {
                Dir = (B.transform.position - Collection.Collection[1].transform.position
                + Collection.Collection[0].transform.right.normalized * StayDis);
                float D = 1f;
                Part.GetComponent<Rigidbody>().AddForce(Dir.normalized * Dir.magnitude * Speed * 0.5f);
                Part.GetComponent<Rigidbody>().drag = 1f + (++D);
            }
            if (Dis < 5f)
            {
                Collection.Collection[1].transform.position = B.transform.position +
                    Collection.Collection[0].transform.right.normalized * StayDis;
                M = true;
                Tip.InputTips("Tips", "对准完毕", 2);
            }
            return M;
        }
        else
        {
            return false;
        }
    }
    /// <summary>
    /// 完成St6确认
    /// </summary>
    public void ButtonClick()
    {
        StarSt6 = true;
    }
    /// <summary>
    /// 开始对齐
    /// </summary>
    public void ButtonConfirm()
    {
        Confirm = true;
    }

    /// <summary>
    /// 已经选择左右端面并且对准完毕，重新选择左右端面
    /// </summary>
    public void R1()
    {
        ClickObj.ClickObj = null;
        ClickObj.ClickObjFamily = null;
        //if (Aline_Move(Collection.GearAxis, Collection.PartAxis, Speed) &&
        //Aline_Rot(Collection.GearAxis, Collection.PartAxis, Speed))
        {
            //显示已经选中的轴和零件的轴线体
            Collection.GearAxis.GetComponent<MeshRenderer>().enabled = true;
            Collection.PartAxis.GetComponent<MeshRenderer>().enabled = true;
            Collection.GearAxis.GetComponent<BoxCollider>().enabled = true;
            Collection.PartAxis.GetComponent<BoxCollider>().enabled = true;
            //复位零件
            Collection.Collection[1].transform.position = InitialPos;
            Collection.Collection[1].transform.eulerAngles = InitialRot;
            M_Left = false;
            M_Right = false;
            Star = false;
        }
        Tip.InputTips("Tips", "重新选择左右端面进行装配", 2);
    }
    /// <summary>
    /// 直接回退St5重选零件对准的自身基准轴
    /// </summary>
    public void R2()
    {
        ClickObj.ClickObj = null;
        ClickObj.ClickObjFamily = null;
        M_Left = false;
        M_Right = false;
        Star = false;
        Collection.Collection[1].transform.position = InitialPos;
        Collection.Collection[1].transform.eulerAngles = InitialRot;
        St5.enabled = true;
        St5.Initialization();
        this.GetComponent<State6>().enabled = false;
        if (!Aline_Move(Collection.GearAxis, Collection.PartAxis, Speed)
                && !Aline_Rot(Collection.GearAxis, Collection.PartAxis, Speed))
        {
            M_Left = false;
            M_Right = false;
            Star = false;
            Collection.Collection[1].gameObject.transform.position = InitialPos;
            Collection.Collection[1].gameObject.transform.eulerAngles = InitialRot;
            St5.enabled = true;
            St5.Initialization();
            this.GetComponent<State6>().enabled = false;
        }
    }
    /// <summary>
    /// 已经对准未选择左右端面，复位零件重新对准
    /// </summary>
    public void R3()
    {
        ClickObj.ClickObj = null;
        ClickObj.ClickObjFamily = null;
        Collection.Collection[1].transform.position = InitialPos;
        Collection.Collection[1].transform.eulerAngles = InitialRot;
        M_Right = false;
        M_Left = false;
        if (!Aline_Move(Collection.GearAxis, Collection.PartAxis, Speed)
                && Aline_Rot(Collection.GearAxis, Collection.PartAxis, Speed))
        {
            Collection.Collection[1].transform.position = InitialPos;
            Collection.Collection[1].transform.eulerAngles = InitialRot;
            M_Right = false;
            M_Left = false;
        }
        Tip.InputTips("Tips", "重新对准零件", 2);
    }
}
