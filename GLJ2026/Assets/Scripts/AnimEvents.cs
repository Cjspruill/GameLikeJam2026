using UnityEngine;

public class AnimEvents : MonoBehaviour
{

    public BoxCollider knifeCollider;
    public Collider punchCollider;

    public AudioSource footStepAudioSource;
    public AudioSource gruntAudioSource;
    public AudioSource boxKnifeAudioSource;

    public AudioClip[] footstepAudioClips;
    public AudioClip[] gruntJumpAudioClips;
    public AudioClip[] gruntLandAudioClips;
    public AudioClip[] boxKnifeSwooshAudioClips;
    public AudioClip[] boxKnifeCutAudioClips;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        DisableKnifeCollider();
        DisablePunchCollider();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void EnableKnifeCollider()
    {
        knifeCollider.enabled = true;
    }

    public void DisableKnifeCollider()
    {
        knifeCollider.enabled = false;
    }

    public void EnablePunchCollider()
    {
        punchCollider.enabled = true;
    }

    public void DisablePunchCollider()
    {
        punchCollider.enabled = false;
    }

    public void PlayFootStep()
    {
        AudioClip footstepToPlay = footstepAudioClips[Random.Range(0, footstepAudioClips.Length)];
        PlayAudioClip(footStepAudioSource, footstepToPlay);
    }
    public void PlayJumpGrunt()
    {
        AudioClip gruntToPlay = gruntJumpAudioClips[Random.Range(0, gruntJumpAudioClips.Length)];
        PlayAudioClip(gruntAudioSource, gruntToPlay);
    }
    public void PlayLandGrunt()
    {
        AudioClip gruntToPlay = gruntLandAudioClips[Random.Range(0, gruntLandAudioClips.Length)];
        PlayAudioClip(gruntAudioSource, gruntToPlay);
    }
    public void PlayWhoosh()
    {
        AudioClip whooshToPlay = boxKnifeSwooshAudioClips[Random.Range(0, boxKnifeSwooshAudioClips.Length)];
        PlayAudioClip(boxKnifeAudioSource, whooshToPlay);
    }
    public void PlayCut()
    {
        AudioClip cutToPlay = boxKnifeCutAudioClips[Random.Range(0, boxKnifeCutAudioClips.Length)];
        PlayAudioClip(boxKnifeAudioSource, cutToPlay);
    }



    public void PlayAudioClip(AudioSource audioSourceToPlay, AudioClip clipToPlay)
    {
        audioSourceToPlay.clip = clipToPlay;
        audioSourceToPlay.Play();
    }
}
