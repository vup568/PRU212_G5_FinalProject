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
    public AudioClip plantClip; // âm thanh trồng cây/hoa
    public AudioClip buildHouseClip; // âm thanh xây nhà
    public AudioClip gameCompleteClip; // âm thanh phá đảo

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

    // âm thanh trồng cây
    public void PlayPlantSound()
    {
        if (plantClip == null) return;
        vfxAudioSource.pitch = Random.Range(0.9f, 1.1f);
        vfxAudioSource.PlayOneShot(plantClip);
    }

    // âm thanh xây nhà
    public void PlayBuildHouseSound()
    {
        if (buildHouseClip == null) return;
        vfxAudioSource.pitch = 1f;
        vfxAudioSource.PlayOneShot(buildHouseClip);
    }

    // âm thanh phá đảo
    public void PlayGameCompleteSound()
    {
        if (gameCompleteClip == null) return;
        vfxAudioSource.pitch = 1f;
        vfxAudioSource.PlayOneShot(gameCompleteClip);
    }

}