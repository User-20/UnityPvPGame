using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// 조이스틱 터치 종료 후 등록된 함수를 호출해줄 delegate
public delegate void OnDragEndEvent();

/**
 *  @brief      플레이어가 조작할 조이스틱을 구현한 클래스 입니다.
 */
public class VirtualJoyStick : MonoBehaviour
{
    /// Red 팀 캐릭터의 회전 값을 계산할 변수
    private const int BOTTOM_TO_TOP = 1;

    /// blue 팀 캐릭터의 회전 값을 계산할 변수
    private const int TOP_TO_BOTTOM = -1;

    /// 조작할 조이스틱
    public Transform stick;

    /// 조이스틱 초기 위치
    private Vector3 origPos;

    /// 조이스틱 방향 벡터
    private Vector3 direction;

    /// 조이스틱 배경의 반 지름(반경)
    private float radius;

    /// 조이스틱을 터치 했는지?
    private bool isJoyStickTouch;

    /// 원점으로 부터의 최종 거리
    private float distance;

    /// 조이스틱 터치 끝났을 때 알려줄 이벤트
    public event OnDragEndEvent OnDragEnd;

    /// 캐릭터 진영에 따라 이동 방향을 바꾸기 위한 변수
    private int move = BOTTOM_TO_TOP;

    // Use this for initialization
    void Start()
    {
        // 조이스틱의 반지름
        radius = GetComponent<RectTransform>().sizeDelta.y * 0.5f;

        if(!stick)
        {
            Debug.LogError("Have not stick object reference");
        }

        // 초기위치 설정
        origPos = stick.transform.position;

        // 캔버스 크기에 대한 반지름 조절
        float CanvasScale = transform.parent.GetComponent<RectTransform>().localScale.x;
        radius *= CanvasScale;

        // 터치중인지 flag 초기화
        isJoyStickTouch = false;
    }


    /**
     *  @brief      조이스틱을 터치했을 때 호출되는 함수 입니다.
     *  
     *  @param      ped         터치에 대한 데이터 정보 (위치 및 몇번 터치했는지?)
     */
    public virtual void Drag(BaseEventData ped)
    {
        // 조이스틱 터치중
        isJoyStickTouch = true;

        PointerEventData Data = ped as PointerEventData;
        Vector3 position = Data.position;

        // 조이스틱의 이동 방향을 구함
        direction = (position - origPos).normalized;

        // 조이스틱의 처음 위치와 현재 터치하고 있는 위치의 거리를 구함
        float currentDistance = Vector3.Distance(position, origPos);

        // 거리가 반지름 보다 작으면 조이스틱을 현재 터치하고 있는 곳으로 이동
        if (currentDistance < radius)
        {
            stick.position = origPos + direction * currentDistance;
        }
        else
        {
            // 거리가 반지름 보다 커지면 조이스틱의 반지름 크기만큼만 이동
            stick.position = origPos + direction * radius;
        }

        // 원점에서 부터 드래그 된 최종 길이를 얻음
        distance = Vector3.Distance(stick.position, origPos);

    }

    /**
     *  @brief      조이스틱 터치가 끝났을 때 호출되는 함수 입니다.
     */
    public virtual void DragEnd()
    {
        // 조이스틱을 초기 상태로 셋팅
        stick.position = origPos;
        direction = Vector3.zero;
        distance = 0.0f;
        isJoyStickTouch = false;

        // 이벤트 리스너가 있다면
        if (OnDragEnd != null)
        {
            // 이벤트 전파
            OnDragEnd();
        }
    }

    /**
     *  @biref      조이스틱 터치 중인지 상태를 반환합니다.
     */
    public bool GetIsJoyStickTouch()
    {
        return isJoyStickTouch;
    }

    /**
     *  @biref      조이스틱 원점으로 부터 드래그 된 각도를 반환합니다.
     */
    public Vector3 GetEulerAngles()
    {
        Vector3 eulerAngles = new Vector3(0, Mathf.Atan2(direction.x * move, direction.y * move) * Mathf.Rad2Deg, 0);
        return eulerAngles;
    }

    /**
     *  @brief      조이스틱 원점으로 부터 드래그 된 거리를 반환합니다.
     */
    public float GetDistance()
    {
        return distance;
    }

    /**
     *  @biref      캐릭터 진영에 따라 이동 방향을 바꿔주는 함수        
     * 
     *  @param       teamInfo        캐릭터 팀 진영
     */
    public void SetMoveDirection(ETeamInfo teamInfo)
    {
        // red - 아래에서 위로 전진
        // blue - 위에서 아래로 전진
        move = teamInfo == ETeamInfo._Blue ? TOP_TO_BOTTOM : BOTTOM_TO_TOP;
    }
}