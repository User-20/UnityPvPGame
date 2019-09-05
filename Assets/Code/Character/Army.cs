using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Photon.Pun;
using Photon.Realtime;

/**
 *  @brief  플레이어 조작할 실제 캐릭터 입니다.
 */
public class Army : UPlayer
{
    /// 데미지가 들어와야 하는 오브젝트의 tag 명
    private const string TAG_DEMAGE = "Demage";

    /// 스킬 조이스틱 드래그 거리에 3D 월드에 표현하기 위한 비율
    private const float RATIO = 10.0f;
    
    [SerializeField]
    /// 캐릭터가 일반 공격에 사용할 prefab object 이름
    public const string bulletName = "Bullet";

    [SerializeField]
    /// 캐릭터가 특수 공격에 사용할 prefab object 이름
    public const string SkillName = "Bomb";

    /// 공격 1회에 발사되는 총알 수
    public int bulletMaxCount = 1;

    /// 연발 사격에 총알 한발 씩 쏘는 딜레이 시간
    public float seriesAttackTime = 0.0f;

    /// 총알 장전 갯수
    public int bulletLoadedCount = 5;

    /// 총알이 발사될 위치
    [SerializeField]
    private GameObject muzzle;

    /// 공격 방향을 보여줄 이미지
    [SerializeField]
    private GameObject attackDirection;

    /// 투척무기(특수공격)가 떨어질 위치
    [SerializeField]
    private GameObject skillPosition;

    public override void Initialize()
    {
        base.Initialize();

        // 캐릭터 체력
        //hp = 10;
        _playerState.hp = 10;
        // 캐릭터 이동속도
        //moveSpeed = 5.0f;
        _playerState.moveSpeed = 5.0f;

        // 공격 및 스킬 조준하는 UI들을 비활성화
        if (attackDirection)
        {
            attackDirection.SetActive(false);
        }

        if (skillPosition)
        {
            skillPosition.SetActive(false);
        }

        // 체력 프로그래스바 초기화
        hpProgressBar.Initialize(_playerState.hp);
    }

    // Start is called before the first frame update
    void Start()
    {
        // 초기화
        Initialize();
    }

    /* 일반 공격 조이스틱 드래그 중일 때 방향을 보여줄 UI를 활성화 시킴 */
    public override void AttackAiming()
    {
        if (attackDirection)
        {
            // 공격 방향을 보여주는 UI를 킴
            attackDirection.SetActive(true);
        }
    }

    public override void SkillAiming(float targetDistance)
    {
        if (skillPosition)
        {
            // 떨어질 위치의 UI를 표시함
            skillPosition.SetActive(true);
            // 조이스틱 움직임에 따라 UI가 움직일 방향 및 거리를 정함
            Vector3 direction = transform.forward * (targetDistance / RATIO);
            skillPosition.transform.position = (transform.position + direction);
        }
    }

    /* bullet을 발사하고 다른 플레이어들에게 알려줌 */
    public override void Attack()
    {
        if (attackDirection)
        {
            // 공격 방향을 보여주는 UI를 끔
            attackDirection.SetActive(false);
        }

        // 발사 위치
        Vector3 muzzlePosition = muzzle.transform.position;
        // 날아갈 방향
        Vector3 direction = transform.forward;

        // 코루틴 시작
        StartCoroutine(CoroutineCreateBullet(muzzlePosition, direction));

        // 리모트 플레이어들에게 실행
        photonView.RPC("RPC_Attack", RpcTarget.Others, muzzlePosition, direction);
    }

    /**
     * @brief       리모트 플레이어들에게 공격을 했다고 알려줍니다.
     * 
     * @param       muzzlePosition      발사될 위치
     * @param       direction           날아가는 방향
     */
    [PunRPC]
    private void RPC_Attack(Vector3 muzzlePosition, Vector3 direction)
    {
        StartCoroutine(CoroutineCreateBullet(muzzlePosition, direction));
    }

