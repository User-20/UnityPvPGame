using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * @brief   지정 된 타겟 오브젝트를 따라다니는 카메라 입니다.
 */

public class FollowCamera : MonoBehaviour
{
    [SerializeField]
    /// 카메라와 플레이어의 거리
    private float distance = 5.0f;               

    [SerializeField]
    /// 카메라 높이
    private float height = 5.0f;

    /// 타겟(플레이어) 위치, 회전, 크기 정보
    public Transform target;
    /// 타겟(플레이어) 게임오브젝트
    public GameObject targetObject;

    /// 타겟으로 부터 떨어질 방향
    private Vector3 direction;

    void LateUpdate()
    {
        // 타겟 오브젝트가 없거나, 타겟 오브젝트가 비활성화라면 함수 종료
        if (!target || !targetObject.activeSelf)
        {
            return;
        }
        
        // 카메라 위치와 바라보는 방향을 지정
        transform.position = target.position - (direction * distance) + (Vector3.up * height);
        transform.LookAt(target);
    }

    /**
     * @brief   카메라가 바라볼 타겟 오브젝트를 설정 합니다.
     * 
     * @param       _target         타겟의 GameObject
     * @param       teamInfo        타겟의 팀 정보
     */
    public void SetTarget(GameObject _target, ETeamInfo teamInfo)
    {
        // 타겟의 gameObject
        targetObject = _target;

        // 타켓의 transform
        target = _target.transform.GetChild(0).transform;

        // red 팀이라면 정면으로 바라봄
        // blue라면 반대방향으로 바라봄
        direction = teamInfo == ETeamInfo._Red ? Vector3.forward : Vector3.back;
    }
}
