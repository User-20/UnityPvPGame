using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Photon.Pun;
using Photon.Realtime;

/**
 * @brief   로비 화면의 UI 출력하는 씬 입니다.
 * 
 * @todo    1:1이 아닌 2:2, 3:3 이 생길 경우 방어 타워를 관리할 자료구조(array 등)을 사용해야함
 */
public class LobbyUI : MonoBehaviourPunCallbacks
{
    [SerializeField]
    /// Red 팀 프로필 정보
    private ProfileInfo redTeamProfileInfo;
    
    [SerializeField]
    /// blue 팀 프로필 정보
    private ProfileInfo blueTeamProfileInfo;

    [SerializeField]
    /// 게임 시작 버튼 또는 준비 버튼
    private StartButton gameStartButton;

    /// 팀 진영 정보
    private ETeamInfo teamInfo;

    // Start is called before the first frame update
    void Start()
    {
        // Red팀 게임오브젝트를 가져옴
        GameObject redTeam = transform.Find("RedTeam").gameObject;
        // 게임오브젝트의 컴포넌트를 가져옴
        redTeamProfileInfo = redTeam.transform.GetChild(0).GetComponent<ProfileInfo>();

        // blue팀 게임오브젝트를 가져옴
        GameObject blueTeam = transform.Find("BlueTeam").gameObject;
        // 게임오브젝트의 컴포넌트를 가져옴
        blueTeamProfileInfo = blueTeam.transform.GetChild(0).GetComponent<ProfileInfo>();

        // 버튼(게임 시작, 준비) 컴포넌트를 가져옴
        gameStartButton = transform.Find("StartButton").GetComponent<StartButton>();

        // 이벤트 리스너 등록
        gameStartButton.startButtonClickEvent += OnButtonClick;

        // 팀 진영을 받아옴
        teamInfo = GameManager._instance.GetTeamInfo();

        // 팀에 맞게 프로필을 설정함
        SetProfile(teamInfo, GameManager._instance.GetPlayerAssetName(), PhotonNetwork.NickName);

        // 현재 나의 정보를 마스터 클라이언트에게 전달
        photonView.RPC("RPC_SetProfile", RpcTarget.MasterClient, teamInfo, GameManager._instance.GetPlayerAssetName(), PhotonNetwork.NickName);
    }

    /**
     *  @brief      준비 버튼을 누르면 콜백 되는 함수입니다.
     *  
     *  @todo       방장이 아닌 플레이어가 준비버튼을 누르지 않아도 방장이 게임시작 버튼을 누르면 게임이 시작됨
     */
    private void OnButtonClick()
    {
        if(!photonView.IsMine)
        {
            return;
        }


        // 준비 버튼을 눌렀다면
        if (gameStartButton.GetIsReady())
        {
            // 모든 클라이언트에게 알려줌
            // 이 부분에서 준비 했다는 연출이 들어가야 함 (현재 작업 되지 않았음)
            photonView.RPC("RPC_SetProfile", RpcTarget.All, teamInfo, GameManager._instance.GetPlayerAssetName());
        }
    }

    /**
     *  @brief      캐릭터의 프로필 정보를 팀 위치에 맞게 UI를 설정하는 함수입니다.
     *  
     *  @param      teamInfo            플레이어의 팀 정보
     *  @param      characterName       캐릭터 prefab 에셋 이름
     *  @param      _nickName           캐릭터 닉네임
     */
    private void SetProfile(ETeamInfo teamInfo, string characterName, string _nickName)
    {
        if (teamInfo == ETeamInfo._Red)
        {
            // Red팀 위치에 프로필 UI를 설정
            redTeamProfileInfo.SetProfile(teamInfo, characterName, _nickName);
        }
        else
        {
            // blue팀 위치에 프로필 UI를 설정
            blueTeamProfileInfo.SetProfile(teamInfo, characterName, _nickName);
        }
    }

    /**
     *  @brief      캐릭터의 프로필 정보를 다른 플레이어들에게 알려주는 RPC 함수입니다.
     *  
     *  @param      teamInfo            플레이어의 팀 정보
     *  @param      characterName       캐릭터 prefab 에셋 이름
     *  @param      _nickName           캐릭터 닉네임
     */
    [PunRPC]
    private void RPC_SetProfile(ETeamInfo teamInfo, string characterName, string _nickName)
    {
        // 프로필 설정
        SetProfile(teamInfo, characterName, _nickName);

        // 마스터 클라이언트라면 (방장이라면)
        if (PhotonNetwork.IsMasterClient)
        {
            // 다른 클라이언트들에게도 나의 프로필 정보를 알려줌
            photonView.RPC("RPC_SetProfile", RpcTarget.Others, GameManager._instance.GetTeamInfo()
                                                         , GameManager._instance.GetPlayerAssetName()
                                                         ,  PhotonNetwork.NickName);
        }

    }
}
