using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;


public class MyOpponentCardManager : MonoBehaviour
{
    public float SetPlaceableSpeed = 5f;
    public bool isGameOver;

    async void Start()
    {
        await SetPlaceable();
    }

    async Task SetPlaceable()
    {
        while (!isGameOver)
        {
            await new WaitForSeconds(SetPlaceableSpeed);

            MyCard randomCard = GetRandomCard();
            Vector3 randomPos = GetRandomPos();
            MyCardManager.Instance.CardUse(randomCard,randomPos,MyUnitBase.Team.Blue);
        }
    }

    MyCard GetRandomCard()
    {
        int randomNum = Random.Range(0, MyCardModel.instance.list.Count);
        return MyCardModel.instance.list[randomNum];
    }

    Vector3 GetRandomPos()
    {
        return new Vector3(Random.Range(-5f, 5f), 0f, Random.Range(3f, 8.5f));
    }
}
