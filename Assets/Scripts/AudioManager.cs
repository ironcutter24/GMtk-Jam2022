using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility.Patterns;

public class AudioManager : Singleton<AudioManager>
{
    [SerializeField]
    AudioClip
        // General
        diceRoll,
        
        // Player
        shoot, parryUse, parryReady, hitPlayer,

        // Enemies
        hitEnemy, hitNoEffect, respawnEnemy, koEnemy, koBoss;

    [Space]

    [SerializeField]
    AudioClip ostIntro;

    [SerializeField]
    AudioClip ostLoop;

    [Space]

    [SerializeField]
    AudioSource sourceOST;

    [SerializeField]
    AudioSource sourceFX;


    public void PlayFX_diceRoll() { PlayFX(diceRoll); }

    public void PlayFX_shoot() { PlayFX(shoot); }

    public void PlayFX_parryUse() { PlayFX(parryUse); }

    public void PlayFX_parryReady() { PlayFX(parryReady); }

    public void PlayFX_hitPlayer() { PlayFX(hitPlayer); }

    public void PlayFX_hitEnemy() { PlayFX(hitEnemy); }

    public void PlayFX_hitNoEffect() { PlayFX(hitNoEffect); }

    public void PlayFX_respawnEnemy() { PlayFX(respawnEnemy); }

    public void PlayFX_koEnemy() { PlayFX(koEnemy); }

    public void PlayFX_koBoss() { PlayFX(koBoss); }


    void PlayFX(AudioClip clip, float volScale = 1f)
    {
        sourceFX.PlayOneShot(clip, volScale);
    }
}
