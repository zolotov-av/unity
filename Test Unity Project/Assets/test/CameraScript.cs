using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraScript : MonoBehaviour {

    //private Camera cam;
    public GameObject target;
    public Text camdbg;
    
    [Header("Camera Settings")]
    public float height = 1.4f;
    public float startDistance = 3f;
	public float startAngle = 20f;

    private float yMouseSensitivity = 3f;
	
    private float mouseX = 0f;
    private float mouseY = 0f;
    private float yMinLimit = -40f;
    private float yMaxLimit = 80f;

    private float distance = 0f;
    private float camAngle = 0f;
    
    #region Utils

    protected string coord(Vector3 v)
    {
        return ((int)v.x).ToString() + ", " + ((int)v.y).ToString() + ", " + ((int)v.z).ToString();
    }

    protected void DbgUpdate()
    {
        if (camdbg)
        {
            camdbg.text = "v22\n" +
				"CamPos: " + coord(transform.position) + "\n" +
                "TargetPos: " + coord(target.transform.position) + "\n" +
                "distance: " + string.Format("{0:0.00}", distance) + "\n" +
                "angle: " + string.Format("{0:0.00}", camAngle) + "\n" +
				"mouse: " + string.Format("{0:0.00}, {1:0.00}", mouseX, mouseY);
        }
    }

    public static float ClampAngle(float angle, float min, float max)
    {
        while (angle > 360) angle -= 360;
        while (angle < -360) angle += 360;
        return Mathf.Clamp(angle, min, max);
    }

    #endregion

	public void UpdateOptions(float distanceDelta, float angleDelta)
	{
		distance -= distanceDelta;
        distance = Mathf.Clamp(distance, 1f, 5f);
		
        camAngle -= angleDelta * yMouseSensitivity;
		camAngle = ClampAngle(camAngle, yMinLimit, yMaxLimit);
	}

    // Use this for initialization
    void Start () {
		distance = startDistance;
		camAngle = startAngle;
    }

    // Update is called once per frame
    protected virtual void LateUpdate()
    {
        Vector3 tp = target.transform.position + target.transform.up * height;

        float rad = camAngle * Mathf.PI / 180;
        Vector3 cv = target.transform.up * (Mathf.Sin(rad) * distance);
        Vector3 sv = target.transform.forward * (-Mathf.Cos(rad) * distance);
        transform.position = tp + cv + sv;

        transform.LookAt(tp);

        DbgUpdate();
    }
}
