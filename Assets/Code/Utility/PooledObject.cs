using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * @brief   GameObject를 관리하는 오브젝트 풀 입니다.
 */

[System.Serializable]
public class PooledObject
{
    /// 객체를 검색할 때 사용할 이름
    public string poolItemName = string.Empty;

    /// 오브젝트풀에 저장할 객체 (프리팹)
    public GameObject prefab = null;

    /// 초기화할 때 생성할 객체의 수
    public int poolCount = 0;

    [SerializeField]
    /// 생성한 객체들을 저장할 리스트
    private Queue<GameObject> poolList = new Queue<GameObject>();

    public void Initialize(Transform parent = null)
    {
        for(int i = 0; i < poolCount; i++)
        {
            // poolCount 에 설정된 개수 만큼 object를 생성함
            GameObject item = CreateItem(parent);
            if(item != null)
            {
                poolList.Enqueue(item);
            }
            
        }
    }

    /**
     *  @brief  pool에 사용할 객체를 Pool 목록에 저장하는 함수입니다.
     *  
     *  @param          item            pool에 담을 객체(GameObject)
     *  @param          parent          부모 계층 관계를 설정할 Transform
     */
    public void PushToPool(GameObject item, Transform parent = null)
    {
        // 위치와 회전값을 다시 초기화
        item.transform.position = Vector3.zero;
        item.transform.rotation = Quaternion.identity;

        // item을 objectPool 안에 생성될 수 있도록 부모를 설정함
        item.transform.SetParent(parent);

        // 현재 시점에서는 필요없으니 활성화를 꺼둠
        item.SetActive(false);

        // object를 poolList에 추가함
        poolList.Enqueue(item);
    }

    /**
     *  @brief  필요한 객체를 Pool 에서 꺼내서 item을 반환합니다.
     *  
     *  @param          parent          (pool에 생성된 객체가 없을 경우를 위한) 부모 계층 관계를 설정할 Transform
     *  
     *  @return         오브젝트 풀에 꺼낼 오브젝트가 존재하면 gameObject 반환, 없다면 null
     */
    public GameObject PopFromPool(Transform parent = null)
    {
        if(poolList.Count == 0)
        {
            Debug.LogWarning("Pool List is Empty");
            return null;
        }

        return poolList.Dequeue();
    }

    /**
     *  @brief  pool 사용할 Prefab 객체를 생성한 후 객체를 반환 합니다.
     *  
     *  @param          parent          부모 계층 관계를 설정할 Transform
     *  
     *  @return         오브젝트 풀에 담을 객체가 존재하면 gameObject 반환, 없다면 null
     */
    private GameObject CreateItem(Transform parent = null)
    {
        // poolItemName 변수에 지정된 이름으로 prefab을 생성함
        GameObject item = Object.Instantiate(prefab) as GameObject;
        
        // 객체가 없다면 null 반환
        if(item == null)
        {
            return null;
        }

        // item 의 이름을 설정함
        item.name = poolItemName;

        // item을 objectPool 안에 생성될 수 있도록 부모를 설정함
        item.transform.SetParent(parent);

        // 현재 시점에서는 필요없으니 활성화를 꺼둠
        item.SetActive(false);

        return item;
    }
    
}
