using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nanosoft
{

public class SoundManager: MonoBehaviour
{
	
	public AudioClip mainMenuClip;
	public AudioClip townClip;
	
	public AudioSource sound;
	
	public static SoundManager instance;
	
	private AudioClip fadeTarget;
	
	public AudioClip findClip(string clipName)
	{
		if ( clipName == "menu" )
		{
			return mainMenuClip;
		}
		
		if ( clipName == "town" )
		{
			return townClip;
		}
		
		return null;
	}
	
	public void Play(string clipName)
	{
		if ( clipName == "menu" )
		{
			//if ( sound.clip != mainMenuClip )
			sound.Stop();
			sound.clip = mainMenuClip;
			sound.Play();
			return;
		}
		
		if ( clipName == "town" )
		{
			sound.Stop();
			sound.clip = townClip;
			sound.Play();
			return;
		}
	}
	
	public void FadePlay(string clipName)
	{
		AudioClip clip = findClip(clipName);
		if ( clip != null )
		{
			fadeTarget = clip;
			return;
		}
	}
	
	void Awake()
	{
		if ( instance == null )
		{
			instance = this;
		}
	}
	
	void Update()
	{
		if ( fadeTarget != null )
		{
			float volume = sound.volume - 0.4f * Time.deltaTime;
			if ( volume > 0f )
			{
				sound.volume = volume;
				return;
			}
			
			sound.Stop();
			sound.clip = fadeTarget;
			sound.volume = 0.5f;
			sound.Play();
			fadeTarget = null;
		}
	}
	
}

} // namespace Nanosoft
