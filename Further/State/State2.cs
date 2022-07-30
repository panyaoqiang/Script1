using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class State2 : MonoBehaviour
{
    public State1 St1;
    public State3 St3;
    public ClickManager ClickObj;
    public AssemblyCollection Collection;
    public WholeTips Tip;
    bool Star = false;
    // Start is called before the first frame update
    //void Start()
    //{
    //    Initialization();
    //}

    // Update is called once per frame
    void Update()
    {
        //ResetSt();
        St2();
    }

    public void St2()
    {
        //选中零件是否属于零件
        if (Star && ClickObj.ClickObjFamily[0] != null && ClickObj.ClickObjFamily[0]
            != Collection.Collection[0] && (ClickObj.ClickObj.tag == "装配零件"|| ClickObj.ClickObj.tag == "壳体")) 
        {
            //拾取器装入零件主体
            Collection.Collection[1] = this.GetComponent<ClickManager>().ClickObjFamily[0];
            if(!UsingFunction.Family(Collection.Collection[0])[4].GetComponent<AxleInfo>().AllPartsName
                .Contains(Collection.Collection[1].name))
            {
                Tip.InputTips("Caution", "建议按照装配方案\n选择与该轴装配的零件", 1);
            }

            St3.enabled = true;
            //初始化St3
            St3.Initialization();
            this.GetComponent<State2>().enabled = false;
        }
        //开始未选择，此时点击物体仍然与上一步选择物体相同，监测开始选择但未确认的状态
        if (ClickObj.ClickObj != null && ClickObj.ClickObjFamily[0]
            != Collection.Collection[0] && ClickObj.ClickObj.tag != "装配零件"
            )//&&ClickObj.ClickObjFamily[0].name!="右一轴"
        {
            Tip.InputTips("False", "请选择一个零件", 2);
        }
        Star = false;
    }

    public void ResetSt()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (Collection.Collection[1] != null)
            {
                //清空选中的零件
                ClickObj.ClickObj = null;
                ClickObj.ClickObjFamily = null;
                Collection.Collection[1] = null;
            }
            else
            {
                St1.enabled = true;
                St1.Initialization();
                this.GetComponent<State2>().enabled = false;
            }
        }
    }

    public void Initialization()
    {
        Collection = this.GetComponent<AssemblyCollection>();
        St3 = this.GetComponent<State3>();
        St1 = this.GetComponent<State1>();
        ClickObj = this.GetComponent<ClickManager>();
        Tip = GameObject.Find("UIControler").GetComponent<WholeTips>();
        Star = false;
        
        if (ClickObj.ClickObj != null)
        {
            ClickObj.ClickObj = null;
            ClickObj.ClickObjFamily = null;
        }
        else { }
        Tip.InputTips("Tips", "请选择一个零件", 2);
    }

    public void ButtonClick()
    {
        Star = true;
    }

    /// <summary>
    /// 取消选择零件
    /// </summary>
    public void R1()
    {
        //if (Collection.Collection[1] != null)
        //{
        //清空选中的零件
        ClickObj.ClickObj = null;
            ClickObj.ClickObjFamily = null;
            Collection.Collection[1] = null;
        //}
    }
    /// <summary>
    /// 回退至St1
    /// </summary>
    public void R2()
    {
        //if (Collection.Collection[1] == null)
        //{
        St1.enabled = true;
            St1.Initialization();
            this.GetComponent<State2>().enabled = false;
        //}
    }
}
