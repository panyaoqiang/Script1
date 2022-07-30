using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 接收socket传输的信息
/// 读取txt记录的信息
/// 分类装载
/// 对单个零件进行分段存储
/// 对单个零件每一段进行分类，并将数据喂入对应的检测程序
/// </summary>
public class detectController : MonoBehaviour
{
    #region
    //#region UI层
    //public InputField InputTXTPath;
    //public GameObject Panel;
    ///// <summary>
    ///// 载入按钮开关
    ///// </summary>
    //public bool Input2Load = false;
    //public string path = "";
    //#endregion


    //#region 数据层
    //public GameObject simsFather;
    //public char separator = ',';
    ///// <summary>
    ///// 迭代开关，状态变化时开始单个零件下一段落检测
    ///// 0初始状态
    ///// 1正在检测
    ///// </summary>
    //public int iter = 0;
    //public InsideDetection inter;
    //public HelicalDetection helical;
    //public EngagementDetection engagement;
    //public StraightDetection straight;

    //List<GameObject> sims = new List<GameObject>();
    ///// <summary>
    ///// 所有零件经过处理的信息
    ///// </summary>
    //Dictionary<string, Dictionary<string, List<float>>> allParts
    //    = new Dictionary<string, Dictionary<string, List<float>>>();
    ///// <summary>
    ///// 所有零件的所有未处理string信息
    ///// </summary>
    //Dictionary<string, string[]> tempInfo = new Dictionary<string, string[]>();

    /////// <summary>
    /////// 检测规划
    /////// </summary>
    //IEnumerator plan;
    //#endregion

    //void Start()
    //{
    //    plan = planDetection();
    //    //StartCoroutine();
    //}

    //// Update is called once per frame
    //void Update()
    //{
    //    if (Input2Load)
    //    {
    //        InputPath();
    //    }
    //}
    ///// <summary>
    ///// 读入信息调用检测程序并建模5
    ///// </summary>
    //void InputPath()
    //{
    //    //使用UI输入地址
    //    if (InputTXTPath.text != null && InputTXTPath.text != "")
    //    {
    //        try
    //        {
    //            for (int j = 0; j < sims.Count; j++)
    //            {
    //                string[] d;
    //                //逐行读取数据
    //                d = File.ReadAllLines(InputTXTPath.text + "\\" + sims[j].name + ".txt");
    //                tempInfo.Add(sims[j].name, d);
    //                //for (int k = 0; k < d.Length; k++)
    //                //{
    //                //    Debug.Log(AllInfo[InfoName[j]][k]);
    //                //}
    //                Panel.SetActive(false);
    //            }
    //        }
    //        catch
    //        {
    //            InputTXTPath.text = "";
    //            InputTXTPath.placeholder.GetComponent<Text>().text = "输入错误，请输入正确文件夹路径";
    //        }
    //        Input2Load = false;
    //    }
    //    //不使用UI输入地址，测试用
    //    else
    //    {
    //        try
    //        {
    //            for (int j = 0; j < sims.Count; j++)
    //            {
    //                string[] d;
    //                //C:\\gear_ (88).txt
    //                d = File.ReadAllLines(path + "\\" + sims[j].name + ".txt");
    //                tempInfo.Add(sims[j].name, d);
    //                //for (int k = 0; k < d.Length; k++)
    //                //{
    //                //    Debug.Log(AllInfo[InfoName[j]][k]);
    //                //}
    //                Panel.SetActive(false);
    //            }
    //        }
    //        catch
    //        {
    //            path = "";
    //        }
    //        Input2Load = false;
    //    }
    //    protocol();
    //    StartCoroutine(plan);
    //}
    ///// <summary>
    ///// 从tempInfo中获取单个零件的尺寸信息并存入字典
    ///// 检测信息（零件名，半径，宽度，相对坐标，）
    ///// </summary>
    ///// <returns>返回单个零件所有规范信息</returns>
    //void protocol()
    //{
    //    //temp存放单个零件所有段落信息
    //    Dictionary<string, List<float>> temp = new Dictionary<string, List<float>>();
    //    for (int i = 0; i < sims.Count; i++)
    //    {
    //        //读取每个零件对应的txt信息文件，每一行存一个数储存单个零件的每段信息
    //        //info对应每个零件的所有段信息，每一个数据表示一段的数据
    //        //（段落类型，半径，宽度，相对坐标）
    //        string[] info = tempInfo[sims[i].name];
    //        for (int j = 0; j < info.Length; j++)
    //        {
    //            //info[j]对应每一段信息
    //            String2Dic(info[j], temp);
    //        }
    //        allParts.Add(sims[i].name, temp);
    //        //迭代完成后temp完整储存了单个零件所有信息
    //        temp.Clear();
    //    }
    //}
    ///// <summary>
    ///// 转换单个零件某一段信息
    ///// 从string(类别名，半径，宽度，零件中心轴线相对坐标)
    ///// 变为string（类别名）+float[3]（半径，宽度，零件中心轴线相对坐标）
    ///// </summary>
    ///// <param name="t">传入txt某一行包含有用信息的字符串</param>
    ///// <returns>返回float[3]（零件名，零件中心轴线相对坐标，宽度，半径）</returns>
    //public void String2Dic(string t, Dictionary<string, List<float>> temp)
    //{
    //    List<float> size = new List<float>();
    //    char[] c = t.ToCharArray();
    //    List<int> breakPoint = new List<int>();
    //    for (int i = 0; i < c.Length; i++)
    //    {
    //        if (c[i] == separator)
    //        {
    //            breakPoint.Add(i);
    //        }
    //    }
    //    string jb = "";
    //    string type = "";
    //    for (int i = 0; i < breakPoint[0]; i++)
    //    {
    //        name += c[i];
    //    }
    //    for (int i = breakPoint[0] + 1; i < breakPoint[1]; i++)
    //    {
    //        jb += c[i];
    //    }
    //    size.Add(float.Parse(jb));
    //    jb = "";
    //    for (int i = breakPoint[1] + 1; i < breakPoint[2]; i++)
    //    {
    //        jb += c[i];
    //    }
    //    size.Add(float.Parse(jb));
    //    jb = "";
    //    for (int i = breakPoint[2] + 1; i < breakPoint[3]; i++)
    //    {
    //        jb += c[i];
    //    }
    //    size.Add(float.Parse(jb));
    //    temp.Add(type, size);
    //}

