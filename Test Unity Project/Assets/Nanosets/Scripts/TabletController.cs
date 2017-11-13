using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Nanosoft
{

/**
 * Контроллер игрового состояния для игр разрабатываемых под планшеты.
 * 
 * Данный контроллер в одном классе управляет и персонажем и камерой и глобльным
 * состоянием игры.
 */
public class TabletController: GameStateBehaviour
{
	
	/**
	 * Ссылка на префаб для загрузки персонажа
	 */
	public GameObject playerPrefab;
	
	/**
	 * Ссылка на префаб камеры
	 */
	public GameObject cameraPrefab;
	
	/**
	 * Ссылка на префаб канвы пользовательского интерфейса
	 */
	public GameObject canvasPrefab;
	
	/**
	 * Ссылка на менеджер квестов
	 */
	public QuestManager questManager;
	
	/**
	 * Ссылка на персонажа (загруженный экземляр)
	 */
	protected GameObject player;
	
	private NavMeshAgent playerNav;
	
	/**
	 * Ссылка на камеру
	 */
	protected GameObject pCamera;
	
	/**
	 * Канва пользовательского интерфейса (загруженный экземляр)
	 */
	[HideInInspector]
	public GameObject canvas;
	
	/**
	 * Ссылка на контроллер канвы
	 */
	[HideInInspector]
	public CanvasScript canvasCtl;
	
	/**
	 * Ссылка на объект держащий AudioListener
	 */
	private Transform audioListener;
	
	/**
	 * Ссылка на объект указывающий цель движения по клику
	 */
	private Transform targetPoint;
	
	/**
	 * Ссылка на объектект указывающий на цель движения (NPC)
	 */
	private Transform targetCircle;
	
	[Header("Player Settings")]
	
	/**
	 * Тип ввода - мобильный (сенсорный экран) или ПК (клавиатура и мышка)
	 */
	public bool mobileInput = false;
	
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
	
	private bool syncCamera = false;
	
	/**
	 * Текущая скорость вращения камеры/персонажа (градус/сек)
	 */
	private float rotationSpeed = 0f;
	
	/**
	 * Режим бега
	 *
	 * В этом режиме персонаж всё время бежит вперед, даже если игрок
	 * не нажимает клавиши движения
	 */
	private bool run = false;
	
	/**
	 * Флаг блокировки курсора
	 *
	 * Используйте это поле только для чтения, для блокировки/разблокировки
	 * используйте метод lockCursor()
	 */
	protected bool cursorLocked = false;
	
	/**
	 * Ссылка на аниматор персонажа
	 */
	private Animator animator;
	
	/**
	 * Ссылка на Rigidbody (твердое тело)
	 */
	private Rigidbody rb;
	
	[Header("Camera Settings")]
	
	/**
	 * Высота полета камеры
	 *
	 * Начало координат в модели персонажа обычно находиться в ногах. Этот
	 * параметр нужен чтобы камера не валялась на земле и смотрела на тело
	 * персонажа, а не на его ноги. Задает вертикальное смещение куда будет
	 * смотреть камера, например на уровне грудной клетки или плечь персонажа.
	 */
	public float height = 1.4f;
	
	/**
	 * Начальное расстояние от персонажа до камеры
	 */
	public float startDistance = 3f;
	
	/**
	 * Начальный угол наклона камеры (тангаж - наклон в сторону земли/неба)
	 */
	public float startAngle = 20f;
	
	/**
	 * Угол наклона камеры (тангаж)
	 */
	private float camAngle = 0f;
	
	/**
	 * Скорость изменения угла наклона камеры (тангажа)
	 */
	private float camAngleSpeed = 0f;
	
	[Header("Limit Settings")]
	
	/**
	 * Минимальное расстояние от персонажа до камеры
	 */
	public float minDistance = 1f;
	
	/**
	 * Максимальное расстояние от персонажа до камеры
	 */
	public float maxDistance = 10f;
	
	/**
	 * Дистанция камеры (расстояние от персонажа до камеры)
	 */
	private float distance = 0f;
	
	/**
	 * Ориентация камеры относительно цели
	 *
	 * Параметр rotation задает направление куда смотрит персонаж (реальное,
	 * в случае следящего режима, или виртуальное, в случае режима свободного
	 * вращения камеры). Реальное положение и направление камеры рассчитывается
	 * автоматически в LateUpdate(), с учетом дистанции и тангажа (наклона
	 * в строну земли/неба).
	 *
	 * В следящем режиме камеры rotation = target.transform.rotation, т.е.
	 * указывает направление куда смотрит персонаж, а сама камера находиться за
	 * спиной персонажа и смотрит примерно в то же самое направление с
	 * поправкой на тангаж (наклон в строну земли или неба).
	 *
	 * В режиме свободного вращения камеры rotation задает виртуальное
	 * направление перснонажа, куда бы он смотрел, если был бы повернут также.
	 * Этот режим позволяет смотреть на персонажа с любой стороны.
	 */
	[HideInInspector]
	public Quaternion rotation;
	
	private RectTransform hRotateBox;
	private RectTransform rotateTap;
	private TouchTracker hRotateTracker;
	private TouchTracker vRotateTracker;
	private TouchTracker rotateTracker;
	
	private const string rotateCameraXInput = "Mouse X";
	private const string rotateCameraYInput = "Mouse Y";
	private const string horizontalInput = "Horizontal";
	private const string verticallInput = "Vertical";
	private const float rotateSensitivity = 300f;
	private const float maxRotationSpeed = 360f;
	private const float syncRotationSpeed = 30f;
	private const float angleSensitivity = 300f;
	private const float minAngle = -10f;
	private const float maxAngle = 80f;
	private const float maxAngleSpeed = 360f;
	
	public static float ClampAngle(float angle, float min, float max)
	{
		while (angle > 360) angle -= 360;
		while (angle < -360) angle += 360;
		return Mathf.Clamp(angle, min, max);
	}
	
	/**
	 * Установить персонажа
	 */
	public void SetPlayer(GameObject obj)
	{
		player = obj;
		animator = obj.GetComponent<Animator>();
		rb = obj.GetComponent<Rigidbody>();
		playerNav = obj.GetComponent<NavMeshAgent>();
		playerNav.enabled = true;
		rotation = player.transform.rotation;
		
		// присоединяем AudioListener к персонажу
		if ( audioListener ) audioListener.SetParent(player.transform, false);
	}
	
	public void SetCamera(GameObject obj)
	{
		pCamera = obj;
	}
	
	/**
	 * Заблокировать/разблокировать курсор
	 */
	public void lockCursor(bool state)
	{
		if ( state )
		{
			if ( ! cursorLocked )
			{
				Cursor.visible = false;
				Cursor.lockState = CursorLockMode.Locked;
				cursorLocked = true;
			}
		}
		else
		{
			if ( cursorLocked )
			{
				Cursor.visible = true;
				Cursor.lockState = CursorLockMode.None;
				cursorLocked = false;
			}
		}
	}
	
	/**
	 * Событие инициализации
	 *
	 * Пользователь должен переопределить этот метод и реализовать в нём
	 * инициализацию глобального состояния.
	 */
	protected override void OnInit()
	{
		// создаем персонажа
		var t = gameObject.transform;
		
		player = Instantiate(playerPrefab, t.position, t.rotation);
		player.name = playerPrefab.name;
		
		// создаем камеру
		distance = startDistance;
		camAngle = startAngle;
		pCamera = Instantiate(cameraPrefab);
		pCamera.name = cameraPrefab.name;
		
		// создаем канву
		canvas = Instantiate(canvasPrefab);
		canvas.name = canvasPrefab.name;
		
		// персонаж и камера не должны удаляться при переключении сцены
		DontDestroyOnLoad(player);
		DontDestroyOnLoad(pCamera);
		DontDestroyOnLoad(canvas);
		
		audioListener = transform.Find("AudioListener");
		
		targetPoint = transform.Find("TargetPoint");
		targetPoint.SetParent(null, true);
		targetPoint.gameObject.SetActive(false);
		DontDestroyOnLoad(targetPoint.gameObject);
		
		targetCircle = transform.Find("TargetCircle");
		targetCircle.SetParent(null, true);
		targetCircle.gameObject.SetActive(false);
		DontDestroyOnLoad(targetCircle.gameObject);
		
		SetPlayer(player);
		SetCamera(pCamera);
		
		Debug.Log("GameState init canvas");
		canvasCtl = canvas.GetComponent<CanvasScript>();
		canvasCtl.questManager = questManager;
		
		hRotateBox = canvas.transform.Find("hRotateBox") as RectTransform;
		hRotateTracker = new TouchTracker();
		hRotateTracker.rt = hRotateBox;
		
		vRotateTracker = new TouchTracker();
		vRotateTracker.rt = canvas.transform.Find("vRotateBox") as RectTransform;
		
		rotateTap = canvas.transform.Find("RotateTap") as RectTransform;
		rotateTap.gameObject.SetActive(false);
		rotateTracker = new TouchTracker();
		rotateTracker.rt = null;
		rotateTracker.active = false;
	}
	
	/**
	 * События загрузки новой сцены
	 *
	 * Если пользователю нужно производить какие-то действия при смене сцены,
	 * например чтобы передвинуть персонажа в определенную позицию, то можно
	 * переопределить этот метод.
	 */
	protected override void OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{
		Debug.Log("TabletController.OnSceneLoaded(" + scene.name + ")");
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
	
	/**
	 * Обработка вращения персонажа/камеры
	 */
	protected void handleRotation()
	{
		var angleDelta = cursorLocked ? Input.GetAxis(rotateCameraYInput) : 0f;
		var distanceDelta = Input.GetAxis("Camera Distance");
		
		camAngleSpeed = Mathf.Clamp(angleDelta * angleSensitivity, -maxAngleSpeed, maxAngleSpeed);
		distance = Mathf.Clamp(distance - distanceDelta, minDistance, maxDistance);
		
		if ( cursorLocked )
		{
			var rotateDelta = Input.GetAxis(rotateCameraXInput) * rotateSensitivity;
			rotationSpeed = Mathf.Clamp(rotateDelta, -maxRotationSpeed, maxRotationSpeed);
		}
		else
		{
			rotationSpeed = 0f;
		}
	}
	
	void HandleMouseInput()
	{
		if ( canvasCtl.inDialog )
		{
			return;
		}
		
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
		
		if ( Input.GetKeyDown("1") )
		{
			animator.SetTrigger("attack");
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
		
		handleRotation();
		handleMovement();
	}
	
	int invert = 1;
	void HandleTouchInput()
	{
		if ( Input.GetKeyDown(KeyCode.Escape) )
		{
			Application.Quit();
			return;
		}
		
		hRotateTracker.HandleTouch();
		vRotateTracker.HandleTouch();
		
		var rotateDelta = -hRotateTracker.deltaPosition.x;
		rotationSpeed = rotateDelta;//Mathf.Clamp(rotateDelta, -maxRotationSpeed, maxRotationSpeed);
		
		//var distanceDelta = 0f;//Input.GetAxis("Camera Distance");
		
		//distance -= distanceDelta;
		//distance = Mathf.Clamp(distance, 1f, maxDistance);
		
		camAngleSpeed = Mathf.Clamp(vRotateTracker.deltaPosition.y, -maxAngleSpeed, maxAngleSpeed);
		
		if ( rotateTracker.active )
		{
			rotateTracker.ProcessTouch();
			rotateTap.position = rotateTracker.position;
			
			if ( rotateTracker.active )
			{
				rotationSpeed = invert * rotateTracker.deltaPosition.x;
				
				//var distanceDelta = 0f;//Input.GetAxis("Camera Distance");
				
				//distance -= distanceDelta;
				//distance = Mathf.Clamp(distance, 1f, maxDistance);
				
				camAngleSpeed = Mathf.Clamp(rotateTracker.deltaPosition.y, -maxAngleSpeed, maxAngleSpeed);
			}
			else
			{
				rotateTap.gameObject.SetActive(false);
			}
			
			if ( rotateTracker.tap )
			{
				Camera c = pCamera.GetComponent<Camera>();
				Ray ray = c.ScreenPointToRay(rotateTracker.position);
				RaycastHit hit;
				if ( Physics.Raycast(ray, out hit, 1000, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore) )
				{
					if ( hit.collider.tag == "Interactable" )
					{
						var t = hit.collider.transform;
						var p = t.position + t.forward * 0.4f;
						targetCircle.position = t.position;
						targetCircle.gameObject.SetActive(true);
						targetPoint.gameObject.SetActive(false);
						playerNav.SetDestination(p);
					}
					else
					{
						targetPoint.position = hit.point;
						targetPoint.gameObject.SetActive(true);
						targetCircle.gameObject.SetActive(false);
						playerNav.SetDestination(hit.point);
					}
				}
			}
		}
		else
		{
			
			int count = Input.touchCount;
			for(int i = 0; i < count; i++)
			{
				var touch = Input.GetTouch(i);
				if ( touch.phase == TouchPhase.Began )
				{
					if ( hRotateTracker.TouchIsFree(touch) && vRotateTracker.TouchIsFree(touch) )
					{
						rotateTap.position = touch.position;
						rotateTap.gameObject.SetActive(true);
						rotateTracker.Bind(touch);
						
						Camera c = pCamera.GetComponent<Camera>();
						Vector3 cp = c.WorldToScreenPoint(player.transform.position);
						invert = (cp.y > touch.position.y) ? 1 : -1;
						break;
					}
				}
			}
			
		}
		
		/*
		int count = Input.touchCount;
		for(int i = 0; i < count; i++)
		{
			var touch = Input.GetTouch(i);
			if ( touch.phase == TouchPhase.Began )
			{
				if ( hRotateTracker.TouchIsFree(touch) && vRotateTracker.TouchIsFree(touch) )
				{
					Camera c = pCamera.GetComponent<Camera>();
					Ray ray = c.ScreenPointToRay(touch.position);
					RaycastHit hit;
					if ( Physics.Raycast(ray, out hit, 1000, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore) )
					{
						if ( hit.collider.tag == "Interactable" )
						{
							var t = hit.collider.transform;
							var p = t.position + t.forward * 0.4f;
							targetCircle.position = t.position;
							targetCircle.gameObject.SetActive(true);
							targetPoint.gameObject.SetActive(false);
							playerNav.SetDestination(p);
						}
						else
						{
							targetPoint.position = hit.point;
							targetPoint.gameObject.SetActive(true);
							targetCircle.gameObject.SetActive(false);
							playerNav.SetDestination(hit.point);
						}
					}
					break;
				}
			}
		}
		*/
		
		syncCamera = playerNav.velocity.magnitude > 0.05f;
		animator.SetBool("walk", playerNav.velocity.magnitude > 0.01f);
	}
	
	void FixedUpdate()
	{
		if ( syncCamera )
		{
			rotation = Quaternion.RotateTowards(rotation, player.transform.rotation, syncRotationSpeed * Time.deltaTime);
		}
		else
		{
			rotation *= Quaternion.Euler(0, rotationSpeed * Time.deltaTime, 0);
		}
		
		if ( velocity > 0f )
		{
			Vector3 move = rotation * localVelocity;
			Quaternion q = Quaternion.LookRotation(move, Vector3.up);
			
			rb.rotation = Quaternion.RotateTowards(rb.rotation, q, maxRotationSpeed * Time.deltaTime);
			rb.position += move * Time.deltaTime;
		}
		else
		{
			if ( rotatePlayer )
			{
				rb.rotation = Quaternion.RotateTowards(rb.rotation, rotation, maxRotationSpeed * Time.deltaTime);
			}
		}
		
	}
	
	void Update()
	{
		if ( mobileInput )
		{
			HandleTouchInput();
		}
		else
		{
			HandleMouseInput();
		}
	}
	
	void LateUpdate()
	{
		camAngle = ClampAngle(camAngle - camAngleSpeed * Time.deltaTime, minAngle, maxAngle);
		
		// точка куда смотрит камера (центр экрана)
		Vector3 tp = player.transform.position + player.transform.up * height;
		
		// вектор смещения камеры (вектор от объекта к камере)
		float rad = camAngle * Mathf.Deg2Rad;
		Vector3 cv = (rotation * Vector3.up) * (Mathf.Sin(rad) * distance);
		Vector3 sv = (rotation * Vector3.forward) * (-Mathf.Cos(rad) * distance);
		Vector3 cdir = cv + sv;
		
		// пускаем луч от цели к камере
		// (проверка, есть ли на пути преграждающие объекты)
		RaycastHit hit;
		if ( Physics.Raycast(tp, cdir, out hit, distance, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore) )
		{
			// если луч нашел препрятствие, то корректируем дистанцию
			pCamera.transform.position = tp + cdir * (hit.distance / distance);
		}
		else
		{
			// если препятствий нет, то полное расстояние
			pCamera.transform.position = tp + cdir;
		}
		
		pCamera.transform.LookAt(tp);
	}
	
} // class TabletController

} // namespace Nanosoft
