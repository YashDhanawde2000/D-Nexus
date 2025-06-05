using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Audio Config", menuName = "Guns/Audio Config", order = 5)]
public class AudioConfigScriptableObject : ScriptableObject
{
    [Range(0f, 1f)]
    public float Volume = 1f;
    public AudioClip FireClip;
    public AudioClip ReloadClip;
    public AudioClip EmptyClip;

    public void PlayShootingClip (AudioSource  audioSource)
    {
        if ( FireClip != null )
        {
            //audioSource.PlayOneShot(FireClip, Volume);
            AudioSource.PlayClipAtPoint(FireClip, audioSource.gameObject.transform.position, Volume);
        }  
    }

    public void PlayReloadClip(AudioSource audioSource)
    {
        if (ReloadClip != null )
        {
            //audioSource.PlayOneShot(ReloadClip, Volume);
            AudioSource.PlayClipAtPoint(ReloadClip, audioSource.gameObject.transform.position, Volume);
        }
    }

    public void PlayEmptyClip (AudioSource audioSource)
    {
        if ( EmptyClip != null )
        {
            //audioSource.PlayOneShot(EmptyClip, Volume);
            AudioSource.PlayClipAtPoint(EmptyClip, audioSource.gameObject.transform.position, Volume);
        }
    }

}
