using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * @brief       기지를 방어할 건물의 상위 클래스 입니다.
 */
public class Tower : Character
{
    /// 최대 체력치
    protected int hpMax = 10;

    /// 충돌용 박스 콜라이더
    protected BoxCollider _collider;

    // Start is called before the first frame update
    void Start()
    {
        Initialize();
    }

    public override void Initialize()
    {
        // 체력을 최대치로 초기화
        _playerState.hp = hpMax;

        // 타워의 오브젝트 이름
        _playerState.nickName = name;

        // 체력 프로그래스바 초기화
        hpProgressBar = transform.parent.GetComponentInChildren<ProgressBar>();
        hpProgressBar.Initialize(hpMax);

        // 충돌용 콜라이더
        _collider = GetComponentInParent<BoxCollider>();
    }

    public override void Attack()
    {

    }
}
