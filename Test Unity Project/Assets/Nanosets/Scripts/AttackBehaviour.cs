﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nanosoft
{

/**
 * Базовый класс для всех объектов-атак, всё что летит в игрока или противника
 */
public class AttackBehaviour: MonoBehaviour
{
	
	/**
	 * Ссылка на персонажа, который запустил атаку
	 */
	public GameObject owner;
	
	public int minDamage = 2;
	public int maxDamage = 3;
	
}

} // namespace Nanosoft
