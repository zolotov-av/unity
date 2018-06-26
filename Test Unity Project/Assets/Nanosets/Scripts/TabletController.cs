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
public class TabletController: MonoBehaviour
{
	
	/**
	 * Ссылка на экземляр
	 */
	public static TabletController instance = null;
	
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
	 * Ссылка на менеджер фоновой музыки
	 */
	public SoundManager soundManager;
	
	/**
	 * Ссылка на персонажа (загруженный экземляр)
	 */
	protected PlayerBehaviour player;
	
	/**
	 * GameObject персонажа
	 */
	protected GameObject playerGO;
	
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
	 * Ссылка на менеджер меню
	 */
	[HideInInspector]
	public MainMenuScript menuCtl;
	
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
	 * Флаг активности игры
	 * true - игра активна
	 * false - игра не началась, на паузе или остановлена
	 */
	protected bool gameActive = false;
	
	/**
	 * Флаг боевого режима
	 * true - персонаж в боевом режиме
	 * false - персонаж в режиме исследования
	 */
	protected bool battleMode = false;
	
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
	 * Разрешить движение по клику мышкой
	 */
	public bool navigateByMouse = false;
	
	/**
	 * Скорость движения персонажа (с которой персонаж может двигаться)
	 */
	public float speed = 2f;
	
	/**
	 * Текущая скорость персонажа (с которой персонаж движется в данный момент)
	 */
	// TODO it right
	public static float velocity = 0f;
	
	/**
	 * Вектор скорости персонажа относительно ориентации камеры
	 */
	private Vector3 localVelocity = new Vector3(0f, 0f, 0f);
	
	public static bool bigJump = false;
	
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
	// TODO
	public static bool run = false;
	
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
	 * Ссылка на коллайдер персонажа
	 */
	private CapsuleCollider capsule;
	
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
	 * Физический материал когда персонаж стоит (без управления игроком)
	 */
	public PhysicMaterial idleFriction;
	
	/**
	 * Физический матераил когда персонаж движется (управляется игроком)
	 */
	public PhysicMaterial movingFriction;
	
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
	private TouchTracker mouse;
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
		playerGO = obj;
		player = obj.GetComponent<PlayerBehaviour>();
		if ( player == null )
		{
			Debug.LogError("player haven't PlayerBehaviour");
		}
		
		animator = obj.GetComponent<Animator>();
		rotation = player.transform.rotation;
		
		capsule = obj.GetComponent<CapsuleCollider>();
		capsule.center = new Vector3(0f, capsule.height * 0.5f, 0f);
		if ( capsule == null )
		{
			Debug.LogError("player haven't CapsuleCollider");
		}
		
		player.ResetNavigation();
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
	 * Инициализация
	 */
	protected void Init()
	{
		SceneManager.sceneLoaded += OnSceneLoaded;
		
		// создаем персонажа
		var t = gameObject.transform;
		
		playerGO = Instantiate(playerPrefab, t.position, t.rotation);
		playerGO.name = playerPrefab.name;
		
		// создаем камеру
		distance = startDistance;
		camAngle = startAngle;
		GameObject cam = Instantiate(cameraPrefab);
		cam.name = cameraPrefab.name;
		
		// создаем канву
		canvas = Instantiate(canvasPrefab);
		canvas.name = canvasPrefab.name;
		
		// персонаж и камера не должны удаляться при переключении сцены
		DontDestroyOnLoad(playerGO);
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
		
		SetPlayer(playerGO);
		SetCamera(cam);
		
		if ( questManager == null ) Debug.LogError("questManager=null");
		questManager.questWindow = canvas.transform.Find("QuestWindow").GetComponent<QuestWindow>();
		if ( questManager.questWindow == null ) Debug.LogError("questWindow not found");
		questManager.questWindow.Init(questManager);
		
		//Debug.Log("GameState init canvas");
		canvasCtl = canvas.GetComponent<CanvasScript>();
		canvasCtl.questManager = questManager;
		dbg = canvas.transform.Find("DebugPanel/Text").GetComponent<Text>();
		dbg.text = "Debug";
		
		menuCtl = canvas.transform.Find("MainMenu").GetComponent<MainMenuScript>();
		menuCtl.ShowMain();
		
		soundManager.Play("menu");
		//soundManager.Play("town");
		
		rotateTap = canvas.transform.Find("RotateTap") as RectTransform;
		rotateTap.gameObject.SetActive(false);
		
		scaleTap = canvas.transform.Find("ScaleTap") as RectTransform;
		scaleTap.gameObject.SetActive(false);
		
		Input.simulateMouseWithTouches = false;
		if ( ! Input.mousePresent )
		{
			mobileInput = true;
		}
		
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
		mouse = new TouchTracker();
	}
	
	private bool loading = false;
	public static string sceneName;
	