    ///// <summary>
    ///// 计划单个零件的检测步骤
    ///// </summary>
    //IEnumerator planDetection()
    //{
    //    //遍历所有检测零件
    //    for (int i = 0; i < sims.Count; i++)
    //    {
    //        //遍历每个零件的所有信息
    //        for (int j = 0; j < allParts[sims[j].name].Count; j++)
    //        {
    //            sims[j].transform.position = Vector3.zero;
    //            //遍历每个零件的每一段信息
    //            List<string> typeOfSegament = new List<string>();
    //            foreach (string name in allParts[sims[j].name].Keys)
    //            {
    //                typeOfSegament.Add(name);
    //            }
    //            int k = 0;
    //            iter = 0;
    //            while (k < typeOfSegament.Count && iter == 0)
    //            {
    //                //获取当前段的类型索引，并储存对应信息
    //                List<float> temp = allParts[sims[j].name][typeOfSegament[k]];
    //                //获取当前段类型数据
    //                //传输数据为检测器初始值赋值
    //                //打开检测开关并保持状态
    //                switch (typeOfSegament[k])
    //                {
    //                    case "helical":
    //                        detectionTeethAssign(typeOfSegament[k], temp, sims[j]);
    //                        break;
    //                    case "straight":
    //                        detectionTeethAssign(typeOfSegament[k], temp, sims[j]);
    //                        break;
    //                    case "ring":
    //                        //直接调用ring建模
    //                        creatShaft(sims[j], temp);
    //                        break;
    //                    case "internal":
    //                        detectionTeethAssign(typeOfSegament[k], temp, sims[j]);
    //                        break;
    //                    case "bevel":
    //                        iter = 1;
    //                        //外接模块化建模程序;
    //                        break;
    //                    case "engagement":
    //                        detectionTeethAssign(typeOfSegament[k], temp, sims[j]);
    //                        break;
    //                    default:
    //                        iter=1;
    //                        break;
    //                }
    //                k++;
    //                yield return 0;
    //            }
    //            while(k < typeOfSegament.Count && iter == 1)
    //            {
    //                print("正在检测");
    //                yield return 0;
    //            }
    //            yield return 0;
    //        }
    //    }
    //}
    ///// <summary>
    ///// 赋值完毕后开始检测，改变状态开关
    ///// </summary>
    ///// <param name="type">段类型</param>
    ///// <param name="info">段信息</param>
    ///// <param name="sim">检测零件</param>
    //void detectionTeethAssign(string type, List<float> info, GameObject sim)
    //{
    //    switch (type)
    //    {
    //        case "straight":
    //            straight.socketInfo.Clear();
    //            straight.socketInfo = info;
    //            straight.sim = sim;
    //            straight.start2DetectStraight = true;
    //            break;
    //        case "helical":
    //            helical.socketInfo.Clear();
    //            helical.socketInfo = info;
    //            helical.sim = sim;
    //            helical.start2DetectHelical = true;
    //            break;
    //        case "engagement":
    //            engagement.socketInfo.Clear();
    //            engagement.socketInfo = info;
    //            engagement.sim = sim;
    //            engagement.start2DetectEngagement = true;
    //            break;
    //        case "internal":
    //            inter.socketInfo.Clear();
    //            inter.socketInfo = info;
    //            inter.sim = sim;
    //            inter.start2DetectInside = true;
    //            break;
    //        default:
    //            break;
    //    }
    //    iter = 1;
    //}

