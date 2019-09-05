using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;

/**
 * @brief       포물선 운동을 하도록 하는 클래스 입니다.
 */

public class BombMove : MonoBehaviourPunCallbacks
{
    /// 오브젝트를 가지고 있는 플레이어 명
    public string owner = string.Empty;

    /// 오브젝트 풀에 저장된 bullet 오브젝트의 이름
    public string poolItemName = "Bomb";

    /// 물체의 발사 각도
    public float angle = 45.0f;

    /// 물체가 회전하는 속도
    public float rotationSpeed = 500.0f;

    /// 중력가속도
    public const float GRAVITY = 9.8f;

    /// 물체의 시작 위치
    private Vector3 startPosition;

    /// 물체가 도달할 목표 지점
    private Vector3 targetPosition;

    /// 물체가 날아가는 동안 회전할 실제 mesh object
    private Transform bomb;

    /// 충돌 했는지 flag
    private bool isCollision = false;

    private void Start()
    {
        // 자식 오브젝트가 있다면
        if(transform.childCount > 0)
        {
            // 자식 오브젝트를 움직일 대상으로 설정
            bomb = transform.GetChild(0);
        }
        else
        {
            // 자식 오브젝트가 없다면 자신을 움직일 대상으로 설정
            bomb = this.transform;
        }

    }

    /**
     * @brief       물체를 포물선 운동을 하도록 하는 RPC 함수입니다.
     * 
     * @param       _startPosition          물체의 시작 위치
     * @param       _targetPosition         물체의 도착 위치
     */
    [PunRPC]
    public void Shoot(Vector3 _startPosition, Vector3 _targetPosition)
    {
        // 시작 위치
        startPosition = _startPosition;
        // 도착 위치
        targetPosition = _targetPosition;
        // 충돌 flag 초기화
        isCollision = false;

        // 코루틴 시작
        StartCoroutine("CouroutineShoot");
    }


    /**
     * @brief       포물선 운동을 연산하는 코루틴 입니다.
     */
    IEnumerator CouroutineShoot()
    {
        // 초기 위치 지정
        transform.position = startPosition;

        // 두 점(시작지점 타겟지점) 사이의 거리를 구함
        float distance = Vector3.Distance(startPosition, targetPosition);

        // 발사 각도를 라디안으로 변경
        float RadianAngle = angle * Mathf.Deg2Rad;

        // 수평도달거리 공식(R = v0^2 * sin2(theta) / g) 을 이용하여 (v0^2)을 구함
        float squareVelocity = distance / (Mathf.Sin(2.0f * RadianAngle) / GRAVITY);

        // v0^2의 제곱을 제거함
        float velocity = Mathf.Sqrt(squareVelocity);

        // 각 성분에 대한 속도를 구함
        float Vx = velocity * Mathf.Cos(RadianAngle);
        float Vy = velocity * Mathf.Sin(RadianAngle);

        // 수평 이동 거리(x = v0 * t)을 이용해서 목표 지점에 도달하는 시간을 구함
        //float duration = distance / Vx;
        
        // 타켓 위치를 바라보도록 회전
        Vector3 direction = (targetPosition - startPosition).normalized;
        transform.rotation = Quaternion.LookRotation(direction);

        // 누적 시간
        float _elapsedtime = 0.0f;

        // 목표 지점에 충돌할 때 까지 반복
        while (isCollision == false)
        {
            // t초 후 물체의 위치를 정해줌
            transform.Translate(0.0f,
                                (Vy - (GRAVITY * _elapsedtime)) * Time.deltaTime,
                                Vx * Time.deltaTime);

            if(bomb)
            {
                // right 축으로 rotationSpeed 만큼 계속 회전시킴
                bomb.Rotate(Vector3.right * rotationSpeed * Time.deltaTime);
            }

            _elapsedtime += Time.deltaTime;

            yield return null;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Untagged")
        {
            return;
        }

        Character _object = other.GetComponent<Character>();
        if(_object)
        {
            // 발사한 오브젝트와 충돌한 오브젝트가 같다면 함수 종료
            if (owner == _object.GetData().nickName)
            {
                return;
            }
        }
        
        // 충돌 했으므로 flag 설정
        isCollision = true;

        // 다른 오브젝트와 접촉했다면 object pool 에 반환함
        ObjectPoolManager.GetInstance().PushToPool(poolItemName, gameObject);
    }

}
