using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 *  @brief  기지를 방어할 실제 건물 오브젝트의 몸체 입니다. 데미지 관련을 담당 합니다.
 */

public class TeamCannonBody : MonoBehaviour
{
    /// 데미지를 입을 수 있는 물체를 감지하기 위한 tag
    private const string TAG_DEMAGE = "Demage";

    /// 부모 오브젝트
    private TeamCannon parentObject;

    private void Start()
    {
        // 부모오브젝트를 가져옴
        parentObject = transform.parent.GetComponent<TeamCannon>();
    }

    private void OnTriggerEnter(Collider other)
    {
        // 현재 충돌된 오브젝트의 tag
        string tagName = other.gameObject.tag;
        if (tagName == TAG_DEMAGE)
        {
            // xml에 저장된 item의 정보를 가져옴
            ItemInfo collisionInfo = GameManager._instance.GetItemInfo(other.gameObject.name);
            if (collisionInfo != null)
            {
                // 충돌된 오브젝트의 데미지
                int demage = collisionInfo.demage;
                
                // 체력 프로그래스바 갱신
                parentObject.SetGage(demage);
            }
        }
    }
}
