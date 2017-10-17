using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Nanosoft
{

/**
 * Скрипт для управления канвой
 *
 * Заставляет камеру следить за указанным объектом
 */
public class CanvasScript: MonoBehaviour
{
	
	/**
	 * Ссылка на менеджер квестов
	 */
	[HideInInspector]
	public QuestManager questManager;
	
	private GameObject dialogPanel;
	private GameObject actorPanel;
	private GameObject actionPanel;
	private GameObject lootPanel;
	private GameObject questPanel;
	private Text message;
	private Text actor;
	private Text actionMessage;
	private Text lootMessage;
	
	/**
	 * Ссылка на скрипт управляющий списком квестов
	 */
	private QuestList questList;
	
	/**
	 * Флаг диалогового режима
	 */
	private bool inDialog = false;
	
	public static CanvasScript instance;
	
	void Awake()
	{
		if ( instance == null )
		{
			instance = this;
		}
	}
	
	void Start()
	{
		dialogPanel = transform.Find("DialogPanel").gameObject;
		dialogPanel.SetActive(false);
		
		actorPanel = dialogPanel.transform.Find("ActorPanel").gameObject;
		actorPanel.SetActive(false);
		actor = actorPanel.transform.Find("Actor").gameObject.GetComponent<Text>();
		message = dialogPanel.transform.Find("Message").gameObject.GetComponent<Text>();
		
		GameObject actionGroup = transform.Find("ActionGroup").gameObject;
		
		actionPanel = actionGroup.transform.Find("ActionPanel").gameObject;
		actionPanel.SetActive(false);
		actionMessage = actionPanel.transform.Find("ActionMessage").gameObject.GetComponent<Text>();
		
		lootPanel = actionGroup.transform.Find("LootPanel").gameObject;
		lootPanel.SetActive(false);
		lootMessage = lootPanel.transform.Find("LootMessage").gameObject.GetComponent<Text>();
		
		questPanel = transform.Find("QuestPanel").gameObject;
		questPanel.SetActive(false);
		questList = questPanel.transform.Find("QuestList").gameObject.GetComponent<QuestList>();
		questList.questManager = questManager;
		
		message.text = "Hello world";
	}
	
	/**
	 * Обработка ввода
	 */
	protected void handleInput()
	{
		if ( Input.GetKey(KeyCode.Escape) )
		{
			CloseDialog();
			return;
		}
	}
	
	void Update()
	{
		if ( inDialog ) handleInput();
		
		if ( Input.GetKeyDown(KeyCode.L) )
		{
			if ( questPanel.active ) HideQuestList();
			else ShowQuestList();
		}
	}
	
	public void ShowQuestList()
	{
		questList.Refresh();
		questPanel.SetActive(true);
	}
	
	public void HideQuestList()
	{
		questPanel.SetActive(false);
	}
	
	public static void ShowMessage(string msg)
	{
		instance.message.text = msg;
		instance.dialogPanel.SetActive(true);
		instance.inDialog = true;
	}
	
	public static void HideMessage()
	{
		instance.CloseDialog();
	}
	
	public void CloseDialog()
	{
		dialogPanel.SetActive(false);
		inDialog = false;
	}
	
	public static void ShowAction(IAction action)
	{
		instance.actionMessage.text = action.GetActionMessage();
		instance.actionPanel.SetActive(true);
	}
	
	public static void HideAction()
	{
		instance.actionPanel.SetActive(false);
	}
	
} // class CanvasScript

} // namespace Nanosoft
