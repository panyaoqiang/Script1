using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckBall : MonoBehaviour
{
    Vector3 startPoint;
    Vector3 dir;

    public IEnumerator detected;
    public delegate void colliderResult(Vector3 oripos,Vector3 endpos);
    public colliderResult onTriggerEnter2Do;
    public colliderResult solidOrHollow;
    public colliderResult onTriggerExit2Do;
    /// <summary>
    /// 调用小球首先对此函数进行调用
    /// 改变小球初始位置排列
    /// 确定小球移动方向
    /// </summary>
    /// <param name="sp">初始点</param>
    /// <param name="md">移动方向</param>
    public void initialized(Vector3 sp,Vector3 md)
    {
        startPoint = sp;
        dir = md;
        detected = star2move(dir);
    }
    /// <summary>
    /// 实心、初始检测
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerStay(Collider other)
    {
        solidOrHollow(startPoint, gameObject.transform.position);
    }
    /// <summary>
    /// 空心、后续检测
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        onTriggerEnter2Do(startPoint, gameObject.transform.position);
    }
    /// <summary>
    /// 零件段落外沿碰撞点
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerExit(Collider other)
    {
        onTriggerExit2Do(startPoint, gameObject.transform.position);
        StopAllCoroutines();
        gameObject.transform.position = Vector3.one * 500f;
    }

    IEnumerator star2move(Vector3 dir)
    {
        transform.Translate(dir * 0.01f);
        yield return 0;
    }

    IEnumerator star2expand(Vector3 scaleExpandV)
    {
        transform.localScale += scaleExpandV;
        yield return 0;
    }
}

public struct ballResult
{
    public float hollow_CountDis(Vector3 star,Vector3 end)
    {
        Vector3 res = end - star;
        return(res.magnitude);
    }
    public Vector3 both_GetExternal(Vector3 pos)
    {
        return (pos);
    }
    public string soh;
}
