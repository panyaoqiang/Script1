using System.Collections.Generic;
using UnityEngine;

//public class CheckGear : MonoBehaviour
//{
//    //public bool socketOrNot = false;
//    /// <summary>
//    /// 由客户端socket接收的yolo检测数据信息
//    /// 包含目标框4个顶点坐标，种类组合的集合，共4个三维向量及1个字符串的集合*n个
//    /// 可求得目标厚度，目标半径，目标离中心距离通过计算后对每一个检测目标装载进此列表
//    /// </summary>
//    public Dictionary<string, List<float>> wholeInfo = new Dictionary<string, List<float>>();

//    public Vector3 safePoint = new Vector3(200, 200, 0);
//    public List<GameObject> balls = new List<GameObject>();
//    public List<GameObject> sim = new List<GameObject>();
//    /// <summary>
//    /// 经检测后得到的数据，每个零件一组
//    /// </summary>
//    public Dictionary<string, List<float>> detectInfo = new Dictionary<string, List<float>>();
//    Dictionary<string, CheckGearBall> ballActions = new Dictionary<string, CheckGearBall>();

//    /// <summary>
//    /// 临时数据集
//    /// 通过迭代，每次抽取一个检测目标的检测数据组进行调用处理
//    /// </summary>
//    public List<float> socketInfo = new List<float>();
//    /// <summary>
//    /// 齿顶圆半径、齿宽、坐标位置、齿数
//    /// </summary>
//    public List<float> tempInfo = new List<float>();
//    public float tempCount = 0;
//    public string tempSimName = "";
//    /// <summary>
//    /// 0小球在模型外，1小球在模型内
//    /// </summary>
//    public int inAndOut = 0;
//    public List<Vector3> tempHelicalDetect = new List<Vector3>();
//    public int tempHelicalState = 0;
//    public int tempInsideState = 0;
//    public int tempEngagementState = 0;

//    public bool start2DetectStraight = false;
//    public bool start2DetectHelical = false;
//    public bool start2DetectInside = false;
//    public bool start2DetectEngagement = false;

//    void Start()
//    {
//        for (int i = 0; i < balls.Count; i++)
//        {
//            ballActions.Add(balls[i].name, balls[i].GetComponent<CheckGearBall>());
//        }
//    }

//    private void Update()
//    {
//        //协程控制喂入sim
//        if (start2DetectStraight)
//        {
//            detectStraight(sim[0],balls[0]);
//        }
//        if (start2DetectHelical)
//        {
//            detectHelical(sim[0], balls[0], balls[1], balls[2]) ;
//        }
//        if (start2DetectInside)
//        {
//            detectInside(sim[0], balls[0],balls[3]);
//        }
//        if (start2DetectEngagement)
//        {
//            detectEngagement(sim[0], balls[0]);
//        }
//    }
//    #region 通用工具集合
//    /// <summary>
//    /// 提取一组目标信息并放入临时数据集socketInfo中
//    /// </summary>
//    void processInfo()
//    {

//    }

//    void iterInitialized()
//    {
//        socketInfo.Clear();
//        tempInfo.Clear();
//        tempCount = 0;
//        tempSimName = "";
//        tempEngagementState = 0;
//        tempHelicalState = 0;
//        tempInsideState = 0;
//        start2DetectStraight = false;
//        start2DetectHelical = false;
//        start2DetectInside = false;
//        start2DetectEngagement = false;
//        for (int i = 0; i < balls.Count; i++)
//        {
//            balls[i].GetComponent<CheckGearBall>().onEnter2Do = null;
//            balls[i].GetComponent<CheckGearBall>().onExit2Do = null;
//            balls[i].GetComponent<CheckGearBall>().onStay2Do = null;
//            balls[i].GetComponent<CheckGearBall>().onRotFin2Do = null;
//            balls[i].GetComponent<CheckGearBall>().onRotInAngleFin2Do = null;
//        }
//    }

