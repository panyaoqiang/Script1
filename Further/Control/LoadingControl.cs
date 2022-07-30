using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

/// <summary>
/// 设置前先导入总承下的所有零件，作为信息索引
/// 管理输入资料，界面输入放置txt信息集合的文件夹的绝对路径
/// 再由程序动态录入所有零件的装配信息，一个零件一个txt
/// 以零件名为键，零件读入的装配信息的string[]为值储存进dictionary中
/// </summary>
public class LoadingControl : MonoBehaviour
{
    /// <summary>
    /// 输入面板，读入信息成功后隐藏
    /// </summary>
    public GameObject Panel;
    /// <summary>
    /// 零件总承
    /// </summary>
    public GameObject Whole;
    /// <summary>
    /// 文件路径输入框
    /// </summary>
    public InputField InputTXTPath;
    /// <summary>
    /// 总承
    /// </summary>
    public List<GameObject> AllParts;
    /// <summary>
    /// 零件名索引
    /// </summary>
    string[] InfoName;
    /// <summary>
    /// 储存读入信息
    /// </summary>
    public Dictionary<string, string[]> AllInfo;
    /// <summary>
    /// 检查输入框文件格式，并触发读入程序
    /// </summary>
    public bool Input2Load;
    /// <summary>
    /// 开始加载轴装配工艺
    /// </summary>
    public bool LoadInfo;
    /// <summary>
    /// 记录所有装配主体的数量
    /// </summary>
    public int AxleNum=0;
    /// <summary>
    /// 记录已经加载的装配主体数量
    /// </summary>
    public int LoadedNum;
    // Start is called before the first frame update
    void Start()
    {
        Input2Load = false;
        LoadInfo = false;
        AllParts = new List<GameObject>();
        AddParts();
        AllInfo = new Dictionary<string, string[]>();
        LoadedNum = 0;
        if (AllParts.Count != 0)
        {
            InfoName = new string[AllParts.Count];
            for (int i = 0; i < AllParts.Count; i++)
            {
                InfoName[i] = (AllParts[i].gameObject.name);
            }
        }
    }

    private void Update()
    {
        if (Input2Load && AllParts.Count != 0)
        {
            InputPath();
            LoadInfo = true;
            Input2Load = false;
        }
    }
    /// <summary>
    /// 循环读入所有装配信息
    /// 输入路径，界面输入文件夹绝对路径，如C:\\Users\\LgN\\Desktop\\新建文件夹
    /// 自动添加文件夹下每一个零件对应的装配信息如+输入轴.txt
    /// </summary>
    public string[] InputInfo(string path)
    {
        string[] output;
        output = File.ReadAllLines(path);
        return output;
    }
    /// <summary>
    /// 读入信息
    /// </summary>
    public void InputPath()
    {
        if (InputTXTPath.text != null && InputTXTPath.text != "")
        {
            try
            {
                if (AllInfo.Count != 0)
                {
                    AllInfo.Clear();
                }
                for (int j = 0; j < InfoName.Length; j++)
                {
                    string[] d;
                    d = InputInfo(InputTXTPath.text + "\\" + InfoName[j] + ".txt");
                    AllInfo.Add(InfoName[j], d);
                    //for (int k = 0; k < d.Length; k++)
                    //{
                    //    Debug.Log(AllInfo[InfoName[j]][k]);
                    //}

                    Panel.SetActive(false);
                }
            }
            catch
            {
                InputTXTPath.text = "";
                InputTXTPath.placeholder.GetComponent<Text>().text = "输入错误，请输入正确文件夹路径";
            }
            Input2Load = false;
        }

    }
    /// <summary>
    /// 拖入总承父物体，获取所有零件
    /// </summary>
    public void AddParts()
    {
        foreach (Transform a in Whole.transform)
        {
            AllParts.Add(GameObject.Find(a.gameObject.name));
            if (a.gameObject.tag == "壳体" || a.gameObject.tag == "装配轴")
            { 
                AxleNum+=1;
            }
        }
    }

    public void Click2Load()
    {
        Input2Load = true;
    }
}
