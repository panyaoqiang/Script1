using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class State9 : MonoBehaviour
{
    public State8 St8;
    public State10 St10;
    public AssemblyCollection Collection;
    public ClickManager ClickObj;
    public Text text;
    public WholeTips Tip;

    public Vector3 InitialPos;
    public Vector3 InitialRot;
    public float Speed = 10f;
    //Vector3 TargetPos;

    //开始对准面
    bool Button = false;
    bool Star = false;
    bool Confirm = false;
    // Start is called before the first frame update
    void Start()
    {
        Initialization();
    }

    // Update is called once per frame
    void Update()
    {
        St9();
        //ResetSt();
    }

    public void St9()
    {
        StarCoplanar();
        if (Button)
        {
            Coplanar(Collection.GearFace, Collection.PartFace, Speed);
            if (Coplanar(Collection.GearFace, Collection.PartFace, Speed))
            {
                if (Star)
                {
                    Tip.InputTips("Tips", "装配完成，确认装配无误后完成装配", 2);
                    //确认装配完成后隐藏所有面
                    UsingFunction.HideAndAppear("Hide", 3, Collection.Collection[0], null);
                    UsingFunction.HideAndAppear("Hide", 3, Collection.Collection[1], null);
                    Button = false;
                    St10.enabled = true;
                    St10.Initialization();
                    this.GetComponent<State9>().enabled = false;
                }
            }
        }
        Star = false;
        Confirm = false;
    }
    //复位已经移动的零件
    public void ResetSt()
    {
        //点击R键重置选择
        if (Input.GetKeyDown(KeyCode.R))
        {
            ClickObj.ClickObj = null;
            ClickObj.ClickObjFamily = null;
            //后退一步，隐藏所有显示的面
            //当完成装配时，发现装配错误，复位零件
            if (!Coplanar(Collection.GearFace, Collection.PartFace, Speed))
            {
                Button = false;
                Collection.Collection[1].transform.position = InitialPos;
                Collection.Collection[1].transform.eulerAngles = InitialRot;
                St8.enabled = true;
                St8.Initialization();
                this.GetComponent<State9>().enabled = false;
            }
            //显示已经隐藏的两个面
            if (Coplanar(Collection.GearFace, Collection.PartFace, Speed))
            {
                Button = false;
                Collection.Collection[1].transform.position = InitialPos;
                Collection.Collection[1].transform.eulerAngles = InitialRot;
            }
        }
    }
    //第二个零件装配时需要重新启动一次
    public void Initialization()
    {
        St8 = this.GetComponent<State8>();
        St10 = this.GetComponent<State10>();
        ClickObj = this.GetComponent<ClickManager>();
        Collection = this.GetComponent<AssemblyCollection>();
    Tip = GameObject.Find("UIControler").GetComponent<WholeTips>();
        Star = false;
        Button = false;
        Confirm = false;
        InitialPos = Collection.Collection[1].transform.position;
        InitialRot = Collection.Collection[1].transform.eulerAngles;
        
        if (ClickObj.ClickObj != null)
        {
            ClickObj.ClickObj = null;
            ClickObj.ClickObjFamily = null;
        }
        else { }
        Tip.InputTips("Tips", "正在装配", 2f);
    }

    public void ButtonClick()
    {
        Star = true;
    }

    public void ButtonConfirm()
    {
        Confirm = true;
    }

    public void StarCoplanar()
    {
        if (Confirm)
        {
            if(Collection.GearFace.transform.localPosition!=Collection.Collection[1].GetComponentInChildren
                <PartInfo>().DatumFace)
            {
                Tip.InputTips("Caution", "建议按照装配方案选择正确的装配基准面\n打开操作面板可查看装配信息", 2);
            }
            Button = true;
        }
    }

    //传入两个面，移动零件
    public bool Coplanar(GameObject Gear, GameObject Part, float Speed)
    {
        //需要获取装配轴的面的位置
        //需要获取零件的面的位置
        //计算面与面之间的距离
        //零件与安装轴面对齐
        //重复解锁，冻结约束
        //传入轴的面和零件的面
        bool All = false;
        if (Button)
        {
            //先解锁零件
            UsingFunction.Family(Part)[0].GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
            //再锁住轴本体
            UsingFunction.Family(Gear)[0].GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;

            float Dis;
            Vector3 Dir;
            //求出零件到轴端面的对准位置的距离
            Dis = (Gear.transform.position - Part.transform.position).magnitude;
            //当距离大于阈值，执行操作
            if (Dis >= 2f)
            {
                Dir = (Gear.transform.position - Part.transform.position);
                UsingFunction.Family(Part)[0].GetComponent<Rigidbody>().AddForce(Dir.normalized * Dir.magnitude * Speed * 1f);
                UsingFunction.Family(Part)[0].GetComponent<Rigidbody>().drag = 8f;
                //旋转装配配合轮齿啮合
                UsingFunction.Family(Part)[0].GetComponent<Rigidbody>().AddTorque
                    (UsingFunction.Family(Part)[0].transform.right.normalized*Speed, ForceMode.Force);
            }
            if (Dis < 2f)
            {

                UsingFunction.Family(Part)[0].transform.position = Gear.transform.position+
                    UsingFunction.Family(Part)[0].transform.position-Part.transform.position;
                //UsingFunction.Family(Part)[0].gameObject.GetComponent<Rigidbody>().constraints =
                //  RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotationY
                //  | RigidbodyConstraints.FreezeRotationZ;


                Vector3 assemblyPos = ((GameObject)UsingFunction.Family(Part)[4].GetComponent<PartInfo>().
                    AssembleInfo["装配基准面"]).transform.position- Collection.Collection[1].transform.position; 
                if ((assemblyPos.magnitude) - ((Vector3)UsingFunction.Family(Part)[4].GetComponent<PartInfo>().
                    AssembleInfo["装配位置"]).magnitude <= 1f)
                {
                    text.text = ("装配成功");
                }
                else
                {
                    text.text = ("装配错误，点击取消装配复位\n点击返回重做步骤");
                }


                UsingFunction.Family(Gear)[0].GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePosition
                    | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
                All = true;
                Tip.InputTips("Tips", "装配完成", 2f);
            }
        }
        return All;
    }

    /// <summary>
    /// 回退St8重选零件对准的自身基准面
    /// </summary>
    public void R1()
    {
        ClickObj.ClickObj = null;
        ClickObj.ClickObjFamily = null;
        //后退一步，隐藏所有显示的面
        //当完成装配时，发现装配错误，复位零件
        //if (!Coplanar(Collection.GearFace, Collection.PartFace, Speed))
        {
            Button = false;
            Collection.Collection[1].transform.position = InitialPos;
            Collection.Collection[1].transform.eulerAngles = InitialRot;
            St8.enabled = true;
            St8.Initialization();
            this.GetComponent<State9>().enabled = false;
        }
    }
    /// <summary>
    /// 重新装配，复位零件
    /// </summary>
    public void R2()
    {
        ClickObj.ClickObj = null;
        ClickObj.ClickObjFamily = null;
        //if (Coplanar(Collection.GearFace, Collection.PartFace, Speed))
        {
            Button = false;
            Collection.Collection[1].transform.position = InitialPos;
            Collection.Collection[1].transform.eulerAngles = InitialRot;
        }
        Tip.InputTips("Tips", "重新装配", 2f);
    }
}
