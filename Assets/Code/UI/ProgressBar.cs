using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Photon.Pun;
using Photon.Realtime;

/**
 *  @brief      체력 등을 UI로 보여줄 프로그래스바 입니다.
 */
public class ProgressBar : MonoBehaviourPunCallbacks
{
    /// UI 카메라 설정
    public string UICameraName = "UICamera";

    /// 오브젝트를 소유하고 있는 플레이어 명
    public string owner = string.Empty;

    [SerializeField]
    /// UI로 표시될 progressBar
    private Slider _gage;

    /// 최대 길이
    private int width = 0;

    // Update is called once per frame
    void Update()
    {
        // 부모의 회전값을 따라가지 않기 위함
        transform.forward = Vector3.down;
    }

    /**
     *  ProgressBar를 업데이트 함
     *  
     *  @param      power       프로그래스바를 감소시킬 값
     */
    public void SetGage(int power)
    {
        // 현재 값이 0 이라면 함수 종료
        if(_gage.value <= 0)
        {
            return;
        }

        // 프로그래스바를 갱신
        _gage.value -= (float)power / width;
    }

    /**
     *  @brief      초기화 함수입니다.
     *  
     *  @param      max         프로그래스바 최대 넓이
     */
    public void Initialize(int max)
    {
        // 최대값 설정
        width = max;

        // 초기값 설정
        _gage.value = max;
    }
}
