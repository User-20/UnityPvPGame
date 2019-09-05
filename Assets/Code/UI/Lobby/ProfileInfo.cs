using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Photon.Pun;

/**
 *  @brief      캐릭터의 프로필 UI를 담당하는 클래스입니다.
 */

public class ProfileInfo : MonoBehaviour
{
    /// 프로필에 보여줄 Red 팀 Text
    private const string RED_TEAP = "Red Team";

    /// 프로필에 보여줄 blue 팀 Text
    private const string BLUE_TEAP = "Blue Team";

    [SerializeField]
    /// 캐릭터 이미지
    public Image characterImage;

    [SerializeField]
    /// 닉네임 배경
    private GameObject nickNameBackground;

    [SerializeField]
    /// 캐릭터 닉네임
    private Text nickName;

    [SerializeField]
    /// 팀 진영 명 (red, blue)
    private Text teamName;

    // Start is called before the first frame update
    void Start()
    {
        // 이미지 컴포넌트 설정
        characterImage = transform.Find("CharacterImage").GetComponent<Image>();

        // 캐릭터 닉네임 배경
        nickNameBackground = transform.Find("Background").gameObject;

        nickNameBackground.SetActive(false);

        // 캐릭터 닉네임 Text 컴포넌트 설정
        nickName = nickNameBackground.GetComponentInChildren<Text>();

        // 팀 이름을 보여줄 Text 컴포넌트 설정
        teamName = transform.Find("TeamName").GetComponent<Text>();
    }

    /**
     *  @brief      캐릭터의 프로필 정보를 UI에 설정하는 함수입니다.
     *  
     *  @param      teamInfo            플레이어의 팀 정보
     *  @param      characterName       캐릭터 prefab 에셋 이름
     *  @param      _nickName           캐릭터 닉네임
     */
    public void SetProfile(ETeamInfo teamInfo, string characterName, string _nickName = null)
    {
        if(_nickName != null)
        {
            nickNameBackground.SetActive(true);

            // 닉네임 출력
            nickName.text = _nickName;
        }

        // 캐릭터 이름을 key로 이미지를 들고옴
        Sprite playerImage = TextureManager.GetInstance().GetTexture(characterName);

        // 이미지가 존재하는지 검사
        if (playerImage)
        {
            // 프로필 이미지 셋팅
            characterImage.enabled = true;
            characterImage.sprite = playerImage;

            // 팀에 따라 profile에 보여줄 Text를 셋팅함
            if (teamInfo == ETeamInfo._Red)
            {
                // 팀 진영 텍스트 출력
                teamName.text = RED_TEAP;

                // 텍스트 색상 설정
                teamName.color = Color.red;
            }
            else
            {
                // 팀 진영 텍스트 출력
                teamName.text = BLUE_TEAP;

                // 텍스트 색상 설정
                teamName.color = Color.blue;
            }
        }
    }
}
