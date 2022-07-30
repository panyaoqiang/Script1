using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State4 : MonoBehaviour
{
    public State5 St5;
    public State3 St3;
    //public State10 St10;
    public AssemblyCollection Collection;
    public ClickManager ClickedObject;
    public WholeTips Tip;
    bool TheAxisBelongToSelectedObj = false;
    public GameObject G;
    bool Star = false;
    // Start is called before the first frame update
    //void Start()
    //{
    //    Initialization();
    //}
    void Update()
    {
        St4();
        //ResetSt();
    }

    public void St4()
    {
        //当脚本从3激活拾取器显示选择的轴和零件
        //轴和零件上的所有轴线显示

        //检查随后点击的物体是否属于选中物体的轴实体
        if (ClickedObject.ClickObj != null)
        {
            //当点击的物体的主体属于选中的轴,且该物体是轴线体
            if (UsingFunction.Family(ClickedObject.ClickObj.gameObject)[0] == Collection.Collection[0]
                && ClickedObject.ClickObj.tag == "装配基准轴")//&&((GameObject)UsingFunction.Family(Collection.Collection[1]
                                                         //)[4].GetComponent<PartInfo>().AssembleInfo["装配基准轴"])==ClickedObject.ClickObj轴线体的名称更改
            {
                G = ClickedObject.ClickObj.gameObject;
                TheAxisBelongToSelectedObj = true;
            }
            //排除未选择，click仍为第二步选择的装配零件,监测已经开始选择
            else if (ClickedObject.ClickObj != Collection.Collection[1] &&
                UsingFunction.Family(ClickedObject.ClickObj)[0] != Collection.Collection[0])
            {
                Tip.InputTips("False", "请选择装配轴上的装配基准轴", 2);
            }
            if (Star && TheAxisBelongToSelectedObj)
            {
                Collection.GearAxis = G;
                //隐藏没选中的轴线
                //Collection.Collection[1] = this.GetComponent<ClickManager>().ClickObjFamily[0];
                //选择了装配主体上的基准面，但此基准面非装配设定的基准面
                if (UsingFunction.Family(Collection.Collection[1])[4].GetComponent<PartInfo>().DatumAxle
                    == (Collection.GearAxis.transform.localPosition))
                {
                    Tip.InputTips("Caution", "建议按照装配方案，选择正确的装配基准轴\n可打开操作面板查看零件装配信息", 2);
                }
                UsingFunction.HideAndAppear("Hide", 2, Collection.Collection[0], Collection.GearAxis);
                St5.enabled = true;
                St5.Initialization();
                this.GetComponent<State4>().enabled = false;
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
            if (Collection.GearAxis == null)
            {
                //发现选择的轴不对。后退一步，隐藏所有已经显示的轴线
                //退回第三部，拾取器显示选择的轴和零件
                St3.enabled = true;
                St3.Initialization();
                this.GetComponent<State4>().enabled = false;
            }
            else
            {
                //发现选择的轴线不对，重新选择轴的轴线
                UsingFunction.HideAndAppear("Appear", 2, Collection.Collection[0], null);
                TheAxisBelongToSelectedObj = false;
                Collection.GearAxis = null;
            }
        }
    }

    public void Initialization()
    {
        Tip = GameObject.Find("UIControler").GetComponent<WholeTips>();

        St3 = this.GetComponent<State3>();
        St5 = this.GetComponent<State5>();
        Collection = this.GetComponent<AssemblyCollection>();
        ClickedObject = this.GetComponent<ClickManager>();
        Star = false;
        G = null;
        TheAxisBelongToSelectedObj = false;
        //显示所有轴的轴线
        UsingFunction.HideAndAppear("Appear", 2, Collection.Collection[0], null);

        if (ClickedObject.ClickObj != null)
        {
            ClickedObject.ClickObj = null;
            ClickedObject.ClickObjFamily = null;
        }
        else { }
        Tip.InputTips("Tips", "选择装配轴上的装配基准轴", 2);
    }
    public void ButtonClick()
    {
        Star = true;
    }

    /// <summary>
    /// 回退至St3重新确认
    /// </summary>
    public void R1()
    {
        ClickedObject.ClickObj = null;
        ClickedObject.ClickObjFamily = null;
        //if (Collection.GearAxis == null)
        //{
        //发现选择的轴不对。后退一步，隐藏所有已经显示的轴线
        //退回第三部，拾取器显示选择的轴和零件
        St3.enabled = true;
        St3.Initialization();
        this.GetComponent<State4>().enabled = false;
        //}
    }
    /// <summary>
    /// 重新选择装配基准轴
    /// </summary>
    public void R2()
    {
        Tip.InputTips("Tips", "重新选择装配基准轴", 2);
        ClickedObject.ClickObj = null;
        ClickedObject.ClickObjFamily = null;
        //if (Collection.GearAxis!=null)
        //{
        //发现选择的轴线不对，重新选择轴的轴线
        UsingFunction.HideAndAppear("Appear", 2, Collection.Collection[0], null);
        TheAxisBelongToSelectedObj = false;
        Collection.GearAxis = null;
        //}
    }
}
