using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 此脚本在实例化小球时挂载在小球上
/// 使用初始化重载定义运动参数
/// 注册委托时间设置检测条件返回检测数据
/// 打开开关，设置finworking位置停止检测
/// 完成检测后刷新归位
/// </summary>
public class CheckGearBall : MonoBehaviour
{
    /// <summary>
    /// 传两个参控制圆柱体放大或自定义操作
    /// </summary>
    public void initialized(Vector3 startP, Vector3 endP)
    {
        gameObject.transform.position = startP;
        endPoint = endP;
        if (gameObject.GetComponent<SphereCollider>() != null)
        {
            gameObject.GetComponent<SphereCollider>().enabled = true;
        }
        if (gameObject.GetComponent<CapsuleCollider>() != null)
        {
            gameObject.GetComponent<CapsuleCollider>().enabled = true;
        }
        if (gameObject.GetComponent<BoxCollider>() != null)
        {
            gameObject.GetComponent<BoxCollider>().enabled = true;
        }
    }
    /// <summary>
    /// 传三个参数控制球体平移
    /// </summary>
    /// <param name="dir"></param>
    public void initialized(Vector3 startP, Vector3 endP, Vector3 dir, Vector3 stopP)
    {
        gameObject.transform.position = startP;
        endPoint = endP;
        stopPoint = stopP;
        moveDir = dir;
        moveV = transform.localScale.x * 0.8f;
        if (gameObject.GetComponent<SphereCollider>() != null)
        {
            gameObject.GetComponent<SphereCollider>().enabled = true;
        }
        if (gameObject.GetComponent<CapsuleCollider>() != null)
        {
            gameObject.GetComponent<CapsuleCollider>().enabled = true;
        }
        if (gameObject.GetComponent<BoxCollider>() != null)
        {
            gameObject.GetComponent<BoxCollider>().enabled = true;
        }
    }
    /// <summary>
    /// 控制定角度旋转，旋转速度与scale有关
    /// </summary>
    /// <param name="dir">旋转轴</param>
    /// <param name="center">旋转中心</param>
    /// <param name="angle">角度</param>
    public void initialized(Vector3 startP, Vector3 endP, Vector3 dir, Vector3 center, float angle)
    {
        gameObject.transform.position = startP;
        endPoint = endP;
        Angle = angle;
        rotCenterInAngle = center;
        rotDirInAngle = dir;
        rotV = 1.5f * Mathf.Rad2Deg * Mathf.Asin(transform.localScale.x * 0.5f / (startP - center).magnitude);
        if (gameObject.GetComponent<SphereCollider>() != null)
        {
            gameObject.GetComponent<SphereCollider>().enabled = true;
        }
        if (gameObject.GetComponent<CapsuleCollider>() != null)
        {
            gameObject.GetComponent<CapsuleCollider>().enabled = true;
        }
        if (gameObject.GetComponent<BoxCollider>() != null)
        {
            gameObject.GetComponent<BoxCollider>().enabled = true;
        }
    }

    Vector3 endPoint;

    public bool zoom = false;
    public bool expand = false;
    float expV = 0.1f;

    public bool move = false;
    Vector3 moveDir;
    float moveV;
    Vector3 stopPoint;

    public bool inAngleRot = false;
    float Angle;
    Vector3 rotDirInAngle;
    Vector3 rotCenterInAngle;
    float rotV;
    float rotInAngle;

    public delegate void onExit(GameObject ontologyBall);
    public onExit onExit2Do;
    public delegate void onStay(GameObject ontologyBall);
    public onStay onStay2Do;
    public delegate void onEnter(GameObject ontologyBall);
    public onEnter onEnter2Do;
    public delegate void onAngleRotFin(GameObject ontologyBall);
    public onAngleRotFin onRotInAngleFin2Do;
    public delegate void onMoveFin(GameObject ontologyCylinder);
    public onMoveFin onMoveFin2Do;

    void FixedUpdate()
    {
        if (move)
        {
            start2Move();
        }
        if (expand)
        {
            start2Expand();
        }
        if (inAngleRot)
        {
            start2RotInAngle();
        }
        if (zoom)
        {
            start2Zoom();
        }
    }

    /// <summary>
    /// 先对初始位置进行赋值，设置expand=false终止条件
    /// </summary>
    public void start2Expand()
    {
        if (transform.name.Contains("ring"))
        {
            this.transform.localScale += new Vector3(0, 0.1f, 0.1f) * expV;
        }
        else
        {
            this.transform.localScale += new Vector3(0.1f, 0, 0.1f) * expV;
        }
    }
    /// <summary>
    /// 定角度旋转
    /// </summary>
    public void start2RotInAngle()
    {
        rotInAngle += rotV;
        if (rotInAngle < Angle)
        {
            this.transform.RotateAround(rotCenterInAngle, rotDirInAngle, rotV);
        }
        else
        {
            rotInAngle = 0;
            inAngleRot = false;
            if (onRotInAngleFin2Do != null)
            {
                onRotInAngleFin2Do(this.gameObject);
            }
        }
    }
    /// <summary>
    /// 先对初始位置赋值，对移动方向赋值，设置move=false的终止条件
    /// </summary>
    public void start2Move()
    {
        //从左到右开始
        if (transform.position.x <= stopPoint.x)
        {
            this.transform.Translate(moveDir * moveV, Space.World);
        }
        else
        {
            move = false;
            if (onMoveFin2Do != null)
            {
                onMoveFin2Do(this.gameObject);
            }
        }
    }

    public void start2Zoom()
    {
        if (transform.name.Contains("ring"))
        {
            this.transform.localScale -= new Vector3(0, 0.1f, 0.1f) * expV;
        }
        else
        {
            this.transform.localScale -= new Vector3(0.1f, 0, 0.1f) * expV;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (move || expand || inAngleRot || zoom)
        {
            if (onEnter2Do != null && other.tag != "detect")
            {
                onEnter2Do(this.gameObject);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (move || expand || inAngleRot || zoom)
        {
            if (onExit2Do != null && other.tag != "detect")
            {
                onExit2Do(this.gameObject);
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (onStay2Do != null && other.tag != "detect")
        {
            onStay2Do(this.gameObject);
        }
    }

    public void finWorking()
    {
        if (gameObject.GetComponent<SphereCollider>() != null)
        {
            gameObject.GetComponent<SphereCollider>().enabled = false;

        }
        if (gameObject.GetComponent<CapsuleCollider>() != null)
        {
            gameObject.GetComponent<CapsuleCollider>().enabled = false;
        }
        if (gameObject.GetComponent<BoxCollider>() != null)
        {
            gameObject.GetComponent<BoxCollider>().enabled = false;
        }
        gameObject.transform.position = endPoint;
        if (gameObject.name.Contains("Cylinder")|| gameObject.name.Contains("ring"))
        {
            gameObject.transform.localScale = Vector3.one * 0.01f;
        }
        onEnter2Do = null;
        onExit2Do = null;
        onStay2Do = null;
        onRotInAngleFin2Do = null;
        onMoveFin2Do = null;
        expand = false;
        inAngleRot = false;
        move = false;
        zoom = false;
        stopPoint = Vector3.zero;
    }
}
