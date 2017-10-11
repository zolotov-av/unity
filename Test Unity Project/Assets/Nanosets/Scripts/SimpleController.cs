using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Nanosoft
{

/**
 * Простой контроллер управления персонажем
 *
 * Данный контроллер содержит минимум функций, минимум анимаций и позволит
 * управлять почти любой моделью персонажа. Для анимации персонажа вам
 * потребуется всего 3-4 анимации (стоит, бежит, атака, падение)
 */
public class SimpleController: PlayerController
{
	
	/**
	 * Ссылка на камеру
	 *
	 * Камера должна управляться скриптом CameraScript
	 */
	public GameObject playerCamera;
	
	/**
	 * Ссылка на контроллер камеры
	 */
	private CameraScript cameraCtl;
	
	/**
	 * Скорость движения персонажа (с которой персонаж может двигаться)
	 */
	public float speed = 2f;
	
	/**
	 * Текущая скорость персонажа (с которой персонаж движется в данный момент)
	 */
	private float velocity = 0f;
	
	/**
	 * Вектор скорости персонажа в локальных координатах
	 */
	private Vector3 localVelocity = new Vector3(0f, 0f, 0f);
	
	/**
	 * Режим вращения камеры
	 *
	 * В этом режиме камера свободно вращается вокруг персонажа
	 */
	private bool rotateCamera = false;
	
	/**
	 * Режим вращения персонажа
	 *
	 * В этом режиме движения мышкой разворачивают персонажа, а камера
	 * разворачивается следом за персонажем
	 */
	private bool rotatePlayer = false;
	
	/**
	 * Режим бега
	 *
	 * В этом режиме персонаж всё время бежит вперед, даже если игрок
	 * не нажимает клавиши движения
	 */
	private bool run = false;
	
	/**
	 * Ссылка на аниматор персонажа
	 */
	private Animator animator;
	
	/**
	 * Ссылка на Rigidbody (твердое тело)
	 */
	private Rigidbody rb;
	
	private bool freeCamera = false;
	
	private const string rotateCameraXInput = "Mouse X";
	private const string rotateCameraYInput = "Mouse Y";
	private const string horizontalInput = "Horizontal";
	private const string verticallInput = "Vertical";
	
	public void UpdateOptions(float rotateDelta)
	{
		//transform.Rotate(Vector3.up * rotateDelta * 3);
		
		bool wasFree = freeCamera;
		freeCamera = Input.GetMouseButton(0);
		if ( freeCamera )
		{
			if ( !wasFree ) cameraCtl.SetMode(CameraScript.FreeCamera);
			Quaternion rot = Quaternion.Euler(0, rotateDelta * 3, 0);
			cameraCtl.Rotate(rot);
		}
		else
		{
			if ( wasFree )
			{
				cameraCtl.SetMode(CameraScript.FollowCamera);
				transform.rotation = cameraCtl.GetRotation();
			}
			transform.Rotate(Vector3.up * rotateDelta * 3);
			cameraCtl.SetRotation(transform.rotation);
		}
	}
	
	/**
	 * Обработка ввода движения
	 */
	protected void handleMovement()
	{
		// прочитаем ввод направления движения
		var inputV = Input.GetAxis(verticallInput);
		var inputH = Input.GetAxis(horizontalInput);
		
		// нормализуем длину вектора, чтобы он был не больше заданной скорости
		var length = Mathf.Sqrt( inputH * inputH + inputV * inputV );
		if ( length > 0.01f )
		{
			velocity = speed * Mathf.Clamp(length, 0f, 1f);
			var scale = velocity / length;
			localVelocity.x = inputH * scale;
			localVelocity.y = 0f;
			localVelocity.z = inputV * scale;
			
			// если игрок нажал клавиши движения, то сбрасываем режим бега
			run = false;
		}
		else
		{
			if ( run )
			{
				velocity = speed;
				localVelocity.x = 0f;
				localVelocity.y = 0f;
				localVelocity.z = speed;
			}
			else
			{
				velocity = 0f;
				localVelocity.x = 0f;
				localVelocity.y = 0f;
				localVelocity.z = 0f;
			}
		}
		
		animator.SetBool("walk", velocity > 0.01f);
	}
	
	void Start()
	{
		cameraCtl = playerCamera.GetComponent<CameraScript>();
		cameraCtl.SetRotation(transform.rotation);
		
		animator = GetComponent<Animator>();
		rb = GetComponent<Rigidbody>();
	}
	
	void FixedUpdate()
	{
		if ( cursorLocked )
		{
			var rotateDelta = Input.GetAxis(rotateCameraXInput);
			var angleDelta = Input.GetAxis(rotateCameraYInput);
			var distanceDelta = Input.GetAxis("Camera Distance");

			UpdateOptions(rotateDelta);
			cameraCtl.UpdateOptions(distanceDelta, angleDelta);
		}
		
		Vector3 move = transform.TransformDirection(localVelocity);
		rb.MovePosition(transform.position + move * Time.deltaTime);
	}
	
	void Update()
	{
		// нажали левую кнопку мыши
		if ( Input.GetMouseButtonDown(0) )
		{
			// переходим в режим свободного вращения камеры
			rotateCamera = true;
			rotatePlayer = false;
			lockCursor(true);
		}
		
		// нажали правую кнопку мыши
		if ( Input.GetMouseButtonDown(1) )
		{
			// переходим в режим вращения персонажа
			rotateCamera = false;
			rotatePlayer = true;
			lockCursor(true);
		}
		
		// отпустили левую кнопку мыши
		if ( Input.GetMouseButtonUp(0) )
		{
			rotateCamera = false;
		}
		
		// отпустили правую кнопку мыши
		if ( Input.GetMouseButtonUp(1) )
		{
			rotatePlayer = false;
		}
		
		// включение/выключение режима бега
		if ( Input.GetButtonDown("Run") )
		{
			run = !run;
		}
		
		// на случай если что-то заглючит Escape принудительно нас возвращает
		// в режим UI, если персонаж в состоянии непрерывного бега, то он
		// также выйдет из него
		if ( Input.GetKeyDown(KeyCode.Escape) )
		{
			rotateCamera = false;
			rotatePlayer = false;
			run = false;
		}
		
		// разблокировать курсор если мы в режиме UI
		if ( cursorLocked && !rotateCamera && !rotatePlayer )
		{
			lockCursor(false);
		}
		
		handleMovement();
	}

} // class SimpleController

} // namespace Nanosoft