    /* 스킬(폭탄)을 발사하고 다른 플레이어들에게 알려줌 */
    public override void UseSkill()
    {
        if (skillPosition)
        {
            // 타겟 지점 UI를 끔
            skillPosition.SetActive(false);
        }

        // 발사 위치
        Vector3 muzzlePosition = muzzle.transform.position;
        // 날아갈 방향
        Vector3 direction = transform.forward;
        // 목표 지점의 위치
        Vector3 targetPosition = skillPosition.transform.position;

        StartCoroutine(CoroutineCreateBomb(muzzlePosition, targetPosition, direction));

        // 리모트 플레이어들에게 실행
        photonView.RPC("RPC_UseSkill", RpcTarget.Others, muzzlePosition, targetPosition, direction);
    }

    /**
     * @brief       리모트 플레이어들에게 특수 공격을 했다고 알려줍니다.
     * 
     * @param       muzzlePosition      발사될 위치
     * @param       targetPosition      특수 공격이 떨어질 위치
     * @param       direction           날아가는 방향
     */
    [PunRPC]
    private void RPC_UseSkill(Vector3 muzzlePosition, Vector3 targetPosition, Vector3 direction)
    {
        StartCoroutine(CoroutineCreateBomb(muzzlePosition, targetPosition, direction));
    }

    /* 캐릭터 이동 함수 */
    public override void Move()
    {
        base.Move();
    }

    /**
     * @brief       총알 발사 후 지정된 방향으로 날아가는 코루틴 입니다.
     * 
     * @param       muzzlePosition      발사 위치
     * @param       direction           날아가는 방향
     */
    IEnumerator CoroutineCreateBullet(Vector3 muzzlePosition, Vector3 direction)
    {
        int count = 0;
        while (count < bulletMaxCount)
        {
            // bullet을 objectPool 에서 꺼냄
            GameObject bulletObject = ObjectPoolManager.GetInstance().PopFromPool(bulletName);
            if (bulletObject && muzzle)
            {
                // pool에서 반환받은 bullet의 위치를 설정해줌
                bulletObject.transform.position = muzzlePosition + direction;
                // 현재 bullet 에셋을 기준으로 전방 방향은 up
                bulletObject.transform.up = direction;
                // 눈에보이도록 활성화 시킴
                bulletObject.SetActive(true);

                BulletMove _bullet = bulletObject.GetComponent<BulletMove>();
                _bullet.owner = _playerState.nickName;
            }

            // 발사한 총알 개수
            count++;

            // 일정 시간 후 다시 시작
            yield return new WaitForSeconds(seriesAttackTime);
        }
    }

    /**
     * @brief       폭탄(특수공격) 발사 하는 코루틴 입니다.
     * 
     * @param       muzzlePosition      발사 위치
     * @param       targetPosition      폭탄이 떨어질 타겟 지점
     * @param       direction           날아가는 방향
     */
    IEnumerator CoroutineCreateBomb(Vector3 muzzlePosition, Vector3 targetPosition, Vector3 direction)
    {
        // 오브젝트 풀에서 해당 오브젝트를 가져옴
        GameObject item = ObjectPoolManager.GetInstance().PopFromPool(SkillName);

        if (item && muzzle)
        {
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

        yield return null;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!photonView.IsMine)
        {
            // 로컬 플레이어가 아니라면 함수 종료
            return;
        }
        
        if (other.tag != TAG_DEMAGE)
        {
            // 충돌 된 오브젝트가 데미지를 줄수 있는게 아니라면 함수 종료
            return;
        }

        // item table.xml에 정의된 오브젝트의 정보를 가져옴
        ItemInfo collisionInfo = GameManager._instance.GetItemInfo(other.gameObject.name);
        if (collisionInfo != null)
        {
            // 오브젝트의 데미지
            int demage = collisionInfo.demage;

            if ((_playerState.hp - demage) <= 0)
            {
                // 내 캐릭터가 체력이 없다면 리스폰 시킴
                photonView.RPC("RPC_SetReswapn", RpcTarget.All, null);
                return;
            }

            // 체력을 깎음
            _playerState.hp -= demage;
            // 다른 클라이언트들에게도 알려줌
            photonView.RPC("RPC_SetGage", RpcTarget.All, demage);
        }
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
        hpProgressBar.SetGage(demage);
    }

    /**
     * @brief       플레이어가 죽은 후 리스폰을 합니다. (모든 클라이언트가 실행)
     */
    [PunRPC]
    private void RPC_SetReswapn()
    {
        GameRuleManager._instance.SetRespawn(gameObject, _playerState.teamInfo);
    }
}
