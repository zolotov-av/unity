using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nanosoft
{

public class TestEffect1: MonoBehaviour
{
	
	public ParticleSystem ps;
	public AudioSource sound;
	public Animator animator;
	
	private float radius;
	private bool play;
	private float speed = 2f;
	private float psTime = 0f;
	private float asTime = 0f;
	public float psDelay = 0.4f;
	
	private bool playSound = false;
	
	void Awake()
	{
		psTime = Time.time + 3f;
		asTime = psTime - psDelay;
		play = false;
		playSound = false;
	}
	
	public void BreathSound()
	{
		sound.Play();
		radius = 2f;
		var shape = ps.shape;
		shape.radius = radius;
		ps.Play();
		play = true;
	}
	
	void PlayVFX()
	{
	}
	
	void Update()
	{
		if ( play )
		{
			radius += speed * Time.deltaTime;
			if ( radius > 6f )
			{
				ps.Stop();
				psTime = Time.time + 5f;
				asTime = psTime - psDelay;
				play = false;
				playSound = false;
				return;
			}
			var shape = ps.shape;
			shape.radius = radius;
			return;
		}
		
		if ( !playSound && Time.time > psTime )
		{
			animator.SetTrigger("Deadbreath");
			playSound = true;
			return;
		}
	}
	
}

} // namespace Nanosoft