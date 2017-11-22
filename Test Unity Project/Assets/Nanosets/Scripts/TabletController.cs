using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine.EventSystems;
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
	
	/**
	 * Ссылка на NavMeshAgent персонажа
	 */
	private NavMeshAgent playerNav;
	
	/**
	 * Ссылка на камеру
	 */
	protected Camera pCamera;
	
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
	
	/**
	 * Флаг навигации (движения по клику)
	 */
	private bool navigate = false;
	
	/**
	 * Флаг фактического движения NavMeshAgent
	 */
	private bool navMoving = false;
	
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
	
	/**
	 * Флаг синхронизации камеры
	 */
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
	 * Порог времени для определения клика/тапа
	 */
	public float clickThreshold = 0.15f;
	
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
	
	private RectTransform rotateTap;
	private RectTransform scaleTap;
	private TouchManager touchManager;
	private TouchTracker rotateTracker;
	private TouchTracker trackerA;
	private TouchTracker trackerB;
	private int touchCount;
	private float touchScale;
	private float touchScaleDistance;
	
	/**
	 * Флаг удержания левой кнопки мыши
	 */
	private bool mouseActive = false;
	
	/**
	 * Время нажатия левой кнопки мыши
	 */
	private float mouseStartTime;
	
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
		rb.isKinematic = true;
		rotation = player.transform.rotation;
		
		playerNav = obj.GetComponent<NavMeshAgent>();
		playerNav.enabled = true;
		playerNav.ResetPath();
		navigate = false;
		navMoving = false;
		
		// присоединяем AudioListener к персонажу
		if ( audioListener ) audioListener.SetParent(player.transform, false);
	}
	
	public void SetCamera(GameObject obj)
	{
		if ( obj != null )
		{
			pCamera = obj.GetComponent<Camera>();
		}
		else
		{
			pCamera = null;
		}
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
	 * Найти элемент UI по экранным координатам
	 */
	public static GameObject RaycastUI(Vector2 p)
	{
		PointerEventData eventData = new PointerEventData(EventSystem.current);
		eventData.position = p;
		List<RaycastResult> rc_list = new List<RaycastResult>(); 
		EventSystem.current.RaycastAll(eventData, rc_list);
		
		int count = rc_list.Count;
		for (int i = 0; i < count; i++)
		{
			GameObject obj = rc_list[i].gameObject;
			if ( obj != null )
			{
				rc_list.Clear();
				return obj;
			}
		}
		
		rc_list.Clear();
		return null;
	}
	
	public Text dbg;
	
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
		GameObject cam = Instantiate(cameraPrefab);
		cam.name = cameraPrefab.name;
		
		// создаем канву
		canvas = Instantiate(canvasPrefab);
		canvas.name = canvasPrefab.name;
		
		// персонаж и камера не должны удаляться при переключении сцены
		DontDestroyOnLoad(player);
		DontDestroyOnLoad(cam);
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
		SetCamera(cam);
		
		Debug.Log("GameState init canvas");
		canvasCtl = canvas.GetComponent<CanvasScript>();
		canvasCtl.questManager = questManager;
		dbg = canvas.transform.Find("Panel/Text").GetComponent<Text>();
		dbg.text = "Debug";
		
		rotateTap = canvas.transform.Find("RotateTap") as RectTransform;
		rotateTap.gameObject.SetActive(false);
		
		scaleTap = canvas.transform.Find("ScaleTap") as RectTransform;
		scaleTap.gameObject.SetActive(false);
		
		touchManager = new TouchManager(4);
		touchCount = 0;
		int w = Screen.width;
		int h = Screen.height;
		float m = Mathf.Sqrt(w * w + h * h);
		touchScale = 6000f / m;
		touchScaleDistance = 32.0f / m;
		
		rotateTracker = null;
		trackerA = null;
		trackerB = null;
		
		mouseActive = false;
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
			
			// и режим движения по клику
			StopNavigation();
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
			syncCamera = false;
		}
		else
		{
			rotationSpeed = 0f;
		}
	}
	
	/**
	 * Обработка ввода с мышкой
	 */
	void HandleMouseInput()
	{
		if ( canvasCtl.inDialog )
		{
			return;
		}
		
		// нажали левую кнопку мыши
		if ( Input.GetMouseButtonDown(0) )
		{
			if ( RaycastUI(Input.mousePosition) == null )
			{
				Debug.Log("BeginTrack()");
				mouseActive = true;
				mouseStartTime = Time.unscaledTime;
			}
		}
		else if ( mouseActive )
		{
			if ( !rotateCamera )
			{
				if ( Time.unscaledTime - mouseStartTime > clickThreshold )
				{
					// переходим в режим свободного вращения камеры
					rotateCamera = true;
					rotatePlayer = false;
					lockCursor(true);
				}
			}
		}
		
		// отпустили левую кнопку мыши
		if ( Input.GetMouseButtonUp(0) )
		{
			if ( mouseActive )
			{
				Debug.Log("EndTrack()");
				mouseActive = false;
				if ( !rotateCamera ) NavigateByScreenPoint(Input.mousePosition);
				rotateCamera = false;
			}
		}
		
		// нажали правую кнопку мыши
		if ( Input.GetMouseButtonDown(1) )
		{
			// переходим в режим вращения персонажа
			rotateCamera = false;
			rotatePlayer = true;
			lockCursor(true);
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
			if ( run ) StopNavigation();
		}
		
		// на случай если что-то заглючит Escape принудительно нас возвращает
		// в режим UI, если персонаж в состоянии непрерывного бега, то он
		// также выйдет из него
		if ( Input.GetKeyDown(KeyCode.Escape) )
		{
			rotateCamera = false;
			rotatePlayer = false;
			run = false;
			StopNavigation();
		}
		
		// разблокировать курсор если мы в режиме UI
		if ( cursorLocked && !rotateCamera && !rotatePlayer )
		{
			lockCursor(false);
		}
		
		handleRotation();
		handleMovement();
	}
	
	/**
	 * Обработка ввода с мультитач
	 */
	void HandleTouchInput()
	{
		if ( Input.GetKeyDown(KeyCode.Escape) )
		{
			Application.Quit();
			return;
		}
		
		int count = Input.touchCount;
		for(int i = 0; i < count; i++)
		{
			var touch = Input.GetTouch(i);
			
			if ( touch.phase == TouchPhase.Began )
			{
				if ( RaycastUI(touch.position) == null )
				{
					TouchTracker tracker = touchManager.AllocTracker(touch.fingerId);
					if ( tracker != null )
					{
						touchCount++;
						tracker.BeginTrack(touch.position);
						tracker.tag = 0;
					}
				}
			}
			
			if ( touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary )
			{
				TouchTracker tracker = touchManager.GetTracker(touch.fingerId);
				if ( tracker != null )
				{
					tracker.Track(touch.position);
				}
			}
			
			if ( touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled )
			{
				TouchTracker tracker = touchManager.GetTracker(touch.fingerId);
				if ( tracker != null )
				{
					touchCount--;
					if ( tracker.tag == 1 && Time.unscaledTime - tracker.startTime <= clickThreshold )
					{
						NavigateByScreenPoint(touch.position);
					}
					tracker.EndTrack();
				}
			}
		}
		
		count = touchManager.touches.Length;
		int minTag = 3;
		for(int i = 0; i < count; i++)
		{
			var touch = touchManager.touches[i];
			if ( touch.active )
			{
				if ( touch.tag < touchCount ) touch.tag = touchCount;
				if ( touch.tag < minTag ) minTag = touch.tag;
			}
		}
		
		rotateTracker = null;
		trackerA = null;
		trackerB = null;
		
		if ( minTag == 1 )
		{
			for(int i = 0; i < count; i++)
			{
				var touch = touchManager.touches[i];
				if ( touch.tag == 1 )
				{
					rotateTracker = touch;
					break;
				}
			}
		}
		else if ( minTag == 2 )
		{
			
			int i = 0;
			for(; i < count; i++)
			{
				var touch = touchManager.touches[i];
				if ( touch.tag == 2 )
				{
					trackerA = touch;
					break;
				}
			}
			
			i++;
			
			for(; i < count; i++)
			{
				var touch = touchManager.touches[i];
				if ( touch.tag == 2 )
				{
					trackerB = touch;
					break;
				}
			}
		}
		
		dbg.text = "touches: " + touchCount.ToString() + ", minTag: " + minTag.ToString();
		
		rotationSpeed = 0f;
		camAngleSpeed = 0f;
		
		if ( trackerA != null && trackerB != null )
		{
			syncCamera = false;
			
			rotateTap.position = trackerA.position;
			rotateTap.gameObject.SetActive(true);
			
			scaleTap.position = trackerB.position;
			scaleTap.gameObject.SetActive(true);
			
			Vector2 a1 = trackerA.position - trackerA.deltaPosition;
			Vector2 b1 = trackerB.position - trackerB.deltaPosition;
			
			Vector2 r1 = a1 - b1;
			Vector2 r2 = trackerA.position - trackerB.position;
			
			float delta = (r2.magnitude - r1.magnitude) * touchScaleDistance;
			distance = Mathf.Clamp(distance - delta, 1f, maxDistance);
			
			r1.Normalize();
			r2.Normalize();
			
			float x = Mathf.Clamp(r1.x * r2.x + r1.y * r2.y, -1f, 1f);
			float y = r2.x * r1.y - r1.x * r2.y;
			
			rotationSpeed = Mathf.Rad2Deg * Mathf.Acos(x) * 12f;
			if ( y > 0f ) rotationSpeed = - rotationSpeed;
			
			return;
		}
		
		if ( rotateTracker != null )
		{
			syncCamera = false;
			
			rotateTap.position = rotateTracker.position;
			rotateTap.gameObject.SetActive(true);
			scaleTap.gameObject.SetActive(false);
			
			rotationSpeed = rotateTracker.deltaPosition.x * touchScale;
			camAngleSpeed = Mathf.Clamp(rotateTracker.deltaPosition.y * touchScale, -maxAngleSpeed, maxAngleSpeed);
			
			return;
		}
		
		scaleTap.gameObject.SetActive(false);
		rotateTap.gameObject.SetActive(false);
	}
	
	/**
	 * Отправить персонажа на указанные координаты (экранные)
	 */
	public bool NavigateByScreenPoint(Vector2 dest)
	{
		Ray ray = pCamera.ScreenPointToRay(dest);
		RaycastHit hit;
		if ( Physics.Raycast(ray, out hit, 1000, Physics.DefaultRaycastLayers & ~(1<<8), QueryTriggerInteraction.Ignore) )
		{
			run = false;
			navigate = true;
			if ( navMoving ) syncCamera = true;
			if ( hit.collider.tag == "Interactable" )
			{
				var t = hit.collider.transform;
				var p = t.position + t.forward * 0.4f;
				targetCircle.position = t.position;
				targetPoint.position = t.position;
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
			return true;
		}
		
		return false;
	}
	
	/**
	 * Остановить/отменить NavMeshAgent
	 */
	public void StopNavigation()
	{
		if ( navigate )
		{
			navigate = false;
			navMoving = false;
			syncCamera = false;
			playerNav.ResetPath();
			targetPoint.gameObject.SetActive(false);
			targetCircle.gameObject.SetActive(false);
		}
	}
	/**
	 * Дополнительные проверки для исправления глюков NavMeshAgent
	 */
	protected void HandleNavigation()
	{
		if ( navigate && !playerNav.pathPending )
		{
			if ( playerNav.pathStatus == NavMeshPathStatus.PathInvalid )
			{
				Debug.Log("PathInvalid => ResetPath()");
				StopNavigation();
				return;
			}
			
			if ( navMoving )
			{
				if ( playerNav.velocity == Vector3.zero )
				{
					Debug.Log("stop walking");
					navMoving = false;
					syncCamera = false;
					return;
				}
			}
			else
			{
				if ( playerNav.velocity != Vector3.zero )
				{
					Debug.Log("start walking");
					navMoving = true;
					syncCamera = true;
				}
			}
		}
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
			if ( rotatePlayer && !navMoving )
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
		
		HandleNavigation();
		
		animator.SetBool("walk", navMoving || velocity > 0.01f);
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
