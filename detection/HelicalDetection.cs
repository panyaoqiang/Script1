using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelicalDetection : detection
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="ball">检测球</param>
    /// <param name="r">齿顶圆半径</param>
    /// <param name="endP">结束点</param>
    /// <param name="centerX">相对坐标x</param>
    /// <param name="rotDir">旋转方向</param>
    public HelicalDetection(GameObject ball, GameObject ball2, GameObject cy, float Ra,
        Vector3 endP, float centerX, Vector3 rotD, float thick)
    {
        Ball = ball;
        Ball1 = ball2;
        Cylinder = cy;
        Z = 0;
        R = Ra;
        endPoint = endP;
        rotDir = -rotD;
        Vector3 startP = new Vector3(centerX, Ra * 0.95f, 0);
        center = new Vector3(centerX, 0, 0);
        if (Ball.GetComponent<CheckGearBall>() == null)
        {
            Ball.AddComponent<CheckGearBall>();
        }
        if (Ball1.GetComponent<CheckGearBall>() == null)
        {
            Ball1.AddComponent<CheckGearBall>();
        }
        if (Cylinder.GetComponent<CheckGearBall>() == null)
        {
            Cylinder.AddComponent<CheckGearBall>();
        }
        //第一个射线碰撞点设置小球，以此为起点启动小球，当计数器触发第二次时，记录碰撞点
        Ball.GetComponent<CheckGearBall>().initialized(startP, endP, rotDir, center, 360f);
        //记录第二个碰撞点
        Ball.GetComponent<CheckGearBall>().onExit2Do += countTeeth;
        Ball.GetComponent<CheckGearBall>().onRotInAngleFin2Do += stopCountTeeth;
        //第一个射线碰撞点设置小球，以此为起点启动小球，当计数器触发第二次时，记录碰撞点
        Ball1.GetComponent<CheckGearBall>().initialized(startP +
            new Vector3(0.3f * thick, 0, 0), endP, rotDir, center, 360f);
        //记录第二个碰撞点
        Ball1.GetComponent<CheckGearBall>().onExit2Do += countTeeth2;
        Ball1.GetComponent<CheckGearBall>().onRotInAngleFin2Do += stopCountTeeth2;
        cylinderCount = 0;
        Cylinder.GetComponent<CapsuleCollider>().enabled = false;
    }

    #region 设置临时变量及返回参数
    List<Vector3> P1 = new List<Vector3>();
    List<Vector3> P2 = new List<Vector3>();
    GameObject Ball;
    GameObject Ball1;
    GameObject Cylinder;
    Vector3 center;
    Vector3 endPoint;
    Vector3 rotDir;
    float R;
    bool f1;
    bool f2;
    public float assign = 0;
    float Z = 0;
    float rightOrLeft;
    float helix_angle;
    int cylinderCount;
    #endregion

    #region 注册委托事件接收返回数据
    /// <summary>
    /// 每次碰撞计算齿数
    /// </summary>
    /// <param name="ball"></param>
    public void countTeeth(GameObject ball)
    {
        Z++;
        P1.Add(ball.transform.position);
    }
    public void countTeeth2(GameObject ball)
    {
        P2.Add(ball.transform.position);
    }
    /// <summary>
    /// 旋转完毕返回总计数值
    /// </summary>
    /// <param name="ball"></param>
    public void stopCountTeeth(GameObject ball)
    {
        Z *= 0.5f;
        Ball.GetComponent<CheckGearBall>().finWorking();
        //print(2 * Ra / (Z + 2)+"?"+分度圆半径);
        f1 = true;
        if (f1 && f2)
        {
            assign = 1;
        }
    }
    public void stopCountTeeth2(GameObject ball)
    {
        Ball1.GetComponent<CheckGearBall>().finWorking();
        f2 = true;
        if (f1 && f2)
        {
            assign = 1;
        }
    }
    public void onCylinderStay(GameObject cy)
    {
        cylinderCount = 1;
    }
    public IEnumerator getHelixAngle()
    {
        assign = 1.5f;
        List<Vector3> p = new List<Vector3>();
        //方向与x轴正向相同
        p.Add((P2[0] + P2[1]) / 2 - (P1[3] + P1[4]) / 2);
        p.Add((P2[1] + P2[2]) / 2 - (P1[3] + P1[4]) / 2);
        p.Add((P2[2] + P2[3]) / 2 - (P1[3] + P1[4]) / 2);
        p.Add((P2[3] + P2[4]) / 2 - (P1[3] + P1[4]) / 2);
        p.Add((P2[4] + P2[5]) / 2 - (P1[3] + P1[4]) / 2);
        p.Add((P2[5] + P2[6]) / 2 - (P1[3] + P1[4]) / 2);
        p.Add((P2[6] + P2[7]) / 2 - (P1[3] + P1[4]) / 2);
        //第一个射线碰撞点设置小球，以此为起点启动小球，当计数器触发第二次时，记录碰撞点
        Cylinder.GetComponent<CheckGearBall>().initialized((P1[1] + P1[2]) / 2, endPoint);
        Cylinder.GetComponent<CheckGearBall>().onStay2Do += onCylinderStay;
        Cylinder.transform.position = (P1[3] + P1[4]) / 2;
        Vector3 helixAngleDir = Vector3.zero;
        Cylinder.GetComponent<CapsuleCollider>().enabled = true;
        for (int i = 0; i < p.Count; i++)
        {
            cylinderCount = 0;
            Cylinder.transform.localScale = new Vector3(0.01f, p[i].magnitude, 0.01f);
            Cylinder.transform.up = p[i];
            yield return new WaitForFixedUpdate();
            if (cylinderCount == 0)
            {
                helixAngleDir = Cylinder.transform.up;
            }
        }
        Cylinder.transform.up = helixAngleDir;
        Vector3 planeNormal = Cylinder.transform.position - center;
        //向上则右旋，向下则左旋
        //右手定则
        Vector3 dirNormal = Vector3.Cross(Vector3.right, helixAngleDir);
        float dir = Mathf.Acos(Vector3.Dot(planeNormal, dirNormal) /
                (planeNormal.magnitude * dirNormal.magnitude)) * Mathf.Rad2Deg;
        //print("截面法向量" + planeNormal + "指向法向量" + dirNormal + "夹角" + dir);
        //计算螺旋角斜线向量求螺旋角
        if (dir > 90)
        {
            //print("左");
            rightOrLeft = 1.0f;
        }
        else
        {
            //print("右");
            rightOrLeft = 0.1f;
        }
        //检测半径下的螺旋角
        helix_angle = Mathf.Acos(Vector3.Dot(helixAngleDir, Vector3.right) /
            (helixAngleDir.magnitude * Vector3.right.magnitude));
        float an = helix_angle * Mathf.Rad2Deg;
        //r为分度圆半径
        float r = R * (Z / (Z + 2));
        helix_angle = Mathf.Atan(r * Mathf.Tan(helix_angle) / (0.95f * R)) * Mathf.Rad2Deg;
        //print("分度圆螺旋角" + helix_angle + "齿数" + Z);

        assign = 2;
        #region
        //for (int i = 0; i < p.Count; i++)
        //{
        //    cylinderCount = 0;
        //    Cylinder.transform.up = p[i];
        //    Cylinder.transform.localScale = new Vector3(0.01f, p[i].magnitude, 0.01f);
        //    if (cylinderCount == 0)
        //    {
        //        //helix_angle=
        //        print("?");
        //    }
        //}
        //if (onExit2DoCount == 2)
        //{
        //    Ball.GetComponent<CheckGearBall>().inAngleRot = false;
        //    //float newR = (hitP[0] - center).magnitude;
        //    //float ballHalfR = ball.transform.localScale.x / 4f;
        //    //float rad = 2f * Mathf.Asin(ballHalfR / newR);
        //    //rotateMatrix("x", ball.transform.position, rad);
        //    t.visibleBall(GameObject.Find("c1"), hitP[0]);
        //    t.visibleBall(GameObject.Find("c2"), hitP[1]);
        //    //叉乘右手定则，得截面法向量
        //    Vector3 sectionDir = Vector3.Cross(rotDir, hitP[0] - hitP[1]);
        //    //求投影夹角
        //    float angle = Mathf.PI / 2f - Mathf.Acos(Vector3.Dot(normal, sectionDir) /
        //        (normal.magnitude * sectionDir.magnitude));
        //    //第一射线点投影向量
        //    normal -= sectionDir.normalized * normal.magnitude * Mathf.Sin(angle);
        //    Ball.GetComponent<CheckGearBall>().initialized(hitP[0], endPoint, normal);
        //    Ball.GetComponent<CheckGearBall>().move = true;
        //    #region
        //    //Ray ray = new Ray(hitP[0], normal);
        //    //RaycastHit hit;
        //    //if (Physics.Raycast(ray, out hit))
        //    //{
        //    //    hitP.Add(hit.point);
        //    //}
        //    //Ray ray1 = new Ray(hitP[2], Vector3.forward);
        //    //RaycastHit hit1;
        //    //if (Physics.Raycast(ray1, out hit1))
        //    //{
        //    //    hitP.Add(hit1.point);
        //    //}


        //    ////计算螺旋角斜线向量求螺旋角
        //    //Vector3 helixDir = ((hitP[2] + hitP[0]) / 2f - (hitP[1] + hitP[0]) / 2f).normalized;

        //    //hitP.Add((hitP[2] + hitP[0]) / 2f);//3
        //    //hitP.Add((hitP[1] + hitP[0]) / 2f);//4


        //    //if (hitP[3].x > hitP[4].x)
        //    //{
        //    //    print("左");
        //    //    rightOrLeft = 1.0f;
        //    //    helix_angle = Mathf.Acos(Vector3.Dot(helixDir, Vector3.right) /
        //    //        (helixDir.magnitude * Vector3.right.magnitude)) * Mathf.Rad2Deg;
        //    //}
        //    //else
        //    //{
        //    //    print("右");
        //    //    rightOrLeft = 0.1f;
        //    //    helix_angle = Mathf.Acos(Vector3.Dot(helixDir, Vector3.left) /
        //    //        (helixDir.magnitude * Vector3.left.magnitude)) * Mathf.Rad2Deg;
        //    //}
        //    ////计算normal及初始碰撞点向量叉乘，求螺旋角正反
        //    //Vector3 oriDir = Vector3.Cross(hitP[2] - hitP[0], hitP[1] - hitP[0]).normalized;
        //    //float rlAngle = Mathf.Acos(Vector3.Dot(sectionDir, oriDir) /
        //    //    (sectionDir.magnitude * oriDir.magnitude)) * Mathf.Rad2Deg;
        //    //if (rlAngle < 90f)
        //    //{
        //    //    //左旋
        //    //}
        //    //else
        //    //{
        //    //    //右旋
        //    //}

        //    //float HL = 2.235201f / (2f * Mathf.Cos(helix_angle * Mathf.Deg2Rad));
        //    //hitP.Add(hitP[4] + helixDir * HL);//5
        //    //hitP.Add(hitP[4] - helixDir * HL);//6


        //    //t.visibleBall(GameObject.Find("c1"), hitP[0]);
        //    //t.visibleBall(GameObject.Find("c2"), hitP[1]);
        //    //t.visibleBall(GameObject.Find("c3"), hitP[2]);
        //    //t.visibleBall(GameObject.Find("c4"), hitP[3]);
        //    //t.visibleBall(GameObject.Find("c5"), hitP[4]);
        //    //t.visibleBall(GameObject.Find("c6"), hitP[5]);
        //    //t.visibleBall(GameObject.Find("c7"), hitP[6]);
        //}
        //else if (onExit2DoCount == 3)
        //{
        //    onExit2DoCount = 0;
        //    Ball.GetComponent<CheckGearBall>().move = false;
        //    hitP.Add(ball.transform.position);
        //    Ray ray = new Ray(ball.transform.position, (hitP[0] - hitP[1]).normalized);
        //    RaycastHit hit;
        //    if (Physics.Raycast(ray, out hit))
        //    {
        //        hitP.Add(hit.point);
        //    }
        //    t.visibleBall(GameObject.Find("c3"), hitP[2]);
        //    t.visibleBall(GameObject.Find("c4"), hitP[3]);
        //}
        #endregion
    }
    /// <summary>
    /// 获取斜齿轮检测信息
    /// </summary>
    /// <returns>螺旋角、齿数、旋向1为左，0.1为右</returns>
    public List<float> getZ()
    {
        assign = 0;
        string a = string.Format("螺旋角{0}，齿数{1}，旋向{2}，齿顶圆直径{3}",
            helix_angle, Z, rightOrLeft, R * 0.95f);
        print(a);
        List<float> info = new List<float>() { Z, helix_angle, rightOrLeft, R };
        return (info);
    }
    #endregion

    #region 控制开关
    public void start2Check()
    {
        Ball.GetComponent<CheckGearBall>().inAngleRot = true;
        Ball1.GetComponent<CheckGearBall>().inAngleRot = true;
    }
    #endregion

    ///// <summary>
    ///// 圆柱斜齿建模开关
    ///// </summary>
    ///// <param name="tempInfo">模数，齿数，齿宽，螺旋角，旋向，相对坐标</param>
    //void creatHelical(List<float> tempInfo)
    //{
    //    string rl;
    //    //齿数、螺旋角、旋向
    //    List<float> hInfo = getZ();
    //    if (hInfo[2] == 1.0f)
    //    {
    //        rl = "左";
    //    }
    //    else
    //    {
    //        rl = "右";
    //    }
    //    //半径3.475525厚度2.235201
    //    GameObject result = get3DModel(exmModel);
    //    GameObject script = result.transform.FindChild("ColliderAndMesh").gameObject;
    //    Helical_gear Helix = script.AddComponent<Helical_gear>();
    //    Helix.Acc = 2;
    //    //分度圆半径                
    //    Helix.Helix_Angle = hInfo[1];
    //    Helix.Z = hInfo[0];
    //    Helix.Right_Left = rl;
    //    Helix.M = 3.475525f / (hInfo[0] + 2);
    //    Helix.thick = 2.235201f;
    //    Helix.mat = exmModel.GetComponent<MeshRenderer>().material;
    //    Helix.Shift = -1.1175f;
    //    Helix.start2Creat = true;
    //}
}