    //void creatShaft(GameObject sim, List<float> tempInfo)
    //{
    //    Ring r1 = sim.transform.GetChild(0).gameObject.AddComponent<Ring>();
    //    r1.Acc = 30;
    //    r1.R = tempInfo[0];
    //    r1.Thick = tempInfo[1];
    //    r1.Inside_R = 0.5f;
    //    r1.Shift = tempInfo[2];
    //    r1.start2Creat = true;
    //}   
    #endregion
    public string modelPath;
    public string reCreatColliderPath;
    /// <summary>
    /// 原三维模型集合
    /// </summary>
    public GameObject oriObj;
    public Vector3 endPoint = new Vector3(200, 200, 0);
    /// <summary>
    /// 标准包裹体
    /// </summary>
    public GameObject standard;
    /// <summary>
    /// 再生零件体集合
    /// 原生模型集合，三维模型存在偏移，装配基准不一定为center。重置后标准包裹体原点设置为center
    /// 此时，再生零件体在新集合内的相对坐标为：原模型集合内相对坐标+center
    /// </summary>
    public GameObject aggregate;
    public GameObject ballList;
    public GameObject cylinderList;
    public GameObject ringList;
    IEnumerator startDetection;
    IEnumerator startRebuild;
    /// <summary>
    /// 总开关
    /// </summary>
    public bool start2Rebuild = false;
    /// <summary>
    /// 最终数据存放点name，[list x，list r，list thick，list type, string partsType]
    /// </summary>
    public Dictionary<string, blockType> allInfo = new Dictionary<string, blockType>();
    Dictionary<string, List<Vector3>> oriTrans = new Dictionary<string, List<Vector3>>();
    /// <summary>
    /// 检测小球集合，内齿齿数碰撞，外齿齿数碰撞，斜齿螺旋角碰撞
    /// </summary>
    List<GameObject> balls = new List<GameObject>();
    /// <summary>
    /// 检测内径碰撞柱体
    /// </summary>
    List<GameObject> cylinders = new List<GameObject>();
    List<GameObject> rings = new List<GameObject>();
    /// <summary>
    /// 装在所有零件
    /// </summary>
    List<GameObject> all3DMs = new List<GameObject>();
    /// <summary>
    /// shaft、gear、ring分类按键存放零件
    /// </summary>
    public Dictionary<string, List<GameObject>> allPartsType = new Dictionary<string, List<GameObject>>();
    [System.Obsolete]
    private void Start()
    {
        reCreatColliderPath = "G:/data/test/AllCollider";
        aggregate.transform.position = Vector3.one * 100f;
        foreach (Transform transform in ballList.transform)
        {
            transform.position = Vector3.one * 50f;
            balls.Add(transform.gameObject);
        }
        foreach (Transform transform in cylinderList.transform)
        {
            transform.position = Vector3.one * 50f;
            cylinders.Add(transform.gameObject);
        }
        foreach (Transform transform in ringList.transform)
        {
            transform.position = Vector3.one * 50f;
            rings.Add(transform.gameObject);
        }
        //tempFeature.Add("external", externalF);
        //tempFeature.Add("internal", internalF);
        startDetection = readInfo2Detect();
        startRebuild = recreatCollider();
    }

    private void FixedUpdate()
    {
        //若无事先赋值，使用直接加载模型
        if (start2Rebuild && oriObj == null)
        {
            get3DModel();
            start2Rebuild = false;
        }
        if (start2Rebuild && oriObj != null)
        {
            incorporated3DModel();
            start2Rebuild = false;
        }
        EveryInternalDetectFin();
        EveryDetectFin();
    }

    /// <summary>
    /// 加载资源文件内的三维模型
    /// </summary>
    public void get3DModel()
    {
        //重建完毕后删除三维模型中的boxcollider
        GameObject temp3DM = Instantiate(Resources.Load("data/model") as GameObject);
        foreach (Transform child in temp3DM.transform)
        {
            GameObject newObj = Instantiate(standard, aggregate.transform);
            GameObject newChild = Instantiate(child.gameObject, aggregate.transform);
            Vector3 oriPos = child.transform.localPosition;
            newObj.transform.localPosition = oriPos;
            newObj.name = child.name.Replace(" ", "");
            newChild.transform.SetParent(newObj.transform.GetChild(4).transform);
            newChild.transform.localPosition = Vector3.zero;
            all3DMs.Add(newChild);
        }
        temp3DM.SetActive(false);
        StartCoroutine(startDetection);
    }

