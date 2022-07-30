using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 查看零件装配轴的装配信息
/// </summary>

public class GlobalControl : MonoBehaviour
{
    public PartInfo PartTips;
    //public AxleInfo ShaftTips;
    public ClickManager ClickObj;

    public Image GlobalControler;
    public Text ClickObjName;
    public Dropdown OperatingObj;
    public Text Tips_Kind;
    public Text Tips_DatumFace;
    public Text Tips_DatumAxis;
    public Text Tips_Order;
    public Material M;

    GameObject DragingUI;
    Vector3 Dir = Vector3.zero;
    public bool Drag = false;

    List<GameObject> HideObj;//已经隐藏的轴
    public GameObject TempOperatePart;
    public Dictionary<string, object> Info_Part;
    
    void Start()
    {
        ClickObj = GameObject.Find("Controler").gameObject.GetComponent<ClickManager>();
        HideObj = new List<GameObject>();
        Info_Part = new Dictionary<string, object>();
        
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(Info_Part.ContainsKey("装配轴"));
        //DontDestroyOnLoad(this.gameObject);
        //初始化
        if (ClickObj.ClickObj != null&&ClickObj.ClickObj.tag=="装配零件"&&GlobalControler.enabled)
        {
            ClickObjName.text = ClickObj.ClickObj.name;
            TempOperatePart = ClickObj.ClickObj;
            if (TempOperatePart.tag == ("装配零件")&&Info_Part.Count==0)
            {
                PartTips = UsingFunction.Family(TempOperatePart)[4].GetComponent<PartInfo>();
                Info_Part.Add("装配轴", GameObject.Find("输出轴总承").transform.Find(PartTips.Shaft).gameObject);
                Debug.Log(Info_Part["装配轴"]);
                foreach( Transform A in UsingFunction.Family(TempOperatePart)[3].transform)
                {
                    if(A.transform.localPosition == PartTips.LeftFace)
                    {
                        Info_Part.Add("左端面", A.transform.gameObject);
                    }
                    else if (A.transform.localPosition == PartTips.RightFace)
                    {
                        Info_Part.Add("右端面", A.transform.gameObject);
                    }
                }
                foreach (Transform K in UsingFunction.Family(GameObject.Find("输出轴总承").
                    transform.Find(PartTips.Shaft).gameObject)[2].transform)
                {
                    if (K.transform.localPosition == PartTips.DatumAxle)
                    {
                        Info_Part.Add("装配基准轴", K.transform.gameObject);
                        Debug.Log("??");
                    }
                }
                foreach(Transform K in UsingFunction.Family(GameObject.Find("输出轴总承").
                    transform.Find(PartTips.Shaft).gameObject)[3].transform)
                {
                    if (K.transform.localPosition == PartTips.DatumFace)
                    {
                        Info_Part.Add("装配基准面", K.transform.gameObject);
                    }
                }
                Info_Part.Add("装配顺序", PartTips.Order);
                //Info_Part.Add("装配轴", PartTips.AssembleInfo["装配轴"]);
                //Info_Part.Add("装配基准面", PartTips.AssembleInfo["装配基准面"]);
                //Info_Part.Add("装配基准轴", PartTips.AssembleInfo["装配基准轴"]);
                //Info_Part.Add("装配顺序", PartTips.AssembleInfo["装配顺序"]);
                //Info_Part.Add("左端面", PartTips.SelfInfo["左端面"]);
                //Info_Part.Add("右端面", PartTips.SelfInfo["右端面"]);
            }
        }
        //UIDrager();
        if (TempOperatePart != null)
        {
            MouseOverTips(TempOperatePart);
        }
    }
    
    public void Initialized()
    {
        TempOperatePart = null;
        Info_Part = new Dictionary<string, object>();
        Info_Part.Clear();
        PartTips = null;
        ClickObjName.text = "";
        Tips_Kind.text = "零件类型为";
        Tips_DatumFace.text = "零件装配基准面";
        Tips_DatumAxis.text = "零件装配基准轴";
        Tips_Order.text = "装配顺序";
        OperatingObj.GetComponent<Dropdown>().value = 0;
        OperatingObj.options.Clear();
        Dropdown.OptionData data = new Dropdown.OptionData();
        data.text = "被隐藏的零件列表";
        OperatingObj.options.Add(data);
    }
    
