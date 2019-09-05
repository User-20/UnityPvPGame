using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;

/**
 *  @brief      캐릭터 애니메이션을 담당하는 클래스입니다.
 */
public class PlayerAnimation : MonoBehaviourPunCallbacks
{
    /// 캐릭터 애니메이션
    private Animator _animator;

    /// 현재 움직이는 중인지?
    private bool isRun = false;

    // Start is called before the first frame update
    void Start()
    {
        // 애니메이션 컴포넌트 설정
        _animator = GetComponent<Animator>();
    }

    /**
     *  @brief      공격 애니메이션을 실행하는 함수 입니다.
     */
    public void Attack()
    {
        // 공격 애니메이션 실행
        _animator.SetTrigger("Attack");
    }

    /**
     *  @brief      달리는 애니메이션을 실행하는 함수 입니다.
     *  
     *  @param      isMove          현재 조이스틱을 터치 중인지
     */
    public void Move(bool isMove)
    {
        // 실행되는 애니메이션과 설정해야 하는 애니메이션이 같다면 함수 종료
        // (달리는 중이라면 애니메이션을 다시 설정할 필요가 없음)
        if(isRun == isMove)
        {
            return;
        }

        // 달리고 있다는 flag 설정
        isRun = isMove;

        // 달리는 애니메이션 실행
        _animator.SetBool("isRun", isMove);
    }
}
