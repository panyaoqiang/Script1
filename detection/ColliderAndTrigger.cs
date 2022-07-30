using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderAndTrigger : MonoBehaviour
{
    /// <summary>
    /// 碰撞检测1；触发检测2；无0
    /// </summary>
    public int colliderOrTrigger = 0;

    public delegate void onTriggeredExit();
    public onTriggeredExit onExit2Do;
    public delegate void onTrigger();
    public onTrigger onTrigger2Do;

    private void OnTriggerExit(Collider other)
    {
        if (colliderOrTrigger == 2 && onExit2Do != null)
        {
            onExit2Do();
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (colliderOrTrigger == 2 && onTrigger2Do != null)
        {
            onTrigger2Do();
        }
    }
}