    public void UIDrager()
    {
        //检测悬停事件
        //拾取
        if (Input.GetMouseButtonDown(0) && UsingFunction.GetUI() != null
            && UsingFunction.GetUI().tag == "Drag")
        {
            DragingUI = UsingFunction.GetUI();
        }
        //拖动
        if (Input.GetMouseButton(0) && Drag && DragingUI != null)
        {
            if (Dir != Vector3.zero) // && DragingUI == UsingFunction.GetUI()
            {
                //EventSystem.current.currentSelectedGameObject.
                //gameObject.GetComponentInParent<RectTransform>().gameObject
                //.GetComponentInParent<RectTransform>().position
                DragingUI.gameObject.transform.position += Input.mousePosition - Dir;
            }
        }
        //放下
        if (Input.GetMouseButtonUp(0))
        {
            DragingUI = null;
        }
        Dir = Input.mousePosition;
    }

    /// <summary>
    /// 控制轴或零件整体，单个轴，单个面
    /// </summary>
    /// <param name="Operation">Hide，Translucent，Appear</param>
    /// <param name="M">半透明材质</param>
    /// <param name="G">点击的物体</param>
    public void Hide_Translucent_Appear(string Operation, GameObject G)//, Material M
    {
        switch (Operation)
        {
            case "Hide":
                if (G.gameObject.tag == "装配轴" || G.gameObject.tag == "装配零件"||
                    G.gameObject.tag == "拨叉轴" || G.gameObject.tag == "拨叉")
                {
                    UsingFunction.HideAndAppear("Hide", 0, G, null);
                }
                if (G.gameObject.tag == "装配基准面" || G.gameObject.tag == "装配基准轴")
                {
                    G.GetComponent<BoxCollider>().enabled = false;
                    G.GetComponent<MeshRenderer>().enabled = false;
                }
                return;
            #region
            //case "Translucent":
            //    if (G.gameObject.tag == "装配轴" || G.gameObject.tag == "装配零件")
            //    {
            //        //隐藏碰撞体
            //        UsingFunction.HideAndAppear("Hide", 1, G, null);
            //        //材质替换
            //        UsingFunction.Family(G)[5].GetComponent<MeshRenderer>().material = M;
            //    }
            //    if (G.gameObject.tag == "装配基准面" || G.gameObject.tag == "装配基准轴")
            //    {
            //        //隐藏碰撞体
            //        G.GetComponent<BoxCollider>().enabled = false;
            //        //材质替换
            //        if (PickingObj == null)
            //        {
            //            PickingObj = G;
            //            TempMat = PickingObj.GetComponent<MeshRenderer>().material;
            //            if (G.GetComponent<MeshRenderer>().material != M)
            //            {
            //                G.GetComponent<MeshRenderer>().material = M;
            //            }
            //        }
            //        if (PickingObj != null)
            //        {

            //        }
            //    }

            //    return;
            #endregion
            case "Appear":
                if (G.gameObject.tag == "装配轴" || G.gameObject.tag == "装配零件" ||
                    G.gameObject.tag == "拨叉轴" || G.gameObject.tag == "拨叉")
                {
                    UsingFunction.HideAndAppear("Appear", 0, G, null);
                }
                if (G.gameObject.tag == "装配基准面" || G.gameObject.tag == "装配基准轴")
                {
                    G.GetComponent<BoxCollider>().enabled = true;
                    G.GetComponent<MeshRenderer>().enabled = true;
                };
                return;
        }
    }

    /// <summary>
    /// 隐藏零件
    /// 隐藏零件碰撞体，3D模型
    /// 添加至隐藏零件列表
    /// 在下拉列表中添加对应date事件
    /// </summary>
    public void Button_Hide()
    {
        if (TempOperatePart != null)
        {
            Hide_Translucent_Appear("Hide", TempOperatePart);
            HideObj.Add(TempOperatePart.gameObject);
            Dropdown.OptionData data = new Dropdown.OptionData();
            data.text = TempOperatePart.name;
            OperatingObj.options.Add(data);
            OperatingObj.GetComponent<Dropdown>().value = 0;
        }
    }

