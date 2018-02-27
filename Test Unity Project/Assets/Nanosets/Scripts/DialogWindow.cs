using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Nanosoft
{

/**
 * Класс управляющий диалоговым окном
 */
public class DialogWindow: WindowBehaviour
{
	
	/**
	 * Ссылка на префаб ответа
	 */
	public GameObject replyPrefab;
	
	/**
	 * Ссылка на панель с аватаркой персонажа
	 */
	public GameObject avatarPanel;
	
	/**
	 * Ссылка на панель с именем персонажа
	 */
	public GameObject actorPanel;
	
	/**
	 * Ссылка панель с сообщением
	 */
	public GameObject messagePanel;
	
	/**
	 * Ссылка на панель с вариантами ответов/действий
	 */
	public GameObject repliesPanel;
	
	/**
	 * Ссылка на объект Text для диалогового сообщения
	 */
	public Text messageText;
	
	/**
	 * Ссылка на объект Image для аватарки персонажа
	 */
	public Image avatarPhoto;
	
	/**
	 * Ссылка на объект Text для имени персонажа
	 */
	public Text avatarName;
	
	/**
	 * Число вариантов ответов
	 */
	private int replyCount;
	
	/**
	 * Максимальное число вариантов ответов
	 */
	private int replyCapacity;
	
	/**
	 * Индекс выделенного ответа
	 */
	private int selectedReply;
	
	/**
	 * Ссылки на элементы для вариантов ответов
	 */
	private GameObject[] items;
	
	private bool lockCursor = true;
	
	public void Init()
	{
		actorPanel.SetActive(false);
		repliesPanel.SetActive(false);
		
		replyCapacity = 5;
		replyCount = 0;
		items = new GameObject[replyCapacity];
		Transform t = repliesPanel.transform;
		for(int i = 0; i < replyCapacity; i++)
		{
			GameObject obj = Instantiate(replyPrefab, t);
			obj.SetActive(false);
			items[i] = obj;
		}
	}
	
	/**
	 * Отобразить сообщение в диалоговом окне
	 */
	public void ShowMessage(string message)
	{
		messageText.text = message;
		repliesPanel.SetActive(false);
		avatarPanel.SetActive(false);
		ShowModal();
	}
	
	public void SetAvatar(string name, Sprite photo)
	{
		avatarName.text = name;
		avatarPhoto.sprite = photo;
		avatarPanel.SetActive(true);
		actorPanel.SetActive(true);
	}
	
	public void SetLockCursor(bool state)
	{
		lockCursor = state;
	}
	
	/**
	 * Отобразить диалог
	 */
	public void ShowDialog(DialogItem dialog)
	{
		if ( lockCursor )
		{
			Cursor.visible = false;
			Cursor.lockState = CursorLockMode.Locked;
		}
		
		// TODO it right
		TabletController.run = false;
		TabletController.velocity = 0f;
		
		messageText.text = dialog.message;
		
		// TODO resize and scrolling
		replyCount = 0;
		foreach(DialogAction action in dialog.replies)
		{
			if ( action.CheckCondition() )
			{
				var obj = items[replyCount];
				obj.transform.Find("Text").gameObject.GetComponent<Text>().text = action.reply;
				var btnClick = obj.GetComponent<Button>().onClick;
				btnClick.RemoveAllListeners();
				btnClick.AddListener( action.RunReply );
				obj.SetActive(true);
				replyCount++;
			}
		}
		
		for(int i = replyCount; i < replyCapacity; i++)
		{
			items[i].SetActive(false);
		}
		
		repliesPanel.SetActive( replyCount > 0 );
		
		if ( replyCount > 0 )
		{
			selectedReply = 0;
			items[0].GetComponent<Button>().Select();
		}
		else
		{
			selectedReply = -1;
			Debug.LogError("replyCount <= 0");
		}
		
		ShowModal();
	}
	
	/**
	 * Закрыть диалоговое окно
	 */
	public void CloseDialog()
	{
		EventSystem.current.SetSelectedGameObject(null);
		Hide();
		
		Cursor.visible = true;
		Cursor.lockState = CursorLockMode.None;
	}
	
	/**
	 * Обработка ввода с клавиатуры
	 */
	public override void HandleInput()
	{
		if ( replyCount <= 0 ) return;
		
		if ( Input.GetKey(KeyCode.Escape) )
		{
			// TODO убрать CanvasScript от сюда, костыль...
			CanvasScript.CloseDialog();
			return;
		}
		
		if ( Input.GetKeyDown(KeyCode.UpArrow) )
		{
			selectedReply = (selectedReply + replyCount - 1) % replyCount;
			items[selectedReply].GetComponent<Button>().Select();
		}
		
		if ( Input.GetKeyDown(KeyCode.DownArrow) )
		{
			selectedReply = (selectedReply + 1) % replyCount;
			items[selectedReply].GetComponent<Button>().Select();
		}
		
		float scroll = Input.GetAxis("Mouse ScrollWheel");
		if ( scroll > 0f )
		{
			selectedReply = (selectedReply + replyCount - 1) % replyCount;
			items[selectedReply].GetComponent<Button>().Select();
		}
		else if ( scroll < 0f )
		{
			selectedReply = (selectedReply + 1) % replyCount;
			items[selectedReply].GetComponent<Button>().Select();
		}
		
		if ( Input.GetKeyDown(KeyCode.Return) )
		{
			items[selectedReply].GetComponent<Button>().onClick.Invoke();
			return;
		}
		
		if ( lockCursor && Input.GetMouseButtonDown(0) )
		{
			items[selectedReply].GetComponent<Button>().onClick.Invoke();
		}
	}
	
} // class DialogWindow

} // namespace Nanosoft
