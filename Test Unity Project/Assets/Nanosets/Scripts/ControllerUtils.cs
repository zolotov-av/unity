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