    /// <summary>
    /// 直接使用场景内的模型
    /// </summary>
    void incorporated3DModel()
    {
        foreach (Transform child in oriObj.transform)
        {
            all3DMs.Add(child.gameObject);
            string newName = child.name.Replace(" ", "");
            child.name = newName;
            List<Vector3> oriT = new List<Vector3>() { child.localPosition, child.localEulerAngles };
            oriTrans.Add(newName, oriT);
        }
        StartCoroutine(startDetection);
    }

    /// <summary>
    /// 获取全部外轮廓，创建对应类型检测对象
    /// 检测是否空心，并在检测完毕下一帧获取结果
    /// 若空心，则按照每一个轴段创建内轮廓检测对象internalDetection
    /// 创建完毕并添加所有检测对象的开关方法、添加所有对象检测完毕的信号列表
    /// </summary>
    IEnumerator readInfo2Detect()
    {
        //print(all3DMs.Count);
        while (i < all3DMs.Count)
        {
            tempObj = restructureObj(all3DMs[i]);
            //清空临时数据
            string name = all3DMs[i].name;
            //单个零件全部段落信息[list x，list r，list thick，list type, string partsType]
            //读取所有外轮廓段落信息，从每段type段落类型确定新建检测、信息对象
            blockType info = allInfo[name];
            //新建一个列表，装载单个零件所有段落的检测数据
            //建立对象后对其进行实例化初始赋值，传入每段x，r，thick
            //列表与allInfo中的索引值一一对应，新建完所有段落信息对象后，暂停协程等待检测完成
            print("第" + i + "个零件" + name);
            int tempDetetionBallCount = 0;
            int tempDetetionCylinderCount = 0;
            int tempDetetionRingCount = 0;
            for (int j = 0; j < info.type.Count; j++)
            {
                //print(info.x[j] + info.type[j]);
                switch (info.type[j])
                {
                    //一个柱体
                    //检测内径
                    case ("Ring"):
                        detectInfo.ring r = new detectInfo.ring();
                        r.R = info.r[j];
                        r.r = info.r[j] * 0.98f;
                        r.thick = info.thick[j];
                        r.x = info.x[j];
                        r.mat = all3DMs[i].GetComponent<MeshRenderer>().material;
                        RingDetection ringD = new RingDetection();
                        tempRingInfo.Add(r);
                        tempRingObj.Add(ringD);
                        ringD.start2Check();
                        break;
                    //一个柱体，两个球
                    //检测模数，齿数，螺旋角，旋向
                    case ("Helical"):
                        tempDetetionBallCount += 2;
                        tempDetetionCylinderCount += 1;
                        detectInfo.helical h = new detectInfo.helical();
                        h.x = info.x[j];
                        h.thick = info.thick[j];
                        h.ra = info.r[j];
                        h.mat = all3DMs[i].GetComponent<MeshRenderer>().material;
                        HelicalDetection helicalD = new HelicalDetection(balls[tempDetetionBallCount - 1],
                            balls[tempDetetionBallCount], cylinders[tempDetetionCylinderCount],
                            info.r[j], endPoint, info.x[j], Vector3.right, info.thick[j]);
                        tempHelixInfo.Add(h);
                        tempHelixObj.Add(helicalD);
                        helicalD.start2Check();
                        break;
                    //一个球
                    //检测模数，齿数
                    case ("Straight"):
                        tempDetetionBallCount += 1;
                        //新建信息类储存检测信息
                        //新建检测对象进行检测
                        detectInfo.straight s = new detectInfo.straight();
                        s.thick = info.thick[j];
                        s.x = info.x[j];
                        s.ra = info.r[j];
                        //print(s.x + "" + s.thick + "计算结果" + (s.x - 0.5f * s.thick));
                        s.mat = all3DMs[i].GetComponent<MeshRenderer>().material;
                        StraightDetection straightD = new StraightDetection(balls[tempDetetionBallCount]
                            , info.r[j], endPoint, info.x[j], Vector3.right);
                        tempStraightInfo.Add(s);
                        tempStraightObj.Add(straightD);
                        straightD.start2Check();
                        break;
                    default: break;
                }
            }
            //实例化圆柱体检测其代理碰撞stay事件，确定空心
            InsideDetection solidOrNot = new InsideDetection(cylinders[tempDetetionCylinderCount + 1],
                balls[tempDetetionBallCount + 1], endPoint, info.x[0], Vector3.right);
            cylinders[tempDetetionCylinderCount + 1].transform.localScale += Vector3.up * 10;
            yield return new WaitForFixedUpdate();
            //下一帧开始检测内轮廓扫描
            if (solidOrNot.soh == 0)
            {
                tempDetetionRingCount += 2;
                cylinders[tempDetetionCylinderCount + 1].GetComponent<CheckGearBall>().finWorking();
                balls[tempDetetionBallCount + 1].GetComponent<CheckGearBall>().finWorking();
                tempOriInternalDetection = new InsidePreDetection
                    (rings[tempDetetionRingCount - 1], rings[tempDetetionRingCount],
                    endPoint, Vector3.right, info.x[0] - info.thick[0] * 0.3f,
                    info.x[info.x.Count - 1] + info.thick[info.thick.Count - 1] * 0.3f);
                tempOriInternalDetection.start2Check();
                GetOnePartInternalInfo = true;
                StopCoroutine(startDetection);
                yield return 0;
                //每一段检测一次内径
                for (int j = 0; j < internalX.Count; j++)//info替换
                {
                    tempDetetionRingCount += 1;
                    tempDetetionBallCount += 1;
                    //检测信息容器
                    //print("内轮廓" + internalX[j]);
                    detectInfo.internalGear inter = new detectInfo.internalGear();
                    inter.x = internalX[j];
                    inter.thick = internalThick[j];
                    inter.mat = all3DMs[i].GetComponent<MeshRenderer>().material;
                    InsideDetection insideD = new InsideDetection(rings[tempDetetionRingCount],
                        balls[tempDetetionBallCount], endPoint, internalX[j], Vector3.right);
                    tempInternalInfo.Add(inter);
                    tempInsideObj.Add(insideD);
                    insideD.start2Check();
                }
            }
            //实心，结束柱体任务
            else
            {
                cylinders[tempDetetionCylinderCount + 1].GetComponent<CheckGearBall>().finWorking();
                balls[tempDetetionBallCount + 1].GetComponent<CheckGearBall>().finWorking();

            }
            finGetOnePartDetectObj = true;
            StopCoroutine(startDetection);
            i++;
            yield return new WaitForFixedUpdate();
            //helixCount = 0;
        }
        print("fin");
        StopCoroutine(startDetection);
    }