//    /// <summary>
//    /// 通用。每次迭代更新初始化信息，提取当个零件检测信息使用
//    /// </summary>
//    /// <param name="sim">传入此次迭代检测样本</param>
//    /// <returns>齿顶圆半径，齿宽，检测段相对坐标</returns>
//    public List<float> detectInitialized(GameObject sim)
//    {
//        tempCount = 0;
//        tempSimName = sim.name;
//        Vector3 simCenter = sim.transform.position;//sim.GetComponent<Rigidbody>().centerOfMass;
//        float r;
//        float thick;
//        float centerX;
//        //if (!socketOrNot)
//        //{
//        //    //半径
//        //    r = sim.GetComponent<BoxCollider>().size.y;
//        //    //齿宽
//        //    thick = sim.GetComponent<BoxCollider>().size.x;
//        //    //目标检测段落的1/2齿宽处，世界坐标
//        //    centerX = simCenter.x + socketInfo[0];
//        //}
//        //else
//        //{
//        r = socketInfo[0];
//        thick = socketInfo[1];
//        centerX = socketInfo[2];
//        //}
//        List<float> info = new List<float>();
//        info.Add(r);
//        info.Add(thick);
//        info.Add(centerX);
//        return (info);
//    }
//    #endregion 
//    #region 直齿检测工具集合
//    public void countStraightTeeth()
//    {
//        tempCount++;
//    }
//    public void stopCountStraightTeeth()
//    {
//        tempInfo.Add(tempCount);
//        //计算模数
//        float m = tempInfo[0] * 2 / (tempInfo[3] + 2);
//        float _m = m % 0.1f;
//        m -= _m;
//        //存入数组，按照模数，齿数，齿宽，相对坐标顺序存取
//        List<float> simInfo = new List<float>();
//        simInfo.Add(m);
//        simInfo.Add(tempCount);
//        simInfo.Add(tempInfo[1]);
//        simInfo.Add(tempInfo[2]);
//        //存入总检测信息数组中
//        detectInfo.Add(tempSimName, simInfo);
//        iterInitialized();
//        ballActions[balls[0].name].onEnter2Do -= countStraightTeeth;
//        ballActions[balls[0].name].onRotFin2Do -= stopCountStraightTeeth;
//    }
//    #endregion
//    /// <summary>
//    /// 使用0号球检测直齿
//    /// boxcollider获取齿顶圆直径，去1/2厚度处释放小球旋转
//    /// 记录齿数，齿顶圆半径，齿宽，计算模数
//    /// </summary>
//    public void detectStraight(GameObject sim, GameObject ball)
//    {
//        CheckGearBall.onEnter enterAction = ballActions[ball.name].onEnter2Do;
//        CheckGearBall.onRotFin rotAction = ballActions[ball.name].onRotFin2Do;
//        List<float> info = detectInitialized(sim);
//        //检测段落半径
//        float r = info[0];
//        //检测段落厚度
//        float thick = info[1];
//        //检测段落的中心，x轴上的坐标点
//        float centerX = info[2];
//        enterAction += countStraightTeeth;
//        rotAction += stopCountStraightTeeth;
//        ball.GetComponent<SphereCollider>().enabled = false;
//        ball.transform.position = new Vector3(centerX, r - 0.05f, 0);
//        ball.GetComponent<SphereCollider>().enabled = true;
//        //打开旋转开关，自动关闭
//        ball.GetComponent<CheckGearBall>().Angle = 360;
//        ball.GetComponent<CheckGearBall>().rotDirInAngle = sim.transform.right;
//        ball.GetComponent<CheckGearBall>().rotCenterInAngle =
//            sim.transform.position + new Vector3(centerX, 0, 0);
//        ball.GetComponent<CheckGearBall>().inAngleRot = true;
//        tempInfo.Add(r);
//        tempInfo.Add(thick);
//        tempInfo.Add(centerX);
//    }
//    #region 斜齿检测工具集合
//    public void countHelicalTeethEnter()
//    {
//        tempHelicalDetect.Add(balls[0].transform.position);
//        tempCount++;
//    }
//    public void countHelicalTeethExit()
//    {
//        tempHelicalDetect.Add(balls[0].transform.position);
//    }
//    /// <summary>
//    /// 第一次检测，返回1则代表在里面
//    /// 第二次检测，返回11则代表在里面
//    /// </summary>
//    public void judgeOnStay()
//    {
//        if (tempHelicalState == 1)
//        {
//            inAndOut = 1;
//        }
//        if (tempHelicalState == 3)
//        {
//            inAndOut = 11;
//        }
//    }

