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
            Debug.Log("Ý’è”Ô†‚ð‰z‚¦‚½‚Ì‚ÅA0”Ô–Ú‚ðÄ¶‚µ‚Ü‚·");
            return;
        }

        if (audioClips[clipNum] == null)
        {
            Debug.Log("SE‚ªnull‚ÅÄ¶‚Å‚«‚Ü‚¹‚ñ‚Å‚µ‚½");
            return;
        }

        if(seAudio_ == null)
        {
            seAudio_ = transform.GetComponent<AudioSource>();
        }
        seAudio_.PlayOneShot(audioClips[clipNum]);
    }

}
