using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMManager : MonoBehaviour
{
    [Header("Values")]
    [SerializeField] private float _songPlayInterval;

    [Header("References")]
    private Sound _currentSong;
    private List<Sound> _daytimeTracks = new List<Sound>();
    private List<Sound> _nighttimeTracks = new List<Sound>();

    private void Start()
    {
        foreach(Sound s in FindObjectOfType<AudioManager>().Sounds)
        {
            if(s.audioType == Sound.AudioTypes.music)
            {
                if(s.isDayMusic)
                {
                    _daytimeTracks.Add(s);
                }
                else if(s.isNightMusic)
                {
                    _nighttimeTracks.Add(s);
                }
            }
        }

        StartCoroutine(PlaySongInList());
    }

    public IEnumerator PlaySongInList()
    {
        yield return new WaitForSeconds(_songPlayInterval);

        //play next song in list

        //wait until song completes

        StartCoroutine(PlaySongInList());
    }
}