//    public void stopCountHelicalTeeth()
//    {
//        balls[0].transform.position =
//            (tempHelicalDetect[0] + tempHelicalDetect[1]) * 0.5f;
//        tempHelicalState = 1;
//        balls[0].GetComponent<CheckGearBall>().onEnter2Do -= countHelicalTeethEnter;
//        balls[0].GetComponent<CheckGearBall>().onExit2Do -= countHelicalTeethExit;
//        balls[0].GetComponent<CheckGearBall>().onRotFin2Do -= stopCountHelicalTeeth;
//        if (inAndOut != 1)
//        {
//            //必须让小球处于stay状态
//            balls[0].transform.position =
//            (tempHelicalDetect[2] + tempHelicalDetect[1]) * 0.5f;
//        }
//        tempHelicalDetect.Clear();
//        tempHelicalDetect.Add(balls[0].transform.position);
//        tempHelicalState = 2;
//    }
//    /// <summary>
//    /// 轴向移动至与斜齿碰撞并停止移动
//    /// </summary>
//    public void detectHelical2Exit()
//    {
//        balls[1].GetComponent<CheckGearBall>().move = false;
//        if (balls[2].GetComponent<CheckGearBall>().move == false)
//        {
//            tempHelicalState = 3;
//            balls[1].GetComponent<SphereCollider>().enabled = false;
//        }
//    }
//    /// <summary>
//    /// 轴向移动至与斜齿碰撞并停止移动
//    /// </summary>
//    public void detectHelical3Exit()
//    {
//        balls[2].GetComponent<CheckGearBall>().move = false;
//        if (balls[1].GetComponent<CheckGearBall>().move == false)
//        {
//            tempHelicalState = 3;
//            balls[2].GetComponent<SphereCollider>().enabled = false;
//        }
//    }
//    public void detectHelicalFinRot()
//    {
//        tempHelicalDetect.Add(balls[1].transform.position);
//        tempHelicalState = 4;
//    }
//    #endregion
//    /// <summary>
//    /// 斜齿
//    /// 获取1/3和2/3厚度处小球碰撞enter、exit的点集，选世界坐标系下z轴上的两点从上层投影到下层，
//    /// 获取点左右两侧的同类型最近点。
//    /// 取下层选中的四个点，按照y轴坐标排序，两两求中点并向内移动一定单位。
//    /// 取上层选中的两个点，求中点，向内移动一定单位
//    /// 此时共得上层一个点，下层两个点，共三个点，按照上连接下的方式，作两个向量
//    /// 使小球沿该向量运动，若发生exit-enter则排除此中点。
//    /// 若一直处于OnTrigger状态，则确认此向量向xoy平面投影的角度为螺旋角度
//    /// </summary>
//    public void detectHelical(GameObject sim, GameObject ball1, GameObject ball2, GameObject ball3)
//    {

//        if (tempHelicalState == 0)
//        {
//            sim.transform.position = Vector3.zero;
//            tempSimName = sim.name;
//            CheckGearBall.onEnter enterAction1 = ballActions[ball1.name].onEnter2Do;
//            CheckGearBall.onExit exitAction1 = ballActions[ball1.name].onExit2Do;
//            CheckGearBall.onRotFin rotAction1 = ballActions[ball1.name].onRotFin2Do;
//            CheckGearBall.onStay stayAction1 = ballActions[ball1.name].onStay2Do;
//            List<float> info = detectInitialized(sim);
//            //检测段落半径
//            float r = info[0];
//            //检测段落厚度
//            float thick = info[1];
//            //检测段落的中心，x轴上的坐标点
//            float centerX = info[2];
//            ////球1在1/3齿宽处
//            //float initalP1 = centerX + (1 / 6) * thick;
//            ////球2在2/3齿宽处
//            //float initalP2 = centerX - (1 / 6) * thick;

