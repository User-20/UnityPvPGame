using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Photon.Pun;

/// 게임 시작 버튼 클릭시 등록된 함수를 호출해줄 delegate
public delegate void GameStartButtonClickEvent();

/**
 *  @brief      로비 UI에서 게임 시작 및 준비버튼의 Text출력 및 행동을 담당합니다.
 */
public class StartButton : MonoBehaviour
{
    /// 방장에게 보여줄 게임 시작 버튼 Text
    private string GAME_START_TEXT = "게임 시작";

    /// 일반 유저들에게 보여줄 준비 버튼 Text
    private string READY_TEXT = "준비";

    /// 일반 유저들에게 보여줄 준비를 취소할수 있는 버튼 Text
    private string CANCEL_TEXT = "취소";

    /// 버튼에 들어갈 텍스트 컴포넌트
    private Text buttonText;

    /// 준비 상태인지?
    private bool isReady = false;

    /// 델리게이트 함수를 실행할 이벤트 핸들러
    public event GameStartButtonClickEvent startButtonClickEvent;

    // Start is called before the first frame update
    void Start()
    {
        // 버튼에 출력할 텍스트 컴포넌트
        buttonText = transform.GetChild(0).GetComponent<Text>();

        if(buttonText)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                // 방장이라면 게임 시작 텍스트 출력
                buttonText.text = GAME_START_TEXT;
            }
            else
            {
                // 일반 유저라면 준비 텍스트 출력
                buttonText.text = READY_TEXT;
            }
        }
    }

    /**
     *  @brief      버튼을 클릭시 맵을 로드하거나 클릭 이벤트를 전파합니다.
     *  
     *  @todo       준비 취소 Text가 출력 되지 않음
     */
    public void OnButtonClick()
    {
        // 방장이라면
        if (PhotonNetwork.IsMasterClient)
        {
            // 게임 씬 로드
            GameManager._instance.LoadArena();
            return;
        }

        if(isReady)
        {
            // 준비 상태이면 취소 텍스트 출력
            buttonText.text = CANCEL_TEXT;
            isReady = true;
        }
        else
        {
            // 준비 상태가 아니면 이면 준비 텍스트 출력
            buttonText.text = READY_TEXT;
            isReady = false;
        }

        // 이벤트 전파
        startButtonClickEvent();
    }

    /**
     *  @brief      현재 준비 중인지 상태를 반환합니다.
     */
    public bool GetIsReady()
    {
        return isReady;
    }
}
