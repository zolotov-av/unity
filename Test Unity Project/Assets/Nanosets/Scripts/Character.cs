﻿using UnityEngine;
using UnityEngine.AI;

namespace Nanosoft
{

/**
 * Базовый класс для всех персонажей
 */
public class Character: MonoBehaviour
{

	/**
	 * Ссылка на Rigidbody (твердое тело)
	 */
	protected Rigidbody rb;
	
	/**
	 * Ссылка на аниматор персонажа
	 */
	protected Animator animator;
	
	/**
	 * Ссылка на коллайдер персонажа
	 */
	protected CapsuleCollider capsule;
	
	/**
	 * Ссылка на NavMeshAgent персонажа
	 */
	protected NavMeshAgent navAgent;
	
	/**
	 * Флаг смерти персонажа
	 */
	protected bool dead = false;
	
	/**
	 * Текущее здоровье персонажа
	 */
	protected int currentHealth = 0;
	
	/**
	 * Максимально возможное здоровье персонажа
	 */
	public int maxHealth = 100;
	
	/**
	 * Инициализация
	 */
	protected void Init()
	{
		rb = GetComponent<Rigidbody>();
		animator = GetComponent<Animator>();
		capsule = GetComponent<CapsuleCollider>();
		navAgent = GetComponent<NavMeshAgent>();
		//sound = GetComponent<AudioSource>();
		
		dead = false;
		currentHealth = maxHealth;
	}
	
} // class Character

} // namespace Nanosoft