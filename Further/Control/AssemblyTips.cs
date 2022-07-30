using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AssemblyTips : MonoBehaviour
{
    public GameObject Pocket;
    public ClickManager click;
    public Dictionary<string, object> Info = new Dictionary<string, object>();
    public List<GameObject> TransformInfo = new List<GameObject>();
    public Text ObjName;
    public Text BodyName;
    public bool CA = false;
    public bool CF = false;
    // Start is called before the first frame update
    void Start()
    {
        Initialized();
    }

    public void Initialized()
    {
        Pocket = null;
        ObjName.text = "";
        BodyName.text = "请选中零件并点击查看";
        if (TransformInfo.Count != 0)
        {
            for (int i = 1; i < TransformInfo.Count; i++)
            {
                AppearAndHide(TransformInfo[2], "H");
                AppearAndHide(TransformInfo[1], "H");
            }
        }
        Info.Clear();
        TransformInfo.Clear();
        CA = false;
        CF = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Pocket != null)
        {
            try
            {
                CheckAxle();
                CheckFace();
            }
            catch { }
        }
    }

    public void Click2Check()
    {
        if (click.ClickObj != null && click.ClickObj.tag != "壳体" && click.ClickObj.tag != "装配轴")
        {
            Pocket = click.ClickObj;
            ObjName.text = Pocket.name;
            Info = UsingFunction.Family(Pocket)[4].GetComponent<PartInfo>().AssembleInfo;
            if (TransformInfo.Count != 0)
            {
                for (int i = 1; i < TransformInfo.Count; i++)
                {
                    AppearAndHide(TransformInfo[2], "H");
                    AppearAndHide(TransformInfo[1], "H");
                }
                TransformInfo.Clear();
            }
            try
            {
                TransformInfo.Add((GameObject)Info["装配轴"]);
                TransformInfo.Add((GameObject)Info["装配基准轴"]);
                TransformInfo.Add((GameObject)Info["装配基准面"]);
                BodyName.text = "装配主体为" + TransformInfo[0].name;
            }
            catch { }
        }
    }
    public void Star_CA()
    {
        CA = !CA;
    }
    public void CheckAxle()
    {
        if (CA)
        {
            //UsingFunction.HideAndAppear("Appear", 2, TransformInfo[0], null);
            //UsingFunction.HideAndAppear("Hide", 2, TransformInfo[0], TransformInfo[1]);
            AppearAndHide(TransformInfo[1], "A");
        }
        else
        {
            //UsingFunction.HideAndAppear("Hide", 2, TransformInfo[0], null);
            AppearAndHide(TransformInfo[1], "H");
        }
    }
    public void Star_CF()
    {
        CF = !CF;
    }
    public void CheckFace()
    {
        if (CF)
        {
            //UsingFunction.HideAndAppear("Appear", 3, TransformInfo[0], null);
            //UsingFunction.HideAndAppear("Hide", 3, TransformInfo[0], TransformInfo[2]);
            AppearAndHide(TransformInfo[2], "A");
        }
        else
        {
            //UsingFunction.HideAndAppear("Hide", 3, TransformInfo[0], null);
            AppearAndHide(TransformInfo[2], "H");
        }
    }
    /// <summary>
    /// 用作查看基准
    /// </summary>
    public void AppearAndHide(GameObject Obj, string AorH)
    {
        if (Obj.GetComponent<MeshRenderer>() != null)
        {
            if (AorH == "A")
            {
                Obj.GetComponent<MeshRenderer>().enabled = true;
            }
            else
            {
                Obj.GetComponent<MeshRenderer>().enabled = false;
            }
        }
    }
}
