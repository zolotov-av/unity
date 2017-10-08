﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Nanosoft;

namespace Nanosoft
{

/**
* Тупой контроллер управления персонажем
*
* Тривиальный контроллер приемущественно для неигровых персонажей
*/
public class DumbController: BaseController
{
	
	public GameObject playerCamera;
	public float walkSpeed = 2f;
	
	private CameraScript cam;
	private Animator animator;
	private Rigidbody rb;
	
	private const string rotateCameraXInput = "Mouse X";
	private const string rotateCameraYInput = "Mouse Y";
	private const string horizontalInput = "Horizontal";
	private const string verticallInput = "Vertical";
	
	private float speed = 0f;
	
	public void UpdateOptions(float rotateDelta)
	{
		transform.Rotate(Vector3.up * rotateDelta * 3);
	}
	
	protected void handleMovement()
	{
		speed = Input.GetAxis(verticallInput) * walkSpeed;
		
		animator.SetBool("walk", speed > 0.01f);
	}
	
	protected virtual void Start()
	{
		cam = playerCamera.GetComponent<CameraScript>();
		animator = GetComponent<Animator>();
		rb = GetComponent<Rigidbody>();
	}
	
	protected virtual void FixedUpdate()
	{
		if ( cursorLocked )
		{
			var rotateDelta = Input.GetAxis(rotateCameraXInput);
			var angleDelta = Input.GetAxis(rotateCameraYInput);
			var distanceDelta = Input.GetAxis("Camera Distance");

			UpdateOptions(rotateDelta);
			cam.UpdateOptions(distanceDelta, angleDelta);
			Vector3 dir = transform.forward * speed;
			rb.MovePosition(transform.position + dir * Time.deltaTime);
		}
	}
	
	protected virtual void Update ()
	{
		if ( ! cursorLocked )
		{
			if ( Input.GetMouseButtonDown(0) )
			{
				lockCursor(true);
			}
		}
		else
		{
			handleMovement();
			
			if ( Input.GetMouseButtonDown(0) )
			{
				animator.SetTrigger("attack");
			}
			
			if ( Input.GetKeyDown(KeyCode.Escape) )
			{
				lockCursor(false);
			}
		}
	}

} // class DumbController

} // namespace Nanosoft
