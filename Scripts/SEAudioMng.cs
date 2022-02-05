using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SEAudioMng : MonoBehaviour
{
    public AudioClip[] audioClips;
    private AudioSource seAudio_;

    void Start()
    {
        seAudio_ = transform.GetComponent<AudioSource>();
    }

    public void OnceShotSE(int clipNum)
    {
        if(audioClips[clipNum] == null)
        {
            Debug.Log("SE��null�ōĐ��ł��܂���ł���");
            return;
        }

        if(seAudio_ == null)
        {
            seAudio_ = transform.GetComponent<AudioSource>();
        }
        seAudio_.PlayOneShot(audioClips[clipNum]);
    }

}
