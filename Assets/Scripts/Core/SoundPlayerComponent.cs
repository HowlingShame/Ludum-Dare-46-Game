using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundPlayerComponent : MonoBehaviour
{
    public void Play(string sound)
    {
        SoundManager.Instance.PlaySound(sound);
    }
}
