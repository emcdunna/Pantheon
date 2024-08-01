using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISoundManager : MonoBehaviour
{

    public AudioClip clickButtonSound;
    public AudioClip unitOrderedSound;

    public enum SoundEffectSet { ArrowFire, BowDraw, Stab, MarchingGrass, SwordSwing, Whoosh, None };

    public List<AudioClip> arrowFireClips = new List<AudioClip>();
    public List<AudioClip> bowDrawClips = new List<AudioClip>();
    public List<AudioClip> stabClips = new List<AudioClip>();
    public List<AudioClip> missingAudioSet = new List<AudioClip>();
    public List<AudioClip> marchingGrassSet = new List<AudioClip>();
    public List<AudioClip> swingSet = new List<AudioClip>();
    public List<AudioClip> whooshSounds = new List<AudioClip>();

    public void ClickButtonSound()
    {
        if (clickButtonSound != null)
        {
            AudioSource.PlayClipAtPoint(clickButtonSound, transform.position);
        }
    }

    public void OrderUnits()
    {
        if (unitOrderedSound != null)
        {
            AudioSource.PlayClipAtPoint(unitOrderedSound, transform.position);
        }
    }

    public List<AudioClip> GetSoundEffectSet(SoundEffectSet set)
    {
        switch (set)
        {
            case SoundEffectSet.ArrowFire:
                return arrowFireClips;
            case SoundEffectSet.BowDraw:
                return bowDrawClips;
            case SoundEffectSet.Stab:
                return stabClips;
            //case SoundEffectSet.MarchingGrass:
            //    return marchingGrassSet;
            case SoundEffectSet.SwordSwing:
                return swingSet;
            case SoundEffectSet.Whoosh:
                return whooshSounds;
            default:
                return missingAudioSet;
        }
    }

    public AudioClip GetClipFromSet(SoundEffectSet set)
    {

        List < AudioClip > sfx_list = GetSoundEffectSet(set);
        if(sfx_list == null)
        {
            return null;
        }
        int len = sfx_list.ToArray().Length;
        if (len > 0)
        {
            int selection = Random.Range(0, len);
            return sfx_list[selection];
        }
        else
        {
            return null;
        }
    }
}