//            enterAction1 = countHelicalTeethEnter;
//            exitAction1 = countHelicalTeethExit;
//            rotAction1 = stopCountHelicalTeeth;
//            ball1.GetComponent<SphereCollider>().enabled = false;
//            ball1.transform.position = new Vector3(centerX, r - 0.05f, 0);
//            ball1.GetComponent<SphereCollider>().enabled = true;
//            //打开旋转开关，自动关闭
//            ball1.GetComponent<CheckGearBall>().Angle = 360;
//            ball1.GetComponent<CheckGearBall>().rotCenterInAngle =
//                sim.transform.position + new Vector3(centerX, 0, 0);
//            ball1.GetComponent<CheckGearBall>().rotDirInAngle = sim.transform.right;
//            ball1.GetComponent<CheckGearBall>().inAngleRot = true;
//            tempInfo.Add(r);
//            tempInfo.Add(thick);
//            tempInfo.Add(centerX);
//        }
//        if (tempHelicalState == 2)
//        {
//            //move
//            CheckGearBall.onExit exitAction2 = ballActions[ball2.name].onExit2Do;
//            CheckGearBall.onExit exitAction3 = ballActions[ball3.name].onExit2Do;
//            CheckGearBall.onEnter enterAction2 = ballActions[ball2.name].onEnter2Do;
//            CheckGearBall.onEnter enterAction3 = ballActions[ball3.name].onEnter2Do;
//            Vector3 axle = sim.transform.right.normalized;
//            ball2.GetComponent<SphereCollider>().enabled = false;
//            ball3.GetComponent<SphereCollider>().enabled = false;
//            ball2.transform.position = ball1.transform.position + axle * 0.01f;
//            ball3.transform.position = ball1.transform.position - axle * 0.01f;
//            ball2.GetComponent<SphereCollider>().enabled = true;
//            ball3.GetComponent<SphereCollider>().enabled = true;
//            ball2.GetComponent<CheckGearBall>().moveDir = axle;
//            ball3.GetComponent<CheckGearBall>().moveDir = -axle;
//            ball2.GetComponent<CheckGearBall>().moveV = 0.5f;
//            ball3.GetComponent<CheckGearBall>().moveV = 0.5f;
//            ball2.GetComponent<CheckGearBall>().move = true;
//            ball3.GetComponent<CheckGearBall>().move = true;
//            exitAction2 = detectHelical2Exit;
//            exitAction3 = detectHelical3Exit;
//        }
//        if (tempHelicalState == 3)
//        {
//            tempHelicalDetect.Add(ball2.transform.position);
//            ball2.transform.position -= sim.transform.forward.normalized * 0.01f;
//            ball2.GetComponent<SphereCollider>().enabled = true;
//            CheckGearBall.onStay stayAction2 = ballActions[ball2.name].onStay2Do;
//            stayAction2 = judgeOnStay;
//            CheckGearBall.onRotFin rotAction2 = ballActions[ball2.name].onRotFin2Do;
//            rotAction2 = detectHelicalFinRot;
//            //左旋
//            if (inAndOut == 11)
//            {
//                //球2开始旋转，并获取第一个exit触发点
//                ball2.GetComponent<CheckGearBall>().inAngleRot = true;
//                ball2.GetComponent<CheckGearBall>().rotDirInAngle = sim.transform.right.normalized;
//                ball2.GetComponent<CheckGearBall>().rotCenterInAngle =
//                    sim.transform.position + new Vector3(detectInitialized(sim)[2], 0, 0);
//                ball2.GetComponent<CheckGearBall>().Angle = 30;
//                tempHelicalDetect.Add(Vector3.left);
//            }
//            //右旋
//            else
//            {
//                //球2开始反向旋转，并获取第一个exit触发点
//                ball2.GetComponent<CheckGearBall>().inAngleRot = true;
//                ball2.GetComponent<CheckGearBall>().rotDirInAngle = -sim.transform.right.normalized;
//                ball2.GetComponent<CheckGearBall>().rotCenterInAngle =
//                    sim.transform.position + new Vector3(detectInitialized(sim)[2], 0, 0);
//                ball2.GetComponent<CheckGearBall>().Angle = 30;
//                tempHelicalDetect.Add(Vector3.right);
//            }
//            if (tempHelicalState == 4)
//            {
//                float z = tempCount;
//                Vector3 midP = 0.5f * (tempHelicalDetect[1] + tempHelicalDetect[2]);
//                Vector3 helicalL = (midP - tempHelicalDetect[0]).normalized;
//                Vector3 axle = sim.transform.right.normalized;
//                float angle = Mathf.Acos(Vector3.Dot(axle, helicalL)) * Mathf.Rad2Deg;
//                //存入数组，按照模数，齿数，齿宽，螺旋角，旋向，相对坐标顺序存取
//                List<float> simInfo = new List<float>();
//                float m = tempInfo[0] * 2 / (z + 2);
//                float _m = m % 0.1f;
//                m -= _m;
//                simInfo.Add(m);
//                simInfo.Add(z);
//                simInfo.Add(tempInfo[1]);
//                simInfo.Add(angle);
//                if (tempHelicalDetect[3] == Vector3.right)
//                {
//                    simInfo.Add(0.1f);
//                }
//                else if (tempHelicalDetect[3] == Vector3.left)
//                {
//                    simInfo.Add(1.0f);
//                }
//                simInfo.Add(tempInfo[2]);
//                //存入总检测信息数组中
//                detectInfo.Add(tempSimName, simInfo);
//                iterInitialized();
//            }
//        }
//    }

