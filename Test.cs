using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System;

/// <summary>
/// 设置前先导入总承下的所有零件，作为信息索引
/// 管理输入资料，界面输入放置txt信息集合的文件夹的绝对路径
/// 再由程序动态录入所有零件的装配信息，一个零件一个txt
/// 以零件名为键，零件读入的装配信息的string[]为值储存进dictionary中
/// </summary>
public class Test : MonoBehaviour
{
    public int a = 0;
    ValueChange whenChange;
    ValueChange.MyValueChanged what2Do;

    public GameObject mObj;
    List<GameObject> Obj = new List<GameObject>();
    List<Vector3> p = new List<Vector3>();
    void Start()
    {
        what2Do += event1;
        what2Do += event2;
        what2Do += event3;

        whenChange = new ValueChange(a, what2Do);

        foreach (Transform child in mObj.transform)
        {
            Obj.Add(child.transform.gameObject);
        }
        for (int i = 0; i < 30; i++)
        {
            for (int j = 0; j < 30; j++)
            {
                p.Add(new Vector3(i * 5f, j * 5f, 0));
            }
        }
        for (int i = 0; i < Obj.Count; i++)
        {
            Obj[i].transform.position = p[i];
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            a = (int)whenChange.MyValue;
            whenChange.MyValue = 11;
            print(whenChange.MyValue);
        }
    }

    public void event1(object value, EventArgs do1)
    {
        print("1");
    }
    public void event2(object value, EventArgs do1)
    {
        print("2");
    }
    public void event3(object value, EventArgs do1)
    {
        print("3");
    }

}
