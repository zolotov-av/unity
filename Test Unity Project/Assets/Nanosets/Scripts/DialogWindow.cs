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
public class DialogWindow: MonoBehaviour
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
	 * Ссылки на элементы для вариантов ответов
	 */
	private GameObject[] items;
	
	void Awake()
	{
		gameObject.SetActive(false);
		actorPanel.SetActive(false);
		repliesPanel.SetActive(false);
		
		replyCapacity = 3;
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
		gameObject.SetActive(true);
	}
	
	public void SetAvatar(string name, Sprite photo)
	{
		avatarName.text = name;
		avatarPhoto.sprite = photo;
		avatarPanel.SetActive(true);
		actorPanel.SetActive(true);
	}
	
	/**
	 * Отобразить диалог
	 */
	public void ShowDialog(DialogItem dialog)
	{
		messageText.text = dialog.message;
		
		// TODO resize and scrolling
		DialogAction[] replies = dialog.replies;
		int len = replies.Length;
		if ( len > replyCapacity ) len = replyCapacity;
		for(int i = 0; i < len; i++)
		{
			var obj = items[i];
			obj.transform.Find("Text").gameObject.GetComponent<Text>().text = replies[i].reply;
			var btnClick = obj.GetComponent<Button>().onClick;
			btnClick.RemoveAllListeners();
			btnClick.AddListener( replies[i].RunReply );
			obj.SetActive(true);
		}
		for(int i = len; i < replyCount; i++)
		{
			items[i].SetActive(false);
		}
		replyCount = len;
		
		repliesPanel.SetActive( replyCount > 0 );
		gameObject.SetActive(true);
		
		if ( replyCount > 0 )
		{
			items[0].GetComponent<Button>().Select();
		}
	}
	
	/**
	 * Закрыть диалоговое окно
	 */
	public void CloseDialog()
	{
		gameObject.SetActive(false);
	}
	
} // class DialogWindow

} // namespace Nanosoft
