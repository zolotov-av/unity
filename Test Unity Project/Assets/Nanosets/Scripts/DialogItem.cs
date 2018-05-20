using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

namespace Nanosoft
{

/**
 * Класс описывающий элемент диалога
 */
public class DialogItem
{
	
	/**
	 * ID элемента, может быть пустым
	 */
	[XmlAttribute("id")]
	public string id;
	
	/**
	 * Ссылка на спрайт лица персонажа
	 */
	public Sprite avatarSprite;
	
	/**
	 * Имя персонажа
	 */
	[XmlElement("avatar-name")]
	public string avatarName;
	
	/**
	 * Текстовое сообщение которое говорит NPC
	 */
	[XmlElement("message")]
	[TextArea(3,10)]
	public string message;
	
	/**
	 * Варианты ответов для игрока
	 */
	[XmlArray("actions")]
    [XmlArrayItem("dialog-action")]
	public DialogAction[] replies;
	
} // DialogItem

} // namespace Nanosoft
