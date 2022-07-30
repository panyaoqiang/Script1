using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 此面板设置零件装配信息，储存零件自身所有基准面，基准轴，装配轴，装配顺序，零件类型
/// </summary>

public class TempInfo : MonoBehaviour
{
    //接收零件临时信息的UI面板
    ClickManager ClickObj;
    PartDate FinSetting;
    public Text SettingPartName;
    public Text ShaftName;
    public Dropdown KindsChoosingList;
    public Dropdown ChoosingRightOrLeft;
    public Dropdown ChoosingROrLNumber;
    public Dropdown ChoosingOutsideNumber;
    public Dropdown AssemblyPartsList;
    public InputField[] AxleXYZ;
    public InputField[] FaceXYZ;
    public Image Part_Setting;
    public Image Shaft_Setting;
    public Image AssemblyShaftChoose;

    //临时变量
    string[] AllPartsKinds;
    public Material M;
    //临时信息结构及装入的零件
    public GameObject SettingPart;
    /// <summary>
    /// 零件装配基准面，装配基准轴，装配轴，装配顺序
    /// </summary>
    public Dictionary<string, object> Temp = new Dictionary<string, object>();
    string Order;//零件装配顺序（临时）
    public List<GameObject> Face;//零件自身基准面
    public List<GameObject> Axle;//零件自身基准轴
    public string Kind;//零件类型

    public GameObject Parts;
    public List<GameObject> AssemblyParts = new List<GameObject>();

    public GameObject LeftDatumFace;
    public GameObject RightDatumFace;
    public Vector3 AssemblyPos;

    //操作开关
    bool Button_Star = false;
    bool Button_CancelSelet = false;
    bool Button_Shaft = false;
    bool Button_Kind = false;
    bool Button_Order = false;
    bool Button_ChooseAxis = false;
    bool Button_ChooseFace = false;
    bool Button_AddAxis = false;
    bool Button_DeletAxis = false;
    bool Button_AddSurface = false;
    bool Button_DeletSurface = false;
    bool Button_MoveAxle = false;
    bool Button_MoveSurface = false;
    bool Button_Shaft_ChooseParts = false;
    bool Button_Shaft_DeletParts = false;
    bool Button_ChooseLeft = false;
    bool Button_ChooseRight = false;
    bool Button_ConfirmLeft = false;
    bool Button_ConfirmRight = false;
    bool Button_CancelLeft = false;
    bool Button_CancelRight = false;
    bool Button_GetPartAssemblyPos = true;

    void Start()
    {
        //初次赋值
        ClickObj = GameObject.Find("Controler").gameObject.GetComponent<ClickManager>();
        FinSetting = GameObject.Find("InfoControler").gameObject.GetComponent<PartDate>();
        //零件临时信息层
        Face = new List<GameObject>();//装入零件第三层
        Axle = new List<GameObject>();//装入零件第二层

        //Shaft = new GameObject();//装入脚本
        //Order = new List<object>();//装入脚本
        //Temp.Add("装配轴", Shaft);//ShaftChoose选择装入脚本
        //Temp.Add("装配基准面", Face);//装入脚本
        //Temp.Add("装配基准轴", Axle);//装入脚本
        //Temp.Add("装配顺序", Order);//装入脚本
        //初始化零件类型下拉列表
        if (KindsChoosingList.options != null)
        {
            KindsChoosingList.options.Clear();
        }
        AllPartsKinds = new string[]
        {
            "   ","轴","齿轮","轴承","轴套","紧固环","拨叉","行星架","壳体"
        };
        for (int i = 0; i < AllPartsKinds.Length; i++)
        {
            Dropdown.OptionData data = new Dropdown.OptionData();
            data.text = AllPartsKinds[i];
            KindsChoosingList.options.Add(data);
        }
        //初始化order组
        AxleXYZ[0].text = "0";
        AxleXYZ[1].text = "0";
        AxleXYZ[2].text = "0";
        FaceXYZ[0].text = "0";
        FaceXYZ[1].text = "0";
        FaceXYZ[2].text = "0";
        //初始化临时信息设置层，设置完毕后直接赋值给零件信息层
        //初始化下拉列表
        Initialization();
    }

