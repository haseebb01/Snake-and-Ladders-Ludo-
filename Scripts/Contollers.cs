using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;


public class Contollers : MonoBehaviour
{
    public RectTransform MainMenu, MatchMakingScreen;
    // Start is called before the first frame update

    public void PlayButton()
    {
        MainMenu.DOAnchorPos(new Vector2(-1600, 0), 0.25f);
        MatchMakingScreen.DOAnchorPos(new Vector2(0, 0), 0.25f);
    }
    
    //public void backarrowButton()
    //{
    //    MatchMakingScreen.DOAnchorPos(new Vector2(-2600, 0), 0.25f);
    //    MainMenu.DOAnchorPos(new Vector2(0, 0), 0.25f);
    //}
}
