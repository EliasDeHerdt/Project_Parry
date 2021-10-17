using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    //Variables
    [SerializeField] protected Transform targetTransform; // What the camera will follow
    [SerializeField] protected Transform cameraTransform; // Actual transform of the camera
    [SerializeField] protected Transform cameraPivotTransform; // How the camera will turn on its swivel
    [SerializeField] protected float lookSpeed = 1f;
    [SerializeField] protected float followSpeed = 1f;
    [SerializeField] protected float pivotSpeed = 1f;
    [SerializeField] protected float minimumPivot = -25;
    [SerializeField] protected float maximumPivot = 30;

    public static CameraController singleton;
    public float mouseX;
    public float mouseY;

    protected float m_LookAngle;
    protected float m_PivotAngle;

    protected bool m_LookAtTarget = false;

    public bool LookAtTarget
    {
        get { return m_LookAtTarget; }
        set { m_LookAtTarget = value; }
    }

    //Functions
    private void Awake()
    {
        singleton = this;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void LateUpdate()
    {
        float delta = Time.deltaTime;

        FollowTarget(delta);
        HandleCameraRotation(delta);
    }

    //Simply follow the given target
    public void FollowTarget(float delta)
    {
        if (targetTransform == null)
            return;

        Vector3 targetPosition = Vector3.Lerp(transform.position, targetTransform.position, delta / followSpeed);
        transform.position = targetPosition;
    }

    //Move the camera if you move the mouse
    public void HandleCameraRotation(float delta)
    {
        if (targetTransform == null)
            return;

        Vector3 rotation = Vector3.zero;
        Quaternion targetRotation;
        if (!m_LookAtTarget)
        {
            m_LookAngle += (mouseX * lookSpeed * 0.00001f) / delta;
            rotation.y = m_LookAngle;
            targetRotation = Quaternion.Euler(rotation);
        }
        else
        {
            targetRotation = Quaternion.Lerp(transform.rotation, targetTransform.rotation,delta*5);
        }

        transform.rotation = targetRotation;

        m_PivotAngle -= (mouseY * pivotSpeed * 0.00001f) / delta;
        m_PivotAngle = Mathf.Clamp(m_PivotAngle, minimumPivot, maximumPivot);
        rotation = Vector3.zero;
        rotation.x = m_PivotAngle;

        targetRotation = Quaternion.Euler(rotation);
        cameraPivotTransform.localRotation = targetRotation;
    }

    public Vector3 GetDirectionVector()
    {
        return (targetTransform.position - cameraTransform.position).normalized;
    }

    public Transform GetCameraTransform()
    {
        return cameraTransform;
    }
}
 