using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class State5 : MonoBehaviour
{
    public State6 St6;
    public State4 St4;
    public AssemblyCollection Collection;
    public ClickManager ClickedObject;

    public WholeTips Tip;
    public GameObject axle;
    bool TheAxisBelongToSelectedObj = false;
    bool Star = false;
    // Start is called before the first frame update
    //void Start()
    //{
    //    Initialization();
    //}

    // Update is called once per frame
    void Update()
    {
        St5();
        //ResetSt();
    }

    public void St5()
    {
        //检查随后点击的物体是否属于选中零件的轴实体且该实体是轴线体
        if (ClickedObject.ClickObj != null)
        {
            axle = null;
            if (UsingFunction.Family(ClickedObject.ClickObj)[0] == Collection.Collection[1]
                && ClickedObject.ClickObj.tag == "装配基准轴")
            {
                axle = ClickedObject.ClickObj;
                TheAxisBelongToSelectedObj = true;
            }
            else if (UsingFunction.Family(ClickedObject.ClickObj)[0] != Collection.Collection[1]
                && ClickedObject.ClickObj != Collection.GearAxis)
            {
                Tip.InputTips("False", "请选择零件上的装配基准轴", 2);
            }
            if (TheAxisBelongToSelectedObj && Star)
            {
                Collection.PartAxis = axle;
                //隐藏零件没被选择的轴线
                UsingFunction.HideAndAppear("Hide", 2, Collection.Collection[1], Collection.PartAxis);
                St6.enabled = true;
                St6.Initialization();
                this.GetComponent<State5>().enabled = false;
            }
        }
        Star = false;

    }

    public void ResetSt()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            ClickedObject.ClickObj = null;
            ClickedObject.ClickObjFamily = null;
            if (Collection.PartAxis == null)
            {
                //发现选择的轴的轴线不对。后退一步，重新选择轴的轴线
                //清空拾取器选择的轴线隐藏所有已经显示的轴线
                St4.enabled = true;
                St4.Initialization();
                this.GetComponent<State5>().enabled = false;
            }
            else
            {
                //发现零件轴选错，需要重新选择轴线
                //重新显示所有轴的轴线
                UsingFunction.HideAndAppear("Appear", 2, Collection.Collection[1], null);
                TheAxisBelongToSelectedObj = false;
                Collection.PartAxis = null;
            }
        }
    }

    public void Initialization()
    {
        Tip = GameObject.Find("UIControler").GetComponent<WholeTips>();
        St4 = this.GetComponent<State4>();
        St6 = this.GetComponent<State6>();
        Collection = this.GetComponent<AssemblyCollection>();
        ClickedObject = this.GetComponent<ClickManager>();
        Star = false;
        axle = null;
        TheAxisBelongToSelectedObj = false;
        //显示所有零件的轴线
        UsingFunction.HideAndAppear("Appear", 2, Collection.Collection[1], null);

        if (ClickedObject.ClickObj != null)
        {
            ClickedObject.ClickObj = null;
            ClickedObject.ClickObjFamily = null;
        }
        else { }
        Tip.InputTips("Tips", "请选择零件装配用的基准轴", 2);
    }
    public void ButtonClick()
    {
        Star = true;
    }

    /// <summary>
    /// 回退至St4重新选择装配基准轴
    /// </summary>
    public void R1()
    {
        ClickedObject.ClickObj = null;
        ClickedObject.ClickObjFamily = null;
        //if (Collection.PartAxis == null)
        //{
        //发现选择的轴的轴线不对。后退一步，重新选择轴的轴线
        //清空拾取器选择的轴线隐藏所有已经显示的轴线
        St4.enabled = true;
        St4.Initialization();
        this.GetComponent<State5>().enabled = false;
        //}
    }
    /// <summary>
    /// 重新选择零件自身基准轴
    /// </summary>
    public void R2()
    {
        Tip.InputTips("Tips", "重新选择装配基准轴", 2);
        ClickedObject.ClickObj = null;
        ClickedObject.ClickObjFamily = null;
        {
            //发现零件轴选错，需要重新选择轴线
            //重新显示所有轴的轴线
            UsingFunction.HideAndAppear("Appear", 2, Collection.Collection[1], null);
            TheAxisBelongToSelectedObj = false;
            Collection.PartAxis = null;
        }
    }
}
