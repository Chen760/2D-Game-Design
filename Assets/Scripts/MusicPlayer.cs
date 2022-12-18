using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//背景音乐
public class MusicPlayer : MonoBehaviour
{
    public AudioSource introSource, loopSource;
    // Start is called before the first frame update
    void Start()
    {
        introSource.Play();
        loopSource.PlayScheduled(AudioSettings.dspTime + introSource.clip.length);
    }


}