    void Update()
    {
        Star();
        ShaftChoose();
        KindChoose();
        Axis();
        Surface();
        try
        {
            MoveAxis(Axle[Axle.Count - 1]);
            MoveFace(Face[Face.Count - 1]);
        }
        catch { }
        OrderChoose();
        AddAssemblyParts();
        ChooseLeftFace();
        ChooseRightFace();
        CheckSideFace();
        PartsAssemblyPos();
    }
    //初始化
    public void Initialization()
    {
        AssemblyPos = Vector3.zero;
        Kind = null;
        Button_Star = false;
        Button_CancelSelet = false;
        Button_Shaft = false;
        Button_Kind = false;
        Button_Order = false;
        Button_ChooseAxis = false;
        Button_ChooseFace = false;
        Button_AddAxis = false;
        Button_DeletAxis = false;
        Button_AddSurface = false;
        Button_DeletSurface = false;
        Button_MoveAxle = false;
        Button_MoveSurface = false;
        Button_Shaft_ChooseParts = false;
        Button_Shaft_DeletParts = false;
        Button_ChooseLeft = false;
        Button_ChooseRight = false;
        Button_ConfirmLeft = false;
        Button_ConfirmRight = false;
        Button_CancelLeft = false;
        Button_CancelRight = false;
        Button_GetPartAssemblyPos = true;

        SettingPartName.text = "";
        ShaftName.text = "";

        AxleXYZ[0].text = "0";
        AxleXYZ[1].text = "0";
        AxleXYZ[2].text = "0";
        FaceXYZ[0].text = "0";
        FaceXYZ[1].text = "0";
        FaceXYZ[2].text = "0";

        KindsChoosingList.GetComponent<Dropdown>().value = 0;
        ChoosingRightOrLeft.GetComponent<Dropdown>().value = 0;
        ChoosingROrLNumber.GetComponent<Dropdown>().value = 0;
        ChoosingOutsideNumber.GetComponent<Dropdown>().value = 0;
        AssemblyPartsList.GetComponent<Dropdown>().value = 0;
        AssemblyPartsList.options.Clear();
        Dropdown.OptionData data = new Dropdown.OptionData();
        data.text = "轴上装配零件";
        AssemblyPartsList.options.Add(data);

        Part_Setting.gameObject.SetActive(false);
        Shaft_Setting.gameObject.SetActive(false);
        AssemblyShaftChoose.gameObject.SetActive(false);

        Face.Clear();
        Axle.Clear();
        Temp.Clear();
        Order = null;
        SettingPart = null;
        LeftDatumFace = null;
        RightDatumFace = null;
        AssemblyParts.Clear();
    }

    /// <summary>
    /// 锁定零件Freeze/取消锁定Free
    /// </summary>
    /// <param name="Operate"></param>
    public void Button_LockOrFree(string Operate)
    {
        if (Operate == "Freeze")
        {
            SettingPart.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        }
        if (Operate == "Free")
        {
            SettingPart.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        }
    }

    public void Button_StarSetting()
    {
        Button_Star = true;
    }

    public void Button_CancelSetting()
    {
        Button_CancelSelet = true;
    }

    public void Button_ChooseConfirm()
    {
        Button_Star = false;
    }

