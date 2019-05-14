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
    public float maxDist;

    //Standard clips
    public AudioClip preStartSong;
    public AudioClip mainSongIntro;
    public AudioClip mainSong;
    public AudioClip stinger;
    public bool started;

    //Song List
    public AudioClip preRound;
    public AudioClip midRound;
    public AudioClip[] roundMusic;
    public AudioClip victory;
   


    //Sets initial volumes and instantiates all instances of the AudioSources
	void Start () {

        am = this;

        //Sets prestart music to play
        music.volume = musicVol;
        //music.clip = preStartSong;
        music.clip = preRound;
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

        //Music changes speed to match timescale changes
        music.pitch = Time.timeScale;
        music.volume = musicVol * Time.timeScale;

        //Music volume further cut if GM cut-in is active
        if (GameManager.gm.cutInActive)
        {
            music.volume = music.volume / 2f;
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
        play(clip, 1f);
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
                a[i].panStereo = 0f;
                a[i].Play();
                return;     
            }
        }
    }

    //The power jingles work similarly to the standard play, but
    //scale off of music volume instead of SFX
    public void playJingle(AudioClip clip, float vol)
    {
        for (int i = 0; i < maxNum; i++)
        {
            if (a[i].isPlaying == false)
            {
                a[i].volume = musicVol * vol;
                a[i].clip = clip;
                a[i].panStereo = 0f;
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

    //Changes the direction of the audio based on distance from players
    public void playTwoPlayerSound(AudioClip clip, float vol, Vector2 pos)
    {
        //First, gets both players' positions...
        Vector2 p1Pos = GameManager.gm.getPlayerPos(1);
        Vector2 p2Pos = GameManager.gm.getPlayerPos(2);
        AudioSource source = null;

        //...then calculates their distance to the sound...
        float p1Dist = Vector2.Distance(pos, p1Pos);
        float p2Dist = Vector2.Distance(pos, p2Pos);

        //Finds a vacant audiosource
        for (int i = 0; i < maxNum; i++)
        {
            if (a[i].isPlaying == false)
            {
                source = a[i];
                source.clip = clip;
                source.panStereo = 0f;
                source.Stop();
                break;
            }
        }

        //If no AudioSource is vacant, cancels the sound
        if(source == null)
        {
            return;
        }

        //Determines L & R channel based on a ratio of the two...
        //If p2 is outside max range and player 1 is not...
        if(p2Dist > maxDist && p1Dist < maxDist)
        {
            source.panStereo = -0.8f;
        }
        //If p1 is outside max range and p2 is not...
        else if (p1Dist > maxDist && p2Dist < maxDist)
        {
            source.panStereo = 0.8f;
        }
        //Otherwise, calculate ratio thusly...
        //The closer the sound is to P1, the lower the ratio will be
        else
        {
            float ratio = p1Dist / (p1Dist + p2Dist);

            //If closer to player 1...
            if(ratio < 0.5f)
            {
                ratio = -(1f - ratio) + 0.5f;
                print(ratio);
            }
            //If closer to player 2...
            else if (ratio > 0.5)
            {
                ratio = ratio - 0.5f;
            }
            //If distance is equal...
            else
            {
                ratio = 0f;
            }

            //Clamps the ratio, then sets pan to it.
            ratio = Mathf.Clamp(ratio, -0.8f, 0.8f);
            source.panStereo = ratio;
        }

        //And Determines volume using the absolute distance...
        //If both players are outside the distance range, the volume is set to 0
        if(p1Dist > maxDist && p2Dist > maxDist)
        {
            vol = 0;
        }
        //If only player 1 is out of range...
        else if (p1Dist > maxDist)
        {
            vol = vol * (Mathf.Abs(maxDist - p2Dist) / maxDist * 0.5f);
        }
        //If only player 2 is out of range...
        else if (p2Dist > maxDist)
        {
            vol = vol * (Mathf.Abs(maxDist - p1Dist) / maxDist * 0.5f);
        }
        //If both player are in range
        else
        {
            vol = vol * ((Mathf.Abs(maxDist - p1Dist) / maxDist * 0.5f) + (Mathf.Abs(maxDist - p2Dist) / maxDist * 0.5f));
        }

        
        source.volume = vol * sfxVol;

        //Finally, plays the sound
        source.Play();
    }

}
