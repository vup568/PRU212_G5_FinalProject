using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    // tao bien luu tru audio source
    public AudioSource musicAudioSource;
    public AudioSource vfxAudioSource;

    // tao bien luu tru audio clip
    public AudioClip musicClip;
    public AudioClip coinClip;
    public AudioClip winClip;
    public AudioClip collectTree;
    public AudioClip digClip; // âm thanh đào đất
    public AudioClip levelUpClip; // âm thanh lên level

    void Start()
    {
        musicAudioSource.clip = musicClip;
        musicAudioSource.Play();
    }

    // hàm phát âm thanh đào
    public void PlayDigSound()
    {
        vfxAudioSource.pitch = Random.Range(0.9f, 1.1f);
        vfxAudioSource.PlayOneShot(digClip);
    }

    public void PlayCollectSound()
    {
        vfxAudioSource.pitch = Random.Range(0.9f, 1.1f);
        vfxAudioSource.PlayOneShot(collectTree);
    }

    // âm thanh lên level
    public void PlayLevelUpSound()
    {
        if (levelUpClip == null) return;
        vfxAudioSource.pitch = 1f;
        vfxAudioSource.PlayOneShot(levelUpClip);
    }

}