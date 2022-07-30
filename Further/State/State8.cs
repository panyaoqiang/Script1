using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class State8 : MonoBehaviour
{
    public State9 St9;
    public State7 St7;
    public Text text;
    public AssemblyCollection Collection;
    public ClickManager ClickedObject;
    public WholeTips Tip;
    GameObject F;
    bool TheFaceBelongToSelectedObj = false;
    bool Star = false;
    char LeftOrRight;
    // Start is called before the first frame update
    //void Start()
    //{
    //    Initialization();
    //}

    // Update is called once per frame
    void Update()
    {
        St8();
        //ResetSt();
    }


    public void St8()
    {
        LeftOrRight = UsingFunction.Family(Collection.Collection[1])[4]
        .GetComponent<PartInfo>().Order.ToCharArray()[0];

        //检查随后点击的物体是否属于选中零件的面实体
        if (ClickedObject.ClickObj != null)
        {
            F = null;
            if (UsingFunction.Family(ClickedObject.ClickObj)[0] == Collection.Collection[1]
                && ClickedObject.ClickObj.tag == "装配基准面")//面片名称更改
            {
                //if (LeftOrRight == '左')
                //{
                //if (ClickedObject.ClickObj == ((GameObject)UsingFunction.Family
                //    (Collection.Collection[1])[4].GetComponent<PartInfo>().SelfInfo["左端面"]))
                //{
                //text.text=("选择正确，零件自身右\n端面与装配基准面对齐");
                F = ClickedObject.ClickObj;
                TheFaceBelongToSelectedObj = true;
                //}
                //else { text.text = ("选择错误，请重新选择基准面"); }
                //}
                //else
                //{
                //    if (ClickedObject.ClickObj == ((GameObject)UsingFunction.Family
                //        (Collection.Collection[1])[4].GetComponent<PartInfo>().SelfInfo["右端面"]))
                //    {
                //        text.text = ("选择正确，零件自身左\n端面与装配基准面对齐");
                //        F = ClickedObject.ClickObj;
                //        TheFaceBelongToSelectedObj = true;
                //    }
                //    else { text.text = ("选择错误，请重新选择基准面"); }
                //}
                //装入点击中的面实体
            }
            else if (UsingFunction.Family(ClickedObject.ClickObj)[0] != Collection.Collection[1]
                && ClickedObject.ClickObj!= Collection.GearFace)
            {
                Tip.InputTips("False", "请选择装配轴上基准面", 2f);
            }
            if (Star && TheFaceBelongToSelectedObj)
            {
                Collection.PartFace = F;
                //从轴左边装配，应选择右端面作为对齐面
                if (LeftOrRight == '左')
                {
                    if (Collection.PartFace.transform.localPosition != UsingFunction.Family
                        (Collection.Collection[1])[4].GetComponent<PartInfo>().RightFace)
                    {
                        Tip.InputTips("Caution", "请按照装配规范装配", 2f);
                    }
                }
                //从轴左边装配，应选择左端面作为对齐面
                else if (LeftOrRight == '右')
                {
                    if (Collection.PartFace.transform.localPosition != UsingFunction.Family
                        (Collection.Collection[1])[4].GetComponent<PartInfo>().LeftFace)
                    {
                        Tip.InputTips("Caution", "请按照装配规范装配", 2f);
                    }
                }
                UsingFunction.HideAndAppear("Hide", 3, Collection.Collection[1], Collection.PartFace);
                St9.enabled = true;
                St9.Initialization();
                this.GetComponent<State8>().enabled = false;
            }
        }
        //装配过程中发现选择面错误，重选零件的面
        //显示所有零件的面
        Star = false;
    }

    public void ResetSt()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            //点击R键重置选择
            if (Input.GetKeyDown(KeyCode.R))
            {
                ClickedObject.ClickObj = null;
                ClickedObject.ClickObjFamily = null;

                //当未选择面时，重做State7，重选的轴的面
                if (Collection.PartFace == null)
                {
                    //后退一步，显示所有隐藏的面
                    St7.enabled = true;
                    St7.Initialization();
                    this.GetComponent<State8>().enabled = false;
                }
                else
                {
                    //重新选择已经选择的零件的面
                    UsingFunction.HideAndAppear("Appear", 3, Collection.Collection[1], null);
                    TheFaceBelongToSelectedObj = false;
                    Collection.PartFace = null;
                }
            }
        }
    }

    public void Initialization()
    {
        Tip = GameObject.Find("UIControler").GetComponent<WholeTips>();
        St7 = this.GetComponent<State7>();
        St9 = this.GetComponent<State9>();
        Collection = this.GetComponent<AssemblyCollection>();
        ClickedObject = this.GetComponent<ClickManager>();
        Star = false;
        F = null;
        TheFaceBelongToSelectedObj = false;

        //显示零件所有的面
        UsingFunction.HideAndAppear("Appear", 3, Collection.Collection[1], null);

        if (ClickedObject.ClickObj != null)
        {
            ClickedObject.ClickObj = null;
            ClickedObject.ClickObjFamily = null;
        }
        else { }
        Tip.InputTips("Tips", "选择零件装配用的基准面", 2f);
    }
    public void ButtonClick()
    {
        Star = true;
    }

    /// <summary>
    /// 回退St7重选装配基准面，隐藏已显示的面
    /// </summary>
    public void R1()
    {
        ClickedObject.ClickObj = null;
        ClickedObject.ClickObjFamily = null;

        //当未选择面时，重做State7，重选的轴的面
        //if (Collection.PartFace == null)
        {
            //后退一步，显示所有隐藏的面
            St7.enabled = true;
            St7.Initialization();
            this.GetComponent<State8>().enabled = false;
        }
    }
    /// <summary>
    /// 重新选择已经选择的零件的面
    /// </summary>
    public void R2()
    {
        ClickedObject.ClickObj = null;
        ClickedObject.ClickObjFamily = null;

        //if (Collection.PartFace != null)
        {
            UsingFunction.HideAndAppear("Appear", 3, Collection.Collection[1], null);
            TheFaceBelongToSelectedObj = false;
            Collection.PartFace = null;
        }
        Tip.InputTips("Tips", "重新选择零件装配用的基准面", 2f);
    }
}
