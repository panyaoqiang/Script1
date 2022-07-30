using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class State3 : MonoBehaviour
{
    public State2 St2;
    public State4 St4;
    public AssemblyCollection Collection;
    public ClickManager ClickObj;

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
        St3();
        //ResetSt();
    }

    public void St3()
    {
        //获取选中物体的轴
        if (Star)
        {
            //显示轴的所有轴线和零件的所有轴线
            //并且替换两个零件的材质为半透明
            UsingFunction.HideAndAppear("Appear", 2, Collection.Collection[0], null);
            UsingFunction.HideAndAppear("Appear", 2, Collection.Collection[1], null);

            //此时拾取器上显示已经选择的轴和零件
            St4.enabled = true;
            St4.Initialization();
            this.GetComponent<State3>().enabled = false;
        }
        Star = false;
    }

    public void ResetSt()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            //ClickObj.ClickObj = null;
            //ClickObj.ClickObjFamily = null;
            St2.enabled = true;
            St2.Initialization();
            this.GetComponent<State3>().enabled = false;
        }
    }

    public void Initialization()
    {
        Tip = GameObject.Find("UIControler").GetComponent<WholeTips>();
        St4 = this.GetComponent<State4>();
        St2 = this.GetComponent<State2>();
        Collection = this.GetComponent<AssemblyCollection>();
        ClickObj = this.GetComponent<ClickManager>();
        UsingFunction.HideAndAppear("Hide", 2, Collection.Collection[0], null);
        UsingFunction.HideAndAppear("Hide", 2, Collection.Collection[1], null);
        Star = false;
        if (ClickObj.ClickObj != null)
        {
            ClickObj.ClickObj = null;
            ClickObj.ClickObjFamily = null;
        }
        else { }
        Tip.InputTips("Tips", "请确认已选择零件", 2);
    }
    public void ButtonClick()
    {
        Star = true;
    }

    /// <summary>
    /// 重新选择轴和零件，回退至St2
    /// </summary>
    public void R1()
    {
        St2.enabled = true;
        St2.Initialization();
        this.GetComponent<State3>().enabled = false;
    }
}
