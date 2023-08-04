using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Profiling;
using UnityRoyale;
using Random = UnityEngine.Random;
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;

//实现卡牌的创建、拖拽等
public class MyCardManager : MonoBehaviour
{
    public static MyCardManager Instance;

    
    //创建卡牌
    public RectTransform canvas;
    public RectTransform startPos, endPos;
    public RectTransform[] cardPos = new RectTransform[3];

    private RectTransform _previewCard;

    //不可放置区域
    public MeshRenderer forbiddenArea;
    private void Awake()
    {
        Instance = this;
    }

    async void Start()
    {
        await AddToPreviewPos(0);
        for (int i = 0; i < 3; i++)
        {
            await AddToPlayPos(i,  0.3f);
            await AddToPreviewPos( 0.6f);
        }
    }

    public async Task AddToPreviewPos(float delay)
    {
        await new WaitForSeconds(delay);

        int randomNum = Random.Range(0, MyCardModel.instance.list.Count);
        MyCard card = MyCardModel.instance.list[randomNum];

        //异步实例化
        GameObject cardPrefab = await Addressables.InstantiateAsync(card.cardPrefab,startPos.position,Quaternion.identity,canvas).Task;
        _previewCard = cardPrefab.GetComponent<RectTransform>();
        //GameObject cardPrefab = Resources.Load<GameObject>(card.cardPrefab);
        //_previewCard = Instantiate(cardPrefab, canvas).GetComponent<RectTransform>();
        _previewCard.localScale = Vector3.one * 0.7f;
        //_previewCard.position = startPos.position;
        _previewCard.DOMove(endPos.position, 0.2f);

        //设置data    
        MyPlayingCardView playingCard = _previewCard.GetComponent<MyPlayingCardView>();
        playingCard.data = card;
    }

    public async Task AddToPlayPos(int posNum, float delay)
    {
        await new WaitForSeconds(delay);
        _previewCard.localScale = Vector3.one;
        _previewCard.DOMove(cardPos[posNum].position, 0.2f);

        //设置卡牌Id、数据
        MyPlayingCardView playingCard = _previewCard.GetComponent<MyPlayingCardView>();
        playingCard.cardId = posNum;
    }

    //适用场景:真人打出手牌/AI随机出牌
    public async void CardUse(MyCard cardData, Vector3 spawnPos, MyUnitBase.Team team)
    {
        //生成兵，并设置兵的阵营
        for (int i = 0; i < cardData.placeablesIndices.Length; i++)
        {
            for (int j = 0; j < MyPlaceableModel.instance.list.Count; j++)
            {
                if (cardData.placeablesIndices[i] == MyPlaceableModel.instance.list[j].id)
                {
                    //Profiler.BeginSample("CreateUnit2");

                    var placeablePrefab = (team == MyUnitBase.Team.Red)
                        ? MyPlaceableModel.instance.list[j].associatedPrefab
                        : MyPlaceableModel.instance.list[j].alternatePrefab;
                    Quaternion quation = (team == MyUnitBase.Team.Red)
                        ? Quaternion.identity
                        : Quaternion.Euler(0, 180, 0);
                    
                    //MyAIBase placeable = Instantiate(placeablePrefab, spawnPos + cardData.relativeOffsets[i], quation);
                    //异步实例化
                    GameObject placeable = await Addressables.InstantiateAsync(placeablePrefab,spawnPos + cardData.relativeOffsets[i],quation).Task;
                    MyAIBase aiPlaceable = placeable.GetComponent<MyAIBase>();
                    
                    //Profiler.EndSample();
                    //给小兵附上数据
                    //小兵的阵营交给placeableManager管理
                    MyPlaceableManager.Instance.SetPlaceable(team, aiPlaceable, MyPlaceableModel.instance.list[j]);

                }
            }
        }
    }
    
    
}