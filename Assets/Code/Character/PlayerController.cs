using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Photon.Pun;
using Photon.Realtime;

/**
 *  @brief      캐릭터 움직임에 대한 Input을 처리하는 클래스 입니다.
 */
public class PlayerController : MonoBehaviourPunCallbacks
{
    /// 모바일 방향키 조이스틱
    public VirtualJoyStick moveJoyStick;
    /// 공격 조이스틱
    public VirtualJoyStick attackJoyStick;
    /// 스킬 조이스틱
    public VirtualJoyStick skillJoyStick;

    /// 플레이어 오브젝트
    private UPlayer _player;
    /// 플레이어 애니메이션
    private PlayerAnimation _playerAnimation;

    /// 일반 공격 조준 중인지
    private bool isAttackAiming = false;

    /// 특수 공격 조준 중인지
    private bool isSkillAiming = false;

    // Start is called before the first frame update
    void Start()
    {
        // 플레이어 컴포넌트 설정
        _player = gameObject.GetComponent<UPlayer>();

        // 플레이어 애니메이션 컴포넌트 설정
        _playerAnimation = gameObject.GetComponent<PlayerAnimation>();

        // 로컬 플레이어인지 체크
        if (!photonView.IsMine)
        {
            return;
        }

        // 조이스틱 초기화
        JoyStickInitialize();
    }

    private void Update()
    {
        // 로컬 플레이어인지 체크
        if (!photonView.IsMine)
        {
            return;
        }

        // 공격 조이스틱 오브젝트가 있다면
        if(attackJoyStick)
        {
            // 일반 공격 조이스틱이 터치 중인지?
            isAttackAiming = attackJoyStick.GetIsJoyStickTouch();
            if (isAttackAiming)
            {
                // 조이스틱의 방향으로 캐릭터 회전
                transform.eulerAngles = attackJoyStick.GetEulerAngles();

                // 공격 조준 UI 출력
                _player.AttackAiming();
            }
        }

        // 특수공격 조이스틱 오브젝트가 있다면
        if(skillJoyStick)
        {
            // 특수 공격 조이스틱이 터치 중인지?
            isSkillAiming = skillJoyStick.GetIsJoyStickTouch();
            if (isSkillAiming)
            {
                // 조이스틱의 방향으로 캐릭터 회전
                transform.eulerAngles = skillJoyStick.GetEulerAngles();

                // 공격 조준 UI 출력
                _player.SkillAiming(skillJoyStick.GetDistance());
            }
        }


        // 이동 조이스틱 오브젝트가 존재한다면
        if (moveJoyStick)
        {
            // 이동 조이스틱이 터치 중인지?
            bool isMove = moveJoyStick.GetIsJoyStickTouch();
            if (isMove)
            {
                // 공격 조준중이 아니라면
                if(!isAttackAiming || !isSkillAiming)
                {
                    // 이동방향으로 캐릭터를 회전
                    transform.eulerAngles = moveJoyStick.GetEulerAngles();
                }
                
                // 캐릭터 위치를 움직임
                _player.Move();
            }

            // 달리는 애니메이션 실행
            _playerAnimation.Move(isMove);
        }
       
    }
    
    /**
     *  @brief      공격 행동과 애니메이션을 실행하는 함수입니다.
     */
    private void Attack()
    {
        // 공격 실행
        _player.Attack();

        // 애니메이션 실행
        _playerAnimation.Attack();
    }

    /**
     * @brief       캐릭터 조작에 필요한 조이스틱들을 초기화 합니다.
     */
    private void JoyStickInitialize()
    {
        // 플레이어 팀 정보
        ETeamInfo teamInfo = GameManager._instance.GetTeamInfo();

        // 이동에 대한 조이스틱을 찾음
        GameObject moveJoyStickObject = GameObject.FindWithTag("Move");
        if (moveJoyStickObject)
        {
            // 조이스틱 컴포넌트 설정
            moveJoyStick = moveJoyStickObject.GetComponent<VirtualJoyStick>();

            // 팀 정보에 맞게 캐릭터의 이동 방향을 설정함
            moveJoyStick.SetMoveDirection(teamInfo);
        }
        else
        {
            Debug.LogError("Have not Move JoyStick reference");
        }

        // 일반 공격에 대한 조이스틱을 찾음
        GameObject attackJoyStickObject = GameObject.FindWithTag("Attack");
        if (attackJoyStickObject)
        {
            // 조이스틱 컴포넌트 설정
            attackJoyStick = attackJoyStickObject.GetComponent<VirtualJoyStick>();

            // 일반 공격시 호출 될 함수 등록
            attackJoyStick.OnDragEnd += Attack;

            // 팀 정보에 맞게 캐릭터의 회전 방향을 설정함
            attackJoyStick.SetMoveDirection(teamInfo);
        }
        else
        {
            Debug.LogError("Have not Attack JoyStick reference");
        }

        // 스킬 공격에 대한 조이스틱을 찾음
        GameObject skillJoyStickObject = GameObject.FindWithTag("Skill");
        if (skillJoyStickObject)
        {
            // 조이스틱 컴포넌트 설정
            skillJoyStick = skillJoyStickObject.GetComponent<VirtualJoyStick>();

            // 특수 공격시 호출 될 함수 등록
            skillJoyStick.OnDragEnd += _player.UseSkill;

            // 팀 정보에 맞게 캐릭터의 회전 방향을 설정함
            skillJoyStick.SetMoveDirection(teamInfo);
        }
        else
        {
            Debug.LogError("Have not Skill JoyStick reference");
        }
    }
}
