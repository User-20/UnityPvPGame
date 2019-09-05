using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;

/**
 *  @brief      MonoBehaviour를 상속받은 객체를 최초 한번만 메모리에 할당 시켜줍니다.
 */
public class Sington<T> : MonoBehaviour where T : MonoBehaviour
{
    /// 현재 객체의 인스턴스
    private static T _instance;

    /**
     *  @brief      객체의 인스턴스를 반환합니다.
     *  
     *  @return     생성된 인스턴스(오브젝트형 또는 컴포넌트형)
     */
    public static T GetInstance()
    {
        // 인스턴스가 없다면
        if (_instance == null)
        {
            // 인자로 받은 오브젝트 형(또는 컴포넌트형) 으로 검색
            _instance = FindObjectOfType(typeof(T)) as T;

            // 인스턴스가 아직 존재하지 않는다면
            if(_instance == null)
            {
                // 에러 로그 출력
                Debug.LogError("Not Created = " + typeof(T));
            }
        }

        return _instance;
    }
}
