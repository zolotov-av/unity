using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

namespace Nanosoft
{

/**
 * Класс описывающий вариант ответа/действия в диалоге
 */
public class DialogAction
{
	
	/**
	 * Текст ответа
	 */
	[XmlElement("reply")]
	public string reply;
	
	/**
	 * Тип действия
	 */
	[XmlAttribute("type")]
	public int actionType;
	
	/**
	 * Тип действия
	 */
	[XmlAttribute("quest")]
	public string actionQuest;
	
	/**
	 * Тип действия
	 */
	[XmlAttribute("trigger")]
	public string actionTrigger;
	
	/**
	 * Тип действия
	 */
	[XmlAttribute("if")]
	public string condition;
	
	/**
	 * Действие - выход из диалога, завершение диалога
	 */
	public const int ExitDialog = 0;
	
	/**
	 * Действие - перейти на следующий диалог
	 */
	public const int GotoNext = 1;
	
	[XmlAttribute("goto")]
	public string nextId;
	
	/**
	 * Ссылка на следующий диалог
	 */
	[XmlElement("next-dialog")]
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
	
	public bool CheckCondition()
	{
		if ( condition == null || condition == "" ) return true;
		
		int pos = condition.IndexOf("!=");
		if ( pos >= 0 )
		{
			string key = condition.Substring(0, pos);
			string val = condition.Substring(pos+2);
			return QuestManager.GetQuestVar(actionQuest, key) != val;
		}
		
		pos = condition.IndexOf("=");
		if ( pos >= 0 )
		{
			string key = condition.Substring(0, pos);
			string val = condition.Substring(pos+1);
			
			return QuestManager.GetQuestVar(actionQuest, key) == val;
		}
		
		return false;
	}
	
} // class DialogAction

} // namespace Nanosoft
