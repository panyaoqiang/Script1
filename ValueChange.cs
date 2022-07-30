using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ValueChange : MonoBehaviour
{
    private object myValue;
    private string myValueClass;
    public ValueChange(object _myValue,MyValueChanged valueChangeEvent)
    {
        myValue = _myValue;
        myValueClass = _myValue.GetType().ToString();
        OnMyValueChanged = valueChangeEvent;
    }
    /// <summary>
    /// 新对象名.MyValue=任意值，使用此方法代替程序内字段
    /// 当此值被赋值改变时，执行对应的委托事件
    /// </summary>
    public object MyValue
    {
        get { return myValue; }
        set
        {
            //ValueChange 对象名.MyValue=即将被赋值的value
            if (value != myValue)
            {
                WhenMyValueChange();
            }
            myValue = value;
        }
    }
    //定义的委托
    public delegate void MyValueChanged(object sender, EventArgs e);
    //与委托相关联的事件
    public event MyValueChanged OnMyValueChanged;
    //事件触发函数
    private void WhenMyValueChange()
    {
        if (OnMyValueChanged != null)
        {
            OnMyValueChanged(this, null);
        }
    }
}
public class PlayerMoveEventArgs : EventArgs
{
    private string m_Message;

    public string Messgae { get { return m_Message; } }

    public PlayerMoveEventArgs(string m)
    {
        m_Message = m;
    }
    PlayerMoveEventArgs player = new PlayerMoveEventArgs("dddd");
}

