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
        if(audioClips.Length - 1 < clipNum)
        {
            seAudio_.PlayOneShot(audioClips[0]);
            Debug.Log("�ݒ�ԍ����z�����̂ŁA0�Ԗڂ��Đ����܂�");
            return;
        }

        if (audioClips[clipNum] == null)
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
