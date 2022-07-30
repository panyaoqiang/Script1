using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tip : MonoBehaviour
{
    public Text[] St1;
    public Text[] St2;
    public Text[] St3;
    public Text[] St4;
    public Text[] St5;
    public Text[] St6;
    public Text[] St7;
    public Text[] St8;
    public Text[] St9;

    public GameObject Controller;
    public AssemblyCollection Collection;
    public ClickManager ClickObj;

    int ActiveState;
    bool Button_TipsInTime = false;
    public bool WhenFalse;
    public GameObject Fork;
    public Text ForkTip;
    public GameObject Caution;
    public Text CautionTip;

    public string TipsText;
    public string CautionOrFalse;
    public float Timer = 0f;

    // Start is called before the first frame update
    void Start()
    {
        InitializedText();
        //Collection = Controller.GetComponent<AssemblyCollection>();
        //选中的轴名称
        St1[1].text = "请选择一根轴作为装配轴";
        //选中的零件名称
        St2[1].text = "请选择对应的零件作为装配零件";
        St3[2].text = "请确认选择的轴和零件";
        //选中的轴的轴线名称
        //St4[1].text = "";
        St4[2].text = "选择已选中轴的轴线作为对准轴线";
        //选中的零件的轴线名称
        //St5[1].text = "";
        St5[2].text = "选择已选中零件的轴线作为对准轴线";

        //St6[2].text = "";
        //选中的轴的装配面
        //St7[1].text = "";
        St7[2].text = "选择已选中轴的装配面";
        //选中的零件装配面
        //St8[1].text = "";
        St8[2].text = "选择已选中零件的装配面";
        St9[2].text = "点击开始装配";
        //St10[2].text = "点击确认完成装配";
    }

    // Update is called once per frame
    void Update()
    {
        ActiveState = UsingFunction.StateListener(Controller);
        ObjNamePrint(ActiveState);
        //Debug.Log(WholeWorldTips());
        if (WhenFalse)
        {
            Button_TipsInTime = true;
            WhenFalse = false;
        }
        WholeWorldTips();
    }

    /// <summary>
    /// 一个装配步骤有：
    /// 开始未选择click已存在，collect未存在，此时click与上一步赋值相同
    /// 选择未确认click已存在，collect未存在，此时clikc与上一步赋值不同
    /// 选择完毕click已存在，collect已存在，此时判断collect
    /// 重新选择click已存在，collect未存在，此时click
    /// </summary>
    /// <param name="ActiveState"></param>
    public void ObjNamePrint(int ActiveState)
    {
        switch (ActiveState)
        {
            case 1:
                {
                    if (Collection.Collection[0] == null)
                    {
                        if (ClickObj.ClickObj != null)
                        {
                            if (ClickObj.ClickObj.tag == "装配轴")
                            {
                                St1[0].text = ClickObj.ClickObj.name;
                                St1[1].text = "已选择" + St1[0].text + "作为装配轴";
                            }
                            else
                            {
                                St1[0].text = ClickObj.ClickObj.name;
                                St1[1].text = "请选择任意轴作为装配轴";
                            }
                        }
                        else
                        {
                            St1[0].text = "装配轴";
                        }
                    }
                    //两种返回情况
                    if (Collection.Collection[0] != null && ClickObj.ClickObj == null)
                    {
                        if (Collection.Collection[0].tag == "装配轴")
                        {
                            St1[0].text = Collection.Collection[0].name;
                            St1[1].text = "已选择" + St1[0].text + "作为装配轴";
                        }
                        else
                        {
                            St1[0].text = Collection.Collection[0].name;
                            St1[1].text = "请选择重新任意轴作为装配轴";
                        }

                    }
                    if (Collection.Collection[0] != null && ClickObj.ClickObj != null)
                    {
                        if (ClickObj.ClickObj.tag == "装配轴")
                        {
                            St1[0].text = ClickObj.ClickObj.name;
                            St1[1].text = "已选择" + St1[0].text + "作为装配轴";
                        }
                        else
                        {
                            St1[0].text = ClickObj.ClickObj.name;
                            St1[1].text = "请选择重新任意轴作为装配轴";
                        }
                    }

                }
                ; return;
            case 2:
                {
                    if (Collection.Collection[1] == null)
                    {
                        if (ClickObj.ClickObj != null)
                        {
                            St2[0].text = ClickObj.ClickObj.name;
                            if (UsingFunction.Family(Collection.Collection[0])[4].GetComponent<AxleInfo>().
                                AllParts.Contains(ClickObj.ClickObj))
                            {
                                St2[1].text = "已选择" + St2[0].text + "进行装配，选择正确";
                            }
                            if (!UsingFunction.Family(Collection.Collection[0])[4].GetComponent<AxleInfo>().
                                AllParts.Contains(ClickObj.ClickObj) && ClickObj.ClickObj.tag == "装配零件")
                            {
                                St2[1].text = "选择错误，该零件的装配轴不是" + St2[0].text + "，请重新选择";
                            }
                            if (ClickObj.ClickObj.tag != "装配零件")
                            {
                                St2[1].text = "请选择一个零件与该轴进行装配";
                            }
                        }
                        else
                        {
                            St2[0].text = "装配零件";
                        }
                    }
                    if (Collection.Collection[1] != null && ClickObj.ClickObj == null)
                    {
                        St2[0].text = Collection.Collection[1].name;
                        if (UsingFunction.Family(Collection.Collection[0])[4].GetComponent<AxleInfo>().
                                AllParts.Contains(Collection.Collection[1]))
                        {
                            St2[1].text = "已选择" + St2[0].text + "进行装配，选择正确";
                        }
                        if (!UsingFunction.Family(Collection.Collection[0])[4].GetComponent<AxleInfo>().
                                AllParts.Contains(Collection.Collection[1]) && Collection.Collection[1].tag == "装配零件")
                        {
                            St2[1].text = "选择错误，该零件的装配轴不是" + St2[0].text + "，请重新选择";
                        }
                        if (Collection.Collection[1].tag != "装配零件")
                        {
                            St2[1].text = "请选择一个零件与该轴进行装配";
                        }
                    }
                    if (Collection.Collection[1] != null && ClickObj.ClickObj != null)
                    {
                        St2[0].text = ClickObj.ClickObj.name;
                        if (UsingFunction.Family(Collection.Collection[0])[4].GetComponent<AxleInfo>().
                                AllParts.Contains(ClickObj.ClickObj))
                        {
                            St2[1].text = "重新选择" + St2[0].text + "进行装配，选择正确";
                        }
                        if (!UsingFunction.Family(Collection.Collection[0])[4].GetComponent<AxleInfo>().
                                AllParts.Contains(ClickObj.ClickObj) && ClickObj.ClickObj.tag == "装配零件")
                        {
                            St2[1].text = "选择错误，该零件的装配轴不是" + St2[0].text + "，请重新选择";
                        }
                        if (ClickObj.ClickObj.tag != "装配零件")
                        {
                            St2[1].text = "请重新选择一个零件与该轴进行装配";
                        }
                    }
                }
                ; return;
            case 3:
                {
                    St3[0].text = Collection.Collection[0].name;
                    St3[1].text = Collection.Collection[1].name;
                }
                ; return;
            case 4:
                {
                    St4[0].text = Collection.Collection[0].name;
                    if (Collection.GearAxis == null)
                    {
                        if (ClickObj.ClickObj != null)
                        {
                            St4[1].text = ClickObj.ClickObj.name;
                            //if (((GameObject)UsingFunction.Family(Collection.Collection[1])[4].GetComponent
                            //    <PartInfo>().AssembleInfo["装配基准轴"]) == ClickObj.ClickObj)
                            //{
                            //    St4[2].text = "已选择轴的基准轴" + St4[1].text + "作为装配基准轴，选择正确";
                            //}
                            //if (!((GameObject)UsingFunction.Family(Collection.Collection[1])[4].GetComponent<PartInfo>().
                            //    AssembleInfo["装配基准轴"]) == ClickObj.ClickObj && ClickObj.ClickObj.tag == "装配基准轴")
                            //{
                            //    St4[2].text = "选择错误，零件的装配基准轴不是" + St4[1].text + "，请重新选择";
                            //}
                            if (ClickObj.ClickObj.tag != "装配基准轴")
                            {
                                St4[2].text = "请选择装配轴的轴线作为装配基准轴";
                            }
                        }
                        else
                        {
                            St4[1].text = "轴的装配基准轴";
                        }
                    }
                    //两种返回
                    if (Collection.GearAxis != null && ClickObj.ClickObj == null)
                    {
                        St4[1].text = Collection.GearAxis.name;
                        //if (((GameObject)UsingFunction.Family(Collection.Collection[1])[4].GetComponent<PartInfo>().
                        //        AssembleInfo["装配基准轴"]) == Collection.GearAxis)
                        //{
                        //    St4[2].text = "已选择轴的基准轴" + St4[1].text + "作为装配基准轴，选择正确";
                        //}
                        //if (!((GameObject)UsingFunction.Family(Collection.Collection[1])[4].GetComponent<PartInfo>().
                        //        AssembleInfo["装配基准轴"]) == Collection.GearAxis && Collection.GearAxis.tag == "装配基准轴")
                        //{
                        //    St4[2].text = "选择错误，零件的装配基准轴不是" + St4[1].text + "，请重新选择";
                        //}
                        if (Collection.GearAxis.tag != "装配基准轴")
                        {
                            St4[2].text = "请重新选择装配轴的轴线作为装配基准轴";
                        }
                    }
                    if (Collection.GearAxis != null && ClickObj.ClickObj != null)
                    {
                        St4[1].text = ClickObj.ClickObj.name;
                        //if (((GameObject)UsingFunction.Family(Collection.Collection[1])[4].GetComponent<PartInfo>().
                        //        AssembleInfo["装配基准轴"]) == ClickObj.ClickObj)
                        //{
                        //    St4[2].text = "重新选择轴的基准轴" + St4[1].text + "作为装配基准轴，选择正确";
                        //}
                        //if (!((GameObject)UsingFunction.Family(Collection.Collection[1])[4].GetComponent<PartInfo>().
                        //        AssembleInfo["装配基准轴"]) == ClickObj.ClickObj && ClickObj.ClickObj.tag == "装配基准轴")
                        //{
                        //    St4[2].text = "选择错误，零件的装配基准轴不是" + St4[1].text + "，请重新选择";
                        //}
                        if (ClickObj.ClickObj.tag != "装配基准轴")
                        {
                            St4[2].text = "请重新选择装配轴的轴线作为装配基准轴";
                        }
                    }
                }
                ; return;
            //零件（齿轮，轴承，轴套，紧固环，同步器除去行星架，拨叉）只有一根轴线
            case 5:
                {
                    St5[0].text = Collection.Collection[1].gameObject.name;
                    if (Collection.PartAxis == null)
                    {
                        if (ClickObj.ClickObj != null && ClickObj.ClickObj.tag == "装配基准轴"
                            && ClickObj.ClickObjFamily[0] == Collection.Collection[1])
                        {
                            St5[1].text = ClickObj.ClickObj.name;
                            St5[2].text = "已选择零件基准轴" + St5[1].text + "与装配基准轴对准";
                        }
                        if (ClickObj.ClickObj != null && (ClickObj.ClickObj.tag != "装配基准轴"
                            || ClickObj.ClickObjFamily[0] != Collection.Collection[1]))
                        {
                            St5[1].text = ClickObj.ClickObj.name;
                            St5[2].text = "请选择零件的基准轴与装配基准轴进行对准";
                        }
                        else
                        {
                            St5[1].text = "零件的装配基准轴";
                        }
                    }
                    if (Collection.PartAxis != null && ClickObj.ClickObj == null)
                    {
                        if (ClickObj.ClickObj != null && Collection.PartAxis.tag == "装配基准轴")
                        {
                            St5[1].text = ClickObj.ClickObj.name;
                            St5[2].text = "已选择零件基准轴" + St5[1].text + "与装配基准轴对准";
                        }
                        if (ClickObj.ClickObj != null && Collection.PartAxis.tag != "装配基准轴")
                        {
                            St5[1].text = ClickObj.ClickObj.name;
                            St5[2].text = "请重新选择零件基准轴与装配基准轴进行对准";
                        }
                    }
                    if (Collection.PartAxis != null && ClickObj.ClickObj != null)
                    {
                        if (ClickObj.ClickObj != null && ClickObj.ClickObj.tag == "装配基准轴")
                        {
                            St5[1].text = ClickObj.ClickObj.name;
                            St5[2].text = "已重新选择零件基准轴" + St5[1].text + "与装配基准轴对准";
                        }
                        if (ClickObj.ClickObj != null && ClickObj.ClickObj.tag != "装配基准轴")
                        {
                            St5[1].text = ClickObj.ClickObj.name;
                            St5[2].text = "请重新选择零件基准轴与装配基准轴进行对准";
                        }
                    }
                }
                ; return;
            //按照GetUI提示
            case 6:
                {
                    St6[0].text = Collection.Collection[0].name;
                    St6[1].text = Collection.Collection[1].name;
                    if (UsingFunction.GetUI() != null)
                    {
                        St6[2].text = State6Tips(UsingFunction.GetUI().name);
                    }
                }
                ; return;
            case 7:
                {
                    St7[0].text = Collection.Collection[0].name;
                    if (Collection.GearFace == null)
                    {
                        if (ClickObj.ClickObj != null)
                        {
                            St7[1].text = ClickObj.ClickObj.name;
                            //if (((GameObject)UsingFunction.Family(Collection.Collection[1])[4].GetComponent<PartInfo>().
                            //    AssembleInfo["装配基准面"]) == ClickObj.ClickObj && ClickObj.ClickObj.tag == "装配基准面"
                            //    && ClickObj.ClickObjFamily[0] == Collection.Collection[0])
                            //{
                            //    St7[2].text = "已选择轴的基准面" + St7[1].text + "作为装配基准面，选择正确";
                            //}
                            //if (!((GameObject)UsingFunction.Family(Collection.Collection[1])[4].GetComponent<PartInfo>().
                            //    AssembleInfo["装配基准面"]) == ClickObj.ClickObj && ClickObj.ClickObj.tag == "装配基准面"
                            //    && ClickObj.ClickObjFamily[0] == Collection.Collection[0])
                            //{
                            //    St7[2].text = "选择错误，零件的装配基准面不是" + St7[1].text + "，请重新选择";
                            //}
                            if (ClickObj.ClickObj.tag != "装配基准面" || ClickObj.ClickObjFamily[0] != Collection.Collection[0])
                            {
                                St7[2].text = "请选择轴的基准面作为装配基准面";
                            }
                        }
                        else
                        {
                            St7[1].text = "轴的装配基准面";
                        }
                    }
                    if (Collection.GearFace != null && ClickObj.ClickObj == null)
                    {
                        St7[1].text = Collection.GearFace.name;
                        //if (((GameObject)UsingFunction.Family(Collection.Collection[1])[4].GetComponent<PartInfo>().
                        //        AssembleInfo["装配基准面"]) == Collection.GearFace && Collection.GearFace.tag == "装配基准面")
                        //{
                        //    St7[2].text = "已选择轴的基准面" + St7[1].text + "作为装配基准面，选择正确";
                        //}
                        //if (!((GameObject)UsingFunction.Family(Collection.Collection[1])[4].GetComponent<PartInfo>().
                        //        AssembleInfo["装配基准面"]) == Collection.GearFace && Collection.GearFace.tag == "装配基准面")
                        //{
                        //    St7[2].text = "选择错误，零件的装配基准面不是" + St7[1].text + "，请重新选择";
                        //}
                        if (Collection.GearFace.tag != "装配基准面")
                        {
                            St7[2].text = "请重新选择轴的一个基准面作为装配基准面";
                        }
                    }
                    if (Collection.GearFace != null && ClickObj.ClickObj != null)
                    {
                        St7[1].text = ClickObj.ClickObj.name;
                        //if (((GameObject)UsingFunction.Family(Collection.Collection[1])[4].GetComponent<PartInfo>().
                        //        AssembleInfo["装配基准面"]) == ClickObj.ClickObj && ClickObj.ClickObj.tag == "装配基准面")
                        //{
                        //    St7[2].text = "重新选择轴的基准面" + St7[1].text + "作为装配基准面，选择正确";

                        //}
                        //if (!((GameObject)UsingFunction.Family(Collection.Collection[1])[4].GetComponent<PartInfo>().
                        //        AssembleInfo["装配基准面"]) == ClickObj.ClickObj && ClickObj.ClickObj.tag == "装配基准面")
                        //{
                        //    St7[2].text = "选择错误，零件的装配基准面不是" + St7[1].text + "，请重新选择";
                        //}
                        if (ClickObj.ClickObj.tag != "装配基准面")
                        {
                            St7[2].text = "请重新选择轴的一个基准面作为装配基准面";
                        }
                    }
                }
                ; return;
            //设置提示或从左边装配则选择右端面。从右边装配则选择左端面反之
            case 8:
                {
                    St8[0].text = Collection.Collection[1].name;
                    if (Collection.PartFace == null)
                    {
                        if (ClickObj.ClickObj != null && ClickObj.ClickObj.tag == "装配基准面"
                            && ClickObj.ClickObjFamily[0] == Collection.Collection[1])
                        {
                            St8[1].text = ClickObj.ClickObj.name;
                            St8[2].text = "已选择零件基准面" + St8[1].text + "作为装配基准面";
                        }
                        if (ClickObj.ClickObj != null && (ClickObj.ClickObj.tag != "装配基准面"
                            || ClickObj.ClickObjFamily[0] != Collection.Collection[1]))
                        {
                            St8[1].text = ClickObj.ClickObj.name;
                            St8[2].text = "请选择零件的一个基准面作为与装配基准面对齐的基准面";
                        }
                        else
                        {
                            St8[1].text = "零件的装配基准面";
                        }
                    }
                    if (Collection.PartFace != null && ClickObj.ClickObj == null)
                    {
                        St8[1].text = Collection.PartFace.name;
                        St8[2].text = "已选择零件基准面" + St8[1].text + "作为装配基准面";
                    }
                    if (Collection.PartFace != null && ClickObj.ClickObj != null)
                    {
                        St8[1].text = ClickObj.ClickObj.name;
                        St8[2].text = "重新选择零件基准面" + St8[1].text + "作为装配基准面";
                    }
                }
                ; return;
            //装配完成后检测装配位置提示点击取消或完成
            case 9:
                {
                    St9[0].text = Collection.Collection[0].name;
                    St9[1].text = Collection.Collection[1].name;
                    if (UsingFunction.GetUI() != null)
                    {
                        switch (UsingFunction.GetUI().name)
                        {
                            case ("开始装配"): St9[2].text = "使选择的装配基准面与零件基准面共面"; break;
                            case ("取消装配"): St9[2].text = "取消装配并复位零件"; break;
                            default: St9[2].text = "点击开始装配按钮进行装配"; break;
                        }
                    }
                }
                ; return;
            default:; return;
        }
    }
    //每次激活脚本必须先初始化
    //初始化用作清空Text显示的内容用作赋值
    public void InitializedText()
    {
        St1[1].text = "请选择一根轴作为装配轴";
        St2[1].text = "请选择对应的零件作为装配零件";
        St3[2].text = "请确认选择的轴和零件";
        St4[2].text = "选择已选中轴的轴线作为对准轴线";
        St5[2].text = "选择已选中零件的轴线作为对准轴线";
        St6[2].text = "先选择对准，再选择轴的左右端面";
        St7[2].text = "选择已选中轴的装配面";
        St8[2].text = "选择已选中零件的装配面";
        St9[2].text = "点击开始装配按钮进行装配";
        //St10[2].text = "点击确认完成装配";

    }

    /// <summary>
    /// 判断state选择物体是否正确
    /// </summary>
    /// <param name="Info">设置好的物体</param>
    /// <param name="Choice">当前选择物体</param>
    /// <param name="TrueTips">选择正确提示</param>
    /// <param name="FalseTips">选择不正确提示</param>
    /// <param name="Broad">打印提示的UI</param>
    public void CheckRightOrFalse(GameObject Info, GameObject Choice, string TrueTips, string FalseTips, Text Broad)
    {
        if (Choice != null)
        {
            if (Choice == Info)
            {
                Broad.text = TrueTips;
            }
            if (Choice != Info)
            {
                Broad.text = FalseTips;
            }
        }
    }

    /// <summary>
    /// 传入UI组件的Tag
    /// </summary>
    /// <param name="UIName">对齐/取消对齐/左端面/右端面/取消</param>
    /// <param name="Tips">打印提示</param>
    public string State6Tips(string UIName)
    {
        string Tips;
        switch (UIName)
        {
            case ("对齐"): Tips = "选中轴线对齐装配基准轴"; break;
            case ("取消对齐"): Tips = "复位零件"; break;
            case ("左端面"): Tips = "从轴的左端满装配"; break;
            case ("右端面"): Tips = "从轴的右端面装配"; break;
            case ("取消"): Tips = "重新选择端面"; break;
            default: Tips = ""; break;
        }
        return Tips;
    }

    //public void Button_WholeWorldTip(float Time)
    //{
    //    Button_TipsInTime = true;
    //    Timer = Time;
    //}

    //public bool WholeWorldTip()
    //{
    //    if (Button_TipsInTime)
    //    {
    //        if (Timer > 0)
    //        {
    //            Timer -= Time.deltaTime;
    //            Tip.gameObject.SetActive(true);
    //            return true;
    //        }
    //        if (Timer <= 0)
    //        {
    //            Timer = 2f;
    //            Button_TipsInTime = false;
    //            Tip.gameObject.SetActive(false);
    //            return false;
    //        }
    //    }
    //    ForkTip.gameObject.SetActive(false);
    //    return false;
    //}
    public void WholeWorldTips()
    {
        if (TipsText != null && CautionOrFalse != null && Timer != 0f)
        {
            if (CautionOrFalse == "Caution")
            {
                if (Timer > 0)
                {
                    Timer -= Time.deltaTime;
                    Caution.SetActive(true);
                    CautionTip.text = TipsText;
                }
                if (Timer <= 0)
                {
                    Caution.SetActive(false);
                    InitializationTips();
                }
            }
            else if (CautionOrFalse == "False")
            {
                if (Timer > 0)
                {
                    Timer -= Time.deltaTime;
                    Fork.SetActive(true);
                    CautionTip.text = TipsText;
                }
                if (Timer <= 0)
                {
                    Fork.SetActive(false);
                    InitializationTips();
                }
            }
        }
    }
    /// <summary>
    /// 调用全局错误信息提醒按钮
    /// </summary>
    /// <param name="COF">Caution或者False</param>
    /// <param name="Texture">要提示的内容</param>
    /// <param name="T">显示时长，内容较少时建议2f</param>
    public void InputTips(string COF, string Texture, float T)
    {
        CautionOrFalse = COF;
        TipsText = Texture;
        Timer = T;
    }
    public void InitializationTips()
    {
        TipsText = null;
        CautionOrFalse = null;
        Timer = 0f;
    }
}

