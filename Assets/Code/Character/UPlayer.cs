using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;

/**
 *  @brief  플레이어 캐릭터에 대한 행동을 세분화 한 클래스 입니다.
 */
[RequireComponent(typeof(CharacterController))]
public class UPlayer : Character
{
    /// 캐릭터 이동을 위한 컴포넌트
    protected CharacterController _characterController;

    // Start is called before the first frame update
    void Start()
    {
        // 초기화
        Initialize();
    }

    public override void Initialize()
    {
        // 로컬플레이어가 아니라면 함수 종료
        if (!photonView.IsMine)
        {
            return;
        }

        // 캐릭터 닉네임
        _playerState.nickName = PhotonNetwork.NickName;

        // 캐릭터 이동 속도
        _playerState.moveSpeed = 5.0f;

        // 현재 나의 팀 정보
        _playerState.teamInfo = GameManager._instance.GetTeamInfo();

        // 캐릭터 컨트롤러를 가져옴
        _characterController = GetComponent<CharacterController>();

        // 리모트 플레이어들에게 나의 팀 정보를 전달
        photonView.RPC("RPC_DeliverlyTempInfo", RpcTarget.Others, _playerState.teamInfo);
    }

    public override void Attack() { }

    /**
     * @brief       플레이어의 이동을 처리하는 함수 입니다.
     */
    public virtual void Move()
    {
        Vector3 movement = transform.forward * _playerState.moveSpeed * Time.deltaTime;
        _characterController.Move(movement);
    }

    /**
     * @brief       플레이어의 특수 공격을 사용할 수 있도록 하는 함수 입니다.
     */
    public virtual void UseSkill() { }

    /**
     * @brief       공격 버튼을 누르고 있는 동안 일반 공격 방향을 조준을 하는 함수 입니다.
     */
    public virtual void AttackAiming() { }

    /**
     * @brief       특수 공격 버튼을 누르고 있는 동안 공격 방향을 조준 하는 함수 입니다.
     * 
     *  @param      targetDistance      조이스틱 원점으로 부터 드래그 된 거리
     */
    public virtual void SkillAiming(float targetDistance = 0.0f) { }

    /**
     * @brief       게임이 시작될 때 리모트 플레이어들에게 나의 팀 정보를 전달합니다.
     */
    [PunRPC]
    public void RPC_DeliverlyTempInfo(ETeamInfo _teamInfo)
    {
        _playerState.teamInfo = _teamInfo;
    }

}