//    #region 内齿检测集合
//    void CylinderExpand()
//    {
//        balls[4].transform.localScale += new Vector3(0.1f, 0, 0.1f) * 0.5f;
//    }
//    /// <summary>
//    /// 触发器代理
//    /// </summary>
//    public void CylinderStopExpand()
//    {
//        //添加内齿齿顶圆半径
//        tempInfo.Add(balls[4].transform.localScale.x);
//        tempInsideState = 2;
//    }

//    void stopCountInsideTeeth()
//    {
//        tempInfo.Add(tempCount);
//        float z = tempCount;
//        //存入数组，按照模数，齿数，齿宽，相对坐标顺序存取
//        List<float> simInfo = new List<float>();
//        float m = tempInfo[3] * 2 / (z - 2);
//        float _m = m % 0.1f;
//        m -= _m;
//        simInfo.Add(m);
//        simInfo.Add(z);
//        simInfo.Add(tempInfo[1]);
//        simInfo.Add(tempInfo[2]);
//        //存入总检测信息数组中
//        detectInfo.Add(tempSimName, simInfo);
//        iterInitialized();
//    }

//    #endregion
//    /// <summary>
//    /// 
//    /// </summary>
//    public void detectInside(GameObject sim, GameObject ball, GameObject Cy)
//    {
//        if (tempInsideState == 0)
//        {
//            tempInfo = detectInitialized(sim);
//            tempSimName = sim.name;
//            CheckGearBall.onEnter enterAction = ballActions[ball.name].onEnter2Do;
//            CheckGearBall.onExit exitAction = ballActions[ball.name].onExit2Do;
//            CheckGearBall.onRotFin rotAction = ballActions[ball.name].onRotFin2Do;
//            Cy.GetComponent<CapsuleCollider>().enabled = false;
//            sim.transform.position = Vector3.zero;
//            Cy.GetComponent<CheckGearBall>().onEnter2Do = CylinderStopExpand;
//            Cy.transform.position = sim.transform.position;
//            Cy.GetComponent<CapsuleCollider>().enabled = true;
//            tempInsideState = 1;
//        }
//        if (tempInsideState == 1)
//        {
//            CylinderExpand();
//        }
//        if (tempInsideState == 2)
//        {
//            balls[4].GetComponent<CapsuleCollider>().enabled = false;
//            balls[4].transform.position = safePoint;
//            balls[4].transform.localScale = new Vector3(0.01f, 1f, 0.01f);
//            balls[0].transform.position =
//                sim.transform.position + new Vector3(tempInfo[2], tempInfo[3] * 0.5f, 0);
//            balls[0].GetComponent<SphereCollider>().enabled = true;
//            CheckGearBall.onEnter enterAction = ballActions[ball.name].onEnter2Do;
//            CheckGearBall.onRotFin rotAction = ballActions[ball.name].onRotFin2Do;
//            enterAction += countStraightTeeth;
//            ball.GetComponent<CheckGearBall>().inAngleRot = true;
//            ball.GetComponent<CheckGearBall>().rotDirInAngle = -sim.transform.right.normalized;
//            ball.GetComponent<CheckGearBall>().rotCenterInAngle =
//                sim.transform.position + new Vector3(detectInitialized(sim)[2], 0, 0);
//            ball.GetComponent<CheckGearBall>().Angle = 30;
//            rotAction += stopCountInsideTeeth;
//        }
//    }