    /// <summary>
    /// 完成轴设置时，添加轴到已设置轴列表和已设置轴list
    /// 完成零件设置时，添加零件到已设置零件列表和已设置零件list
    /// 重置编辑零件状态
    /// </summary>
    public void Button_FinSetting()
    {
        //设置的不是装配轴，此时零件信息层的axleinfo为空
        if (SettingPart.tag == "装配零件"||SettingPart.tag=="拨叉")
        {
            UsingFunction.Family(SettingPart)[4].GetComponent<PartInfo>().AssembleInfo.Clear();
            UsingFunction.Family(SettingPart)[4].GetComponent<PartInfo>().SelfInfo.Clear();
            if (Temp.Count == 4 && AssemblyPos != null && Axle != null && Face != null
                && Kind != null && LeftDatumFace != null && RightDatumFace != null)
            {
                UsingFunction.Family(SettingPart)[4].GetComponent<PartInfo>().AssembleInfo.
                Add("装配轴", Temp["装配轴"]);
                UsingFunction.Family(SettingPart)[4].GetComponent<PartInfo>().AssembleInfo.
                    Add("装配基准面", Temp["装配基准面"]);
                UsingFunction.Family(SettingPart)[4].GetComponent<PartInfo>().AssembleInfo.
                    Add("装配基准轴", Temp["装配基准轴"]);
                UsingFunction.Family(SettingPart)[4].GetComponent<PartInfo>().AssembleInfo.
                    Add("装配顺序", Temp["装配顺序"]);
                UsingFunction.Family(SettingPart)[4].GetComponent<PartInfo>().AssembleInfo.
                    Add("装配位置", AssemblyPos);
                UsingFunction.Family(SettingPart)[4].GetComponent<PartInfo>().SelfInfo.
                    Add("自身基准轴", Axle);
                UsingFunction.Family(SettingPart)[4].GetComponent<PartInfo>().SelfInfo.
                    Add("自身基准面", Face);
                UsingFunction.Family(SettingPart)[4].GetComponent<PartInfo>().SelfInfo.
                    Add("零件类型", Kind);
                UsingFunction.Family(SettingPart)[4].GetComponent<PartInfo>().SelfInfo.
                    Add("左端面", LeftDatumFace);
                UsingFunction.Family(SettingPart)[4].GetComponent<PartInfo>().SelfInfo.
                    Add("右端面", RightDatumFace);
                UsingFunction.Family(SettingPart)[4].GetComponent<PartInfo>().TranslationInfo();
            }
            
            
            //设置零件完成后，删除对应未设置列表选项，添加已设置列表选项
            //更新列表
            if (!FinSetting.SettedShafts[((GameObject)Temp["装配轴"]).name].Contains(SettingPart))
            {
                //添加到已设置列表
                FinSetting.SettedShafts[((GameObject)Temp["装配轴"]).name].Add(SettingPart);
                Dropdown.OptionData data = new Dropdown.OptionData();
                data.text = SettingPart.name;
                FinSetting.SettedPartsList.options.Add(data);
                //删除未设置列表中的零件项
                for (int i = 0; i < FinSetting.UnsettedPartsList.options.Count; i++)
                {
                    if (FinSetting.UnsettedPartsList.options[i].text == SettingPart.name)
                    {
                        FinSetting.UnsettedPartsList.options.RemoveAt(i);
                        FinSetting.UnsettedPartsList.GetComponent<Dropdown>().value = 0;
                    }
                }
            }
        }
        //设置的为装配轴，此时装配轴信息层的partinfo为空
        if (SettingPart.tag == "装配轴"||SettingPart.tag=="拨叉轴"|| SettingPart.tag == "壳体")
        {
            if (AssemblyPos != null && Axle != null && Face != null && AssemblyParts.Count!=0
                && Kind != null && LeftDatumFace != null && RightDatumFace != null)
            {
                UsingFunction.Family(SettingPart)[4].GetComponent<AxleInfo>().ShaftInfo.Clear();
                UsingFunction.Family(SettingPart)[4].GetComponent<AxleInfo>().AllParts.Clear();

                UsingFunction.Family(SettingPart)[4].GetComponent<AxleInfo>().ShaftInfo.
                    Add("零件类型", Kind);
                UsingFunction.Family(SettingPart)[4].GetComponent<AxleInfo>().ShaftInfo.
                    Add("自身基准轴", Axle);
                UsingFunction.Family(SettingPart)[4].GetComponent<AxleInfo>().ShaftInfo.
                    Add("自身基准面", Face);
                UsingFunction.Family(SettingPart)[4].GetComponent<AxleInfo>().ShaftInfo.
                    Add("左端面", LeftDatumFace);
                UsingFunction.Family(SettingPart)[4].GetComponent<AxleInfo>().ShaftInfo.
                    Add("右端面", RightDatumFace);
                for (int i = 0; i < AssemblyParts.Count; i++)
                {
                    UsingFunction.Family(SettingPart)[4].GetComponent<AxleInfo>().AllParts.Add(AssemblyParts[i]);
                }
                UsingFunction.Family(SettingPart)[4].GetComponent<AxleInfo>().TranslateInfo();
            }
            

            //轴设置完成后，添加轴到已设置轴列表中
            if (!FinSetting.SettedShafts.ContainsKey(SettingPart.name))
            {
                FinSetting.SettedShafts.Add(SettingPart.name, AssemblyParts);
                Dropdown.OptionData date = new Dropdown.OptionData();
                date.text = SettingPart.name;
                FinSetting.SettedShaftsList.options.Add(date);
            }
        }
        Initialization();
    }
    //零件编辑专用
    public void Button_ShaftChoose()
    {
        Button_Shaft = true;
    }
    //零件编辑专用
    public void Button_ShaftConfirm()
    {
        if (ClickObj.ClickObj.tag == "装配轴"||ClickObj.ClickObj.tag=="拨叉轴"|| ClickObj.ClickObj.tag == "壳体")
        {
            Temp.Add("装配轴", ClickObj.ClickObj.gameObject);
            Button_Shaft = false;
        }
    }

