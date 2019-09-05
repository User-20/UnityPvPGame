using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Photon.Pun;
using Photon.Realtime;

using PlayFab;
using PlayFab.ClientModels;

/**
 *  @biref      로그인, 회원가입 클라우드 서버 연결을 담당하는 클래스입니다.
 */
public class PhotonLauncher : MonoBehaviourPunCallbacks
{
    /// 로비 씬 이름
    private const string LOBBY_SCENE_NAME = "Lobby";

    /// 게임 버전
    private const string gameVersion = "1.0";

    /// 로그인 성공시 보여줄 Text
    private const string LOGIN_SUCCESS_TEXT = "Success!";

    /// 회원가입 성공시 보여줄 Text
    private const string REGISTER_SUCCESS_TEXT = "Register Success!";

    [SerializeField]
    /// InputField와 Play Button을 가지고 있는 Panel
    private GameObject controlPanel;

    /// Id를 저장할 변수
    private string userName = string.Empty;
    /// 비밀번호를 저장할 변수
    private string password = string.Empty;

    [SerializeField]
    /// 로그인, 회원가입 등 반응을 보여줄 Text gameObject
    private GameObject progressLabelObejct;

    /// 로그인, 회원가입 등 반응을 보여줄 TextField
    private Text progressLabel;

    [SerializeField]
    /// 게임룸 최대 입장 수 (인스펙터에서 변경 가능함)
    private byte maxPlayerPerRoom = 4;

    [SerializeField]
    /// 디버깅용 변수
    private bool we_need_log = false;

    /// 현재 포톤 클라우드에 연결이 됐는지? flag
    private bool isConnecting = false;

    
    private void Awake()
    {
        // true 면 Master 클라이언트에서 LoadLevel()을 호출할 수 있음 (레벨 동기화)
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        // 게임 시작 전 Button과 InputField를 보여줌
        controlPanel.SetActive(true);

        // text를 보여줄 게임오브젝트가 있다면
        if(progressLabelObejct)
        {
            // text 컴포넌트를 설정
            progressLabel = progressLabelObejct.GetComponent<Text>();
        }
    }

    /**
     *  @brief      로그인을 담당하는 함수입니다.
     */
    public void Login()
    {
        // 닉네임과 패스워드를 설정
        LoginWithPlayFabRequest request = new LoginWithPlayFabRequest { Username = userName, Password = password };

        // PlayFab 서버로 로그인
        // 로그인 성공 시 OnLoginSuccess 호출
        // 실패 시 OnLoginFailed 호출
        PlayFabClientAPI.LoginWithPlayFab(request, OnLoginSuccess, OnLoginFailed);
    }

    /**
     *  @brief      회원가입을 담당하는 함수입니다.
     */
    public void Register()
    {
        // 닉네임과 패스워드를 설정
        RegisterPlayFabUserRequest request = new RegisterPlayFabUserRequest { Username = userName, Password = password };

        // username 및 email 매개 변수가 모두 필요한지 여부를 지정하는 변수
        // true 면 username 과 email 모두 입력 해야함
        request.RequireBothUsernameAndEmail = false;
        PlayFabClientAPI.RegisterPlayFabUser(request, RegisterSuccess, RegisterFailed);
    }

    /**
     *  @brief      로그인 성공시 불리는 콜백함수입니다.
     *  
     *  @param      result      로그인 결과
     */
    private void OnLoginSuccess(LoginResult result)
    {
        // 로그인 성공 했다는 text 출력
        progressLabel.text = LOGIN_SUCCESS_TEXT;

        // 포톤 클라우드에 연결
        Connect();
    }

    /**
     *  @brief      로그인 실패시 불리는 콜백함수입니다.
     *  
     *  @param      error      로그인 실패에 대한 error 정보
     */
    private void OnLoginFailed(PlayFabError error)
    {
        // 로그인 실패 text 출력
        progressLabel.text = error.GenerateErrorReport();
    }

