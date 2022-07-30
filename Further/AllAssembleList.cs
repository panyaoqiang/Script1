using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AllAssembleList : MonoBehaviour
{
    public State0 St0;
    public State10 St10;
    public AssemblyCollection Collection;

    //信息可视化面板UI组件
    public Dropdown AssemblyShaft;
    public Dropdown AssemblyParts;

    //刷新变量
    public int ShaftListValue = 10000;
    public bool ListInitialization = false;
    public bool ShaftListRefresh = false;
    public bool PartListRefresh = false;

    /// <summary>
    /// 装载所有装配体，传入轴名为key，装配零件集合为value
    /// </summary>
    public Dictionary<string, List<GameObject>> Assembly = new Dictionary<string, List<GameObject>>();


    void Start()
    {
        #region//通过场景中每一个物体共有的特性搜索物体，得出的所有物体对名字进行筛选，获取特定的零件集合
        //AllObj = Resources.FindObjectsOfTypeAll<Rigidbody>();
        //int AxleCount = 0;
        //for (int i = 0; i <= AllObj.Length; i++)
        //{
        //    if (AllObj[i].gameObject.name.Contains("轴"))
        //    {
        //        //以轴为装配主体的数组，装该轴和轴上装配的每一个零件
        //        AxleCount++;
        //    }
        //}

        //for (int j = 0; j < AxleCount; j++)
        //{
        //    List<GameObject> Axle = new List<GameObject>();
        //    AssembleList.Add(Axle);
        //}
        #endregion
    }

    // Update is called once per frame
    void Update()
    {
        OnChangeOption();
        //St10完成后添加零件到Assembly中，当点击列表选择查看时，触发列表刷新，实时获取数据
        if (St10.TheGameObjSendToList)
        {
            if (Collection.Collection[1].GetComponent<FixedJoint>() == null)
            {
                Collection.Collection[1].AddComponent<FixedJoint>();
                Collection.Collection[1].GetComponent<FixedJoint>().connectedBody=
                    Collection.Collection[0].GetComponent<Rigidbody>(); ;
            }
            //查看此零件是否已经与轴装配
            foreach (string K in Assembly.Keys)
            {
                if (Assembly[K].Contains(Collection.Collection[1]))
                {
                    //传出错误信息
                    Debug.Log("此零件已与其他轴装配，装配错误");
                }
            }
            //此轴第一次装配，添加轴
            if (!Assembly.ContainsKey(Collection.Collection[0].name))
            {
                Assembly.Add(Collection.Collection[0].name,
                    new List<GameObject> { Collection.Collection[1] });
                ShaftListRefresh = true;
                PartListRefresh = true;
                //PartsCount.Add(Collection.Collection[0].name, Assembly[Collection.Collection[0].name].Count);
            }
            //此轴已有装配关系
            else
            {
                //此轴与此零件已装配过一次
                if (Assembly[Collection.Collection[0].name].Contains(Collection.Collection[1]))
                {
                    //传出错误信息
                    Debug.Log("此零件已与此轴装配，不可重复装配");
                }
                //此零件与此轴第一次装配，添加零件
                else
                {
                    Assembly[Collection.Collection[0].name].Add(Collection.Collection[1]);
                    PartListRefresh = true;
                }
            }

            #region

            ////初次赋值，先调用AssembleList里的第一个List，装载该轴和装配的零件
            //if (AssembleList.Count == 0)
            //{
            //    //为第一个装配的轴创建它的List
            //    List<GameObject> Axle = new List<GameObject>();
            //    AssembleList.Add(Axle);
            //    //每个List的第一个物体必须为轴
            //    AssembleList[0].Add(Collection.Collection[0]);
            //    AssembleList[0].Add(Collection.Collection[1]);
            //    //锁定该零件
            //    AssembleList[0][0].gameObject.
            //        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePosition
            //        | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;

            //    AssembleList[0][1].gameObject.AddComponent<FixedJoint>();
            //    AssembleList[0][1].gameObject.GetComponent<FixedJoint>().connectedBody
            //        = AssembleList[0][0].GetComponent<Rigidbody>();
            //    AssembleList[0][1].gameObject.GetComponent<Rigidbody>().constraints= RigidbodyConstraints.FreezePosition;
            //    goto loop;
            //}
            ////非初次赋值，判断装入的轴是否已经装入列表，装入的零件是否已经存在于该列表中
            //if (AssembleList.Count != 0)
            //{
            //    //Q:装入时判断为break，无装入零件和轴到新的List中，且无创建新列表
            //    for (int i = 0; i < AssembleList.Count; i++)
            //    {
            //        //判断该轴是否已经装入到其中的一个List中
            //        if (AssembleList[i][0] == Collection.Collection[0])
            //        {
            //            Debug.Log("轴已存在于另一个列表中");
            //            //再判断该零件是否已经存在该轴所在的List中
            //            for (int j = 0; j < AssembleList[i].Count; j++)
            //            {
            //                //判断装配零件是否已经在列表中
            //                //如果轴不在列表中，则装入
            //                if (AssembleList[i][j] == Collection.Collection[1])
            //                {
            //                    Debug.Log("此列表已存在零件,请重新选择装配");
            //                    //如果已经存在了，跳出当前循环，跳出第二层循环
            //                    goto loop;
            //                }
            //                #region
            //                //当循环到最后一次迭代
            //                //if (j == AssembleList[i].Count - 1)
            //                //{
            //                //    //最后一个物体不是collection[1]里面的物体，则装入
            //                //    if(AssembleList[i][j] != Collection.Collection[1])
            //                //    {
            //                //        //最后一次迭代发现此列表没有该零件
            //                //        Debug.Log("此列表没该零件");
            //                //        AssembleList[i].Add(Collection.Collection[1]);
            //                //    }
            //                //    //最后一个物体是Collection[1]
            //                //    if(AssembleList[i][j] == Collection.Collection[1])
            //                //    {
            //                //        Debug.Log("此列表已经存在零件");
            //                //        //跳出两层循环
            //                //        break;
            //                //    }
            //                //}
            //                //此处Break后无条件判断时继续循环；加入bool判断条件成立
            //                #endregion
            //            }
            //            Debug.Log("循环完毕，同列表装载装配体");
            //            Collection.Collection[1].gameObject.AddComponent<FixedJoint>();
            //            Collection.Collection[1].gameObject.GetComponent<FixedJoint>()
            //                .connectedBody = AssembleList[i][0].GetComponent<Rigidbody>();
            //            Collection.Collection[1].gameObject.GetComponent<Rigidbody>().constraints
            //                = RigidbodyConstraints.FreezePosition;
            //            AssembleList[i].Add(Collection.Collection[1]);
            //            //AssembleList[i][AssembleList[0].Count - 1].gameObject.GetComponent<Rigidbody>()
            //            //    .constraints = RigidbodyConstraints.FreezePosition
            //            //    | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
            //            //装入完毕，跳出程序
            //            goto loop;
            //        }
            //        #region
            //        //最后一次迭代
            //        //判断最后一个List中是否有装入该轴
            //        //if (i == AssembleList[i].Count - 1)
            //        //{
            //        //    //最后一个List中有该轴
            //        //    if (AssembleList[i][0].name == Collection.Collection[0].name)
            //        //    {
            //        //        Debug.Log("该轴存在于最近新建的列表中");
            //        //        //再判断该零件是否已经存在该轴所在的List中
            //        //        for (int j = 0; j < AssembleList[i].Count; j++)
            //        //        {
            //        //            //判断装配零件是否已经在列表中
            //        //            //如果轴不在列表中，则装入
            //        //            if (AssembleList[i][j] == Collection.Collection[1])
            //        //            {
            //        //                //如果已经存在了，跳出当前循环
            //        //                Debug.Log("零件已经存在该列表中");
            //        //                break;
            //        //            }
            //        //            //当循环到最后一次迭代
            //        //            if (j == AssembleList[i].Count - 1)
            //        //            {
            //        //                //最后一个物体不是collection[1]里面的物体，则装入
            //        //                if (AssembleList[i][j] != Collection.Collection[1])
            //        //                {
            //        //                    Debug.Log("装入零件在新列表中");
            //        //                    AssembleList[i].Add(Collection.Collection[1]);
            //        //                }
            //        //                //最后一个物体是Collection[1]
            //        //                if (AssembleList[i][j] == Collection.Collection[1])
            //        //                {
            //        //                    Debug.Log("该零件已存在");
            //        //                    break;
            //        //                }
            //        //            }
            //        //        }
            //        //    }
            //        //    //该轴没有被装入
            //        //    if (AssembleList[i][0] != Collection.Collection[0])
            //        //    {
            //        //        Debug.Log("该轴没有装入列表");
            //        //        //新建一个List，同时装入零件和轴
            //        //        List<GameObject> Axle = new List<GameObject>();
            //        //        Axle[0].Add(Collection.Collection[0]);
            //        //        Axle[1].Add(Collection.Collection[1]);
            //        //        //Axle[0].GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
            //        //        AssembleList.Add(Axle);
            //        //        Debug.Log("第" + (i + 1) + "个List的轴" + AssembleList[i + 1][0].gameObject.name);
            //        //    }
            //        //}
            //        #endregion
            //    }
            //    //所有列表中没有该轴
            //    //把新选择装配的轴和零件装入新的List中，再整个添加到AssembleList
            //    List<GameObject> Axle = new List<GameObject>();
            //    Axle.Add(Collection.Collection[0]);
            //    Axle.Add(Collection.Collection[1]);
            //    Axle[0].gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePosition
            //        |RigidbodyConstraints.FreezeRotationY|RigidbodyConstraints.FreezeRotationZ;
            //    Axle[1].gameObject.AddComponent<FixedJoint>();
            //    Axle[1].gameObject.GetComponent<FixedJoint>().connectedBody
            //        = Axle[0].GetComponent<Rigidbody>();
            //    Axle[1].gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePosition;
            //    ////锁定最新添加的AssembleList里的List，再找该List里最新一个GameObject

            //    Debug.Log("循环完毕，新列表装载新转配体");
            //    AssembleList.Add(Axle);
            //    goto loop;
            //}
            //loop: { }
            #endregion
            St0.enabled = true;
            St0.Initialization();
            St10.TheGameObjSendToList = false;
            St10.enabled = false;
        }
        if (ShaftListRefresh)
        {
            AssemblyShaftList();
        }

        if (PartListRefresh)
        {
            AssemblyPartsList();
        }

    }

    /// <summary>
    /// 此列表装载为dic中的key
    /// </summary>
    public void AssemblyShaftList()
    {
        ListInitialization = true;
        PartListInitialization(AssemblyShaft, "已有装配关系的轴");
        if (Assembly.Count!=0)
        {
            //Debug.Log("轴列表刷新");
            foreach(string K in Assembly.Keys)
            {
                Dropdown.OptionData data = new Dropdown.OptionData();
                data.text = K;
                AssemblyShaft.options.Add(data);
            }
            ShaftListRefresh = false;
        }
    }

    public void AssemblyPartsList()
    {
        ListInitialization = true;
        PartListInitialization(AssemblyParts, "已与该轴装配的零件");
        if (AssemblyShaft.captionText.text != null && AssemblyShaft.captionText.text!="已有装配关系的轴")
        {
            //Debug.Log("零件列表刷新");
            //获取选中的轴
            GameObject Shaft = GameObject.Find(AssemblyShaft.captionText.text);
            List<GameObject> Parts = new List<GameObject>();
            //获取已经与选中轴装配的零件
            for (int i = 0; i < Assembly[Shaft.name].Count; i++)
            {
                Parts.Add(Assembly[Shaft.name][i]);
            }
            //添加到已装配零件列表中
            for (int k = 0; k < Parts.Count; k++)
            {
                Dropdown.OptionData data = new Dropdown.OptionData();
                data.text = Parts[k].name;
                AssemblyParts.options.Add(data);
            }
            PartListRefresh = false;
        }
    }

    /// <summary>
    /// 只循环一次
    /// </summary>
    /// <param name="dropdown"></param>
    /// <param name="Option0"></param>
    public void PartListInitialization(Dropdown dropdown, string Option0)
    {
        if (ListInitialization)
        {
            dropdown.options.Clear();
            //dropdown.GetComponent<Dropdown>().value = 0;
            Dropdown.OptionData data = new Dropdown.OptionData();
            data.text = Option0;
            dropdown.options.Add(data);
            ListInitialization = false;
        }
        
    }

    /// <summary>
    /// 点击更改轴的列表选项触发
    /// </summary>
    public void OnChangeOption()
    {
        if (ShaftListValue != 10000 && AssemblyShaft.options.Count >=1
            && ShaftListValue != AssemblyShaft.GetComponent<Dropdown>().value)
        {
            //Debug.Log("我变");
            PartListRefresh = true;
        }
        ShaftListValue = AssemblyShaft.GetComponent<Dropdown>().value;
    }
    
    /// <summary>
    /// 在任意时刻点击移除按钮触发
    /// 删除已装配零件，解除零件与轴的约束关系
    /// </summary>
    public void Reselect()
    {
        //获取已经选择的轴和零件名字，移除Dictionary中对应的项目
        if (AssemblyShaft.captionText.text != null && AssemblyParts.captionText.text != null
            && AssemblyShaft.captionText.text != "已有装配关系的轴" 
            && AssemblyParts.captionText.text != "已与该轴装配的零件")
        {
            //解除零件约束关系
            if (GameObject.Find(AssemblyParts.captionText.text).GetComponent<FixedJoint>() != null)
            {
                Destroy(GameObject.Find(AssemblyParts.captionText.text).gameObject.GetComponent<FixedJoint>());
            }
            //删除零件列表项目
            for(int i = 0; i < AssemblyParts.options.Count ; i++)
            {
                if (AssemblyParts.options[i].text==AssemblyParts.captionText.text)
                {
                    AssemblyParts.options.RemoveAt(i);
                    AssemblyParts.GetComponent<Dropdown>().value = 0;
                }
            }
            //删除数组中对应轴的零件项目
            Assembly[AssemblyShaft.captionText.text].Remove(GameObject.Find(AssemblyParts.captionText.text));
            //如果移除装配轴仅有的一个装配关系的零件，则同时移除装配轴
            if (Assembly[AssemblyShaft.captionText.text].Count <=1)
            {
                PartListRefresh = true;
                //移除轴
                //移除轴列表对应项目
                Assembly.Remove(AssemblyShaft.captionText.text);
                for(int i = 0; i <= AssemblyShaft.options.Count; i++)
                {
                    if(AssemblyShaft.options[i].text == AssemblyShaft.captionText.text)
                    {
                        AssemblyShaft.options.RemoveAt(i);
                    }
                }
                AssemblyShaft.GetComponent<Dropdown>().value = 0;
            }
        }
        //删除对应下拉列表中的项目
    }
}
