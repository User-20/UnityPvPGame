using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Xml;

using Photon.Pun;
using Photon.Realtime;

/**
 * @enum       플레이어의 팀 정보
 */
public enum ETeamInfo
{
    _Red,
    _Blue
};


/**
 * @brief       게임의 레벨을 로드하고 플레이어의 입장 및 퇴장을 관리합니다.
 * 
 * @todo        캐릭터가 많아 질 수 있으므로 playerAssetName 을 동적으로 결정 되도록 변경해야 함
 */
public class GameManager : MonoBehaviourPunCallbacks
{
    /// bool을 정수형으로 쓰기 위함
    private const byte TRUE = 1;

    static public GameManager _instance;

    /// 플레이어의 진영 정보
    static private ETeamInfo teamInfo;

    /// 생성될 플레이어 에셋 이름
    private string playerAssetName = "Prefab_Army";

    [SerializeField]
    /// 디버깅용 변수
    private bool we_need_log = false;

    /// 총알 및 발사체 정보 (이름, 데미지 등)
    private Dictionary<string, ItemInfo> itemTable;

    private void Awake()
    {
        // 씬이 변하더라도 객체를 제거하지 않음
        DontDestroyOnLoad(gameObject);

        _instance = this;

        // item xml 파일 Load
        XmlLoader xmlLoader = new XmlLoader();
        itemTable = xmlLoader.LoadItemTableXml();

        // 현재 게임룸에 접속한 플레이어의 수
        int playerCount = PhotonNetwork.CurrentRoom.PlayerCount;

        // 홀수 번째 들어온 사람은 Red팀
        // 짝수 번째 들어온 사람은 Blue팀
        teamInfo = (playerCount & 1) == TRUE ? ETeamInfo._Red : ETeamInfo._Blue;
    }

    /**
     * @brief       게임을 나가면 Photon에서 함수가 콜백 됩니다.
     */
    public override void OnLeftRoom()
    {
        // 다시 로비 Secne을 로드함
        SceneManager.LoadScene(0);
    }

    /**
     * @brief       Photon cloud에 게임룸을 나갔다고 알려 줍니다.
     */
    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    /**
     * @brief       맵을 Load 합니다.
     */
    public void LoadArena()
    {
        if(!PhotonNetwork.IsMasterClient)
        {
            Debug.LogError("we are not the Master Client");
        }

        // 맵을 동기화 시키기 위해 마스터클라이언트가 로드함
        PhotonNetwork.LoadLevel("GameScene");
    }


    /**
     * @brief       플레이어가 퇴장하면 Photon 에서 함수가 콜백 됩니다.
     * 
     * @param       otherPlayer       플레이어 객체 (Photon 전용)
     * 
     * @todo        플레이어 퇴장시 인원이 맞지 않다면 게임 종료
     */

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.LogFormat("OnPlayerLeftRoom() {0}", otherPlayer.NickName);

        if(PhotonNetwork.IsMasterClient)
        {
            if(we_need_log)
            {
                Debug.LogFormat("OnPlayerLeftRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient);
            }
        }
    }

    /**
     * @brief       팀 진영을 반환합니다
     * 
     * @return      teamInfo
     */
    public ETeamInfo GetTeamInfo()
    {
        return teamInfo;
    }

    /**
     * @brief       팀 진영을 변경 합니다.
     * 
     * @param      _info        변경할 팀 정보
     */
    public void SetTeamInfo(ETeamInfo _info)
    {
        teamInfo = _info;
    }


    /**
     * @brief       플레이어 prefab object 이름을 반환 합니다.
     * 
     * @return      playerAssetName
     */
    public string GetPlayerAssetName()
    {
        return playerAssetName;
    }

    /**
     * @brief       총알, 폭탄 등 데미지 연산이 필요한 객체의 정보(데미지값, 이름 등)를 반환합니다.
     * 
     * @param       itemName            찾고자 하는 Item 이름
     * @return      playerAssetName
     */
    public ItemInfo GetItemInfo(string itemName)
    {
        if (itemTable.ContainsKey(itemName))
        {
            return itemTable[itemName];
        }
        return null;
    }

}

/**
 * @brief       xml 파일을 Load하는 클래스 입니다.
 */
class XmlLoader
{
    /// xml 파일이 있는 폴더 경로
    private const string PATH = "XmlFile/";
    /// xml 파일 이름
    private const string ITEM_TABLE_FILE_NAME = "ItemTable";

    /**
     * @brief       bullet 등 데미지 연산이 필요한 오브젝트의 xml 파일을 Load한 후 Dictionary 자료형을 반환하는 함수 입니다.
     * 
     * @return      xml 데이터를 담은 Dictionary
     */
    public Dictionary<string, ItemInfo> LoadItemTableXml()
    {
        Dictionary<string, ItemInfo> table = new Dictionary<string, ItemInfo>();

        string file = PATH + ITEM_TABLE_FILE_NAME;
        // 텍스트 에셋으로 가지고온 xml 파일
        TextAsset textAsset = (TextAsset)Resources.Load(file);

        // 텍스트로 가져온 파일을 XmlDocument로 변환
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(textAsset.text);

        // 최상위 노드를 가져옴
        XmlNodeList xmlNodes = xmlDoc.SelectNodes("ItemAttribute/Item");
        
        // 자식노드들을 탐색
        foreach (XmlNode node in xmlNodes)
        {
            ItemInfo itemInfo = new ItemInfo();
            // item의 이름을 가져옴
            string name = node.SelectSingleNode("ItemName").InnerText;

            // Dictionary에 저장된 데이터가 없다면
            if (!table.ContainsKey(name))
            {
                itemInfo.itemName = name;
                // item의 데미지 값을 가져옴
                itemInfo.demage = System.Convert.ToInt32(node.SelectSingleNode("Demage").InnerText);

                // Dictionary에 item 을 저장함
                table[name] = itemInfo;
            }
        }

        return table;
    }
}
