using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class State0 : MonoBehaviour
{
    public State1 St1;
    public State10 St10;
    public ClickManager ClickObj;
    public AssemblyCollection Collection;
    public AllAssembleList AllList;
    public WholeTips Tip;

    bool Star = false;
    // Start is called before the first frame update
    void Start()
    {
        Collection = this.GetComponent<AssemblyCollection>();
        Collection.Initialization();
        Initialization();
    }

    // Update is called once per frame
    void Update()
    {
        St0();
    }

    public void St0()
    {
        //提示
        if (Star)
        {
            Tip.InputTips("Tips", "开始装配，可拖动装配面板；\r\n可使用操作面板查看零件信息", 1);
            St1.enabled = true;
            //初始化St1
            St1.Initialization();
            this.GetComponent<State0>().enabled = false;
        }
        Star = false;
    }

    public void Initialization()
    {
        Tip = GameObject.Find("UIControler").GetComponent<WholeTips>();
        AllList = this.GetComponent<AllAssembleList>();
        Collection = this.GetComponent<AssemblyCollection>();
        ClickObj = this.GetComponent<ClickManager>();
        St1 = this.gameObject.GetComponent<State1>();
        St10 = this.gameObject.GetComponent<State10>();

        Star = false;
        Collection.Initialization();
        //if (Collection.Collection != null)
        //{
        //    //初始化拾取器
        //    Collection.Collection[0] = null;
        //    Collection.Collection[1] = null;
        //    Collection.GearFace = null;
        //    Collection.GearAxis = null;
        //    Collection.PartFace = null;
        //    Collection.PartAxis = null;
        //}

        //if (ClickObj.ClickObj != null)
        //{
        //    //清空ClickObj
        //    ClickObj.ClickObj = null;
        //    ClickObj.ClickObjFamily = null;
        //}
        //else { }
    }

    public void ButtonClick()
    {
        Star = true;
    }


}
