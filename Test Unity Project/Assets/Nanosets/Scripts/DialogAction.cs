using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nanosoft
{

/**
 * Класс описывающий вариант ответа/действия в диалоге
 */
[System.Serializable]
public class DialogAction
{
	
	/**
	 * Текст ответа
	 */
	public string reply;
	
	/**
	 * Тип действия
	 */
	public int actionType;
	
	/**
	 * Действие - выход из диалога, завершение диалога
	 */
	public const int ExitDialog = 0;
	
	/**
	 * Действие - перейти на следующий диалог
	 */
	public const int GotoNext = 1;
	
	/**
	 * Ссылка на следующий диалог
	 */
	public DialogItem nextDialog;
	
	/**
	 * Обработчик события для кнопки.
	 *
	 * Возможно не самое лучше место для этой функции, но пока лучшего не
	 * придумал.
	 */
	public void RunReply()
	{
		QuestManager.RunAction(this);
	}
	
} // class DialogAction

} // namespace Nanosoft
