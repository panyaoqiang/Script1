using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class State10 : MonoBehaviour
{
    //第十个状态需要清空数据，添加支座约束，保存已经装配完成的零件
    //装配完毕后清除显示状态
    public State9 St9;
    public State0 St0;
    public AssemblyCollection Collection;
    public ClickManager ClickObj;
    public WholeTips Tip;

    public bool TheGameObjSendToList = false;
    bool Star = false;
    // Start is called before the first frame update
    void Start()
    {
        Initialization();
    }

    // Update is called once per frame
    void Update()
    {
        St10();
        //ResetSt();
    }

    public void St10()
    {
        //将拾取器的零件装入数据库
        //添加固定关节
        if (Star)
        {
            Tip.InputTips("Tips", "完成装配以开始下一个零件装配", 2f);
            FixedJoint f = Collection.Collection[1].gameObject.GetComponent<FixedJoint>();
            if (f != null)
            {
                Destroy(f);
            }
            UsingFunction.Family(Collection.Collection[1])[0].gameObject.GetComponent<Rigidbody>().constraints
                = RigidbodyConstraints.None;

            TheGameObjSendToList = true;
            //St0.enabled = true;
            //St0.Initialization();
            //this.GetComponent<State10>().enabled = false;
        }
        Star = false;
    }

    public void ResetSt()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            St9.enabled = true;
            Collection.Collection[1].gameObject.transform.position = St9.InitialPos;
            Collection.Collection[1].gameObject.transform.eulerAngles = St9.InitialRot;
            St9.Initialization();
            this.GetComponent<State10>().enabled = false;
        }
    }

    public void Initialization()
    {
        St0 = this.gameObject.GetComponent<State0>();
        St9 = this.gameObject.GetComponent<State9>();
        ClickObj = this.GetComponent<ClickManager>();
        Collection = this.GetComponent<AssemblyCollection>();
        Tip = GameObject.Find("UIControler").GetComponent<WholeTips>();
        Star = false;
        if (ClickObj.ClickObj != null)
        {
            ClickObj.ClickObj = null;
            ClickObj.ClickObjFamily = null;
        }
        else { }
        Tip.InputTips("Tips", "请确认装配效果", 2f);
    }

    public void ButtonClick()
    {
        Star = true;
    }

    /// <summary>
    /// 回退St9
    /// </summary>
    public void R1()
    {
        St9.enabled = true;
        Collection.Collection[1].gameObject.transform.position = St9.InitialPos;
        Collection.Collection[1].gameObject.transform.eulerAngles = St9.InitialRot;
        St9.Initialization();
        this.GetComponent<State10>().enabled = false;
    }
}
