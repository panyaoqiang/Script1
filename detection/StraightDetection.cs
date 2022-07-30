using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StraightDetection : detection
{
    /// <summary>
    /// 实例化对象所需材料，球、起点、终点
    /// 对象实例化完毕，即对对应检测球前两部初始化完毕
    /// </summary>
    /// <param name="ball">检测球</param>
    /// <param name="endP">终点</param>
    public StraightDetection(GameObject ball, float r, Vector3 endP, float centerX, Vector3 rotDir)
    {
        Ball = ball;
        Z = 0;
        assign = 0;
        //countPoint.Clear();
        Vector3 startP = new Vector3(centerX, r * 0.95f, 0);

        //Ray ray = new Ray(startP, Vector3.forward);
        //RaycastHit hit;
        //if (Physics.Raycast(ray, out hit))
        //{
        //    Vector3 sp = (hit.point + startP) / 2;
        //    startP = sp;
        //}
        Vector3 center = new Vector3(centerX, 0, 0);
        if (Ball.GetComponent<CheckGearBall>() == null)
        {
            Ball.AddComponent<CheckGearBall>();
        }
        Ball.GetComponent<CheckGearBall>().initialized(startP, endP, rotDir, center, 360f);
        Ball.GetComponent<CheckGearBall>().onExit2Do += countStraightTeeth;
        Ball.GetComponent<CheckGearBall>().onRotInAngleFin2Do += stopCountStraightTeeth;
    }

    #region 设置临时变量及返回参数
    GameObject Ball;
    float Z = 0;
    //未能提取检测数据为0，否则1
    public int assign;
    #endregion

    #region 注册委托事件接收返回数据
    /// <summary>
    /// 每次碰撞计算齿数
    /// </summary>
    /// <param name="ball"></param>
    public void countStraightTeeth(GameObject ball)
    {
        Z++;
    }
    /// <summary>
    /// 旋转完毕返回总计数值
    /// </summary>
    /// <param name="ball"></param>
    public void stopCountStraightTeeth(GameObject ball)
    {
        assign = 1;
        Ball.GetComponent<CheckGearBall>().finWorking();
    }

    public List<float> getZ()
    {
        assign = 0;
        List<float> a = new List<float>() { Z / 2 };
        return (a);
    }
    #endregion

    #region 控制开关
    public void start2Check()
    {
        Ball.GetComponent<CheckGearBall>().inAngleRot = true;
    }
    #endregion


    ///// <summary>
    ///// 引用范例
    ///// </summary>
    //public List<float> a1 = new List<float>();
    //private void Start()
    //{
    //    GameObject a0 = new GameObject();
    //    StraightDetection a = new StraightDetection(a0, 1f, Vector3.one, -1f, Vector3.right);
    //    a.getZ += example;
    //}

    //public void example(float z)
    //{
    //    a1.Add(z);
    //}

    //    /// <summary>
    //    /// 建立直齿，thick，shift均向x轴负向延伸
    //    /// </summary>
    //    /// <param name="tempInfo">模数、齿数、齿宽、相对坐标</param>
    //    void creatStraight(List<float> tempInfo)
    //    {
    //        CreatMesh straight= sim.transform.GetChild(0).gameObject.AddComponent<CreatMesh>();
    //        straight.Acc = 2;
    //        straight.M = tempInfo[0];
    //        straight.Z = tempInfo[1];
    //        straight.thick = tempInfo[2];
    //        straight.Shift = -(sim.transform.position.x + tempInfo[3] + 0.5f * tempInfo[2]);
    //        straight.start2Creat = true;
    //    }
}
