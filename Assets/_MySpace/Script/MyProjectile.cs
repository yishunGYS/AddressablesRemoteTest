using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MyProjectile : MonoBehaviour
{
    public MyAIBase owner;
    private Vector3 _initPos;
    private float _progress;
    private Vector3 offset = new Vector3(0f, 1.2f, 0f);
    private void Awake()
    {
        _initPos = transform.position;
    }

    public float Move()
    {
        _progress += Time.deltaTime * 3;
        
        Vector3 targetPos = owner.target.transform.position + offset;
        transform.LookAt(targetPos);
        transform.position = Vector3.Lerp(_initPos, targetPos, _progress);
        return _progress;
    }
}
