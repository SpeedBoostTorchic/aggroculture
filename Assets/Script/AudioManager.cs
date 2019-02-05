using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour {

    //Singleton Instance
    public static AudioManager am;

    //Audio-related objects
    public GameObject audioObj;
    public AudioSource music;
    public AudioSource[] a;     //List of usable audiosources
    public int maxNum;          //Total number of concurrent audio sources

    //Volume and other variables
    public float musicVol;
    public float sfxVol;

    //Standard clips
    public AudioClip preStartSong;
    public AudioClip mainSongIntro;
    public AudioClip mainSong;
    public AudioClip stinger;
    public bool started;
   


    //Sets initial volumes and instantiates all instances of the AudioSources
	void Start () {

        am = this;

        //Sets prestart music to play
        music.volume = musicVol;
        music.clip = preStartSong;
        music.loop = true;
        music.Play();

        //Instiates X number of audio sources, then adds them to the list
        a = new AudioSource[maxNum];
        for(int i = 0; i < maxNum; i++)
        {
            a[i] = Instantiate(audioObj, gameObject.transform).GetComponent<AudioSource>();
            a[i].volume = sfxVol;
            a[i].gameObject.transform.SetParent(gameObject.transform, true);
        }
	}

    //Used to update the main music track
    void Update()
    {
        //If the games' started, and there IS an intro to the track...
       if(started && mainSongIntro != null)
        {
            if (!music.isPlaying)
            {
                music.Stop();
                music.clip = mainSong;
                music.loop = true;
                music.Play();
            }
        }
    }

    //starts up the main song for the round
    public void startMain()
    {
        //Stops any other song that might be playing
        music.Stop();
        
        //Chooses appropriate clip
        if (mainSongIntro != null)
        {
            music.clip = mainSongIntro;
            music.loop = false;
        }
        else
        {
            music.clip = mainSong;
            music.loop = true;
        }

        //Begins playing the music
        music.Play();
        started = true;
    }

    //Plays the stinger when the game ends
    public void playSting()
    {
        music.Stop();
        music.clip = stinger;
        music.loop = false;
        music.Play();
        started = false;
    }


    //Reicieves an audioClip, and finds an available AudioSource
    //If no sources available, doesn't play the clip
    public void play(AudioClip clip)
    {
        for(int i = 0; i < maxNum; i++)
        {
            if(a[i].isPlaying == false)
            {
                a[i].volume = sfxVol;
                a[i].clip = clip;
                a[i].Play();
                return;
            }
        }
    }

    //Overload of main function - plays at altered volume
    public void play(AudioClip clip, float vol)
    {
        for (int i = 0; i < maxNum; i++)
        {
            if (a[i].isPlaying == false)
            {
                a[i].volume = sfxVol * vol;
                a[i].clip = clip;
                a[i].Play();
                return;     
            }
        }
    }

    //Pauses all instances of audio source
    public void pauseAll()
    {
        music.Pause();
        for(int i = 0; i < maxNum; i++)
        {
            a[i].Pause();
        }
    }

    //This function resumes them all again
    public void unpauseAll()
    {
        music.UnPause();
        for (int i = 0; i < maxNum; i++)
        {
            a[i].UnPause();
        }
    }

}
