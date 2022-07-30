using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartDate : MonoBehaviour
{
    ////总零件信息库，挂载在InfoController储存

    ////父类：零件数据
    ////子类：行星架，齿轮，轴套，轴承，紧固环，拨叉
    ////父类数据结构包含：该零件装配的轴，与该零件配合的零件，该零件装配对准的轴端面
    ////                  该零件装配对应的基准面，该零件装配对应的基准轴，装配顺序
    ////父类数据结构：
    ////第一层：所有轴，每个轴为一个List，包含其上的所有装配零件
    //public List<List<Dictionary<string, object>>> AllAxle
    //    = new List<List<Dictionary<string, object>>>();
    ////第二层：该轴上装配的每一类零件（行星架，齿轮，轴套，轴承，紧固环，拨叉）
    ////第一层索引为该轴名称，string=轴.gameobject.name，对应该轴下的所有零件信息
    ////第二层dictionary索引为零件类型，对应该类型下的所有同类型零件
    ////第三层dictionary索引为零件名称，对应单个零件信息
    //public Dictionary<string,Dictionary<string, Dictionary
    //    <string, Dictionary<string, object>>>>KindOfAxlePart
    //    = new Dictionary<string, Dictionary<string, Dictionary
    //        <string, Dictionary<string, object>>>>();
    ////需要挂载的脚本
    ////第三层：该轴上装配的每一类零件的单个零件信息
    ////索引为零件信息名称（装配轴，与其装配的零件，装配基准面，装配基准轴，装配进入端面，装配顺序）
    //public Dictionary<string, object> EveryPartInfo 
    //    = new Dictionary<string, object>();

    /// <summary>
    /// 已经设置的轴
    /// </summary>
    public Dictionary<string, List<GameObject>> SettedShafts;
    /// <summary>
    /// 对应轴已经设置好的零件
    /// </summary>
    
    public Dropdown SettedShaftsList;
    public Dropdown SettedPartsList;
    public Dropdown UnsettedPartsList;

    private int OptionValue = 10000;
    public bool ListRefresh = false;
    public bool Manager = false;

    void Start()
    {
        SettedShafts = new Dictionary<string, List<GameObject>>();
        //SettedParts = new List<GameObject>();
    }

    void Update()
    {
        //DontDestroyOnLoad(this.gameObject);
        //Debug.Log("轴" + SettedShafts.Count + "零件" + UnSettedParts.Count);
        //UsingFunction.SaveAlways(this.gameObject);
        OnChangeOption();
        if (ListRefresh)
        {
            UnSettedList();
            SettedList();
        }
    }



    /// <summary>
    /// 刷新零件列表
    /// </summary>
    /// <param name="dropdown"></param>
    public void PartListInitialization(Dropdown dropdown, string Option0)
    {
        dropdown.options.Clear();
        //dropdown.GetComponent<Dropdown>().value = 0;
        Dropdown.OptionData data = new Dropdown.OptionData();
        data.text = Option0;
        dropdown.options.Add(data);
    }

    /// <summary>
    /// 按钮方法必须带有bool=true；对已设置零件列表进行操作，传入两个列表；
    /// 对未设置零件列表进行操作，已设置列表传入空；
    /// </summary>
    /// <param name="dropdown">已设置零件列表dictionary</param>
    /// <param name="Unsetted">未设置零件列表Fam[4].AlxeInfo.AllParts</param>
    /// <param name="Operation">Delet，Hide，Appear</param>
    /// <param name="objects">已设置轴零件dictionary</param>
    public void ListManager(Dropdown dropdown, Dropdown Unsetted, string Operation, Dictionary<string, List<GameObject>> objects)//List<GameObject> UnsettedParts
    {
        if (Manager)
        {
            GameObject Shaft = GameObject.Find(SettedShaftsList.captionText.text);
            switch (Operation)
            {
                case ("Delet"):
                    //未设置零件列表删除零件
                    if (dropdown == null && SettedShaftsList.GetComponent<Dropdown>().value != 0
                        )//&& Unsetted.GetComponent<Dropdown>().value != 0
                    {
                        for (int i = 0; i < Unsetted.options.Count; i++)
                        {
                            if (Unsetted.options[i].text == Unsetted.captionText.text)
                            {
                                //在已选择轴的Fam[4].AxleInfo.AllParts中获取未设置零件，删除对应项
                                for (int j = 0;
                                    j < UsingFunction.Family(Shaft)[4].GetComponent<AxleInfo>().AllParts.Count; j++)
                                {
                                    if (UsingFunction.Family(Shaft)[4].GetComponent<AxleInfo>().AllParts[j].name
                                        == Unsetted.captionText.text)
                                    {
                                        UsingFunction.Hide_Translucent_Appear("Appear",
                                            UsingFunction.Family(Shaft)[4].GetComponent<AxleInfo>().AllParts[j]);
                                        UsingFunction.Family(Shaft)[4].GetComponent<AxleInfo>().AllParts.RemoveAt(j);
                                    }
                                }
                                //删除未设置零件列表选中项
                                //刷新列表
                                Unsetted.options.RemoveAt(i);
                                Unsetted.GetComponent<Dropdown>().value = Unsetted.options.Count-1;
                                ListRefresh = true;
                                Manager = false;
                            }
                        }
                    }
                    //已设置零件列表删除零件
                    if (dropdown != null && SettedShaftsList.GetComponent<Dropdown>().value != 0
                        )//&& dropdown.GetComponent<Dropdown>().value != 0
                    {
                        for (int i = 0; i < dropdown.options.Count; i++)
                        {
                            if (dropdown.options[i].text == dropdown.captionText.text)
                            {
                                //在已设置零件dic中获取该轴对应的list，删除对应项并隐藏
                                for (int j = 0; j < objects[SettedShaftsList.captionText.text].Count; j++)
                                {
                                    if (objects[SettedShaftsList.captionText.text][j].name
                                        == dropdown.captionText.text)
                                    {
                                        UsingFunction.Hide_Translucent_Appear("Hide"
                                            , objects[SettedShaftsList.captionText.text][j]);
                                        objects[SettedShaftsList.captionText.text].RemoveAt(j);
                                        objects[SettedShaftsList.captionText.text][j].
                                            GetComponentInChildren<PartInfo>().Initialization();
                                    }
                                }
                                //添加至未设置零件列表
                                Dropdown.OptionData data = new Dropdown.OptionData();
                                data.text = dropdown.options[i].text;
                                Unsetted.options.Add(data);
                                //删除已设置零件选中项
                                dropdown.options.RemoveAt(i);
                                dropdown.GetComponent<Dropdown>().value = dropdown.options.Count-1;
                                ListRefresh = true;
                                Manager = false;
                            }
                        }
                    }
                ; return;
                //查看功能只能用于零件列表，按照已设置轴列表除选择的轴，查看对应轴的零件
                //此步骤用作定义轴完毕后，选择该轴上的零件进行逐一定义
                case ("Appear"):
                    //对未设置零件进行查看
                    if (dropdown == null && SettedShaftsList.GetComponent<Dropdown>().value != 0
                        )//&& Unsetted.GetComponent<Dropdown>().value != 0
                    {
                        for (int i = 0; i < UsingFunction.Family(Shaft)[4].
                            GetComponent<AxleInfo>().AllParts.Count; i++)
                        {
                            if (UsingFunction.Family(Shaft)[4].GetComponent<AxleInfo>().
                                AllParts[i].name == Unsetted.captionText.text)
                            {
                                UsingFunction.Hide_Translucent_Appear("Appear", 
                                    UsingFunction.Family(Shaft)[4].GetComponent<AxleInfo>().AllParts[i]);
                            }
                        }
                    }
                    //对已设置零件进行查看
                    if (dropdown != null && SettedShaftsList.GetComponent<Dropdown>().value != 0
                        )//&& dropdown.GetComponent<Dropdown>().value != 0
                    {
                        for (int i = 0; i < objects[SettedShaftsList.captionText.text].Count; i++)
                        {
                            if (objects[SettedShaftsList.captionText.text][i].name == dropdown.captionText.text)
                            {
                                UsingFunction.Hide_Translucent_Appear
                                    ("Appear", objects[SettedShaftsList.captionText.text][i]);
                            }
                        }

                    }
                ; return;
                case ("Hide"):
                    //对未设置零件进行隐藏
                    if (dropdown == null && SettedShaftsList.GetComponent<Dropdown>().value != 0
                        )//&& Unsetted.GetComponent<Dropdown>().value != 0
                    {
                        for (int i = 0; i < UsingFunction.Family(Shaft)[4].
                            GetComponent<AxleInfo>().AllParts.Count; i++)
                        {
                            if (UsingFunction.Family(Shaft)[4].GetComponent<AxleInfo>().
                                AllParts[i].name == Unsetted.captionText.text)
                            {
                                UsingFunction.Hide_Translucent_Appear("Hide",
                                    UsingFunction.Family(Shaft)[4].GetComponent<AxleInfo>().AllParts[i]);
                            }
                        }
                    }
                    //对已设置零件进行隐藏
                    if (dropdown != null && SettedShaftsList.GetComponent<Dropdown>().value != 0
                        )//&& dropdown.GetComponent<Dropdown>().value != 0
                    {
                        for (int i = 0; i < objects[SettedShaftsList.captionText.text].Count; i++)
                        {
                            if (objects[SettedShaftsList.captionText.text][i].name == dropdown.captionText.text)
                            {
                                UsingFunction.Hide_Translucent_Appear
                                    ("Hide", objects[SettedShaftsList.captionText.text][i]);
                            }
                        }

                    }
                    ; return;
                default: return;
            }
        }
        Manager = false;
    }

    public void UnSettedList()
    {
        PartListInitialization(UnsettedPartsList, "该轴未设置的零件");
        //每帧检查每个设置完成的轴的已设置零件，当dic中的list包含零件，剔除信息层中的装配零件显示在列表中
        if (SettedShaftsList.captionText.text != null)
        {
            //获取选中的轴
            GameObject Shaft = GameObject.Find(SettedShaftsList.captionText.text);
            //获取选中轴信息层装配零件列表
            List<GameObject> Parts = UsingFunction.Family(Shaft)[4].gameObject.GetComponent<AxleInfo>().AllParts;
            List<GameObject> Output = Parts;
            //已经选定一个轴，查看该轴未设置的零件
            for (int j = 0; j < Parts.Count; j++)
            {
                for (int i = 0; i < SettedShafts[SettedShaftsList.captionText.text].Count; i++)
                {
                    if (SettedShafts[SettedShaftsList.captionText.text][i].name == Parts[j].name)
                    {
                        Output.Remove(Parts[j]);
                    }
                }
            }
            //获取未设置零件后，添加到未设置零件列表中
            for (int k = 0; k < Output.Count; k++)
            {
                Dropdown.OptionData data = new Dropdown.OptionData();
                data.text = Output[k].name;
                UnsettedPartsList.options.Add(data);
            }
            ListRefresh = false;
        }
    }
    //获取当前选中轴的已设置零件并添加选项到列表中
    public void SettedList()
    {
        if (SettedShaftsList.captionText.text != null)
        {
            PartListInitialization(SettedPartsList, "该轴已设置的零件");
            for (int i = 0; i < SettedShafts[SettedShaftsList.captionText.text].Count; i++)
            {
                Dropdown.OptionData data = new Dropdown.OptionData();
                data.text = SettedShafts[SettedShaftsList.captionText.text][i].name;
                SettedPartsList.options.Add(data);
            }
        }
    }

    public void OnChangeOption()
    {
        if (OptionValue != 10000 && SettedShaftsList.options.Count != 0)
        {
            if (OptionValue != SettedShaftsList.GetComponent<Dropdown>().value)
            {
                //Debug.Log("对");
                ListRefresh = true;
            }
            else
            {
                ListRefresh = false;
            }
        }
        OptionValue = SettedShaftsList.GetComponent<Dropdown>().value;
    }

    public void Button_CheckUnsettedList(string Operate)
    {
        Manager = true;
        if (Manager && UnsettedPartsList.GetComponent<Dropdown>().value != 0)
        {
            //Debug.Log("？？");
            ListManager(null, UnsettedPartsList, Operate, SettedShafts);
        }
    }

    public void Button_CheckSettedList(string Operate)
    {
        Manager = true;
        if (Manager && SettedPartsList.GetComponent<Dropdown>().value != 0)
        {
            ListManager(SettedPartsList, UnsettedPartsList, Operate, SettedShafts);
        }
    }

    public void Button_DeletUnSettedList()
    {
        Manager = true;
        if (Manager && UnsettedPartsList.GetComponent<Dropdown>().value != 0
            && SettedShaftsList.GetComponent<Dropdown>().value != 0) 
        {
            ListManager(null, UnsettedPartsList, "Delet", SettedShafts);
            ListRefresh = true;
        }
    }

    public void Button_DeletSettedList()
    {
        Manager = true;
        if (Manager && SettedPartsList.GetComponent<Dropdown>().value != 0
            &&SettedShaftsList.GetComponent<Dropdown>().value!=0)
        {
            ListManager(SettedPartsList, UnsettedPartsList, "Delet", SettedShafts);
            ListRefresh = true;
        }
    }
}
