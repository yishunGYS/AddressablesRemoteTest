using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MyEvent : UnityEvent<int>
{
}

public class TestUnityEvent : MonoBehaviour
{
    public MyEvent Event1 = new MyEvent();
    public UnityAction<int> FunAction;
    
    private void Start()
    {
        // if (event1 == null)
        // {
        //     event1 = new MyEvent();
        // }
        FunAction += Fun1;
        FunAction += Fun2;
        Event1.AddListener(FunAction);
        
        Event1.Invoke(10);
    }


    void Fun1(int x)
    {
        Debug.Log("Fun1" + x);
    }
    
    void Fun2(int x)
    {
        Debug.Log("Fun2" + 2 * x);
    }
}