    //装配轴编辑专用
    public void Button_AssemblyPartsChoose()
    {
        Button_Shaft_ChooseParts = true;
    }
    //装配轴编辑专用
    public void Button_AssemblyPartsDelet()
    {
        Button_Shaft_DeletParts = true;
    }

    public void Button_ChooseLeftFace()
    {
        Button_ChooseLeft = true;
    }

    public void Button_CancelLeftFace()
    {
        Button_CancelLeft = true;
    }

    public void Button_ChooseRightFace()
    {
        Button_ChooseRight = true;
    }

    public void Button_CancelRightFace()
    {
        Button_CancelRight = true;
    }

    public void Button_ConfirmLeftFace()
    {
        Button_ConfirmLeft = true;
    }

    public void Button_ConfirmRightFace()
    {
        Button_ConfirmRight = true;
    }

    public void Button_KindChoose()
    {
        Button_Kind = true;
    }
    //零件编辑专用
    public void Button_GetAssemblyPos()
    {
        //Button_GetPartAssemblyPos = true;
    }

    public void Button_OrderChoose()
    {
        Button_Order = true;
    }

    public void Button_MoveAxis()
    {
        Button_MoveAxle = true;
    }

    public void Button_MoveFace()
    {
        Button_MoveSurface = true;
    }

    public void Button_Add_Axis()
    {
        Button_AddAxis = true;
        Button_LockOrFree("Freeze");
    }

    public void Button_Delet_Axis()
    {
        Button_DeletAxis = true;
    }

    public void Button_Add_Face()
    {
        Button_AddSurface = true;
        Button_LockOrFree("Freeze");
    }

    public void Button_Delet_Face()
    {
        Button_DeletSurface = true;
    }

    public void Button_ConfirmAddAxis()
    {
        for (int i = 0; i < Axle.Count; i++)
        {
            if (Axle[i].transform.parent != UsingFunction.Family(SettingPart)[2])
            {
                Axle[i].gameObject.transform.SetParent
                    (UsingFunction.Family(SettingPart)[2].transform);
                Axle[i].gameObject.GetComponent<MeshRenderer>().enabled = false;
            }
        }
        Button_LockOrFree("Free");
    }

    public void Button_ConfirmAddFace()
    {
        for (int i = 0; i < Face.Count; i++)
        {
            if (Face[i].transform.parent != UsingFunction.Family(SettingPart)[3])
            {
                Face[i].gameObject.transform.SetParent
                    (UsingFunction.Family(SettingPart)[3].transform);
                Face[i].gameObject.GetComponent<MeshRenderer>().enabled = false;
            }
        }
        Button_LockOrFree("Free");
    }
    //零件编辑专用
    public void Button_ChooseDatumAxis()
    {
        Button_ChooseAxis = true;
        foreach (Transform Child in UsingFunction.Family((GameObject)Temp["装配轴"])[2].transform)
        {
            Child.GetComponentInChildren<MeshRenderer>().enabled = true;
            Child.GetComponentInChildren<BoxCollider>().enabled = true;
        }
    }
    //零件编辑专用
    public void Button_ConfirmDatumAxis()
    {
        if (Button_ChooseAxis && ClickObj.ClickObj != null && ClickObj.ClickObj.gameObject.tag == "装配基准轴")
        {
            Temp.Add("装配基准轴", ClickObj.ClickObj.gameObject);
        }
        Button_ChooseAxis = false;
        foreach (Transform Child in UsingFunction.Family((GameObject)Temp["装配轴"])[2].transform)
        {
            Child.GetComponentInChildren<MeshRenderer>().enabled = false;
            Child.GetComponentInChildren<BoxCollider>().enabled = false;
        }
    }
    //零件编辑专用
    public void Button_ChooseDatumFace()
    {
        Button_ChooseFace = true;
        foreach (Transform Child in UsingFunction.Family((GameObject)Temp["装配轴"])[3].transform)
        {
            Child.GetComponentInChildren<MeshRenderer>().enabled = true;
            Child.GetComponentInChildren<BoxCollider>().enabled = true;
        }
    }
    //零件编辑专用
    public void Button_ConfirmDatumFace()
    {
        if (Button_ChooseFace && ClickObj.ClickObj != null && ClickObj.ClickObj.gameObject.tag == "装配基准面")
        {
            Temp.Add("装配基准面", ClickObj.ClickObj.gameObject);
        }
        Button_ChooseFace = false;
        foreach (Transform Child in UsingFunction.Family((GameObject)Temp["装配轴"])[3].transform)
        {
            Child.GetComponentInChildren<MeshRenderer>().enabled = false;
            Child.GetComponentInChildren<BoxCollider>().enabled = false;
        }
    }

