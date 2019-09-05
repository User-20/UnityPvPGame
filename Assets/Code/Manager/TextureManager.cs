using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/**
 *  @brief      게임에 필요한 이미지들을 관리합니다.
 *  
 *  @todo       이미지를 저장하는 방식을 개선할 필요가 있음
 */
public class TextureManager : Sington<TextureManager>
{
    /// 이미지들을 관리할 변수
    private Dictionary<string, Sprite> textures = new Dictionary<string, Sprite>();

    // Start is called before the first frame update
    void Start()
    {
        // 프로필에 보여줄 Army 캐릭터 이미지를 Load
        textures["Prefab_Army"] = Resources.Load<Sprite>("Textures/CharacterImage/Prefab_Army");
    }

    /**
     *  @brief      찾고자 하는 이미지를 반환합니다.
     *  
     *  @param      key         찾고자 하는 이미지 이름
     *  
     *  @return     Sprite 자료형 이미지 반환, 없다면 null
     */
    public Sprite GetTexture(string key)
    {
        if(textures.ContainsKey(key))
        {
            return textures[key];
        }

        return null;
    }
}
