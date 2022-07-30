using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class State7 : MonoBehaviour
{
    public Text text;
    public State6 St6;
    public State8 St8;
    public AssemblyCollection Collection;
    public ClickManager ClickedObject;
    bool TheFaceBelongToSelectedObj = false;
    public WholeTips Tip;
    GameObject G;
    bool Star = false;
    // Start is called before the first frame update
    //void Start()
    //{
    //    Initialization();
    //}

    // Update is called once per frame
    void Update()
    {
        St7();
        //ResetSt();
    }

    public void St7()
    {
        //检查随后点击的面是否属于选中物体的轴实体
        if (ClickedObject.ClickObj != null)
        {
            G = null;
            //当帧点击的物体的子物体是否属于选中轴的面
            if (UsingFunction.Family(ClickedObject.ClickObj)[0] == Collection.Collection[0]
                && ClickedObject.ClickObj.tag=="装配基准面")//面片名称更改
            {
                G = ClickedObject.ClickObj.gameObject;
                TheFaceBelongToSelectedObj = true;
            }
            else if (UsingFunction.Family(ClickedObject.ClickObj)[0] != Collection.Collection[0]
                && ClickedObject.ClickObj !=Collection.PartAxis )
            {
                Tip.InputTips("False", "请选择装配轴上的基准面", 2);
            }

            if (Star && TheFaceBelongToSelectedObj)
            {
                //装入点击中的面实体
                Collection.GearFace = G;
                //装入实体同时，其余面全部隐藏
                if (UsingFunction.Family(Collection.Collection[1])[4].GetComponent<PartInfo>().DatumFace
                    == (Collection.GearFace.transform.localPosition))
                {
                    Tip.InputTips("Caution", "建议按照装配方案，选择正确的装配基准面\n可打开操作面板查看零件装配信息", 2);
                }
                UsingFunction.HideAndAppear("Hide", 3, Collection.Collection[0], Collection.GearFace);
                St8.enabled = true;
                St8.Initialization();
                this.GetComponent<State7>().enabled = false;
            }
        }
        Star = false;
    }

    public void ResetSt()
    {
        //点击R键重置选择
        if (Input.GetKeyDown(KeyCode.R))
        {
            ClickedObject.ClickObj = null;
            ClickedObject.ClickObjFamily = null;
            //当未选择面时，重做State6，并归位已经移动的零件
            if (Collection.GearFace == null)
            {
                //后退一步，隐藏所有显示的面
                UsingFunction.HideAndAppear("Hide", 3, Collection.Collection[0], null);
                UsingFunction.HideAndAppear("Hide", 3, Collection.Collection[1], null);
                Collection.Collection[1].gameObject.transform.position = St6.InitialPos;
                Collection.Collection[1].gameObject.transform.eulerAngles = St6.InitialRot;
                St6.enabled = true;
                St6.Initialization();
                this.GetComponent<State7>().enabled = false;
            }
            else
            {
                //重新选择轴的装配面体
                UsingFunction.HideAndAppear("Appear", 3, Collection.Collection[0], null);
                TheFaceBelongToSelectedObj = false;
                Collection.GearFace = null;
            }
        }
    }

    public void Initialization()
    {
    Tip = GameObject.Find("UIControler").GetComponent<WholeTips>();

        St6 = this.GetComponent<State6>();
        St8 = this.GetComponent<State8>();
        Collection = this.GetComponent<AssemblyCollection>();
        ClickedObject = this.GetComponent<ClickManager>();

        TheFaceBelongToSelectedObj = false;
        G = null;
        Star = false;
        //Collection.Collection[0].GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        Collection.Collection[1].GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        //显示已选零件的所有面
        UsingFunction.HideAndAppear("Appear", 3, Collection.Collection[0], null);

        if (ClickedObject.ClickObj != null)
        {
            ClickedObject.ClickObj = null;
            ClickedObject.ClickObjFamily = null;
        }
        else { }
    }
    public void ButtonClick()
    {
        Star = true;
    }

    /// <summary>
    /// 回退至St6重新选择对准位置并归位零件
    /// </summary>
    public void R1()
    {
        ClickedObject.ClickObj = null;
        ClickedObject.ClickObjFamily = null;
        //if (Collection.GearFace == null)
        {
            //后退一步，隐藏所有显示的面
            UsingFunction.HideAndAppear("Hide", 3, Collection.Collection[0], null);
            UsingFunction.HideAndAppear("Hide", 3, Collection.Collection[1], null);
            Collection.Collection[1].gameObject.transform.position = St6.InitialPos;
            Collection.Collection[1].gameObject.transform.eulerAngles = St6.InitialRot;
            St6.enabled = true;
            St6.Initialization();
            this.GetComponent<State7>().enabled = false;
        }
    }
    /// <summary>
    /// 重新选择轴的装配面体
    /// </summary>
    public void R2()
    {
        ClickedObject.ClickObj = null;
        ClickedObject.ClickObjFamily = null;
        //if (Collection.GearFace != null)
        {
            UsingFunction.HideAndAppear("Appear", 3, Collection.Collection[0], null);
            TheFaceBelongToSelectedObj = false;
            Collection.GearFace = null;
        }
        Tip.InputTips("Tips", "重新选择装配基准面", 2);
    }
}
