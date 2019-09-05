using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;

/**
* @struct CharacterState
* @brief 캐릭터의 정보를 담고 있는 구조체
*/

[System.Serializable]
public struct CharacterState
{
    /// 진영 정보
    public ETeamInfo teamInfo;
    /// 캐릭터 닉네임
    public string nickName;
    /// 캐릭터에 대한 체력
    public int hp;
    /// 캐릭터 이동 속도
    public float moveSpeed;
}


/**
 *  @brief  캐릭터를 생성하기 위한 최상위 추상 클래스 입니다
 */
public abstract class Character : MonoBehaviourPunCallbacks
{
    [SerializeField]
    /// 캐릭터의 정보를 담고 있는 구조체
    protected CharacterState _playerState;

    [SerializeField]
    /// 캐릭터 체력 프로그래스 바
    protected ProgressBar hpProgressBar;

    /**
     * @brief       맴버 변수들을 초기화 하는 함수 입니다.
     */
    public abstract void Initialize();

    /**
     * @brief       캐릭터 공격 행동을 처리하는 함수 입니다
     */
    public abstract void Attack();

    /**
     * @brief       플레이어 정보를 가지는 구조체를 반환 합니다
     * @return      _playerState
     */
    public CharacterState GetData()
    {
        return _playerState;
    }

}
