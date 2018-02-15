using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nanosoft
{

public class ZoneTrigger: MonoBehaviour
{
	public string zoneName;
	public bool playSound = false;
	public bool playOnce = false;
	private bool played = false;
	private AudioSource audio;
	
	void Start()
	{
		audio = GetComponent<AudioSource>();
	}
	
	protected void play()
	{
		if ( playSound )
		{
			if ( audio == null ) return;
			if ( audio.isPlaying ) return;
			audio.Play();
		}
	}
	
	public void OnTriggerEnter(Collider other)
	{
		if ( playOnce )
		{
			if ( !played )
			{
				CanvasScript.Zone(zoneName);
				play();
				played = true;
			}
			return;
		}
		
		CanvasScript.Zone(zoneName);
		play();
	}
	
}

} // namespace Nanosoft
