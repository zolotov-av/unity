using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nanosoft
{

/**
 * Менеджер квестов
 */
public class QuestManager: MonoBehaviour
{
	
	/**
	 * Ссылка на актуальный экземпляр менеджера квестов
	 */
	protected static QuestManager instance;
	
	/**
	 * Список квестов
	 */
	public Quest[] quests;
	
	void Awake()
	{
		if ( instance == null )
		{
			instance = this;
		}
		else
		{
			Debug.LogError("Only one QuestManager is allowed");
		}
	}
	
	/**
	 * Начать диалог
	 */
	public static void StartDialog(string avatarName, Sprite avatarPhoto, DialogItem dialog)
	{
		CanvasScript.SetAvatar(avatarName, avatarPhoto);
		CanvasScript.ShowDialog(dialog);
	}
	
	/**
	 * Запустить ответное действие
	 *
	 * Данная функция вызывается при нажатии соответствующей кнопки в диалоговом
	 * окне.
	 */
	public static void RunAction(DialogAction reply)
	{
		switch ( reply.actionType )
		{
		
		case DialogAction.ExitDialog:
			CanvasScript.instance.CloseDialog();
			break;
		
		case DialogAction.GotoNext:
			CanvasScript.ShowDialog(reply.nextDialog);
			break;
		
		default:
			Debug.LogError("Unknown DialogAction's type: " + reply.actionType);
			break;
		
		}
	}
	
} // class QuestManager

} // namespace Nanosoft
