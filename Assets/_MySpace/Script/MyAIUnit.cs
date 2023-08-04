using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MyAIUnit : MyAIBase
{
    private NavMeshAgent _navMeshAgent;
    private Animator _anim;
    
    //死亡溶解参数
    private Renderer[] _renderers;
    private float _dissolveProgress = 0;
    private int _dissolveDuration = 3;
    private Color _dissolveColor;

    private Collider[] _colliders;
    private void Awake()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _anim = GetComponent<Animator>();

        _renderers = GetComponentsInChildren<Renderer>();
        _colliders = GetComponentsInChildren<Collider>();
    }

    private void Update()
    {
        if (state == State.Die)
        {
            DissolveEffect();
        }
    }
    
    
    public override void OnSeek()
    {
        base.OnSeek();
        if (!target)
        {
            return;
        }
        _navMeshAgent.enabled = true;
        _navMeshAgent.SetDestination(target.transform.position);
        
        //移动动画，
        _anim.SetBool("IsMoving",true);
    }

    public override void OnAttack()
    {
        base.OnAttack();
        //print("停止移动，开始攻击");
        _navMeshAgent.enabled = false;
        
        _anim.SetBool("IsMoving",false);
    }

    public override void DealAttack()
    {
        base.DealAttack();
        //朝向敌人攻击
        transform.LookAt(target.transform.position);
        _anim.SetTrigger("Attack");
    }

    public override void OnDie()
    {
        base.OnDie();
        DeadEvent -= OnDie;
        _navMeshAgent.enabled = false;
        
        //死亡动画、死亡溶解特效
        _anim.SetTrigger("IsDead");

        _dissolveColor = (team == Team.Red) ? Color.red : Color.blue;
        foreach (var render in _renderers)
        {
            render.material.SetColor("_DissolveColor",_dissolveColor * 8);
            render.material.SetFloat("_DissolveWidth",0.05f);
        }
        //销毁该物体
        Destroy(gameObject,_dissolveDuration );
        foreach (var collider in _colliders)
        {
            collider.enabled = false;
        }
        MyPlaceableManager.Instance.RemovePlaceable(team,this); 
    }

    void DissolveEffect()
    {
        if (_dissolveProgress > 1)
        {
            return;
        }
        _dissolveProgress += Time.deltaTime  /  _dissolveDuration;
        foreach (var render in _renderers)
        {
            render.material.SetFloat("_DissolveFactor",_dissolveProgress);
        }
    }
}
