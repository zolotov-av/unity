using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nanosoft
{

/**
 * Класс описывающий элемент диалога
 */
[System.Serializable]
public class DialogItem
{
	
	/**
	 * Ссылка на спрайт лица персонажа
	 */
	public Sprite avatarSprite;
	
	/**
	 * Имя персонажа
	 */
	public string avatarName;
	
	/**
	 * Текстовое сообщение которое говорит NPC
	 */
	[TextArea(3,10)]
	public string message;
	
	/**
	 * Варианты ответов для игрока
	 */
	public DialogAction[] replies;
	
} // DialogItem

} // namespace Nanosoft