    public void Star()
    {
        if (ClickObj.ClickObj != null && Button_Star)// && ClickObj.ClickObj.tag != "装配轴" 
        {
            //当设置零件非装配轴或壳体
            if (ClickObj.ClickObj.tag != "装配轴"&&ClickObj.ClickObj.tag!="拨叉轴"&& ClickObj.ClickObj.tag != "壳体")
            {
                //ClickObj.ClickObj.
                Part_Setting.gameObject.SetActive(true);
                Shaft_Setting.gameObject.SetActive(false);
                AssemblyShaftChoose.gameObject.SetActive(true);
                SettingPart = ClickObj.ClickObj;
                UsingFunction.Family(SettingPart)[4].GetComponent<PartInfo>().Initialization();
                SettingPartName.text = "当前设置中的零件：" + SettingPart.name;
                Button_Star = false;
            }
            //当设置零件为装配轴或壳体
            if (ClickObj.ClickObj.tag == "装配轴"||ClickObj.ClickObj.tag=="拨叉轴"|| ClickObj.ClickObj.tag == "壳体")
            {
                Part_Setting.gameObject.SetActive(false);
                Shaft_Setting.gameObject.SetActive(true);
                AssemblyShaftChoose.gameObject.SetActive(false);
                SettingPart = ClickObj.ClickObj;
                UsingFunction.Family(SettingPart)[4].GetComponent<AxleInfo>().Initialization();
                SettingPartName.text = "当前设置中的零件：" + SettingPart.name;

                Button_Star = false;
                //关闭装配顺序设定和关闭装配轴选择框。
                //打开装配零件选择框
                //隐藏未选择轴及其装配零件
                //左右端面
                //自身装配基准面
                //自身基准轴
                //零件类型
            }
        }
        if (Button_CancelSelet)
        {
            Initialization();
            Button_CancelSelet = false;
        }
    }

    public void Axis()
    {
        if (Button_AddAxis && SettingPart.gameObject != null)
        {
            //Instantiate()
            Axle.Add((GameObject)Instantiate(Resources.Load("_Prefabs/axle"),
                SettingPart.gameObject.transform.position, SettingPart.transform.rotation));
            Axle[Axle.Count - 1].gameObject.GetComponent<MeshRenderer>().enabled = true;
            //Axle[Axle.Count - 1].gameObject.transform.SetParent
            //    (UsingFunction.Family(SettingPart)[2].transform);
            Button_AddAxis = false;
        }
        if (Button_DeletAxis && Axle.Count != 0)
        {
            Destroy(Axle[Axle.Count - 1].gameObject);
            Axle.Remove(Axle[Axle.Count - 1].gameObject);
            Button_DeletAxis = false;
        }

    }

    public void Surface()
    {
        if (Button_AddSurface && SettingPart.gameObject != null)
        {
            Face.Add((GameObject)Instantiate(Resources.Load("_Prefabs/face"),
            SettingPart.gameObject.transform.position, SettingPart.transform.rotation));
            Face[Face.Count - 1].gameObject.GetComponent<MeshRenderer>().enabled = true;
            //Face[Face.Count - 1].gameObject.transform.SetParent
            //    (UsingFunction.Family(SettingPart)[2].transform);
            Button_AddSurface = false;
        }
        if (Button_DeletSurface && Face.Count != 0)
        {
            Destroy(Face[Face.Count - 1].gameObject);
            Face.Remove(Face[Face.Count - 1].gameObject);
            Button_DeletSurface = false;
        }
    }

