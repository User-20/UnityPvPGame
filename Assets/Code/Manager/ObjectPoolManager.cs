using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;

/**
 * @brief       오브젝트 풀을 관리하는 클래스 입니다.
 */

public class ObjectPoolManager : Sington<ObjectPoolManager>
{
    /// PooledObject 을 관리할 리스트
    public List<PooledObject> objectPool = new List<PooledObject>();

    /// 오브젝트풀의 총 개수
    private int poolLength = 0;

    private void Awake()
    {
        // 사용할 objectPool 을 여러개 사용할 경우가 있으므로
        // 반복문을 돌면서 모두 초기화 함
        poolLength = objectPool.Count;
        for(int i = 0; i < poolLength; i++)
        {
            objectPool[i].Initialize(transform);
        }
    }

    /**
     *  @brief 사용한 객체를 ObjectPool에 반환(저장)하는 함수입니다.
     *  
     *  @param          itemName        반환할 객체의 pool 오브젝트 이름
     *  @param          item            반환할 객체(GameObject)
     *  @param          parent          부모 계층 관계를 설정할 Transform
     *  
     *  @return         객체를 담을 오브젝트 풀이 존재하면 true, 없다면 false
     */
    public bool PushToPool(string itemName, GameObject item, Transform parent = null)
    {
        // itemName 을 관리하는 ObjectPool을 검색
        PooledObject pool = GetPoolItem(itemName);

        if(pool == null)
        {
            return false;
        }

        // pool 이 존재한다면 item을 pooList에 저장
        pool.PushToPool(item, parent == null ? transform : parent);
        return true;
    }

    /**
     *  @brief 필요한 객체를 Object Pool 에서 꺼내고 객체를 반환합니다.
     *  
     *  @param          itemName        요청할 객체의 pool 오브젝트 이름
     *  @param          parent          부모 계층 관계를 설정할 Transform
     *  
     *  @return         오브젝트 풀이 존재하면 오브젝트 풀에 있는 item 객체를 반환, 없다면 null
     */
    public GameObject PopFromPool(string itemName, Transform parent = null)
    {
        // itemName 을 관리하는 ObjectPool을 검색
        PooledObject pool = GetPoolItem(itemName);

        if(pool == null)
        {
            return null;
        }

        // pool 이 존재한다면 item을 pooList에서 꺼내고 반환함
        return pool.PopFromPool(parent);
    }

    /**
     *  @brief  오브젝트 풀이 존재하는지 검색 후 오브젝트 풀을 반환합니다.
     *  
     *  @param          itemName        pool에서 관리하는 Object 이름
     *  
     *  @return         오브젝트 풀이 존재하면 검색한 오브젝트 풀을 반환, 없다면 null
     */
    private PooledObject GetPoolItem(string itemName)
    {
        for (int i = 0; i < poolLength; i++)
        {
            if(objectPool[i].poolItemName.Equals(itemName))
            {
                return objectPool[i];
            }
        }

        Debug.LogWarning("object is not matched pool list");
        return null;
    }


}
