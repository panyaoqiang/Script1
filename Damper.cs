using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damper : MonoBehaviour
{
    public float C = 1;
    //空气密度
    public float rou = 1.3f;
    public GameObject ObjDamperActOn;
    public float S = 0f;
    public Vector3 V;
    public Vector3 DamperF;
    public Vector3 ExternalF;
    //木材密度0.5-0.8
    //木材直径5mm-10mm，0.005-0.01
    //选段长度为直径两倍0.01-0.02
    //木材单段体积：Pi*R*R*L
    //总质量0.0005g
    //选择叶片面积，由叶片浓密程度决定树叶迎风面积。选取固定面积参数随机选择方案

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        DamperF=AirDamper(S, rou, C, GetParameter());
        ExternalF = ExternalEffect();
        //外力与阻力的向量和
        ObjDamperActOn.GetComponent<Rigidbody>().AddForce(DamperF+ExternalF);            
    }

    public Vector3 GetParameter()
    {
        V = ObjDamperActOn.GetComponent<Rigidbody>().velocity;
        return V;
    }
    /// <summary>
    /// 输入迎风面积，及物体速度
    /// </summary>
    /// <param name="S">迎风面积</param>
    /// <param name="rou">空气密度1.3千克每立方米</param>
    /// <param name="C">常数迎风面为90度取1,0度取0</param>
    /// <param name="V">物体运动速度</param>
    /// <returns>输出一个与运动速度方向相反的力</returns>
    public Vector3 AirDamper(float S, float rou, float C, Vector3 V)
    {
        Vector3 F = Vector3.zero;
        float f = 0.5F * S * C * rou * V.magnitude * V.magnitude;
        F = f * -(V.normalized);
        return F;
    }
    /// <summary>
    /// 除去空气阻力外的力，如风力
    /// </summary>
    /// <returns>风力</returns>
    public Vector3 ExternalEffect()
    {
        Vector3 ef = Vector3.zero;
        return ef;
    }
}