    public void KindChoose()
    {
        //"齿轮","轴承","轴套","紧固环","拨叉","行星架"
        if (Button_Kind)
        {
            Kind = KindsChoosingList.captionText.text;
        }
    }
    //零件编辑专用
    public void ShaftChoose()
    {
        if (Button_Shaft && ClickObj.ClickObj != null && (ClickObj.ClickObj.tag == "装配轴"||
            ClickObj.ClickObj.tag == "壳体" || ClickObj.ClickObj.tag=="拨叉轴"))
        {
            ShaftName.text = ClickObj.ClickObj.gameObject.name;
        }
    }
    //零件编辑专用
    public void OrderChoose()
    {
        if (Button_Order)
        {
            //连字
            Order = (ChoosingRightOrLeft.captionText.text + ChoosingROrLNumber.captionText.text
                + ChoosingOutsideNumber.captionText.text);
            Temp.Add("装配顺序", Order);
            Button_Order = false;
        }
    }

    public void MoveAxis(GameObject Axle)
    {
        if (Button_MoveAxle && Axle != null)
        {
            float x = float.Parse(AxleXYZ[0].text);
            float y = float.Parse(AxleXYZ[1].text);
            float z = float.Parse(AxleXYZ[2].text);
            Axle.gameObject.transform.position = SettingPart.transform.position + new Vector3(x, y, z);
            Button_MoveAxle = false;
        }
    }

    public void MoveFace(GameObject Face)
    {
        if (Button_MoveSurface && Face != null)
        {
            float x = float.Parse(FaceXYZ[0].text);
            float y = float.Parse(FaceXYZ[1].text);
            float z = float.Parse(FaceXYZ[2].text);
            Face.gameObject.transform.position = SettingPart.transform.position + new Vector3(x, y, z);
            Button_MoveSurface = false;
        }
    }
    //装配轴编辑专用
    public void AddAssemblyParts()
    {
        //添加零件到装配轴的装配零件列表
        //新建下拉列表事件
        //回复下拉列表默认选择状态
        //添加至List
        //被选中零件自动隐藏
        //移除后回复显示
        if (Button_Shaft_ChooseParts && ClickObj.ClickObj != null && !ClickObj.ClickObj.tag.Contains("轴")) 
        {
            Parts = ClickObj.ClickObj;
            AssemblyParts.Add(Parts);
            Dropdown.OptionData date = new Dropdown.OptionData();
            date.text = Parts.name;
            AssemblyPartsList.options.Add(date);
            AssemblyPartsList.GetComponent<Dropdown>().value = 0;
            UsingFunction.Hide_Translucent_Appear("Hide", Parts);
            Parts = null;
            //ClickObj.Initialization();
            Button_Shaft_ChooseParts = false;
        }
        if (Button_Shaft_DeletParts && AssemblyPartsList.options.Count != 0)
        {
            for (int i = 0; i < AssemblyParts.Count; i++)
            {
                if (AssemblyPartsList.captionText.text == AssemblyParts[i].name)
                {
                    for (int j = 0; j < AssemblyPartsList.options.Count; j++)
                    {
                        if (AssemblyPartsList.captionText.text
                            == AssemblyPartsList.options[j].text)
                        {
                            AssemblyPartsList.GetComponent<Dropdown>().value = 0;
                            AssemblyPartsList.options.Remove(AssemblyPartsList.options[j]);
                            Button_Shaft_DeletParts = false;
                        }
                    }
                    UsingFunction.Hide_Translucent_Appear("Appear", AssemblyParts[i]);
                    AssemblyParts.Remove(AssemblyParts[i]);
                }
            }
        }
        if (AssemblyPartsList.options.Count >= 1 && UsingFunction.GetUI() != null)
        {
            if (UsingFunction.GetUI().name == "查看零件")
            {
                for (int i = 0; i < AssemblyParts.Count; i++)
                {
                    if (AssemblyParts[i].name == AssemblyPartsList.captionText.text)
                    {
                        UsingFunction.Hide_Translucent_Appear("Appear", AssemblyParts[i]);
                    }
                }
            }
            //if (UsingFunction.GetUI().name == "选择轴上的装配零件")
            else
            {
                for (int i = 0; i < AssemblyParts.Count; i++)
                {
                    if (AssemblyParts[i].name == AssemblyPartsList.captionText.text)
                    {
                        UsingFunction.Hide_Translucent_Appear("Hide", AssemblyParts[i]);
                    }
                }
            }
        }
    }

