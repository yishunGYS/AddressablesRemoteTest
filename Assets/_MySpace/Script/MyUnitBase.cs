using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MyUnitBase : MonoBehaviour
{
    public enum Team
    {
        None,
        Red,
        Blue,
    }
    
    public Team team;
    
    public UnityAction DeadEvent;

}