	/**
	 * Событие загрузки новой сцены
	 *
	 * Если пользователю нужно производить какие-то действия при смене сцены,
	 * например чтобы передвинуть персонажа в определенную позицию, то можно
	 * переопределить этот метод.
	 */
	protected void OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{
		Debug.Log("TabletController OnSceneLoaded(" + scene.name + ")");
		sceneName = scene.name;
		if ( loading )
		{
			loading = false;
			StopNavigation();
			player.ResetMovement();
			menuCtl.EndLoading();
			var entry = GameObject.FindWithTag("EntryPoint");
			if ( entry != null )
			{
				var t = entry.transform;
				var pt = player.transform;
				pt.position = t.position;
				pt.rotation = t.rotation;
				rotation = t.rotation;
				var te = entry.GetComponent<TabletEntry>();
				if ( te != null )
				{
					if ( te.backgroundMusic != null && te.backgroundMusic != "" )
					{
						soundManager.FadePlay(te.backgroundMusic);
					}
				}
			}
			else
			{
				Debug.LogError("EntryPoint not found");
			}
		}
		player.GroundCheck();
		
		if ( gameActive )
		{
			// если игра активна, то запустить отсчет времени
			Time.timeScale = 1f;
		}
	}
	
	public static void LoadScene(int i)
	{
		instance.loading = true;
		Time.timeScale = 0f;
		instance.player.RemoveAction();
		instance.menuCtl.ShowLoading();
		SceneManager.LoadScene(i);
	}
	
	public static void LoadScene(string sceneName)
	{
		instance.loading = true;
		Time.timeScale = 0f;
		instance.player.RemoveAction();
		instance.menuCtl.ShowLoading();
		SceneManager.LoadScene(sceneName);
	}
	
	/**
	 * Загрузить новую игру
	 */
	public static void LoadNewGame()
	{
		instance.loading = true;
		Time.timeScale = 0f;
		instance.player.RemoveAction();
		instance.menuCtl.ShowLoading();
		instance.gameActive = true;
		SceneManager.LoadScene(1);
	}
	
