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
	private AudioSource _audio;
	
	void Start()
	{
		_audio = GetComponent<AudioSource>();
	}
	
	protected void play()
	{
		if ( playSound )
		{
			if ( _audio == null ) return;
			if ( _audio.isPlaying ) return;
			_audio.Play();
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
