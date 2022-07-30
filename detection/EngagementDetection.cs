using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EngagementDetection : MonoBehaviour
{
//    /// <summary>
//    /// 永久赋值
//    /// </summary>
//    public Vector3 safePoint = new Vector3(200, 200, 0);
//    public List<GameObject> balls = new List<GameObject>();
//    public detectController controller;
//    Dictionary<string, CheckGearBall> ballActions = new Dictionary<string, CheckGearBall>();

//    /// <summary>
//    /// 半径，厚度，相对坐标
//    /// </summary>
//    public List<float> socketInfo = new List<float>();
//    public GameObject sim;
//    public bool start2DetectEngagement = false;
//    /// <summary>
//    /// 厚度，半径，槽深，槽宽
//    /// </summary>
//    List<float> tempInfo = new List<float>();
//    string tempSimName = "";
//    int tempEngagementState = 0;

//    void Start()
//    {
//        for (int i = 0; i < balls.Count; i++)
//        {
//            ballActions.Add(balls[i].name, balls[i].GetComponent<CheckGearBall>());
//        }
//    }

//    private void Update()
//    {
//        if (start2DetectEngagement)
//        {
//            detectEngagement(sim, balls[0]);
//        }
//    }
//    void iterInitialized()
//    {
//        socketInfo.Clear();
//        tempInfo.Clear();
//        tempSimName = "";
//        tempEngagementState = 0;
//        start2DetectEngagement = false;
//        sim = null;
//        for (int i = 0; i < balls.Count; i++)
//        {
//            balls[i].GetComponent<CheckGearBall>().onEnter2Do = null;
//            balls[i].GetComponent<CheckGearBall>().onExit2Do = null;
//            balls[i].GetComponent<CheckGearBall>().onStay2Do = null;
//            balls[i].GetComponent<CheckGearBall>().onRotFin2Do = null;
//            balls[i].GetComponent<CheckGearBall>().onRotInAngleFin2Do = null;
//            balls[i].GetComponent<SphereCollider>().enabled = false;
//            balls[i].transform.position = safePoint;
//        }
//    }

//    #region 啮合套检测工具集

//    void detectDepth()
//    {
//        tempInfo.Add(balls[0].transform.position.y);
//        tempEngagementState = 2;
//    }
//    void detectWidth()
//    {
//        tempInfo.Add(2f * balls[0].transform.position.x);
//        creatEngagement();
//        iterInitialized();
//    }

//    #endregion

//    /// <summary>
//    /// 啮合套
//    /// boxcollider获取零件x轴向厚度，内径得内齿齿宽，齿顶圆半径
//    /// 取1/2厚度释放小球平移，内径顶端释放小球旋转
//    /// 平移小球第一次碰撞获取槽深并重置，旋转小球旋转一周计算内齿齿数
//    /// 平移小球重置后原地倒退一个自身单位直径，往x向移动
//    /// 平移小球碰撞后获取槽宽*0.5
//    /// </summary>
//    public void detectEngagement(GameObject sim, GameObject ball)
//    {
//        if (tempEngagementState == 0)
//        {
//            tempSimName = sim.name;
//            CheckGearBall.onEnter enterAction = ballActions[ball.name].onEnter2Do;
//            if (sim.GetComponent<MeshCollider>())
//            {
//                sim.GetComponent<MeshCollider>().enabled = false;
//            }
//            balls[0].GetComponent<SphereCollider>().enabled = false;
//            if (!sim.GetComponent<BoxCollider>())
//            {
//                sim.AddComponent<BoxCollider>();
//            }
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
//            balls[0].GetComponent<SphereCollider>().enabled = false;
//            balls[0].transform.position = sim.transform.position + new Vector3(0, R, 0);
//            balls[0].GetComponent<SphereCollider>().enabled = true;
//            balls[0].GetComponent<CheckGearBall>().moveDir = -Vector3.up;
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
//            balls[0].GetComponent<CheckGearBall>().moveDir = Vector3.right;
//            balls[0].GetComponent<CheckGearBall>().moveV = 0.5f;
//            balls[0].GetComponent<CheckGearBall>().move = true;
//            tempEngagementState = 10;
//        }
//    }
//    /// <summary>
//    /// thick,R,r,width
//    /// </summary>
//    void creatEngagement()
//    {
//        Ring r1 = sim.transform.GetChild(0).gameObject.AddComponent<Ring>();
//        r1.Acc = 30;
//        r1.Thick = 0.5f * (tempInfo[0] - tempInfo[3]);
//        r1.R = tempInfo[1];
//        r1.Inside_R = tempInfo[2];
//        r1.Shift = 0.5f * tempInfo[0];
//        r1.start2Creat = true;
//        Ring r2 = sim.transform.GetChild(0).gameObject.AddComponent<Ring>();
//        r2.Acc = 30;
//        r2.Thick = 0.5f * (tempInfo[0] - tempInfo[3]);
//        r2.R = tempInfo[1];
//        r2.Inside_R = tempInfo[2];
//        r2.Shift = -0.5f * tempInfo[3];
//        r2.start2Creat = true;
//        //底层
//        Ring r3 = sim.transform.GetChild(0).gameObject.AddComponent<Ring>();
//        r3.Acc = 30;
//        r3.Thick = tempInfo[0];
//        r3.R = tempInfo[2];
//        r3.Inside_R = tempInfo[2] - 1f;
//        r3.Shift = tempInfo[0] * 0.5f;
//        r3.start2Creat = true;
//    }
}
