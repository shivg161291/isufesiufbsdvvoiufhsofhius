using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioClip flip;
    public AudioClip match;
    public AudioClip mismatch;
    public AudioClip gameover;

    AudioSource src;

    void Awake()
    {
        src = gameObject.AddComponent<AudioSource>();
        src.playOnAwake = false;
    }

    public void PlayFlip() { if (flip) src.PlayOneShot(flip); }
    public void PlayMatch() { if (match) src.PlayOneShot(match); }
    public void PlayMismatch() { if (mismatch) src.PlayOneShot(mismatch); }
    public void PlayGameOver() { if (gameover) src.PlayOneShot(gameover); }
}
