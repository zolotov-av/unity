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
	
	private GameObject avatarPanel;
	private GameObject actorPanel;
	private GameObject messagePanel;
	private GameObject repliesPanel;
	
	public Text messageText;
	public Image avatarPhoto;
	public Text avatarName;
	
	private DialogItem dialog;
	
	private int replyCapacity;
	private int count;
	
	/**
	 * Ссылки на элементы
	 */
	private GameObject[] items;
	
	void Awake()
	{
		avatarPanel = transform.Find("AvatarPanel").gameObject;
		
		actorPanel = avatarPanel.transform.Find("ActorPanel").gameObject;
		actorPanel.SetActive(false);
		messagePanel = transform.Find("MessagePanel").gameObject;
		repliesPanel = messagePanel.transform.Find("Replies").gameObject;
		
		replyCapacity = 3;
		count = 0;
		items = new GameObject[replyCapacity];
		Transform t = repliesPanel.transform;
		for(int i = 0; i < replyCapacity; i++)
		{
			GameObject obj = Instantiate(replyPrefab, t);
			obj.SetActive(false);
			items[i] = obj;
		}
	}
	
	public void ShowMessage(string message)
	{
		messageText.text = message;
		repliesPanel.SetActive(false);
		avatarPanel.SetActive(false);
		gameObject.SetActive(true);
	}
	
	public void ShowDialog(DialogItem d)
	{
		dialog = d;
		messageText.text = dialog.message;
		
		// TODO resize and scrolling
		string[] replies = dialog.replies;
		int len = replies.Length;
		if ( len > replyCapacity ) len = replyCapacity;
		for(int i = 0; i < len; i++)
		{
			var obj = items[i];
			obj.transform.Find("Text").gameObject.GetComponent<Text>().text = replies[i];
			obj.SetActive(true);
		}
		for(int i = len; i < count; i++)
		{
			items[i].SetActive(false);
		}
		count = len;
		
		if ( count > 0 )
		{
			repliesPanel.SetActive(true);
		}
		else
		{
			repliesPanel.SetActive(false);
		}
		
		avatarPhoto.sprite = dialog.avatarSprite;
		avatarName.text = dialog.avatarName;
		avatarPanel.SetActive(true);
		actorPanel.SetActive(true);
		
		gameObject.SetActive(true);
		
		if ( count > 0 )
		{
			items[0].GetComponent<Button>().Select();
		}
	}
	
} // class DialogWindow

} // namespace Nanosoft