    //装配轴编辑专用
    public void ChooseLeftFace()
    {
        if (Button_ChooseLeft && Face.Count != 0 && SettingPart != null)
        {
            for (int i = 0; i < Face.Count; i++)
            {
                UsingFunction.Hide_Translucent_Appear("Appear", Face[i]);
            }
            if (ClickObj.ClickObj != null && ClickObj.ClickObj.tag == "装配基准面"
                && Button_ConfirmLeft)
            {
                LeftDatumFace = ClickObj.ClickObj;
                for (int i = 0; i < Face.Count; i++)
                {
                    UsingFunction.MaterialChange(ClickObj.PickingObj, ClickObj.OrigionalMaterial);
                    UsingFunction.Hide_Translucent_Appear("Hide", Face[i]);
                }
                Button_ChooseLeft = false;
                Button_ConfirmLeft = false;
            }
        }
        if (Button_CancelLeft && Face.Count != 0 &&
            SettingPart != null && LeftDatumFace != null)
        {
            LeftDatumFace = null;
            Button_ChooseLeft = true;
            Button_CancelLeft = false;
        }
    }

    //装配轴编辑专用
    public void ChooseRightFace()
    {
        if (Button_ChooseRight && Face.Count != 0 && SettingPart != null)
        {
            for (int i = 0; i < Face.Count; i++)
            {
                UsingFunction.Hide_Translucent_Appear("Appear", Face[i]);
            }
            if (ClickObj.ClickObj != null && ClickObj.ClickObj.tag == "装配基准面"
                && Button_ConfirmRight)
            {
                RightDatumFace = ClickObj.ClickObj;
                for (int i = 0; i < Face.Count; i++)
                {
                    UsingFunction.MaterialChange(ClickObj.PickingObj, ClickObj.OrigionalMaterial);
                    UsingFunction.Hide_Translucent_Appear("Hide", Face[i]);
                }
                Button_ChooseRight = false;
                Button_ConfirmRight = false;
            }
        }
        if (Button_CancelRight && Face.Count != 0 &&
            SettingPart != null && RightDatumFace != null)
        {
            RightDatumFace = null;
            Button_ChooseRight = true;
            Button_CancelRight = false;
        }
    }

    //装配轴编辑专用
    public void CheckSideFace()
    {
        if (LeftDatumFace != null && RightDatumFace != null &&
            UsingFunction.GetUI() != null && UsingFunction.GetUI().tag == "Tips"
            && !Button_ConfirmRight && !Button_ConfirmLeft
            && !Button_ChooseLeft && !Button_ChooseRight)
        {
            if (UsingFunction.GetUI().name == "查看左端面")
            {
                UsingFunction.Hide_Translucent_Appear("Appear", LeftDatumFace);
            }
            if (UsingFunction.GetUI().name == "查看右端面")
            {
                UsingFunction.Hide_Translucent_Appear("Appear", RightDatumFace);
            }
            if (UsingFunction.GetUI().name == "选择轴上的装配零件"|| UsingFunction.GetUI().name == "选择零件装配顺序")
            {
                UsingFunction.Hide_Translucent_Appear("Hide", LeftDatumFace);
                UsingFunction.Hide_Translucent_Appear("Hide", RightDatumFace);
            }
        }
        else if(LeftDatumFace != null && RightDatumFace != null && UsingFunction.GetUI() == null
            && !Button_ConfirmRight && !Button_ConfirmLeft && !Button_ChooseLeft && !Button_ChooseRight)
        {
            UsingFunction.Hide_Translucent_Appear("Hide", LeftDatumFace);
            UsingFunction.Hide_Translucent_Appear("Hide", RightDatumFace);
        }
    }

    //装配零件编辑专用
    /// <summary>
    /// 零件装配基准面选择前，必须先轴线对准共线，rotation必须相同
    /// </summary>
    public void PartsAssemblyPos()
    {
        if (SettingPart != null && (SettingPart.tag == "装配零件"||SettingPart.tag=="拨叉")
            && Temp.ContainsKey("装配基准面") && Button_GetPartAssemblyPos)
        {
            if (Temp["装配基准面"] != null)
            {
                AssemblyPos = ((GameObject)Temp["装配基准面"]).transform.position - SettingPart.transform.position;
                Button_GetPartAssemblyPos = false;
            }
        }
    }
}
