using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Nanosoft
{

/**
 * Вспомогательные функции для контроллеров персонажей
 */
public class ControllerUtils
{
	
	/**
	 * Вернуть координаты центра капсулы (в мировых координатах)
	 */
	public static Vector3 CapsuleCenter(CapsuleCollider capsule)
	{
		return capsule.transform.TransformPoint(capsule.center);
	}
	
	/**
	 * Вернуть координаты основания капсулы (в мировых координатах)
	 */
	public static Vector3 CapsuleBase(CapsuleCollider capsule)
	{
		Vector3 p = capsule.center;
		p.y = p.y - capsule.height / 2;
		return capsule.transform.TransformPoint(p);
	}
	
	/**
	 * Проверка стоит ли персонаж на земле
	 */
	public static bool GroundCheck(CapsuleCollider capsule)
	{
		Vector3 position = CapsuleBase(capsule);
		
		// зазор на расстоянии которого будет определяться столкновение
		const float gap = 0.4f;
		
		RaycastHit hitInfo;
		
		float s_radius = capsule.radius;
		Vector3 s_position = position + Vector3.up * (s_radius + gap);
		
		return Physics.SphereCast(s_position, s_radius, Vector3.down, out hitInfo,
			2f*gap, Physics.AllLayers, QueryTriggerInteraction.Ignore);
	}
	
	/**
	 * Проверка стоит ли персонаж на земле
	 */
	public static bool GroundCheck(Vector3 position, CapsuleCollider capsule)
	{
		// зазор на расстоянии которого будет определяться столкновение
		const float gap = 0.05f;
		
		RaycastHit hitInfo;
		
		float s_radius = capsule.radius - gap;
		Vector3 s_position = position + Vector3.up * (s_radius + gap);
		
		return Physics.SphereCast(s_position, s_radius, Vector3.down, out hitInfo,
			2f*gap, Physics.AllLayers, QueryTriggerInteraction.Ignore);
	}
	
} // class ControllerUtils

} // namespace Nanosoft
