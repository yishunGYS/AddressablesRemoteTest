using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Profiling;
using UnityEngine.UI;
public class MyPlayingCardView : MonoBehaviour,IPointerDownHandler,IPointerUpHandler,IDragHandler
{
    public int cardId;
    public MyCard data;

    private Transform _previewHolder;

    private Camera _cameraMain;
    
    private void Start()
    {
        _cameraMain = Camera.main;
        _previewHolder = GameObject.Find("PreviewHolder").transform;
    }

    public void OnPointerDown(PointerEventData pointerEvent)
    {
        // 实现鼠标按下动作
        transform.SetAsLastSibling();   //选中卡牌时，渲染在所有卡牌上方
        //渲染不可可放置区域
        MyCardManager.Instance.forbiddenArea.enabled = true;
    }

    private bool _isActive = false;         //是否变过兵
    public void OnDrag(PointerEventData eventData)
    {
        //卡牌跟随鼠标位置
        transform.Translate(eventData.delta);   //用偏移量实现,很巧妙，需要对每个方法很熟悉

        //打一根射线，
        RaycastHit hit;
        Ray ray = _cameraMain.ScreenPointToRay(eventData.position);
        bool isHit = Physics.Raycast(ray,out hit, Mathf.Infinity, LayerMask.GetMask("PlayingField"));
        Debug.DrawRay(ray.origin,ray.direction*1000,Color.green);
        
        //如果射线打中可放置区，则卡牌变兵
        if (isHit)
        {
            _previewHolder.position = hit.point;
            if (_isActive == false)
            {
                _isActive = true;
                GetComponent<CanvasGroup>().alpha = 0;
                //创建实体预览兵
                CreatePlaceable();
            }
        }
        //如果在卡牌区，如果卡牌之前变过兵，则兵变卡牌。
        else
        {
            if (_isActive)
            {
                _isActive = false;
                DestroyPlaceable();
                GetComponent<CanvasGroup>().alpha = 1;
            }
        }
    }
    
    public async void OnPointerUp(PointerEventData pointerEvent)
    {
        //取消渲染禁止区域
        MyCardManager.Instance.forbiddenArea.enabled = false;
        
        RaycastHit hit;
        Ray ray = _cameraMain.ScreenPointToRay(pointerEvent.position);
        bool isHit = Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("PlayingField"));
        //如果在可放置区，则在该位置生成兵
        if (isHit)
        {
            //建兵
            MyCardManager.Instance.CardUse(data,hit.point,MyUnitBase.Team.Red);
            
            //销毁previewHolder的预览兵，并销毁卡牌
            DestroyPlaceable();
            Destroy(gameObject);
            //发牌
            await MyCardManager.Instance.AddToPlayPos(cardId, 0.5f);
            await MyCardManager.Instance.AddToPreviewPos( 0.5f);
        }
        //如果在卡牌区，则回到原位
        else
        {
            //DOMove
            transform.DOMove(MyCardManager.Instance.cardPos[cardId].position,0.2f);
        }
    }

    void CreatePlaceable()
    {
        for (int i = 0; i < data.placeablesIndices.Length; i++)
        {
            for (int j = 0; j < MyPlaceableModel.instance.list.Count; j++)
            {
                if (data.placeablesIndices[i] == MyPlaceableModel.instance.list[j].id)
                {
                    //Profiler.BeginSample("CreateUnit1");
                    
                    var placeablePrefab = MyPlaceableModel.instance.list[j].associatedPrefab;
                    Addressables.InstantiateAsync(placeablePrefab,
                        _previewHolder.position + data.relativeOffsets[i], Quaternion.identity, _previewHolder);
                    
                    
                    //MyAIBase placeablePrefab = Resources.Load<MyAIBase>(MyPlaceableModel.instance.list[j].associatedPrefab);
                    //Instantiate(placeablePrefab, _previewHolder.position+data.relativeOffsets[i], Quaternion.identity,_previewHolder);
                    
                    //Profiler.EndSample();
                }
            }
        }
    }

    void DestroyPlaceable()
    {
        for (int i = 0; i < _previewHolder.childCount; i++)
        {
            Destroy(_previewHolder.GetChild(i).gameObject);
        }
    }

    
}
