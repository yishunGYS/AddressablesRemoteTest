using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyPlaceableManager : MonoBehaviour
{
    public static MyPlaceableManager Instance;
    
    public List<MyAIBase> players, enemys;
    public List<MyAIBase> allPlaceables;
    public List<MyProjectile> allProjectiles;

    public MyAIBase PlayerTower;
    public MyAIBase EnemyTower;
    private void Awake()
    {
        Instance = this;
        
    }

    private void Start()
    {
        players.Add(PlayerTower);
        enemys.Add(EnemyTower);
        allPlaceables.Add(PlayerTower);
        allPlaceables.Add(EnemyTower);
    }

    private void Update()
    {
        //Update游戏单位
        for (int i = 0; i < allPlaceables.Count; i++)
        {
            MyAIBase p = allPlaceables[i];
            //状态机
            switch (p.state)
            {
                case MyAIBase.State.Idle:
                    //找最近对象（兵、塔）
                    MyAIBase target = FindNearestEnemy(p);
                    //切换到Seek状态
                    p.SetTarget(target);
                    p.OnSeek();
                    break;
                case MyAIBase.State.Seek:
                    //如果范围内有敌人、塔，并切换攻击状态
                    if (p.IsTargetInRange())
                    {
                        p.OnAttack();
                    }
                    else
                    {
                        p.OnSeek();
                    }
                    break;
                case MyAIBase.State.Attack:
                    if (Time.time - p.lastAtkTime > p.attackRatio)
                    {
                        if (p.IsTargetInRange())
                        {
                            //通过使用动画，会自动调用dealDamage
                            p.DealAttack();
                        }
                        else
                        {
                            p.OnSeek();
                        }
                    }

                    break;
                case MyAIBase.State.Die:
                    //不要将溶解update在placeableManager里，因为它已经死了，需要从list中移除不然会导致找不了另一个target。
                    //p.DissolveEffect();
                    break;
            }
        }
        //update飞行物
        List<MyProjectile> desProjectiles = new List<MyProjectile>();
        for (int i = 0; i < allProjectiles.Count; i++)
        {
            MyProjectile curProjectile = allProjectiles[i];
            float progress =  curProjectile.Move();
            if (progress >= 1f )
            {
                curProjectile.owner.target.SufferDamage(curProjectile.owner.damagePerAttack,curProjectile.owner);
                desProjectiles.Add(curProjectile);
                
                Destroy(curProjectile.gameObject);
            }
        }

        for (int i = 0; i < desProjectiles.Count; i++)
        {
            allProjectiles.Remove(desProjectiles[i]);
        }
    }

    public void SetPlaceable(MyUnitBase.Team team, MyAIBase placeable,MyPlaceable pData)
    {
        //初始化小兵数据。
        placeable.ConstructPlaceable(team,pData);
        //加入PlaceManager管理。
        if (team == MyUnitBase.Team.Red)
        {
            players.Add(placeable);
            
        }
        else if (team == MyUnitBase.Team.Blue)
        {
            enemys.Add(placeable);
        }

        allPlaceables.Add(placeable);
        //注册死亡事件
        placeable.DeadEvent += placeable.OnDie;
    }


    public void RemovePlaceable(MyUnitBase.Team team, MyAIBase placeable)
    {
        if (team == MyUnitBase.Team.Red)
        {
            Instance.players.Remove(placeable);
        }
        else if (team == MyUnitBase.Team.Blue)
        { 
            Instance.enemys.Remove(placeable);
        }
        allPlaceables.Remove(placeable);

    }


    public MyAIBase FindNearestEnemy(MyAIBase p)
    {
        List<MyAIBase> enemyList = FindEnemyList(p);
        Vector3 curPos = p.transform.position;
        float minDis = 10000;
        MyAIBase target = null;
        if (enemyList.Count == 0)
        {
            print("GameOver");
            return null;   //应该只发生在gameOver
            
        }
        for (int i = 0; i < enemyList.Count; i++)
        { 
            float dis = Vector3.Distance(curPos, enemyList[i].transform.position);
            if (dis <=  minDis )
            {
                minDis = dis;
                target = enemyList[i];
            }
        }
        return target;
    }

    public List<MyAIBase> FindEnemyList(MyAIBase p)
    {
        return (p.team == MyUnitBase.Team.Red) ? enemys : players;
    }
}
