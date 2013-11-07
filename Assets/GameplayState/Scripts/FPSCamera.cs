using UnityEngine;
using System.Collections;

public class FPSCamera : MonoBehaviour {

    public enum RotationAxes { MouseXAndY = 0, MouseX = 1, MouseY = 2 };

    public RotationAxes axes = RotationAxes.MouseXAndY;
    public float SensitivityX = 15.0f;
    public float SensitivityY = 15.0f;

    public float minimumX = -360.0f;
    public float maximumX = 360.0f;

    public float minimumY = -60.0f;
    public float maximumY = 60.0f;

    private float rotationX = 0.0f;
    private float rotationY = 0.0f;

    private Quaternion origRotation;

	void Start()
    {
        if (rigidbody)
        {
            rigidbody.freezeRotation = true;
        }

        origRotation = transform.localRotation;
	}
	
	void Update()
    {
        Quaternion yQuaternion;
        Quaternion xQuaternion;

        if (axes == RotationAxes.MouseXAndY)
        {
            rotationX += Input.GetAxis("Mouse X") * SensitivityX;
            rotationY += Input.GetAxis("Mouse Y") * SensitivityY;

            rotationX = ClampAngle(rotationX, minimumX, maximumX);
            rotationY = ClampAngle(rotationY, minimumY, maximumY);

            xQuaternion = Quaternion.AngleAxis(rotationX, Vector3.up);
            yQuaternion = Quaternion.AngleAxis(rotationY, Vector3.left);

            transform.localRotation = origRotation * xQuaternion * yQuaternion;
        }
        else if (axes == RotationAxes.MouseX)
        {
            rotationX += Input.GetAxis("Mouse X") * SensitivityX;
            rotationX = ClampAngle(rotationX, minimumX, maximumX);

            xQuaternion = Quaternion.AngleAxis(rotationX, Vector3.up);
            transform.localRotation = origRotation * xQuaternion;
        }
        else
        {
            rotationY += Input.GetAxis("Mouse Y") * SensitivityY;
            rotationY = ClampAngle(rotationY, minimumY, maximumY);

            yQuaternion = Quaternion.AngleAxis(rotationY, Vector3.left);
            transform.localRotation = origRotation * yQuaternion;
        }
	}

    private float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360.0f)
        {
            angle += 360.0f;
        }
        if (angle > 360.0f)
        {
            angle -= 360.0f;
        }

        return Mathf.Clamp(angle, min, max);
    }
}
