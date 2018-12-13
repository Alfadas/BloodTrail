using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
	{
	[SerializeField] AudioClip[] mapmusic;
	[SerializeField] AudioClip[] fightmusic;
	[SerializeField] AudioClip[] othermusic;
	[SerializeField] int minpause = 2;
	[SerializeField] int maxpause = 20;
	[SerializeField] AudioClip[] effectclips;
	[SerializeField] Slider musicslider;
	[SerializeField] Slider sfxslider;

	public static System.Random random = new System.Random();

	private AudioSource mapplayer;
	private AudioSource secondaryplayer;
	private AudioSource effectplayer;
	private bool mapactive;
	private Dictionary<string, AudioClip> titles;
	private float starttime;
	private float pausetime;
	private Dictionary<string, AudioClip> soundeffects;

	void Start()
		{
		// Get AudioSource components
		AudioSource[] players = gameObject.GetComponentsInChildren<AudioSource>();
		mapplayer = players[0];
		secondaryplayer = players[1];
		effectplayer = players[2];

		// Setup title database and add every known title
		titles = new Dictionary<string, AudioClip>(mapmusic.Length + fightmusic.Length);
		foreach(AudioClip title in mapmusic)
			{
			titles.Add(title.name, title);
			}
		foreach(AudioClip title in fightmusic)
			{
			titles.Add(title.name, title);
			}
		foreach(AudioClip title in othermusic)
			{
			titles.Add(title.name, title);
			}

		starttime = -1;
		pausetime = minpause;

		soundeffects = new Dictionary<string, AudioClip>(effectclips.Length);
		foreach(AudioClip effectclip in effectclips)
			{
			soundeffects.Add(effectclip.name, effectclip);
			}

		// Play map music
		playMapMusic();
		}

	void FixedUpdate()
		{
		if(mapactive)
			{
			if(!mapplayer.isPlaying)
				{
				if(starttime < 0)
					{
					pausetime = random.Next(minpause, maxpause);	// Random pausetimes between titles, because everything else would be lame
					starttime = Time.time;
					}
				else if(Time.time - starttime > pausetime)
					{
					mapplayer.clip = null;
					playMapMusic();
					}
				}
			}
		else
			{
			if(!secondaryplayer.isPlaying)
				{
				playFightMusic();
				}
			}
		}

	public void playTitle(string title)
		{
		prepareSecondaryPlayer();
		secondaryplayer.clip = titles[title];
		secondaryplayer.Play();
		}

	public void playSFX(string sfx)
		{
		effectplayer.PlayOneShot(soundeffects[sfx]);
		}

	public void playMapMusic()
		{
		secondaryplayer.Stop();

		if(mapplayer.clip == null)
			{
			mapplayer.clip = mapmusic[random.Next(mapmusic.Length)];
			}
		mapactive = true;
		mapplayer.Play();
		}

	public void playFightMusic()
		{
		prepareSecondaryPlayer();
		secondaryplayer.clip = fightmusic[random.Next(fightmusic.Length)];
		secondaryplayer.Play();
		}

	public void updateMusicVolume()
		{
		mapplayer.volume = Mathf.Clamp(musicslider.value, 0, 1.0f);
		secondaryplayer.volume = Mathf.Clamp(musicslider.value, 0, 1.0f);
		}

	public void updateEffectVolume()
		{
		effectplayer.volume = Mathf.Clamp(sfxslider.value, 0, 1.0f);
		}

	private void prepareSecondaryPlayer()
		{
		mapplayer.Pause();
		mapactive = false;
		secondaryplayer.Stop();
		}
	}
