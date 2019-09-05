using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Photon.Pun;
using Photon.Realtime;

/**
 * @brief       플레이어 생성 및 게임 종료시 행동을 담당합니다.
 */

public class GameRuleManager : MonoBehaviourPunCallbacks
{
    static public GameRuleManager _instance;

    /// 게임 승리시 보여줄 Text
    private const string VICTORY_TEXT = "승리";
    /// 게임 패배시 보여줄 Text
    private const string LOSE_TEXT = "패배";
    /// 방어 건물 prefab object 이름
    private const string TOWER_ASSET_NAME = "cannon_upgrade_01";
    /// 방어 건물 prefab object 가 존재하는 폴더명
    private const string TOWER_ASSET_PATH = "BattleCannon/Prefabs";

    /// 플레이어의 진영 정보
    private ETeamInfo teamInfo;

    [SerializeField]
    /// 캐릭터 죽은 후 리스폰 지연 시간
    private float respawnDelay = 0.0f;

    /// 캐릭터가 생성될 위치들 (스폰 위치)
    public Transform redPosition;
    public Transform bluePosition;

    /// red 팀 타워 객체
    public GameObject redTower;
    /// blue 팀 타워 객체
    public GameObject blueTower;

    /// 플레이어 스폰 위치들을 담아둘 변수
    private List<Transform> startingList = new List<Transform>();

    /// 각 팀 지켜야 할 타워 컴포넌트를 담아둘 변수
    private List<Tower> teamTowers = new List<Tower>();

    [SerializeField]
    /// 승리 및 패배 텍스트 출력할 변수
    private GameObject victoryTextObject;

    // Start is called before the first frame update
    void Start()
    {
        _instance = this;

        // 현재 플레이어의 팀 정보
        teamInfo = GameManager._instance.GetTeamInfo();

        // 각 팀 스타팅 위치를 담음
        startingList.Add(redPosition);
        startingList.Add(bluePosition);

        // 캐릭터 생성
        CreatePlayer();

        // 인스펙터에서 설정하지 않았다면
        if(victoryTextObject == null)
        {
            // victoryText 를 찾음
            victoryTextObject = GameObject.Find("ExitGameText");

            // 존재하지 않는다면
            if(victoryTextObject == null)
            {
                // 로그에러 출력
                Debug.LogError("Have not (victory Text) gameObject");
            }
        }

        // 게임 종료시 보여줄 Text 숨김
        victoryTextObject.SetActive(false);
    }

    /**
     * @brief       게임에 접속한 모든 클라이언트에 플레이어를 생성합니다.
     */
    private void CreatePlayer()
    {
        if (startingList.Count <= 0)
        {
            Debug.LogError("No start position information");
        }

        // 진영 정보로 스폰 위치를 받음
        Vector3 _starting = startingList[(int)teamInfo].position;

        // 플레이어 캐릭터 생성
        GameObject playerObject = PhotonNetwork.Instantiate(GameManager._instance.GetPlayerAssetName(), _starting, Quaternion.identity, 0);

        // 카메라를 모두 가져옴 (main Camera 및 UI Camera)
        int length = Camera.allCamerasCount;
        Camera[] cameras = Camera.allCameras;

        // 카메라에 타겟을 지정해줌
        for (int i = 0; i < length; i++)
        {
            FollowCamera _followCamera = cameras[i].GetComponent<FollowCamera>();
            _followCamera.SetTarget(playerObject, teamInfo);
        }
        
        // blue 팀일 경우
        if (teamInfo == ETeamInfo._Blue)
        {
            // 캐릭터의 방향을 반전 시킴 (위쪽에서 아래 방향으로 내려 와야 하기 때문)
            Vector3 direction = startingList[(int)teamInfo].forward;
            playerObject.transform.forward = direction;
        }
    }