    /**
     *  @brief      회원가입 성공시 불리는 콜백함수입니다.
     *  
     *  @param      result      회원가입 결과에 대한 정보
     */
    private void RegisterSuccess(RegisterPlayFabUserResult result)
    {
        // 플레이팹 회원가입 성공 Text 출력
        progressLabel.text = REGISTER_SUCCESS_TEXT;
    }

    /**
     *  @brief      회원가입 실패시 불리는 콜백함수입니다.
     *  
     *  @param      error      회원가입 실패 결과에 대한 error 정보
     */
    private void RegisterFailed(PlayFabError error)
    {
        // 플레이팹 회원가입 실패 Text 출력
        progressLabel.text = error.GenerateErrorReport();
    }

    /**
     *  @brief      포톤 클라우드에 연결하고 게임 방에 접속을 합니다.
     */
    public void Connect()
    {
        // 방에 들어갔는지 기록
        // 없다면 게임을 나갔을 경우 OnConnectedToMaster() 가 계속 호출되면서 게임이 join 됨
        isConnecting = true;

        // 다른 UI(버튼, InputField 등)들을 숨김
        controlPanel.SetActive(false);

        if (PhotonNetwork.IsConnected)
        {
            // 랜덤으로 방에 접속
            PhotonNetwork.JoinRandomRoom();
        }
        else
        {
            // 게임 버전을 포톤클라우드에 설정
            PhotonNetwork.GameVersion = gameVersion;
            // 포톤 클라우드에 접속
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    /**
     *  @brief      마스터 클라이언트가 접속하면 포톤에서 콜백되는 함수입니다.
     */
    public override void OnConnectedToMaster()
    {
        if(we_need_log)
        {
           Debug.Log("OnConnectedToMaster() was Called");
        }

        if (isConnecting)
        {
            // 랜덤으로 방에 접속
            PhotonNetwork.JoinRandomRoom();
        }
       
    }

    /**
     *  @brief      플레이어가 접속이 끊겼을때 호출되는 함수입니다.
     *  
     *  @param      cause           연결이 어떻게 끊겼는지에 대한 정보
     */
    public override void OnDisconnected(DisconnectCause cause)
    {
        if(we_need_log)
        {
            Debug.LogWarningFormat("OnDisconnected() was Called {0}", cause);
        }

        // 연결이 끊겼으므로 다시 UI들을 보여줌 (Play Button, InputField)
        progressLabelObejct.SetActive(false);
        controlPanel.SetActive(true);
    }

    /**
     *  @brief      랜덤으로 방 접속에 실패시 호출되는 함수입니다.
     *  
     *  @param      returnCode           포톤 클라우드에서 반환하는 코드
     *  @param      message              error 메세지
     */
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        if(we_need_log)
        {
            Debug.Log("OnJoinRandomFailed() was Called. No random room available, create room");
        }
        
        // 게임 방을 새로 생성함
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = maxPlayerPerRoom });
    }

    /**
     *  @brief      게임 방에 접속을 성공하면 호출되는 함수입니다.
     */
    public override void OnJoinedRoom()
    {
        if(we_need_log)
        {
            Debug.Log("OnJoinedRoom() was Called. this Client is in a room");
        }

        if(we_need_log)
        {
            Debug.Log("load the game scene");
        }

        // 로비 씬을 로드
        PhotonNetwork.LoadLevel(LOBBY_SCENE_NAME);
    }

    /**
     *  @brief      유저 Id에 대한 Input을 받는 함수 입니다.
     *  
     *  @param      nickName        유저 Id
     */
    public void UserNameInput(string nickName)
    {
        if (string.IsNullOrEmpty(nickName))
        {
            Debug.LogError("nickName is null or Empty");
            return;
        }

        // Photon에 닉네임을 설정
        PhotonNetwork.NickName = nickName;

        userName = nickName;
    }

    /**
     *  @brief      유저 비밀번호에 대한 Input을 받는 함수 입니다.
     *  
     *  @param      _password        유저 비밀번호
     */
    public void PasswordInput(string _password)
    {
        if (string.IsNullOrEmpty(_password))
        {
            Debug.LogError("password is null or Empty");
            return;
        }

        password = _password;
    }
}
