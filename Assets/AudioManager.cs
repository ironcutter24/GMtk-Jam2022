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
}