	/**
	 * Обработка ввода движения
	 */
	protected void handleMovement()
	{
		if ( player.falling || player.jumping ) return;
		
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
	
	public void JumpHeight(float height)
	{
		StopNavigation();
		player.JumpHeight(height);
	}
	
	/**
	 * Обработка ввода с клавиатуры (действия)
	 */
	protected void HandleKeyboard()
	{
		if ( player.grounded )
		{
			if ( Input.GetKeyDown("space") )
			{
				if ( bigJump ) JumpHeight(12f);
				else JumpHeight(2.4f);
			}
		}
		
		if ( Input.GetKey("1") )
		{
			player.Attack1();
		}
		
		if ( Input.GetKeyDown("2") )
		{
			player.Attack2();
		}
	}
	
	public static void RunActionTouch()
	{
		instance.player.RunActionTouch();
	}
	
	/**
	 * Обработка ввода с мышкой
	 */
	void HandleMouseInput()
	{
		mouse.TrackMouse();
		
		if ( WindowBehaviour.current != null )
		{
			WindowBehaviour.current.HandleInput();
			return;
		}
		
		if ( battleMode )
		{
			if ( Input.GetKeyDown(KeyCode.F) )
			{
				battleMode = false;
				rotateCamera = false;
				rotatePlayer = false;
			}
			
			if ( Input.GetMouseButtonDown(0) )
			{
				player.Attack1();
			}
			
			handleRotation();
			handleMovement();
			return;
		}
		
		if ( Input.GetKeyDown(KeyCode.F) )
		{
			battleMode = true;
			rotateCamera = false;
			rotatePlayer = true;
			lockCursor(true);
			handleRotation();
			handleMovement();
			return;
		}
		
		if ( mouseActive )
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
			
			// отпустили левую кнопку мыши
			if ( mouse.up )
			{
				//Debug.Log("EndTrack()");
				mouseActive = false;
				if ( navigateByMouse && !rotateCamera ) NavigateByScreenPoint(Input.mousePosition);
				rotateCamera = false;
			}
		}
		else
		{
			// нажали левую кнопку мыши
			if ( mouse.down && mouse.hoverUI == null )
			{
				//Debug.Log("BeginTrack()");
				mouseActive = true;
				mouseStartTime = Time.unscaledTime;
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
			TouchTracker tracker;
			var touch = Input.GetTouch(i);
			
			switch ( touch.phase )
			{
			
			case TouchPhase.Began:
				tracker = touchManager.AllocTracker(touch.fingerId);
				if ( tracker != null )
				{
					tracker.BeginTrack(touch.position);
					if ( tracker.user ) touchCount++;
				}
				break;
			
			case TouchPhase.Moved:
			case TouchPhase.Stationary:
				tracker = touchManager.GetTracker(touch.fingerId);
				if ( tracker != null ) tracker.Track(touch.position);
				break;
			
			case TouchPhase.Ended:
				tracker = touchManager.GetTracker(touch.fingerId);
				if ( tracker != null )
				{
					if ( tracker.user )
					{
						if ( tracker.tag == 1 && Time.unscaledTime - tracker.startTime <= clickThreshold )
						{
							NavigateByScreenPoint(touch.position);
						}
						touchCount--;
					}
					tracker.EndTrack();
				}
				break;
			
			case TouchPhase.Canceled:
				tracker = touchManager.GetTracker(touch.fingerId);
				if ( tracker != null )
				{
					if ( tracker.user ) touchCount--;
					tracker.EndTrack();
				}
				break;
			
			}
		}
		
		count = touchManager.touches.Length;
		int minTag = 3;
		for(int i = 0; i < count; i++)
		{
			var touch = touchManager.touches[i];
			if ( touch.user )
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
		
		rotationSpeed = 0f;
		camAngleSpeed = 0f;
		
		if ( trackerA != null && trackerB != null )
		{
			syncCamera = false;
			
			rotateTap.position = trackerA.position;
			rotateTap.gameObject.SetActive(true);
			
			scaleTap.position = trackerB.position;
			scaleTap.gameObject.SetActive(true);
			
			Vector2 a1 = trackerA.position - trackerA.delta;
			Vector2 b1 = trackerB.position - trackerB.delta;
			
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
			
			rotationSpeed = rotateTracker.delta.x * touchScale;
			camAngleSpeed = Mathf.Clamp(rotateTracker.delta.y * touchScale, -maxAngleSpeed, maxAngleSpeed);
			
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
				player.Navigate(p);
			}
			else
			{
				targetPoint.position = hit.point;
				targetPoint.gameObject.SetActive(true);
				targetCircle.gameObject.SetActive(false);
				player.Navigate(hit.point);
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
			targetPoint.gameObject.SetActive(false);
			targetCircle.gameObject.SetActive(false);
			player.StopNavigation();
		}
	}
	/**
	 * Дополнительные проверки для исправления глюков NavMeshAgent
	 */
	protected void HandleNavigation()
	{
		if ( navigate )
		{
			if ( navMoving )
			{
				if ( player.NavVelocity == Vector3.zero )
				{
					navMoving = false;
					syncCamera = false;
					return;
				}
			}
			else
			{
				if ( player.NavVelocity != Vector3.zero )
				{
					navMoving = true;
					syncCamera = true;
				}
			}
		}
	}
	
	public static void UpdateLabel(Vector3 position, Transform label)
	{
		label.position = instance.pCamera.WorldToScreenPoint(position);
	}
	
	void Awake()
	{
		if ( instance == null )
		{
			instance = this;
			DontDestroyOnLoad(gameObject);
			Init();
		}
	}
	
	void FixedUpdate()
	{
		player.GroundCheck();
		
		if ( syncCamera )
		{
			rotation = Quaternion.RotateTowards(rotation, player.transform.rotation, syncRotationSpeed * Time.deltaTime);
		}
		else
		{
			rotation *= Quaternion.Euler(0, rotationSpeed * Time.deltaTime, 0);
		}
		
		// TODO нужен рефакторинг
		if ( player.busy )
		{
			// если персонаж занят анимацией удара или заморожен, то им нельзя
			// управлять (двигать)
			capsule.material = idleFriction;
			return;
		}
		
		if ( velocity > 0f )
		{
			Vector3 cameraVelocity = rotation * localVelocity;
			Quaternion q = Quaternion.LookRotation(cameraVelocity, Vector3.up);
			
			capsule.material = movingFriction;
			
			if ( battleMode )
			{
				player.RotateTowards(rotation, maxRotationSpeed);
			}
			else
			{
				player.RotateTowards(q, maxRotationSpeed);
			}
			
			if ( player.grounded )
			{
				player.MoveVelocity(cameraVelocity);
			}
		}
		else
		{
			capsule.material = idleFriction;
			
			if ( rotatePlayer && !navMoving )
			{
				player.RotateTowards(rotation, maxRotationSpeed);
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
			HandleKeyboard();
			HandleMouseInput();
		}
		
		HandleNavigation();
		
		animator.SetBool("walk", navMoving || velocity > 0.01f);
		animator.SetFloat("speedh", localVelocity.x);
		animator.SetFloat("speedv", localVelocity.z);
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
		
		CanvasScript.UpdateLabels(pCamera);
		CanvasScript.UpdateHealth(player);
		
		// TODO нужен рефакторинг...
		if ( WindowBehaviour.current == null )
		{
			CanvasScript.RaycastInfo(null);
			return;
		}
		
		Vector3 dest = new Vector3(Screen.width*0.5f, Screen.height*0.5f, 0f);
		Ray ray = pCamera.ScreenPointToRay(dest);
		//Ray ray = new Ray(ControllerUtils.CapsuleCenter(capsule), player.transform.forward);
		if ( Physics.SphereCast(ray, 1f, out hit, 100f, Physics.DefaultRaycastLayers & ~(1<<8), QueryTriggerInteraction.Ignore) )
		{
			CanvasScript.RaycastInfo(hit.collider.transform.root.gameObject);
		}
		else
		{
			CanvasScript.RaycastInfo(null);
		}
	}
	
} // class TabletController

} // namespace Nanosoft
