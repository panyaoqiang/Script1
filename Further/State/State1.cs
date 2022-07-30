using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class State1 : MonoBehaviour
{
    public State2 St2;
    public State0 St0;
    public ClickManager ClickObj;
    public AssemblyCollection Collection;
    public WholeTips Tip;

    /// <summary>
    /// 未选择，正常选择，返回重选，返回未重选
    /// </summary>
    //public int TrueOrFalse = 0;

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
        St1();
    }

    public void St1()
    {
        //提示

        if (Star && ClickObj.ClickObjFamily[0] != null)
        {
            //选中物体是否为轴
            if (ClickObj.ClickObjFamily[0].tag == "装配轴"|| ClickObj.ClickObjFamily[0].tag == "壳体")// && AllList[0] == null
            {
                //拾取器装入轴主体
                Collection.Collection[0] = ClickObj.ClickObjFamily[0];
                //初始化St2
                St2.enabled = true;
                St2.Initialization();
                this.GetComponent<State1>().enabled = false;
            }
        }
        //已选择但选择错误
        else if (ClickObj.ClickObjFamily[0].tag != "装配轴" && ClickObj.ClickObjFamily[0].tag != "壳体" && ClickObj.ClickObj != null)//||ClickObj.ClickObj==null
        {
            Tip.InputTips("False", "请选择轴或壳体", 2);
        }
        Star = false;
    }

    public void ResetSt()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            //清空拾取器内已经选择的轴
            if (Collection.Collection[0] != null)
            {
                ClickObj.ClickObj = null;
                ClickObj.ClickObjFamily = null;
                Collection.Collection[0] = null;
                //材质替换
            }
            //回退一步
            if (Collection.Collection[0] == null)
            {
                ClickObj.ClickObj = null;
                ClickObj.ClickObjFamily = null;
                //初始化St0
                St0.enabled = true;
                St0.Initialization();
                //关闭St1
                this.GetComponent<State1>().enabled = false;
            }
        }
    }
    /// <summary>
    /// 重新选择轴
    /// </summary>
    public void R1()
    {
        //if (Collection.Collection[0] != null)
        //{
        Tip.InputTips("Tips", "重新选择", 2);
        ClickObj.ClickObj = null;
        ClickObj.ClickObjFamily = null;
        Collection.Collection[0] = null;
        //材质替换
        //}
    }
    /// <summary>
    /// 回退至St0
    /// </summary>
    public void R2()
    {
        //if (Collection.Collection[0] == null)
        //{
        Tip.InputTips("Tips", "取消装配", 2);
        ClickObj.ClickObj = null;
        ClickObj.ClickObjFamily = null;
        //初始化St0
        St0.enabled = true;
        St0.Initialization();
        //关闭St1
        this.GetComponent<State1>().enabled = false;
        //}
    }

    public void Initialization()
    {
        Collection = this.GetComponent<AssemblyCollection>();
        ClickObj = this.GetComponent<ClickManager>();
        Tip = GameObject.Find("UIControler").GetComponent<WholeTips>();
        St2 = this.GetComponent<State2>();
        St0 = this.gameObject.GetComponent<State0>();

        Star = false;
        //if (ClickObj.ClickObj != null)
        //{
        //    ClickObj.ClickObj = null;
        //    ClickObj.ClickObjFamily = null;
        //}
        //else { }
        Tip.InputTips("Tips", "请选择装配轴或壳体", 2);
    }

    public void ButtonClick()
    {
        Star = true;
    }
}
