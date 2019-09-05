using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;

/**
 *  @brief  기지를 방어할 실제 건물 오브젝트(대포) 입니다.
 */

public class TeamCannon : Tower
{
    /// 플레이어만 감지하기 위한 tag
    private const string TAG_PLAYER = "Player";

    ///  공격에 사용할 prefab object 이름
    [SerializeField]
    private const string POOL_ITEM_NAME = "Cannon_ball";

    /// 공격하기 전 준비시간 (딜레이)
    private float DELAY_TIME = 2.0f;

    [SerializeField]
    /// 발사 위치
    private Transform muzzle;

    /// 코루틴 실행 중인지?
    bool isRunning = false;

    // Start is called before the first frame update
    void Start()
    {
        // 초기화
        Initialize();
    }

    public override void Initialize()
    {
        base.Initialize();
    }

    /**
     * @brief       공격을 실행하는 함수입니다. (RPC 실행)
     * 
     * @param       targetPosition      폭탄이 떨어질 목표 지점
     */
    private void CannonAttack(Vector3 targetPosition)
    {
        // 모든 클라이언트가 실행하도록 함
        photonView.RPC("RPC_Attack", RpcTarget.All, targetPosition);
    }

    private void OnTriggerStay(Collider other)
    {
        // 로컬플레이어가 아니라면 함수 종료
        if(!photonView.IsMine)
        {
            return;
        }

        // 플레이어만 감지 함
        if (other.tag != TAG_PLAYER)
        {
            return;
        }

        // 현재 감지된 플레이어의 정보를 가져옴
        UPlayer target = other.gameObject.GetComponentInChildren<UPlayer>();

        // 감지된 플레이어가 같은 팀이라면 함수 종료
        if(_playerState.teamInfo == target.GetData().teamInfo)
        {
            return;
        }

        // 감지된 오브젝트로 바라보는 방향벡터를 구함
        Vector3 direction = (other.transform.position) - (transform.position);
        transform.forward = direction.normalized;

        // 코루틴이 실행 중이 아니라면
        if (!isRunning)
        {
            isRunning = true;
            // 공격 시작
            CannonAttack(other.transform.position);
        }
    }

    public void SetGage(int demage)
    {
        // 로컬플레이어가 아니라면 함수 종료
        if(!photonView.IsMine)
        {
            return;
        }

        // 모든 클라이언트 실행
        photonView.RPC("RPC_SetGage", RpcTarget.All, demage);

        // 체력이 없다면 게임 종료
        if (_playerState.hp <= 0)
        {
            GameRuleManager._instance.ExitGameText(_playerState.teamInfo);
            return;
        }
    }

    /**
     * @brief       공격을 실행하는 함수입니다. (모든 클라이언트 실행)
     * 
     * @param       targetPosition      폭탄이 떨어질 목표 지점
     */
    [PunRPC]
    private void RPC_Attack(Vector3 targetPosition)
    {
        StartCoroutine(CoroutineCreateBomb(targetPosition));
    }

    /**
     * @brief       체력 프로그래스바를 갱신합니다. (모든 클라이언트가 실행)
     * 
     * 
     * @param       demage      플레이어가 받은 데미지
     */
    [PunRPC]
    private void RPC_SetGage(int demage)
    {
        _playerState.hp -= demage;
        hpProgressBar.SetGage(demage);
    }

    /**
     * @brief       공격을 실행 하는 코루틴 입니다.
     * 
     * @param       targetPosition      폭탄이 떨어질 목표 지점
     */
    IEnumerator CoroutineCreateBomb(Vector3 targetPosition)
    {
        // 쏘기전 잠시 기다림
        yield return new WaitForSeconds(DELAY_TIME);

        // 오브젝트 풀에서 해당 오브젝트를 가져옴
        GameObject item = ObjectPoolManager.GetInstance().PopFromPool(POOL_ITEM_NAME);
        if (item && muzzle)
        {
            // 발사 위치를 받음
            Vector3 muzzlePosition = muzzle.position;

            // pool에서 반환받은 bullet의 위치를 설정해줌
            item.transform.position = muzzlePosition + transform.forward;

            // 눈에보이도록 활성화 시킴
            item.SetActive(true);

            // 포물선 연산하는 함수를 가져오기 위해 BombMove 스크립트를 가져옴 
            BombMove bomb = item.GetComponent<BombMove>();

            // 소유자 설정
            bomb.owner = _playerState.nickName;
            // 포물선 연산 시작
            bomb.Shoot(muzzlePosition, targetPosition);
        }

        // 코루틴 종료 flag
        isRunning = false;
    }
}