//    #region 啮合套检测工具集

//    void detectDepth()
//    {
//        tempInfo.Add(balls[0].transform.position.y);
//    }
//    void detectWidth()
//    {
//        tempInfo.Add(2f * balls[0].transform.position.x);
//        detectInfo.Add(tempSimName, tempInfo);
//        iterInitialized();
//    }

//    #endregion

//    /// 啮合套
//    /// boxcollider获取零件x轴向厚度，内径得内齿齿宽，齿顶圆半径
//    /// 取1/2厚度释放小球平移，内径顶端释放小球旋转
//    /// 平移小球第一次碰撞获取槽深并重置，旋转小球旋转一周计算内齿齿数
//    /// 平移小球重置后原地倒退一个自身单位直径，往x向移动
//    /// 平移小球碰撞后获取槽宽*0.5
//    public void detectEngagement(GameObject sim, GameObject ball)
//    {
//        if (tempEngagementState == 0)
//        {
//            sim.transform.position = Vector3.zero;
//            tempSimName = sim.name;
//            CheckGearBall.onEnter enterAction = ballActions[ball.name].onEnter2Do;
//            if (sim.GetComponent<MeshCollider>())
//            {
//                sim.GetComponent<MeshCollider>().enabled = false;
//            }
//            balls[0].GetComponent<SphereCollider>().enabled = false;
//            sim.AddComponent<BoxCollider>();
//            float thick = sim.GetComponent<BoxCollider>().size.x;
//            float R = sim.GetComponent<BoxCollider>().size.y;
//            Destroy(sim.GetComponent<BoxCollider>());
//            if (!sim.GetComponent<MeshCollider>())
//            {
//                sim.AddComponent<MeshCollider>();
//                sim.GetComponent<MeshCollider>().enabled = true;
//            }
//            //按照厚度，半径，槽深，槽宽存储
//            tempInfo.Add(thick);
//            tempInfo.Add(R);
//            balls[0].transform.position = sim.transform.position + new Vector3(thick * 0.5f, R, 0);
//            balls[0].GetComponent<SphereCollider>().enabled = true;
//            balls[0].GetComponent<CheckGearBall>().moveDir = -sim.transform.up;
//            balls[0].GetComponent<CheckGearBall>().moveV = 0.5f;
//            balls[0].GetComponent<CheckGearBall>().move = true;
//            enterAction = detectDepth;
//            tempEngagementState = 10;
//        }
//        if (tempEngagementState == 2)
//        {
//            balls[0].transform.position -= new Vector3(0, 0.01f, 0);
//            CheckGearBall.onEnter enterAction = ballActions[ball.name].onEnter2Do;
//            enterAction = null;
//            enterAction = detectWidth;
//            balls[0].GetComponent<SphereCollider>().enabled = true;
//            balls[0].GetComponent<CheckGearBall>().moveDir = -sim.transform.right;
//            balls[0].GetComponent<CheckGearBall>().moveV = 0.5f;
//            balls[0].GetComponent<CheckGearBall>().move = true;
//            tempEngagementState = 10;
//        }
//    }
//}
