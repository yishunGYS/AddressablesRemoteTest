using System;
using System.Collections;
using System.Collections.Generic;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using UnityEngine;
using UnityEngine.Events;

public class MyAIBase : MyUnitBase
{
    public enum State
    {
        None,
        Idle,
        Seek,
        Attack,
        Die,
    }
    public State state = State.Idle;

    public MyProjectile projectilePrefab;
    public Transform projectileSpawnPoint;

    public float hitPoints;
    public float attackRange;
    public float attackRatio;
    public float damagePerAttack;
    
    [HideInInspector]public float lastAtkTime;
    public MyAIBase target;

    public List<MyAIBase> targetList ;
    public void ConstructPlaceable(Team pTeam,MyPlaceable pData)
    {
        team = pTeam;
        hitPoints = pData.hitPoints;
        attackRange = pData.attackRange;
        attackRatio = pData.attackRatio;
        damagePerAttack = pData.damagePerAttack;

        MyAIBase tower = (pTeam == Team.Red)
            ? MyPlaceableManager.Instance.EnemyTower
            : MyPlaceableManager.Instance.PlayerTower;
        targetList = new List<MyAIBase>(){tower};
    }
    

    public virtual void OnIdle()
    {
        
    }

    public virtual void OnSeek()
    {
        state = State.Seek;
    }

    public virtual void OnAttack()
    {
        state = State.Attack;
    }

    public virtual void DealAttack()
    {
        lastAtkTime = Time.time;
    }

    public virtual void OnDie()
    {
        state = State.Die;
    }
    
    
    public void SetTarget(MyAIBase p)
    {
        target = p;
        if (!targetList.Contains(p))
        {
            targetList.Add(p);
            //添加targetDie的委托？
            p.DeadEvent += TargetIsDead;
        }
    }


    public void TargetIsDead()
    {
        target.DeadEvent -= TargetIsDead;
        targetList.Remove(target);
        //target = targetList[targetList.Count - 1];
        state = State.Idle;
    }

    public bool IsTargetInRange()
    {
        if (!target)
        {
            return false;
        }
        float dis = Vector3.Distance(transform.position, target.transform.position);
        return (dis <= attackRange) ? true : false;
    }

    public void MeleeAttack()
    {
        //音效
        
        //目标敌人若还活着，则扣血
        float damage = damagePerAttack;
        target.SufferDamage(damage,this);
    }

    public void RangeAttack()
    {
        //音效
        
        //生成飞行物
        MyProjectile _projectile = Instantiate(projectilePrefab, projectileSpawnPoint.position, Quaternion.identity);
        _projectile.owner = this;
        MyPlaceableManager.Instance.allProjectiles.Add(_projectile);
        
    }

    public void SufferDamage(float damage,MyAIBase atker)
    {
        float curHp = (hitPoints - damage <= 0) ? 0 : hitPoints - damage;
        hitPoints = curHp;
        if (curHp <= 0)
        {
            if (DeadEvent != null)
            {
                DeadEvent();
            }
        }
        //目标转为施暴者
        if (!targetList.Contains(atker))
        {
            targetList.Add(atker);
            atker.DeadEvent += TargetIsDead;
            target = atker;
        }
        //更新敌人扣血UI
        
    }
    
}