    /**
     * @brief       캐릭터가 죽을 시 리스폰을 시킵니다.
     * 
     * @param       playerObject        리스폰 시킬 플레이어 게임오브젝트
     * @param       _teamInfo           리스폰 시킬 플레이어의 팀 정보
     */
    public void SetRespawn(GameObject playerObject, ETeamInfo _teamInfo)
    {
        StartCoroutine(CouroutinRespawn(playerObject, _teamInfo));
    }

    /**
     * @brief       게임 종료시 Text를 출력합니다.
     * 
     * @param       _teamInfo           플레이어의 팀 정보
     */
    public void ExitGameText(ETeamInfo _teamInfo)
    {
        photonView.RPC("RPC_ExitGameText", RpcTarget.All, _teamInfo);
    }

    /**
     * @brief       게임 종료시 Text를 출력할 RPC 함수입니다.(모든 클라이언트 실행)
     * 
     * @param       _teamInfo           플레이어의 팀 정보
     */
    [PunRPC]
    private void RPC_ExitGameText(ETeamInfo _teamInfo)
    {
        /*
         * 타워의 체력이 없으면 타워가 속해 있는 팀 정보를 전달 해주기 때문에
         * 현재 내 팀 진영 정보와 타워가 속해 있는 진영 정보가 같다면 패배
         * 팀 진영 정보가 다르다면 승리
         */
        string text = teamInfo == _teamInfo ? LOSE_TEXT : VICTORY_TEXT;

        // 코루틴 실행
        StartCoroutine(CouroutinExitGame(text));
    }

    /**
     * @brief       캐릭터가 죽을 시 리스폰을 하는 코루틴 입니다.
     * 
     * @param       playerObject        리스폰 시킬 플레이어 게임오브젝트
     * @param       _teamInfo           리스폰 시킬 플레이어의 팀 정보
     */
    IEnumerator CouroutinRespawn(GameObject playerObject, ETeamInfo _teamInfo)
    {
        // 플레이어 오브젝트가 없다면 코루틴 종료
        if (!playerObject)
        {
            Debug.LogError("Have not Player Information");
            yield break;
        }

        // 캐릭터를 잠시 비활성화 시킴
        playerObject.SetActive(false);

        // 설정된 리스폰 딜레이 시간만큼 기다림
        yield return new WaitForSeconds(respawnDelay);

        // 플레이어 오브젝트의 최상위 부모를 가져옴
        Transform playerTransform = playerObject.transform;

        // 위치를 스타팅 지점으로 초기화
        playerTransform.position = startingList[(int)_teamInfo].position;
        // 캐릭터 회전값 초기화
        playerTransform.forward = startingList[(int)_teamInfo].forward;

        // 플레이어 컴포넌트를 가져옴
        UPlayer _playerComponent = playerObject.GetComponent<UPlayer>();
        if (_playerComponent)
        {
            // 플레이어의 체력 등 필요한 정보를 초기화
            _playerComponent.Initialize();
        }

        // 리스폰 시간이 지났다면 다시 활성화
        playerObject.SetActive(true);

        yield break;
    }

    /**
     * @brief       게임 종료시 Text를 출력할 코루틴 입니다.
     * 
     * @param       printfText           화면에 출력할 Text
     */
    IEnumerator CouroutinExitGame(string printfText)
    {
        // 존재하지 않는다면
        if (victoryTextObject == null)
        {
            // 로그에러 출력
            Debug.LogError("Have not (victoryText) gameObject");

            // 출력을 위한 UI text object가 없으므로 2초 기다린 후
            yield return new WaitForSeconds(2.0f);

            // 방을 나감
            GameManager._instance.LeaveRoom();

            yield break;
        }

        Text victoryText = victoryTextObject.GetComponentInChildren<Text>();
        // 텍스트 컴포넌트가 존재한다면
        if (victoryText)
        {
            // 컴포넌트 활성화
            victoryTextObject.SetActive(true);

            // 텍스트 출력
            victoryText.text = printfText;

            // 일정 시간 대기
            yield return new WaitForSeconds(4.0f);
        }
        else
        {
            yield return new WaitForSeconds(2.0f);
        }

        // 방을 나감
        GameManager._instance.LeaveRoom();

    }
}