    #region 临时数据。单个零件完成创建后初始化
    int i = 0;
    bool GetOnePartInternalInfo = false;
    bool finGetOnePartDetectObj = false;
    int helixCount = 0;
    int straightCount = 0;
    int internalCount = 0;
    GameObject tempObj;
    InsidePreDetection tempOriInternalDetection;
    List<float> internalX = new List<float>();
    List<float> internalThick = new List<float>();
    List<HelicalDetection> tempHelixObj = new List<HelicalDetection>();
    List<StraightDetection> tempStraightObj = new List<StraightDetection>();
    List<InsideDetection> tempInsideObj = new List<InsideDetection>();
    List<RingDetection> tempRingObj = new List<RingDetection>();
    List<detectInfo.helical> tempHelixInfo = new List<detectInfo.helical>();
    List<detectInfo.straight> tempStraightInfo = new List<detectInfo.straight>();
    List<detectInfo.internalGear> tempInternalInfo = new List<detectInfo.internalGear>();
    List<detectInfo.ring> tempRingInfo = new List<detectInfo.ring>();
    #endregion

    //Dictionary<detection, detectInfo> tempDetect = new Dictionary<detection, detectInfo>();
    /// <summary>
    /// 获取所有检测对象，激活后获取其检测状态
    /// 装入对应的信息类对象
    /// </summary>
    /// <param name="h"></param>
    void EveryDetectFin()
    {
        if (finGetOnePartDetectObj)
        {
            //监听斜齿检测协程
            for (int i = 0; i < tempHelixObj.Count; i++)
            {
                IEnumerator getAngle = tempHelixObj[i].getHelixAngle();
                switch (tempHelixObj[i].assign)
                {
                    default: break;
                    case 1: StartCoroutine(getAngle); break;
                    case 2:
                        StopCoroutine(getAngle);
                        helixCount++;
                        detectInfo.helical h = tempHelixInfo[i];
                        h.helixAngle_Deg = tempHelixObj[i].getZ()[0];
                        h.z = tempHelixObj[i].getZ()[1];
                        if (tempHelixObj[i].getZ()[2] == 1.0f)
                        {
                            h.RightOrLeft = "左";
                        }
                        else
                        {
                            h.RightOrLeft = "右";
                        }
                        h.m = 2 * h.ra / (h.z + 2);
                        break;
                }
            }
            //监听直齿检测信号
            for (int i = 0; i < tempStraightObj.Count; i++)
            {
                if (tempStraightObj[i].assign == 1)
                {
                    //返回齿数
                    float z = tempStraightObj[i].getZ()[0];
                    detectInfo.straight s = tempStraightInfo[i];
                    s.z = z;
                    if (z > 14)
                    {
                        s.m = s.ra * 2 / (z + 2);
                    }
                    else
                    {
                        s.m = 0;
                    }
                    straightCount++;
                }
            }
            List<float> ra = new List<float>();
            //监听内轮廓检测信号
            for (int i = 0; i < tempInsideObj.Count; i++)
            {
                if (tempInsideObj[i].assign == 1)
                {
                    detectInfo.internalGear insideInfo = tempInternalInfo[i];
                    insideInfo.z = tempInsideObj[i].getZ()[0];
                    insideInfo.ra = tempInsideObj[i].getZ()[1];
                    ra.Add(insideInfo.ra);
                    if (insideInfo.z >= 14)
                    {
                        insideInfo.m = insideInfo.ra * 2 / (insideInfo.z + 0.25f);
                    }
                    else
                    {
                        insideInfo.m = 0;
                    }
                    internalCount++;
                }
            }
            //全部检测完成，获得tempDetect字典内所有信息赋值，开始建模
            if (helixCount == tempHelixObj.Count &&
                straightCount == tempStraightObj.Count &&
                internalCount == tempInsideObj.Count
                )
            {
                tempHelixObj.Clear();
                tempInsideObj.Clear();
                tempStraightObj.Clear();
                tempRingObj.Clear();
                helixCount = 0;
                straightCount = 0;
                internalCount = 0;
                finGetOnePartDetectObj = false;
                StartCoroutine(startRebuild);
            }
            //建模完毕后刷新临时变量并开始协程
        }
    }

