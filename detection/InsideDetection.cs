using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InsideDetection : detection
{
    //检测开始，必有空心轮廓结构，键槽、内齿轮、圆
    //实例化圆柱体扩大至触碰边缘
    //在边缘半径上实例化小球
    public InsideDetection(GameObject Cylinder, GameObject ball, Vector3 endP,
        float centerX, Vector3 rotD)
    {
        Ball = ball;
        cylinder = Cylinder;
        endPoint = endP;
        Z = 0;
        inside_r = 0;
        rotDir = rotD;
        center = new Vector3(centerX, 0, 0);
        assign = 0;
        soh = 0;
        //先实例化圆柱体检测内轮廓半径
        if (cylinder.GetComponent<CheckGearBall>() == null)
        {
            cylinder.AddComponent<CheckGearBall>();
        }
        cylinder.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
        cylinder.GetComponent<CheckGearBall>().initialized(center, endPoint);
        cylinder.GetComponent<CheckGearBall>().onStay2Do += solidOrHollow;
    }
    #region 设置私有化变量参数
    GameObject cylinder;
    GameObject Ball;
    float inside_r;
    float Z;
    Vector3 rotDir;
    Vector3 center;
    Vector3 endPoint;

    /// <summary>
    /// 0为空心，1为实心
    /// 初始值为0，主线程调用对象后，首先检测空心与否，主线程等待下一帧检测碰撞触发返回assign
    /// </summary>
    public int assign;
    public int soh;
    #endregion

    #region 注册委托事件接收返回数据
    /// <summary>
    /// 检测是否空心，碰撞触发需要协程跳过当前帧获取结果
    /// </summary>
    /// <param name="ball"></param>
    public void solidOrHollow(GameObject ball)
    {
        soh = 1;
    }
    /// <summary>
    /// 检测内轮廓半径并随即开启检测齿数
    /// </summary>
    public void onContact(GameObject ball)
    {
        inside_r = cylinder.transform.localScale.z / 2f;
        Vector3 startP = center + new Vector3(0, inside_r * 1.03f, 0);
        //再实例化小球检测内轮廓特征
        if (Ball.GetComponent<CheckGearBall>() == null)
        {
            Ball.AddComponent<CheckGearBall>();
        }
        Ball.GetComponent<CheckGearBall>().initialized(startP, endPoint, rotDir, center, 360f);
        Ball.GetComponent<CheckGearBall>().onExit2Do += countInsideTeeth;
        Ball.GetComponent<CheckGearBall>().onRotInAngleFin2Do += stopCountInsideTeeth;
        Ball.GetComponent<CheckGearBall>().inAngleRot = true;
        cylinder.GetComponent<CheckGearBall>().finWorking();
    }

    /// <summary>
    /// 每次碰撞计算齿数
    /// </summary>
    /// <param name="ball"></param>
    public void countInsideTeeth(GameObject ball)
    {
        Z++;
    }
    /// <summary>
    /// 旋转完毕返回总计数值
    /// </summary>
    /// <param name="ball"></param>
    public void stopCountInsideTeeth(GameObject ball)
    {
        assign = 1;
        Ball.GetComponent<CheckGearBall>().finWorking();
    }
    /// <summary>
    /// 内轮廓特征，返回整圆0或齿数
    /// </summary>
    /// <returns>齿数，内齿齿顶圆半径</returns>
    public List<float> getZ()
    {
        assign = 0;
        List<float> a = new List<float>();
        //键槽等价于整圆
        if (Z <= 28)
        {
            a.Add(0);
        }
        else
        {
            a.Add(Z / 2);
            //print(Z);
        }
        a.Add(inside_r);
        return (a);
    }
    #endregion

    #region 控制开关

    /// <summary>
    /// 确认空心，开始检测
    /// </summary>
    public void start2Check()
    {
        cylinder.GetComponent<CheckGearBall>().onEnter2Do += onContact;
        cylinder.GetComponent<CheckGearBall>().expand = true;
    }
    #endregion
    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    /// <param name="tempInfo">模数，齿数，齿宽，相对坐标</param>
    //    void creatInternal(List<float> tempInfo)
    //    {
    //        CreatInsideTooths inter = sim.transform.GetChild(0).gameObject.AddComponent<CreatInsideTooths>();
    //        inter.Acc = 2;
    //        inter.M = tempInfo[0];
    //        inter.Z = tempInfo[1];
    //        inter.Thick = tempInfo[2];
    //        inter.Shift = -(tempInfo[3] * 0.5f + sim.transform.position.x);
    //        inter.star2Creat = true;
    //    }
    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    /// <param name="tempInfo">半径、齿宽、相对坐标</param>
    //    void creatRing(List<float> tempInfo)
    //    {
    //        Ring r1 = sim.transform.GetChild(0).gameObject.AddComponent<Ring>();
    //        r1.Acc = 30;
    //        r1.Thick = tempInfo[1];
    //        r1.R = tempInfo[0] + 1f;
    //        r1.Inside_R = tempInfo[0];
    //        r1.Shift = 0.5f * tempInfo[1];
    //        r1.start2Creat = true;
    //    }
}
