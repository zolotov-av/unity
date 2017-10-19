using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nanosoft
{

public class SimpleDialog: MonoBehaviour, IAction
{
	
	public string actionMessage = "";
	
	public DialogItem dialog;
	
	public PlayerController GetPlayer(Collider other)
	{
		var player = other.gameObject;
		if ( player ) return player.GetComponent<PlayerController>();
		return null;
	}
	
	/**
	 * Обработчик триггера, тут мы сохраняем ссылку на персонажа (и нужные ему
	 * объекты) в свое поле. Отмечаем все объекты которые надо переместить
	 * на новую карту как неудаляемые.
	 */
	public void OnTriggerEnter(Collider other)
	{
		Debug.Log(gameObject.name + "[SimpleAction].OnTriggerEnter()\n");
		var pb = other.gameObject.GetComponent<PlayerBehaviour>();
		if ( pb )
		{
			pb.SetAction(this);
		}
	}
	
	public void OnTriggerExit(Collider other)
	{
		Debug.Log("SimpleAction.OnTriggerExit()\n");
		var pb = other.gameObject.GetComponent<PlayerBehaviour>();
		if ( pb )
		{
			pb.RemoveAction(this);
		}
	}
	
	public string GetActionMessage()
	{
		return actionMessage;
	}
	
	public void RunAction(PlayerBehaviour pb)
	{
		Debug.Log("SimpleAction.RunAction()");
		QuestManager.StartDialog(dialog);
	}
	
} // class SimpleAction

} // namespace Nanosoft