    void EveryInternalDetectFin()
    {
        if (GetOnePartInternalInfo)
        {
            if (tempOriInternalDetection.assign == 1)
            {
                blockType internalThickAndX = tempOriInternalDetection.getRa();
                internalX.AddRange(internalThickAndX.x);
                internalThick.AddRange(internalThickAndX.thick);
                StartCoroutine(startDetection);
                GetOnePartInternalInfo = false;
            }
        }
    }

    [System.Obsolete]
    IEnumerator recreatCollider()
    {
        while (true)
        {
            GameObject script = tempObj.transform.FindChild("ColliderAndMesh").gameObject;
            Destroy(tempObj.transform.FindChild("3DModel").GetChild(0).GetComponent<MeshCollider>());
            Destroy(tempObj.transform.FindChild("3DModel").GetChild(0).GetComponent<BoxCollider>());
            Vector3 pos = tempObj.transform.FindChild("3DModel").GetChild(0).localPosition;
            Vector3 rot = tempObj.transform.FindChild("3DModel").GetChild(0).localEulerAngles;
            string outputInfo = pos + "\n" + rot + "\n";
            //print(tempObj.name);
            for (int i = 0; i < tempHelixInfo.Count; i++)
            {
                Helical_gear helix = script.AddComponent<Helical_gear>();
                //Helix.helix = tempHelixInfo[i];
                //Helix.assign();
                //helix.mat = tempHelixInfo[i].mat;
                helix.Shift = tempHelixInfo[i].x -
                    0.5f * tempHelixInfo[i].thick;
                helix.M = tempHelixInfo[i].m;
                helix.Z = tempHelixInfo[i].z;
                helix.thick = tempHelixInfo[i].thick;
                helix.Helix_Angle = tempHelixInfo[i].helixAngle_Deg;
                helix.Right_Left = tempHelixInfo[i].RightOrLeft;
                helix.Acc = 2;
                helix.start2Creat = true;

                //类型，shift，m，z，thick，Helix_Angle，Right_Left，Acc;
                outputInfo +=
                    "Helical_gear" + ", " +
                    helix.Shift.ToString() + ", " +
                    helix.M.ToString() + ", " +
                    helix.Z.ToString() + ", " +
                    helix.thick.ToString() + ", " +
                    helix.Helix_Angle.ToString() + ", " +
                    helix.Right_Left + ", " + helix.Acc.ToString() + "\n";


                yield return 0;
            }
            for (int i = 0; i < tempStraightInfo.Count; i++)
            {
                if (tempStraightInfo[i].z > 14)
                {
                    CreatMesh Straight = script.AddComponent<CreatMesh>();
                    //Straight.straight = tempStraightInfo[i];
                    //Straight.assign();
                    //Straight.mat = tempStraightInfo[i].mat;
                    //print(tempStraightInfo[i].x + "对应齿数" + tempStraightInfo[i].z);
                    Straight.Shift = -(tempStraightInfo[i].x + tempStraightInfo[i].thick * 0.5f);
                    Straight.M = tempStraightInfo[i].m;
                    Straight.Z = tempStraightInfo[i].z;
                    Straight.thick = tempStraightInfo[i].thick;
                    Straight.Acc = 2;
                    Straight.start2Creat = true;

                    //类型，shift，m，z，thick，Acc;
                    outputInfo +=
                    "CreatMesh" + ", " +
                    Straight.Shift.ToString() + ", " +
                    Straight.M.ToString() + ", " +
                    Straight.Z.ToString() + ", " +
                    Straight.thick.ToString() + ", " +
                    Straight.Acc.ToString() + "\n";

                }
                else if (tempStraightInfo[i].z == 0)
                {
                    Ring ring = script.AddComponent<Ring>();
                    //detectInfo.ring r = new detectInfo.ring();
                    ring.Shift = tempStraightInfo[i].x + tempStraightInfo[i].thick * 0.5f;
                    ring.R = tempStraightInfo[i].ra;
                    ring.Inside_R = tempStraightInfo[i].ra * 0.98f;
                    ring.Thick = tempStraightInfo[i].thick;
                    ring.Acc = 20;
                    //ring.ring = r;
                    //ring.assign();
                    ring.start2Creat = true;


                    //类型，shift，R，Inside_R，thick，Acc;
                    outputInfo +=
                   "Ring" + ", " +
                   ring.Shift.ToString() + ", " +
                   ring.R.ToString() + ", " +
                   ring.Inside_R.ToString() + ", " +
                   ring.Thick.ToString() + ", " +
                   ring.Acc.ToString() + "\n";

                }
                yield return 0;
            }
            for (int i = 0; i < tempInternalInfo.Count; i++)
            {
                if (tempInternalInfo[i].z == 0)
                {
                    Ring ring = script.AddComponent<Ring>();
                    //detectInfo.ring r = new detectInfo.ring();
                    ring.Shift = tempInternalInfo[i].x + tempInternalInfo[i].thick * 0.5f;
                    ring.R = tempInternalInfo[i].ra / 0.98f;
                    ring.Inside_R = tempInternalInfo[i].ra;
                    ring.Thick = tempInternalInfo[i].thick;
                    ring.Acc = 20;
                    //ring.mat = tempInternalInfo[i].mat;
                    //ring.ring = r;
                    //ring.assign();
                    ring.start2Creat = true;


                    //类型，shift，R，Inside_R，thick，Acc;
                    outputInfo +=
                   "Ring" + ", " +
                   ring.Shift.ToString() + ", " +
                   ring.R.ToString() + ", " +
                   ring.Inside_R.ToString() + ", " +
                   ring.Thick.ToString() + ", " +
                   ring.Acc.ToString() + "\n";
                }
                else
                {
                    CreatInsideTooths InternalGear = script.AddComponent<CreatInsideTooths>();
                    //InternalGear.inside = tempInternalInfo[i];
                    //InternalGear.assign();
                    //InternalGear.mat = tempInternalInfo[i].mat;
                    InternalGear.Shift = -(tempInternalInfo[i].x + tempInternalInfo[i].thick * 0.5f);
                    InternalGear.M = tempInternalInfo[i].m;
                    InternalGear.Z = tempInternalInfo[i].z;
                    InternalGear.Thick = tempInternalInfo[i].thick;
                    InternalGear.Acc = 2;
                    InternalGear.star2Creat = true;



                    //类型，shift，m，z，thick，Acc;
                    outputInfo +=
                   "CreatInsideTooths" + ", " +
                   InternalGear.Shift.ToString() + ", " +
                   InternalGear.M.ToString() + ", " +
                   InternalGear.Z.ToString() + ", " +
                   InternalGear.Thick.ToString() + ", " +
                   InternalGear.Acc.ToString() + "\n";
                }
                yield return 0;
            }
            for (int i = 0; i < tempRingInfo.Count; i++)
            {
                if (tempRingInfo[i].thick >= tempRingInfo[i].R * 0.1f)
                {
                    Ring ring = script.AddComponent<Ring>();
                    //ring.ring = tempRingInfo[i];
                    //ring.assign();
                    //ring.mat = tempRingInfo[i].mat;
                    ring.Shift = tempRingInfo[i].x +
                        tempRingInfo[i].thick * 0.5f;
                    ring.R = tempRingInfo[i].R;
                    ring.Inside_R = tempRingInfo[i].r;
                    ring.Thick = tempRingInfo[i].thick;
                    ring.Acc = 20;
                    ring.start2Creat = true;


                    //类型，shift，R，Inside_R，thick，Acc;
                    outputInfo +=
                   "Ring" + ", " +
                   ring.Shift.ToString() + ", " +
                   ring.R.ToString() + ", " +
                   ring.Inside_R.ToString() + ", " +
                   ring.Thick.ToString() + ", " +
                   ring.Acc.ToString() + "\n";
                }
                yield return 0;
            }
            WriteFileByLine(reCreatColliderPath, tempObj.name, outputInfo);
            tempHelixInfo.Clear();
            tempInternalInfo.Clear();
            tempRingInfo.Clear();
            tempStraightInfo.Clear();
            internalX.Clear();
            internalThick.Clear();
            tempObj.transform.SetParent(aggregate.transform);
            tempObj.transform.localPosition = oriTrans[tempObj.name][0];
            tempObj.transform.localEulerAngles = oriTrans[tempObj.name][1];
            tempObj = null;
            Destroy(tempOriInternalDetection);
            for (int i = 0; i < balls.Count; i++)
            {
                balls[i].GetComponent<CheckGearBall>().finWorking();
            }
            for (int i = 0; i < cylinders.Count; i++)
            {
                cylinders[i].GetComponent<CheckGearBall>().finWorking();
            }
            for (int i = 0; i < rings.Count; i++)
            {
                rings[i].GetComponent<CheckGearBall>().finWorking();
            }


            StartCoroutine(startDetection);
            StopCoroutine(startRebuild);
            yield return 0;
        }
    }
    /// <summary>
    /// 传入三维模型物体，编入父物体
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    public GameObject restructureObj(GameObject model)
    {
        Vector3 shift;
        if (model.GetComponent<BoxCollider>() == null)
        {
            model.AddComponent<BoxCollider>();
        }
        shift = model.GetComponent<BoxCollider>().center;
        model.GetComponent<BoxCollider>().enabled = false;
        Destroy(model.GetComponent<BoxCollider>());
        if (model.GetComponent<MeshCollider>() == null)
        {
            model.AddComponent<MeshCollider>();
        }
        GameObject newObj = Instantiate(standard);
        newObj.transform.position = Vector3.zero;
        newObj.name = model.name;
        GameObject newModel = Instantiate(model);
        newObj.transform.localEulerAngles = newModel.transform.localEulerAngles;
        newModel.transform.SetParent(newObj.transform.GetChild(4).transform);
        newModel.transform.localPosition = Vector3.zero;
        newModel.transform.position -= newModel.transform.right * shift.x;
        newModel.transform.position -= newModel.transform.forward * shift.z;
        newModel.transform.position -= newModel.transform.up * shift.y;

        newModel.transform.localEulerAngles = Vector3.zero;
        //model.transform.localScale = Vector3.one;
        //print(newObj.name + "每次迭代收编三维模型");
        return (newObj);
    }

