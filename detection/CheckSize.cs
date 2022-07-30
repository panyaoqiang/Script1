using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class CheckSize : MonoBehaviour
{
    public Vector3 startPoint;
    public Vector3 finPoint;
    List<CheckBall> balls = new List<CheckBall>();
    IEnumerator pretreat;
    IEnumerator checkIterator;
    IEnumerator scaleIterator;
    IEnumerator readerIterator;
    public List<GameObject> checkBall = new List<GameObject>();
    public int best_Size;

    public List<GameObject> sim = new List<GameObject>();
    public List<Vector3> sizeData = new List<Vector3>();
    public bool start2CheckSize = false;
    List<GameObject> s = new List<GameObject>();
    List<float> sizeRange = new List<float>();
    string scaleDate;
    string scaleDate_Use;
    bool finOneClass = false;
    string shaft_name;
    string not_shaft_name;

    void Start()
    {
        pretreat = pretreatSim();
        //readerIterator = loadScale();
        //StartCoroutine(readerIterator);
    }

    private void Update()
    {
        if (start2CheckSize)
        {
            StartCoroutine(pretreat);
            start2CheckSize = false;
        }
    }

    void initialized()
    {
        sizeRange.Clear();
        s.Clear();
        scaleDate = "";
        scaleDate_Use = "";
        shaft_name = "";
        not_shaft_name = "";
        finOneClass = true;
    }

    /// <summary>
    /// 添加检测球及样本模型
    /// </summary>
    /// <returns></returns>
    IEnumerator pretreatSim()
    {
        for (int i = 0; i < checkBall.Count; i++)
        {
            balls.Add(checkBall[i].GetComponent<CheckBall>());
            yield return 0;
        }
        for (int i = 0; i < sim.Count; i++)
        {
            finOneClass = false;
            foreach (Transform child in sim[i].transform)
            {
                s.Add(child.transform.gameObject);
                yield return 0;
            }
            checkIterator = changeSim(sim[i].name);
            StartCoroutine(checkIterator);
            yield return new WaitUntil(() => finOneClass == true);
        }
        StopCoroutine(pretreat);
    }

    /// <summary>
    /// 第一步移动零件
    /// 第二步添加碰撞属性
    /// 第三步从零件质心发射小球
    /// 第四步获取信息
    /// 第五步停止发射小球
    /// 第六步取max并分级
    /// 第七步按照指定级别进行阈值缩放调整
    /// 第八步移除零件，清除数据并循环
    IEnumerator changeSim(string name)
    {
        //sim内包含零件集合的各类父物体
        for (int j = 0; j < s.Count; j++)
        {
            //移动零件
            s[j].transform.localPosition = startPoint;
            yield return new WaitForSeconds(0.1f);
            #region
            //Vector3 meshCenter = s[j].GetComponent<Rigidbody>().worldCenterOfMass;
            ////print(s[j].transform.position);
            ////获取零件质心
            ////print(s[j].name + meshCenter);
            ////获取零件基准轴方向
            //List<Vector3> moveDir = new List<Vector3>();
            //for (int i = 0; i < checkBall.Count; i++)
            //{
            //    switch (i)
            //    {
            //        case 0: moveDir.Add(s[j].transform.up); break;
            //        case 1: moveDir.Add(-s[j].transform.up); break;
            //        case 2: moveDir.Add(s[j].transform.right); break;
            //        case 3: moveDir.Add(-s[j].transform.right); break;
            //        case 4: moveDir.Add(s[j].transform.forward); break;
            //        case 5: moveDir.Add(-s[j].transform.forward); break;
            //        default: break;
            //    }
            //}
            ////每个小球进行发射
            //for (int i = 0; i < checkBall.Count; i++)
            //{
            //    balls[i].dir = moveDir[i];
            //    balls[i].gameObject.transform.position = meshCenter;
            //    //print(meshCenter+ balls[i].gameObject.name+s[j].transform.position);
            //    balls[i].move = true;
            //}
            //while (sizeData.Count != 6)
            //{
            //    yield return null;
            //}
            ////print(sizeData.Count);
            ////获取尺寸信息
            //float size = 1;
            //if (s[j].name.Contains("shaft"))
            //{
            //    size = maxSize(sizeData)[0];
            //}
            //else
            //{
            //    size = maxSize(sizeData)[0];
            //}
            //sizeRange.Add(size);
            //print(size + s[j].name);
            ////调整缩放比例
            ////s[j].transform.localScale *= size;
            ////移除零件
            #endregion
            float size;
            float[] box_size = facingCam(s[j]);
            if (s[j].name.Contains("shaft"))
            {
                size = box_size[1];
            }
            else
            {
                size = box_size[0];
            }
            sizeRange.Add(size);
            print(size + s[j].name);
            s[j].transform.localPosition = finPoint;
            //清空数据
            sizeData.Clear();
            yield return null;
        }
        StopCoroutine(checkIterator);
        scaleIterator = sizeInfo(name);
        StartCoroutine(scaleIterator);
    }

    /// <summary>
    /// 执行前，此次迭代已经完成单批零件的最值尺寸采样，每个零件的最值存放于sizeRange中
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    IEnumerator sizeInfo(string name)
    {
        #region
        //float max = 0;
        //float min = 0;
        ////获取此批次零件最值里的最值
        //for (int i = 0; i < sizeRange.Count; i++)
        //{
        //    if (max < sizeRange[i])
        //    {
        //        max = sizeRange[i];
        //    }
        //    if (min > sizeRange[i])
        //    {
        //        min = sizeRange[i];
        //    }
        //}
        //零件最值分级
        //float acc = (max - min) / 6f;
        //for (int i = 0; i < 7; i++)
        //{
        //    range.Add(acc * i + min);
        //}
        //int a1 = 0;
        //int a2 = 0;
        //int a3 = 0;
        //int a4 = 0;
        //int a5 = 0;
        //int a6 = 0;
        //for (int i = 0; i < sizeRange.Count; i++)
        //{
        //    if (sizeRange[i] >= range[0] && sizeRange[i] < range[1])
        //    {
        //        a1++;
        //    }
        //    else if (sizeRange[i] >= range[1] && sizeRange[i] < range[2])
        //    {
        //        a2++;
        //    }
        //    else if (sizeRange[i] >= range[2] && sizeRange[i] < range[3])
        //    {
        //        a3++;
        //    }
        //    else if (sizeRange[i] >= range[4] && sizeRange[i] < range[5])
        //    {
        //        a4++;
        //    }
        //    else if (sizeRange[i] >= range[5] && sizeRange[i] < range[6])
        //    {
        //        a5++;
        //    }
        //    else
        //    {
        //        a6++;
        //    }
        //}
        //sizeCount.AddRange(new int[6] { a1, a2, a3, a4, a5, a6 });
        //int maxCount = 0;
        //for (int i = 0; i < sizeCount.Count; i++)
        //{
        //    if (sizeCount[i] > maxCount)
        //    {
        //        maxCount = sizeCount[i];
        //    }
        //}
        //List<float> scale = new List<float>();
        //float mainMin = sizeRange[maxCount];
        //float mainMax = sizeRange[maxCount + 1];
        //float mainMed = mainMin + (mainMax - mainMin) / 2f;
        //for (int i = 0; i < sizeCount.Count; i++)
        //{
        //    float mid = range[i] + (range[i + 1] - range[i]) / 2f;
        //    float k = mainMed / mid;
        //    scale.Add(k);
        //    //print(k + "缩放因子");
        //}
        #endregion
        for (int i = 0; i < s.Count; i++)
        {
            float k = best_Size / sizeRange[i];
            s[i].transform.localScale *= k;
            scaleDate += k + "\n";
            scaleDate_Use += s[i].name + ":" + k + "\n";
            yield return null;
        }
        WriteFileByLine("data", name + "1.txt", scaleDate);
        WriteFileByLine("data", name + "_Use.txt", scaleDate_Use);
        WriteFileByLine("data", "shaft_name.txt", shaft_name);
        WriteFileByLine("data", "not_shaft_name.txt", not_shaft_name);
        initialized();
        print("fin");
        StopCoroutine(scaleIterator);
    }
    /// <summary>
    /// 对于轴系零件找长度
    /// 对于回转体零件找半径
    /// </summary>
    /// <param name="size"></param>
    /// <returns>返回max，min</returns>
    float[] maxSize(List<Vector3> size)
    {
        float up = (size[0] - size[1]).magnitude;
        float right = (size[2] - size[3]).magnitude;
        float forward = (size[4] - size[5]).magnitude;
        float max = up;
        float min = up;
        if (right > max)
        {
            max = right;
            if (forward >= max)
            {
                max = forward;
            }
        }
        else
        {
            if (forward >= max)
            {
                max = forward;
            }
        }
        if (right < min)
        {
            min = right;
            if (forward <= min)
            {
                min = forward;
            }
        }
        else
        {
            if (forward <= min)
            {
                min = forward;
            }
        }
        return new float[2] { max, min };
    }

    void WriteFileByLine(string file_path, string file_name, string str_info)//写入文件
    {
        StreamWriter sw;
        //if (!File.Exists(file_path + "//" + file_name))
        //{

        if (file_path == "")
        {
            sw = File.CreateText(file_name);//创建一个用于写入 UTF-8 编码的文本  
        }
        else
        {
            sw = File.CreateText(file_path + "//" + file_name);//创建一个用于写入 UTF-8 编码的文本  
        }
        //Debug.Log("文件创建成功！");
        //}
        //else
        //{
        //    if (file_path == "")
        //    {
        //        sw = File.AppendText(file_name);//打开现有 UTF-8 编码文本文件以进行读取  
        //    }
        //    else
        //    {
        //        sw = File.AppendText(file_path + "//" + file_name);//打开现有 UTF-8 编码文本文件以进行读取  
        //    }
        //}
        sw.WriteLine(str_info);//以行为单位写入字符串  
        sw.Close();
        sw.Dispose();//文件流释放  
    }

    IEnumerator loadScale()
    {
        for (int j = 0; j < sim.Count; j++)
        {
            List<float> sizeScale = new List<float>();
            List<GameObject> scaleObj = new List<GameObject>();
            string path = "data/" + sim[j].name + "1.txt";
            string[] scale = File.ReadAllLines(path);
            print(scale.Length);
            for (int i = 0; i < scale.Length; i++)
            {
                if (i == scale.Length - 1)
                {
                    print(scale[i] + sim[j].name + i);
                }
                else
                {
                    sizeScale.Add(float.Parse(scale[i]));
                }
            }
            //循环完毕sizeScale中存放该类别下所有零件的缩放比例
            foreach (Transform chile in sim[j].transform)
            {
                scaleObj.Add(chile.gameObject);
            }
            //() => scaleObj.Count == sizeScale.Count;
            for (int i = 0; i < scaleObj.Count; i++)
            {
                scaleObj[i].transform.localScale = scaleObj[i].transform.localScale * sizeScale[i];
                //if (sim[j].name.Contains("sleeve"))
                //{
                //    scaleObj[i].transform.localScale = scaleObj[i].transform.localScale * sizeScale[i] * 0.6f;
                //}
            }
            yield return null;
        }
        StopCoroutine(readerIterator);

    }

    /// <summary>
    /// 使零件面向相机x轴，
    /// </summary>
    /// <param name="box">传入零件并获取零件的boxcollider</param>
    /// <returns>r,l</returns>
    float[] facingCam(GameObject box)
    {
        float scale = 0;
        float r = 0;
        float l = 0;
        float x = box.GetComponent<BoxCollider>().size.x;
        float y = box.GetComponent<BoxCollider>().size.y;
        float z = box.GetComponent<BoxCollider>().size.z;
        float xy = Mathf.Abs(y - x);
        float yz = Mathf.Abs(y - z);
        float xz = Mathf.Abs(x - z);
        float min = xy;
        if (yz < min)
        {
            min = yz;
        }
        if (xz < min)
        {
            min = xz;
        }
        //z轴为正面，绕y轴旋转90°
        if (min == xy)
        {
            box.transform.eulerAngles += new Vector3(0, 90, 0);
            scale = (z / x);
            r = x;
            l = z;
        }
        //y轴为正面,绕z轴旋转90°
        else if (min == xz)
        {
            box.transform.eulerAngles += new Vector3(0, 0, 90);
            scale = (y / x);
            r = x;
            l = y;
        }
        //x轴为正面不用转动
        else
        {
            scale = (x / y);
            r = y;
            l = x;
        }

        if (l / r >= 1.2)
        {
            shaft_name += box.name + "\n";
            print("轴");
        }
        else
        {
            not_shaft_name += box.name + "\n";
            print("回转体");
        }
        return new float[2] { r, l };
    }
}