    /// <summary>
    /// 从零件列表里选择零件显示
    /// 显示零件碰撞体，3D模型
    /// 将零件从隐藏列表中移除
    /// 将对应下拉列表中的零件date项目移除
    /// </summary>
    public void Button_Appear()
    {
        for (int i = 0; i < HideObj.Count; i++)
        {
            if (HideObj[i].name == OperatingObj.captionText.text
                && OperatingObj.captionText.text != null)
            {
                Hide_Translucent_Appear("Appear", HideObj[i]);
                for (int j = 0; j < OperatingObj.options.Count; j++)
                {
                    if (OperatingObj.options[j].text == HideObj[i].name)
                    {
                        //OperatingObj.captionText.text = OperatingObj.options[0].text;
                        OperatingObj.GetComponent<Dropdown>().value = 0;
                        OperatingObj.options.Remove(OperatingObj.options[j]);
                        HideObj.Remove(HideObj[i]);
                    }
                }
            }
        }
    }

    /// <summary>
    /// 信息查看功能只能针对零件
    /// </summary>
    /// <param name="G">传入被点击的零件主体</param>
    public void MouseOverTips(GameObject G)
    {
        if (UsingFunction.GetUI() != null && UsingFunction.GetUI().tag == "Tips")
        {
            //Debug.Log(UsingFunction.GetUI());
            //Debug.Log(Info_Part.Count);
            Debug.Log(Info_Part.ContainsKey("装配基准面").ToString()
                + Info_Part.ContainsKey("装配基准轴").ToString()
                + Info_Part.ContainsKey("装配轴").ToString());
            if (G.gameObject.tag == "装配零件"&&Info_Part.ContainsKey("装配基准面")
                && Info_Part.ContainsKey("装配基准轴")&& Info_Part.ContainsKey("装配轴")
                && Info_Part.ContainsKey("左端面") && Info_Part.ContainsKey("右端面"))
            {
                if (UsingFunction.GetUI() != null)
                {
                    switch (UsingFunction.GetUI().name)
                    {
                        case "T1":
                            //Debug.Log(PartTips.SelfInfo["零件类型"].ToString());
                            Tips_Kind.text = "零件类型为" + PartTips.Kind;
                            ; return;
                        case "T2":
                            Hide_Translucent_Appear("Appear", (GameObject)Info_Part["装配基准面"]);
                            return;
                        case "T3":
                            Hide_Translucent_Appear("Appear", (GameObject)Info_Part["装配基准轴"]);
                            return;
                        case "T4":
                            Tips_Order.text = ("轴" + PartTips.Order.ToString().ToCharArray()[0]
                                + "端面，第" + PartTips.Order.ToString().ToCharArray()[1]
                                + "排零件,第" + PartTips.Order.ToString().ToCharArray()[2]
                                + "层");
                            return;
                        case "左端面":
                            Hide_Translucent_Appear("Appear", (GameObject)Info_Part["左端面"]);
                            return;
                        case "右端面":
                            Hide_Translucent_Appear("Appear", (GameObject)Info_Part["右端面"]);
                            return;
                        default:
                            Hide_Translucent_Appear("Hide", (GameObject)Info_Part["装配基准轴"]);
                            Hide_Translucent_Appear("Hide", (GameObject)Info_Part["装配基准面"]);
                            Hide_Translucent_Appear("Hide", (GameObject)Info_Part["左端面"]);
                            Hide_Translucent_Appear("Hide", (GameObject)Info_Part["右端面"]);
                            ; return;
                    }
                }
                if (UsingFunction.GetUI() == null)
                {
                    Hide_Translucent_Appear("Hide", (GameObject)Info_Part["装配基准轴"]);
                    Hide_Translucent_Appear("Hide", (GameObject)Info_Part["装配基准面"]);
                    Hide_Translucent_Appear("Hide", (GameObject)Info_Part["左端面"]);
                    Hide_Translucent_Appear("Hide", (GameObject)Info_Part["右端面"]);
                }
            }

        }
    }
    
}
