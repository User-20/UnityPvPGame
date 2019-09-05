using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;

/**
 * @brief       총알의 움직임을 연산하는 클래스입니다.
 */

public class BulletMove : MonoBehaviourPunCallbacks
{
    /// 총알을 가지고 있는 플레이어 명
    public string owner = string.Empty;

    /// 오브젝트 풀에 저장된 bullet 오브젝트의 이름
    public string poolItemName = "Bullet";

    /// 총알의 이동 속도
    public float moveSpeed = 10.0f;

    /// 총알의 수명
    public float lifeTime = 3.0f;

    /// 총알이 활성화된 뒤 경과시간을 계산하기 위한 변수
    public float _elapsedTime = 0.0f;

    /// 충돌에 대한 flag
    private bool isCollision = false;

    // Update is called once per frame
    void Update()
    {
        // 총알 발사 함수 실행
        Fire();
    }


    /**
     * @brief   총알 발사 후 누적 시간을 반환합니다.
     */
    float GetTimer()
    {
        return (_elapsedTime += Time.deltaTime);
    }

    /**
     * @brief   총알 발사 후 누적 시간을 초기화 합니다.
     */
    void SetTimer()
    {
        _elapsedTime = 0.0f;
    }

    /**
     * @brief   총알 발사 후 움직임을 연산하는 함수입니다.
     */
    private void Fire()
    {
        // 전방으로 moveSpeed 만큼 이동함
        transform.position += transform.up * moveSpeed * Time.deltaTime;

        // 경과시간이 지났거나 충돌을 했다면
        if (GetTimer() > lifeTime || isCollision)
        {
            // 타이머 초기화
            SetTimer();

            // 오브젝트를 pool에 반환
            ObjectPoolManager.GetInstance().PushToPool(poolItemName, gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // tag가 설정 되어있지 않다면 무시
        if (other.tag == "Untagged")
        {
            return;
        }

        // 충돌 했으므로 flag 설정
        isCollision = true;
    }
}
