using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ForkAssembly : MonoBehaviour
{
    public ClickManager ClickObj;
    public GameObject AssemblyWithShaft;
    public GameObject AssemblyWithSleeve;

    //选择变量
    public GameObject Fork;
    public GameObject ShaftOrPart;
    public Text ForkName;
    public Text ShaftOrPartName;
    public bool ChooseFork = false;
    public bool ChooseShaftOrPart = false;
    public bool DeletFork = false;
    public bool DeletShaftOrPart = false;
    public GameObject ForkAxle;
    public GameObject ShaftOrPartAxle;
    public GameObject ForkFace;
    public GameObject ShaftOrPartFace;
    public Text ForkAxleName;
    public Text ShaftOrPartAxleName;
    public Text ForkFaceName;
    public Text ShaftOrPartFaceName;
    public bool ChooseForkAxle = false;
    public bool ChooseShaftOrPartAxle = false;
    public bool DeletForkAxle = false;
    public bool DeletShaftOrPartAxle = false;
    public bool ChooseForkFace = false;
    public bool ChooseShaftOrPartFace = false;
    public bool DeletForkFace = false;
    public bool DeletShaftOrPartFace = false;


    //操作变量
    public GameObject StaticPart;
    public GameObject MovePart;
    public GameObject StaticPartAxle;
    public GameObject MovePartAxle;
    public GameObject StaticPartFace;
    public GameObject MovePartFace;
    public bool ConfirmChoosing = false;
    public float Speed = 100f;
    public Vector3 BeforeAline_Pos = Vector3.zero;
    public Vector3 BeforeAline_Rot = Vector3.zero;
    public Vector3 BeforeCoplanar_Pos = Vector3.zero;
    public Vector3 BeforeCoplanar_Rot = Vector3.zero;
    public GameObject SR;
    public GameObject SL;
    public GameObject MR;
    public GameObject ML;

    public bool StarForkAline = false;
    public bool StarForkCoplanar = false;
    public bool StarForkAssemble = false;
    public bool StarShaftAline = false;
    public bool StarShaftCoplanar = false;
    public bool StarShaftAssemble = false;
    public bool FinAssemble = false;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        ForkChoosing();
        ShaftOrPartChoosing();
        AppearSettingPanel();
        AppearAndChoose();
        ForkAxleChoosing();
        ShaftOrPartAxleChoosing();
        ForkFaceChoosing();
        ShaftOrPartFaceChoosing();
        SortMoveAndStatic();
        ChooseDatum();

        ForkAline();
        ForkCoplanar();
        ForkAssemble();
        ShaftAline();
        ShaftCoplanar();
        //Debug.Log(UsingFunction.Family(StaticPart)[4].GetComponent<PartInfo>().AssembleInfo.Count);
        //Debug.Log(UsingFunction.Family(StaticPart)[4].GetComponent<PartInfo>().SelfInfo.Count);
        //Debug.Log(UsingFunction.Family(MovePart)[4].GetComponent<AxleInfo>().ShaftInfo.Count);
        //Debug.Log(MovePart.GetComponent<PartInfo>().AssembleInfo.Count);
    }

    //设置流程内引起的变化有，变量赋值，零件移动，零件自由度锁定，零件基准显示,面板显示
    public void Initialization()
    {
        if (StaticPart != null && MovePart != null && BeforeAline_Pos != Vector3.zero
            && BeforeAline_Rot != Vector3.zero)
        {
            MovePart.transform.position = BeforeAline_Pos;
            MovePart.transform.eulerAngles = BeforeAline_Rot;
            UsingFunction.HideAndAppear("Hide", 2, MovePart, null);
            UsingFunction.HideAndAppear("Hide", 2, StaticPart, null);
            UsingFunction.HideAndAppear("Hide", 3, MovePart, null);
            UsingFunction.HideAndAppear("Hide", 3, StaticPart, null);
            StaticPart.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
            MovePart.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
            Debug.Log("1");
            
        }
        if (Fork != null && ShaftOrPart != null)
        {
            UsingFunction.HideAndAppear("Hide", 2, Fork, null);
            UsingFunction.HideAndAppear("Hide", 2, ShaftOrPart, null);
            UsingFunction.HideAndAppear("Hide", 3, Fork, null);
            UsingFunction.HideAndAppear("Hide", 3, ShaftOrPart, null);
            Debug.Log("2");
        }

        Fork = null;
        ShaftOrPart = null;
        ChooseFork = false;
        ChooseShaftOrPart = false;
        DeletFork = false;
        DeletShaftOrPart = false;
        ForkAxle = null;
        ShaftOrPartAxle = null;
        ForkFace = null;
        ShaftOrPartFace = null;
        ChooseForkAxle = false;
        ChooseShaftOrPartAxle = false;
        DeletForkAxle = false;
        DeletShaftOrPartAxle = false;
        ChooseForkFace = false;
        ChooseShaftOrPartFace = false;
        DeletForkFace = false;
        DeletShaftOrPartFace = false;
        StaticPart = null;
        MovePart = null;
        StaticPartAxle = null;
        MovePartAxle = null;
        StaticPartFace = null;
        MovePartFace = null;
        ConfirmChoosing = false;
        StarForkAline = false;
        StarForkCoplanar = false;
        StarForkAssemble = false;
        StarShaftAline = false;
        StarShaftCoplanar = false;
        StarShaftAssemble = false;
        FinAssemble = false;
        AssemblyWithShaft.gameObject.SetActive(false);
        AssemblyWithSleeve.gameObject.SetActive(false);
        BeforeAline_Pos = Vector3.zero;
        BeforeAline_Rot = Vector3.zero;
        BeforeCoplanar_Pos = Vector3.zero;
        BeforeCoplanar_Rot = Vector3.zero;

    }


    //拾取零件
    public void Button_ChooseFork()
    {
        ChooseFork = true;
    }

    public void ForkChoosing()
    {
        if (ChooseFork && ClickObj.ClickObj != null)
        {
            Fork = ClickObj.ClickObj;
            ForkName.text = Fork.name;

            ChooseFork = false;
        }
        if (DeletFork && Fork != null)
        {
            UsingFunction.HideAndAppear("Hide", 2, Fork, null);
            UsingFunction.HideAndAppear("Hide", 3, Fork, null);
            Fork = null;
            ForkName.text = "";
            //ClickObj.Initialization();
            DeletFork = false;
        }
    }

    public void Button_DeletFork()
    {

        DeletFork = true;
    }

    public void Button_ChooseShaftOrPart()
    {
        ChooseShaftOrPart = true;
    }

    public void ShaftOrPartChoosing()
    {
        if (ChooseShaftOrPart && ClickObj.ClickObj != null)
        {
            ShaftOrPart = ClickObj.ClickObj;
            ShaftOrPartName.text = ShaftOrPart.name;
            ChooseShaftOrPart = false;
        }
        if (DeletShaftOrPart && ShaftOrPart != null)
        {
            UsingFunction.HideAndAppear("Hide", 2, ShaftOrPart, null);
            UsingFunction.HideAndAppear("Hide", 3, ShaftOrPart, null);
            ShaftOrPart = null;
            ShaftOrPartName.text = "";
            //ClickObj.Initialization();
            DeletShaftOrPart = false;
        }
    }

    public void Button_DeletShaftOrPart()
    {

        DeletShaftOrPart = true;
    }

    /// <summary>
    /// 根据选择零件类型，显示对应的操作面板
    /// </summary>
    public void AppearSettingPanel()
    {
        if (Fork != null && ShaftOrPart != null && ShaftOrPart.tag == "装配零件")
        {
            AssemblyWithSleeve.SetActive(true);
            AssemblyWithShaft.SetActive(false);
        }
        if (Fork != null && ShaftOrPart != null && ShaftOrPart.tag == "拨叉轴")
        {
            AssemblyWithSleeve.SetActive(false);
            AssemblyWithShaft.SetActive(true);
        }
    }

    /// <summary>
    /// 装入临时数据，显示基准要素
    /// </summary>
    public void AppearAndChoose()
    {
        //初始选择
        if (Fork != null && ShaftOrPart != null && ForkAxle == null && ForkFace == null
            && ShaftOrPartFace == null && ShaftOrPartAxle == null)
        {
            UsingFunction.HideAndAppear("Appear", 2, Fork, null);
            UsingFunction.HideAndAppear("Appear", 2, ShaftOrPart, null);
            UsingFunction.HideAndAppear("Appear", 3, Fork, null);
            UsingFunction.HideAndAppear("Appear", 3, ShaftOrPart, null);
        }
    }

    //选择并隐藏基准
    //public void ChooseAndHide()
    //{
    //    UsingFunction.HideAndAppear("Hide", 2, StaticPart, StaticPartAxle);
    //    UsingFunction.HideAndAppear("Hide", 2, MovePart, MovePartAxle);
    //    UsingFunction.HideAndAppear("Hide", 3, StaticPart, StaticPartFace);
    //    UsingFunction.HideAndAppear("Hide", 3, MovePart, MovePartFace);
    //}

    //拾取基准（需要添加判定）
    public void Button_ChooseForkAxle()
    {
        ChooseForkAxle = true;
    }

    public void ForkAxleChoosing()
    {
        //if (ForkAxle == null && ClickObj.ClickObj == null)
        //{
        //    ForkAxleName.text = "";
        //}
        //if (ForkAxle == null && ClickObj.ClickObj != null)
        //{
        //    ForkAxleName.text = ClickObj.ClickObj.name;
        //}
        if (ChooseForkAxle && ClickObj.ClickObj != null && ClickObj.ClickObj.tag == "装配基准轴")
        {
            ForkAxle = ClickObj.ClickObj;
            ForkAxleName.text = ForkAxle.name;
            ChooseForkAxle = false;
        }
        if (DeletForkAxle && ForkAxle != null)
        {
            ForkAxle = null;
            ForkAxleName.text = "";
            //ClickObj.Initialization();
            DeletForkAxle = false;
        }
    }

    public void Button_DeletForkAxle()
    {
        DeletForkAxle = true;
    }

    public void Button_ChooseForkFace()
    {
        ChooseForkFace = true;
    }

    public void ForkFaceChoosing()
    {
        if (ChooseForkFace && ClickObj.ClickObj != null && ClickObj.ClickObj.tag == "装配基准面")
        {
            ForkFace = ClickObj.ClickObj;
            ForkFaceName.text = ForkFace.name;
            ChooseForkFace = false;
        }
        if (DeletForkFace && ForkFace != null)
        {
            ForkFace = null;
            ForkFaceName.text = "";
            //ClickObj.Initialization();
            DeletForkFace = false;
        }
    }

    public void Button_DeletForkFace()
    {
        DeletForkFace = true;
    }

    public void Button_ChooseShaftOrPartAxle()
    {
        ChooseShaftOrPartAxle = true;
    }

    public void ShaftOrPartAxleChoosing()
    {
        //if (ShaftOrPartAxle == null && ClickObj.ClickObj == null)
        //{
        //    ShaftOrPartName.text = "";
        //}
        //if (ShaftOrPartAxle == null && ClickObj.ClickObj != null)
        //{
        //    ShaftOrPartName.text = ClickObj.ClickObj.name;
        //}
        if (ChooseShaftOrPartAxle && ClickObj.ClickObj != null && ClickObj.ClickObj.tag == "装配基准轴")
        {
            ShaftOrPartAxle = ClickObj.ClickObj;
            ShaftOrPartAxleName.text = ShaftOrPartAxle.name;
            ChooseShaftOrPartAxle = false;
        }
        if (DeletShaftOrPartAxle && ShaftOrPartAxle != null)
        {
            ShaftOrPartAxle = null;
            ShaftOrPartAxleName.text = "";
            //ClickObj.Initialization();
            DeletShaftOrPartAxle = false;
        }
    }

    public void Button_DeletShaftOrPartAxle()
    {
        DeletShaftOrPartAxle = true;
    }

    public void Button_ChooseShaftOrPartFace()
    {
        ChooseShaftOrPartFace = true;
    }

    public void ShaftOrPartFaceChoosing()
    {
        if (ChooseShaftOrPartFace && ClickObj.ClickObj != null && ClickObj.ClickObj.tag == "装配基准面")
        {
            ShaftOrPartFace = ClickObj.ClickObj;
            ShaftOrPartFaceName.text = ShaftOrPartFace.name;
            ChooseShaftOrPartFace = false;
        }
        if (DeletShaftOrPartFace && ShaftOrPartFace != null)
        {
            ShaftOrPartFace = null;
            ShaftOrPartFaceName.text = "";
            //ClickObj.Initialization();
            DeletShaftOrPartFace = false;
        }
    }

    public void Button_DeletShaftOrPartFace()
    {
        DeletShaftOrPartFace = true;
    }

    public void Button_ConfirmChoosing()
    {
        ConfirmChoosing = true;
    }

    /// <summary>
    /// 所有零件要素选择完毕，点击确认装入数据后自动分类筛选
    /// </summary>
    public void SortMoveAndStatic()
    {
        if (Fork != null && ShaftOrPart != null && ConfirmChoosing)
        {
            //拨叉轴插入拨叉孔
            if (ShaftOrPart.tag == "拨叉轴")
            {
                StaticPart = Fork;
                MovePart = ShaftOrPart;
                StaticPartAxle = ForkAxle;
                MovePartAxle = ShaftOrPartAxle;
                StaticPartFace = ForkFace;
                MovePartFace = ShaftOrPartFace;
                Fork = null;
                ShaftOrPart = null;
                ForkAxle = null;
                ShaftOrPartAxle = null;
                ForkFace = null;
                ShaftOrPartFace = null;
            }
            //拨叉插入啮合套
            else if (ShaftOrPart.tag == "装配零件")
            {
                StaticPart = ShaftOrPart;
                MovePart = Fork;
                StaticPartAxle = ShaftOrPartAxle;
                MovePartAxle = ForkAxle;
                StaticPartFace = ShaftOrPartFace;
                MovePartFace = ForkFace;
                Fork = null;
                ShaftOrPart = null;
                ForkAxle = null;
                ShaftOrPartAxle = null;
                ForkFace = null;
                ShaftOrPartFace = null;
            }
            ConfirmChoosing = false;
        }

    }

    /// <summary>
    /// 筛选分类后的零件及基准自动隐藏基准轴
    /// </summary>
    public void ChooseDatum()
    {
        if (StaticPartAxle == null && StaticPart != null)
        {
            UsingFunction.HideAndAppear("Appear", 2, StaticPart, null);
        }
        if (StaticPartFace == null && StaticPart != null)
        {
            UsingFunction.HideAndAppear("Appear", 3, StaticPart, null);
        }
        if (MovePartAxle == null && MovePart != null)
        {
            UsingFunction.HideAndAppear("Appear", 2, MovePart, null);
        }
        if (MovePartFace == null && MovePart != null)
        {
            UsingFunction.HideAndAppear("Appear", 3, MovePart, null);
        }
        //装入确认后隐藏
        if (StaticPartAxle != null && StaticPart != null)
        {
            UsingFunction.HideAndAppear("Hide", 2, StaticPart, null);
        }
        if (StaticPartFace != null && StaticPart != null)
        {
            UsingFunction.HideAndAppear("Hide", 3, StaticPart, null);
        }
        if (MovePartAxle != null && MovePart != null)
        {
            UsingFunction.HideAndAppear("Hide", 2, MovePart, null);
        }
        if (MovePartFace != null && MovePart != null)
        {
            UsingFunction.HideAndAppear("Hide", 3, MovePart, null);
        }
    }

    /// <summary>
    /// 拨叉与啮合套装配专用，轴线平行
    /// </summary>
    public void Button_StarForkAline()
    {
        StarForkAline = true;
        BeforeAline_Pos = MovePart.transform.position;
        BeforeAline_Rot = MovePart.transform.eulerAngles;
    }

    /// <summary>
    /// 拨叉对准啮合套
    /// </summary>
    public void ForkAline()
    {
        if (MovePart != null && StaticPart != null && MovePart.GetComponent<Rigidbody>() != null && StarForkAline
            && StaticPart.GetComponent<Rigidbody>() != null && MovePartAxle != null && StaticPartAxle != null)
        {
            //先解锁零件
            MovePart.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;

            //对准轴轴线线的方向
            Vector3 TargetEngle = StaticPartAxle.transform.right;
            //装配零件轴线的方向
            Vector3 ChangeEngle = MovePartAxle.transform.right;
            //计算两个向量之间的夹角弧度值
            float Fai = Mathf.Acos(Vector3.Dot(TargetEngle.normalized, ChangeEngle.normalized));
            //计算旋转轴
            Vector3 RotAxle = Vector3.Cross(ChangeEngle.normalized, TargetEngle.normalized);
            //旋转零件
            if (Fai * Mathf.Rad2Deg >= 2f)
            {
                float D = 1f;
                MovePart.GetComponent<Rigidbody>().AddTorque(RotAxle * Fai * Mathf.Rad2Deg * Speed * 10f);
                MovePart.GetComponent<Rigidbody>().angularDrag =
                    Mathf.Abs(Fai * Mathf.Rad2Deg - Fai * Mathf.Rad2Deg) + (++D) * (++D);
            }
            if (Fai * Mathf.Rad2Deg < 2f && Fai * Mathf.Rad2Deg > 0f)
            {
                MovePart.transform.eulerAngles += TargetEngle - ChangeEngle;
                //固定RY,RZ
                MovePart.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationY
                    | RigidbodyConstraints.FreezeRotationZ;
                //MovePart.transform.right = StaticPart.transform.right;
            }
        }
    }

    public void Button_CancelForkAline()
    {
        MovePart.transform.position = BeforeAline_Pos;
        MovePart.transform.eulerAngles = BeforeAline_Rot;
        StarForkAline = false;
    }

    /// <summary>
    /// 拨叉与啮合套装配专用，控制拨叉插入啮合套
    /// </summary>
    public void Button_StarForkCoplanar()
    {
        StarForkCoplanar = true;
        BeforeCoplanar_Pos = MovePart.transform.position;
        BeforeCoplanar_Rot = MovePart.transform.eulerAngles;
    }

    /// <summary>
    /// 拨叉与啮合套装配完结点
    /// </summary>
    public void ForkCoplanar()
    {
        if (StarForkCoplanar)
        {
            float Dis;
            Vector3 Dir;
            //求出零件到轴端面的对准位置的距离
            Dis = (StaticPartFace.transform.position - MovePartFace.transform.position + new Vector3(0, 200, 0)).magnitude;
            //当距离大于阈值，执行操作
            if (Dis >= 2f)
            {
                Dir = StaticPartFace.transform.position - MovePartFace.transform.position + new Vector3(0, 200, 0);//new Vector3(Dis, StaticPartFace.transform.position.y+100f, 0)
                MovePart.GetComponent<Rigidbody>().AddForce(Dir.normalized * Dir.magnitude * Speed);
                MovePart.GetComponent<Rigidbody>().drag = 5f;
            }
            if (Dis < 2f)
            {
                MovePart.transform.position += StaticPartFace.transform.position + new Vector3(0, 200, 0)
                    - MovePartFace.transform.position;
                //锁轴，防止装配过程发生碰撞偏移
                //固定RY,RZ
                MovePart.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationY
                    | RigidbodyConstraints.FreezeRotationZ;
                //StarForkCoplanar = false;
            }
        }
    }

    public void Button_CancelForkCoplanar()
    {
        MovePart.transform.position = BeforeCoplanar_Pos;
        MovePart.transform.eulerAngles = BeforeCoplanar_Rot;
        StarForkCoplanar = false;
    }

    public void Button_StarForkAssemble()
    {
        StarForkCoplanar = false;
        StarForkAssemble = true;
    }

    /// <summary>
    /// 拨叉与啮合套装配
    /// </summary>
    public void ForkAssemble()
    {
        if (StarForkAssemble)
        {
            MovePart.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
            float Dis;
            Vector3 Dir;
            //求出零件到轴端面的对准位置的距离
            Dis = (StaticPartFace.transform.position - MovePartFace.transform.position).magnitude;
            //当距离大于阈值，执行操作
            if (Dis >= 5f)
            {
                Dir = StaticPartFace.transform.position - MovePartFace.transform.position;

                MovePart.GetComponent<Rigidbody>().AddForce(Dir.normalized * Dir.magnitude * Speed * 0.5f);// * 5f
                MovePart.GetComponent<Rigidbody>().drag = 5f;
            }
            if (Dis < 5f)
            {
                MovePart.transform.position += StaticPartFace.transform.position
                    - MovePartFace.transform.position;
                FinAssemble = true;
            }
        }
    }

    public void Button_CancelForkAssemble()
    {
        MovePart.transform.position = BeforeAline_Pos;
        MovePart.transform.eulerAngles = BeforeAline_Rot;
        StarForkAssemble = false;
    }

    /// <summary>
    /// 拨叉与拨叉轴装配专用，控制拨叉轴插入拨叉孔
    /// </summary>
    public void Button_StarShaftAline()
    {
        StarShaftAline = true;
        BeforeAline_Pos = MovePart.transform.position;
        BeforeAline_Rot = MovePart.transform.eulerAngles;

    }
    /// <summary>
    /// 拨叉轴插入拨叉孔中，按照先共轴，再移动到对准位置顺序执行
    /// </summary>
    public void ShaftAline()
    {
        if (StarShaftAline)
        {
            //对准轴轴线线的方向
            Vector3 TargetEngle = StaticPartAxle.transform.right;
            //装配零件轴线的方向
            Vector3 PartEngle = MovePartAxle.transform.right;
            //计算两个向量之间的夹角弧度值
            float Fai = Mathf.Acos(Vector3.Dot(TargetEngle.normalized, PartEngle.normalized));
            //计算旋转轴
            Vector3 RotAxle = Vector3.Cross(PartEngle.normalized, TargetEngle.normalized);
            //旋转零件
            if (Fai * Mathf.Rad2Deg >= 5f)
            {
                float D = 1f;
                MovePart.GetComponent<Rigidbody>().AddTorque(RotAxle * Fai * Mathf.Rad2Deg * 5f * Speed);
                MovePart.GetComponent<Rigidbody>().angularDrag =
                    Mathf.Abs(Fai * Mathf.Rad2Deg - Fai * Mathf.Rad2Deg) + (++D) * (++D) * 0.1f;
            }
            if (Fai * Mathf.Rad2Deg < 5f)
            {
                MovePart.transform.eulerAngles += StaticPartAxle.transform.right -
                    MovePartAxle.transform.right;
            }

            //拨叉静止，获取两端面。拨叉轴用基准面对齐
            foreach (Transform T in UsingFunction.Family(StaticPart)[3].transform)
            {
                if (T.localPosition == UsingFunction.Family(StaticPart)[4].GetComponent<PartInfo>()
                    .LeftFace)
                {
                    SL = T.transform.gameObject;
                }
                else if (T.localPosition == UsingFunction.Family(StaticPart)[4].GetComponent<PartInfo>()
                    .RightFace)
                {
                    SR = T.transform.gameObject;
                }
            }
            foreach (Transform T in UsingFunction.Family(MovePart)[3].transform)
            {
                if (T.localPosition == UsingFunction.Family(MovePart)[4].GetComponent<AxleInfo>()
                    .LeftDatumFace)
                {
                    ML = T.transform.gameObject;
                }
                else if (T.localPosition == UsingFunction.Family(MovePart)[4].GetComponent<AxleInfo>()
                    .RightDatumFace)
                {
                    MR = T.transform.gameObject;
                }
            }
            //按照拨叉从拨叉轴左边装配，则拨叉轴以拨叉右端面为基准面对准
            char LeftOrRight = UsingFunction.Family(StaticPart)[4]
                .GetComponent<PartInfo>().Order.ToCharArray()[0];

            //如果拨叉从拨叉轴的右边装配，则拨叉轴的右端面需要对准拨叉的左端面
            if (LeftOrRight == '右')
            {
                //获取零件中心到端面的距离
                float Dis;
                Vector3 Dir;
                float StayDis = 100f;

                GameObject S = SL;
                GameObject M = MR;
                Dis = (S.transform.position - M.transform.position
                    - S.transform.right.normalized * StayDis).magnitude;
                //Debug.Log(Dis);
                //当距离大于阈值，执行操作
                if (Dis >= 5f)
                {
                    //Dir = (Gear.gameObject.transform.position - A.gameObject.transform.position -
                    //    Gear.gameObject.transform.right.normalized * StayDis);
                    Dir = (S.transform.position - M.transform.position
                    - StaticPart.transform.right.normalized * StayDis);
                    float D = 1f;
                    MovePart.GetComponent<Rigidbody>().AddForce(Dir.normalized * Dir.magnitude * Speed * 0.5f);
                    MovePart.GetComponent<Rigidbody>().drag = 1f + (++D);
                }
                if (Dis < 5f)
                {
                    //获取拨叉轴的右端面到拨叉左端面坐标减去安全距离
                    MovePart.transform.position += S.transform.position -
                        StaticPart.transform.right.normalized * StayDis - M.transform.position;
                }
            }
            //如果拨叉从拨叉轴的左边装配，则拨叉轴的左端面需要对准拨叉的右端面
            if (LeftOrRight == '左')
            {
                float Dis;
                Vector3 Dir;
                float StayDis = 100f;
                //装载和解锁
                GameObject M = ML;

                GameObject S = SR;

                Dis = (S.transform.position - M.transform.position
                    + StaticPart.transform.right.normalized * StayDis).magnitude;
                if (Dis >= 5f)
                {
                    Dir = (S.transform.position - M.transform.position
                    + StaticPart.transform.right.normalized * StayDis);
                    float D = 1f;
                    MovePart.GetComponent<Rigidbody>().AddForce(Dir.normalized * Dir.magnitude * Speed * 0.5f);
                    MovePart.GetComponent<Rigidbody>().drag = 1f + (++D);
                }
                if (Dis < 5f)
                {
                    //用拨叉轴的左端面到拨叉右端面加上安全距离
                    MovePart.transform.position += S.transform.position +
                        StaticPart.transform.right.normalized * StayDis - M.transform.position;
                }
            }
        }
    }

    public void Button_CancelShaftAline()
    {
        MovePart.transform.position = BeforeAline_Pos;
        MovePart.transform.eulerAngles = BeforeAline_Rot;
        StarShaftAline = false;
    }

    public void Button_StarShaftCoplanar()
    {
        StarShaftCoplanar = true;
        BeforeCoplanar_Pos = MovePart.transform.position;
        BeforeCoplanar_Rot = MovePart.transform.eulerAngles;
    }
    /// <summary>
    /// 拨叉轴上的装配基准面与拨叉轴的装配面对准
    /// </summary>
    public void ShaftCoplanar()
    {
        if (StarShaftCoplanar)
        {
            float Dis;
            Vector3 Dir;
            //求出零件到轴端面的对准位置的距离，StaticF为拨叉上的装配基准面，MoveF为拨叉轴上的装配基准面
            Dis = (StaticPartFace.transform.position - MovePartFace.transform.position).magnitude;
            //当距离大于阈值，执行操作
            if (Dis >= 2f)
            {
                Dir = (StaticPartFace.transform.position - MovePartFace.transform.position);
                MovePart.GetComponent<Rigidbody>().AddForce(Dir.normalized * Dir.magnitude * Speed);//* 5f
                MovePart.GetComponent<Rigidbody>().drag = 5f;
            }
            if (Dis < 2f)
            {
                MovePart.transform.position += StaticPartFace.transform.position -
                    MovePartFace.transform.position;

                FinAssemble = true;
            }
        }
    }

    public void Button_CancelShaftCoplanar()
    {
        MovePart.transform.position = BeforeCoplanar_Pos;
        MovePart.transform.eulerAngles = BeforeCoplanar_Rot;
        StarShaftCoplanar = false;
    }

    public void Button_Fin()
    {
        if (FinAssemble)
        {
            //当拨叉与啮合套装配，添加链条关节到拨叉上
            //锁定拨叉PY,PZ,RY,RZ；锁定拨叉啮合套RY,RZ,PY,PZ
            if (MovePart.tag == "拨叉")
            {
                MovePart.AddComponent<FixedJoint>();
                MovePart.GetComponent<FixedJoint>().connectedBody = StaticPart.GetComponent<Rigidbody>();
            }
            //当拨叉与拨叉轴装配，添加固定关节到拨叉上
            //锁定拨叉RY,RZ
            if (MovePart.tag == "拨叉轴")
            {
                StaticPart.AddComponent<FixedJoint>();
                StaticPart.GetComponent<FixedJoint>().connectedBody = MovePart.GetComponent<Rigidbody>();
            }

            //Initialization();
        }
    }


}