    void WriteFileByLine(string file_path, string file_name, string str_info)//写入文件
    {
        StreamWriter sw;
        if (file_path == "")
        {
            sw = File.CreateText(file_name + ".txt");//创建一个用于写入 UTF-8 编码的文本  
        }
        else
        {
            sw = File.CreateText(file_path + "//" + file_name + ".txt");//创建一个用于写入 UTF-8 编码的文本  
        }
        sw.WriteLine(str_info);//以行为单位写入字符串  
        sw.Close();
        sw.Dispose();//文件流释放  
    }
}
/// <summary>
/// 每一个对象储存一个段落的轮廓信息，一个零件多个段落多个对象
/// 最终获取所有的检测信息，对应建立碰撞体脚本所需参数
/// </summary>
public class detectInfo
{
    /// <summary>
    /// 相对坐标x，模数，齿数，厚度，齿顶圆半径
    /// </summary>
    public class straight : detectInfo
    {
        public bool assign;
        public float x;
        public float m;
        public float z;
        public float thick;
        //(z+2)*m
        public float ra;
        //(z-2.5)*m
        public Material mat;
        //public float rf;
    }
    /// <summary>
    /// 相对坐标x，模数，齿数，厚度，螺旋角，旋向，齿顶圆半径，齿根圆半径
    /// </summary>
    public class helical : detectInfo
    {
        public bool assign;
        public float x;
        public float m;
        public float z;
        public float thick;
        public float helixAngle_Deg;
        public string RightOrLeft;
        //(z+2)*m
        public float ra;
        public Material mat;
        //(z-2.5)*m
        //public float rf;
    }
    /// <summary>
    /// 相对坐标x，模数，齿数，厚度，齿顶圆半径
    /// </summary>
    public class internalGear : detectInfo
    {
        public bool assign;
        public float x;
        public float m;
        public float z;
        public float thick;
        //(z+2)*m
        public float ra;
        public Material mat;
        //(z-2.5)*m
        //public float rf;
    }
    /// <summary>
    /// 相对坐标x，厚度，外径，内径
    /// </summary>
    public class ring : detectInfo
    {
        public bool assign;
        public float x;
        public float R;
        public float r;
        public float thick;
        public Material mat;
    }
}
public class detection : MonoBehaviour
{

}
public class RingDetection : detection
{
    public int assign;
    public void start2Check()
    {
        assign = 1;
    }
}
/// <summary>
/// 返回读取txt轴段信息
/// </summary>
public struct blockType
{
    public string partsType;
    public List<float> x;
    public List<float> r;
    public List<float> thick;
    public List<string> type;
}
public struct simInfo
{
    public float camSize;
    public Vector3 simAngle;
}

