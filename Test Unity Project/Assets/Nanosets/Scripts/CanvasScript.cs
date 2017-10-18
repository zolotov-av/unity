using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Nanosoft
{

/**
 * Скрипт для управления канвой
 *
 * Управляет всеми окнами пользовательского интерфейса
 */
public class CanvasScript: MonoBehaviour
{
	
	/**
	 * Ссылка на менеджер квестов
	 */
	[HideInInspector]
	public QuestManager questManager;
	
	private GameObject actionPanel;
	private GameObject lootPanel;
	private Text message;
	private Text actionMessage;
	private Text lootMessage;
	
	/**
	 * Ссылка на скрипт управляющий списком квестов
	 */
	private QuestList questList;
	
	/**
	 * Ссылка на скрипт управляющий диалоговым окном
	 */
	public DialogWindow dialogWindow;
	
	/**
	 * Ссылка на скрипт управляющий окном списка квестов
	 */
	public QuestWindow questWindow;
	
	/**
	 * Флаг диалогового режима
	 */
	[HideInInspector]
	public bool inDialog = false;
	
	private bool actionActive = false;
	
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
		Debug.Log("CanvasScript.Start()");
		
		GameObject actionGroup = transform.Find("ActionGroup").gameObject;
		
		actionPanel = actionGroup.transform.Find("ActionPanel").gameObject;
		actionPanel.SetActive(false);
		actionMessage = actionPanel.transform.Find("ActionMessage").gameObject.GetComponent<Text>();
		
		lootPanel = actionGroup.transform.Find("LootPanel").gameObject;
		lootPanel.SetActive(false);
		lootMessage = lootPanel.transform.Find("LootMessage").gameObject.GetComponent<Text>();
		
		questWindow.SetQuestManager(questManager);
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
			questWindow.ToggleWindow();
		}
	}
	
	public static void ShowMessage(string msg)
	{
		instance.dialogWindow.ShowMessage(msg);
		instance.inDialog = true;
	}
	
	public static void ShowDialog(DialogItem dialog)
	{
		instance.dialogWindow.ShowDialog(dialog);
		instance.inDialog = true;
		instance.actionPanel.SetActive(false);
	}
	
	public void CloseDialog()
	{
		dialogWindow.CloseDialog();
		actionPanel.SetActive(actionActive);
		inDialog = false;
	}
	
	public static void ShowAction(IAction action)
	{
		instance.actionActive = true;
		instance.actionMessage.text = action.GetActionMessage();
		instance.actionPanel.SetActive(true);
	}
	
	public static void HideAction()
	{
		instance.actionActive = false;
		instance.actionPanel.SetActive(false);
	}
	
} // class CanvasScript

} // namespace Nanosoft
