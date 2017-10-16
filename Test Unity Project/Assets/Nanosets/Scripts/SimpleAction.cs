using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nanosoft
{

public class SimpleAction: MonoBehaviour, IAction
{
	
	public string actionMessage = "";
	
	[TextArea(3,10)]
	public string runMessage = "";
	
	private bool empty = false;
	
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
		Debug.Log("SimpleAction.OnTriggerEnter()\n");
		if ( !empty )
		{
			var pb = other.gameObject.GetComponent<PlayerBehaviour>();
			if ( pb )
			{
				pb.SetAction(this);
			}
		}
	}
	
	public void OnTriggerExit(Collider other)
	{
		Debug.Log("SimpleAction.OnTriggerExit()\n");
		if ( !empty )
		{
			var pb = other.gameObject.GetComponent<PlayerBehaviour>();
			if ( pb )
			{
				pb.RemoveAction(this);
			}
		}
	}
	
	public string GetActionMessage()
	{
		return actionMessage;
	}
	
	public void RunAction(PlayerBehaviour pb)
	{
		Debug.Log("SimpleAction.RunAction()");
		pb.RemoveAction(this);
		empty = true;
		gameObject.GetComponent<Collider>().enabled = false;
		CanvasScript.ShowMessage(runMessage);
	}
	
} // class SimpleAction

} // namespace Nanosoft
