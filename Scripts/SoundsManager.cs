using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundsManager : MonoBehaviour
{
    public static SoundsManager instance;
    public AudioSource myAudioSource;
    public AudioSource timerTickingClock;
   // public AudioClip playerTurn;
    
    //public AudioClip opponentTurn;
    public AudioClip gameWin;
    public AudioClip gameLose;
    public AudioClip tokenMove;
    public AudioClip gameStartClip;
    public AudioClip stairClip;
    public AudioClip snakeEatClip;

    private void Awake()
    {
        instance = this;
    }



    // public void PlayerTurnSound()
    // {
    //     myAudioSource.PlayOneShot(playerTurn);
    // }

    // public void OpponentTurnSound()
    // {
    //     myAudioSource.PlayOneShot(opponentTurn);

    // }

    public void GameWinSound()
    {
        myAudioSource.PlayOneShot(gameWin);

    }
    public void GameLoseSound()
    {
        myAudioSource.PlayOneShot(gameLose);

    }

    public void MovementSound()
    {
        myAudioSource.PlayOneShot(tokenMove);

    }

    public void PlayGameStartSound()
    {
        myAudioSource.PlayOneShot(gameStartClip);

    }

    public void PlayStairSound()
    {
        myAudioSource.PlayOneShot(stairClip);
    }

    public void PlaySnakeEatSound()
    {
        myAudioSource.PlayOneShot(snakeEatClip);
    }

    public void TickingTimeStart()
    {
        timerTickingClock.Play();
    }
    public void TickingTimeStop()
    {
        timerTickingClock.Stop();
    }

    public void StopTickingTime()
    {
        timerTickingClock.Stop();

    }
}
