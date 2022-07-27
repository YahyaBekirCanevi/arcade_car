using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    #region parameters
    [SerializeField] private Transform follow;
    [SerializeField] private Vector3 positionOffset;
    [SerializeField] private Camera cam;
    [SerializeField] private float moveSpeed, rotationSpeed;
    [SerializeField] private float minfieldOfView = 68, maxfieldOfView = 110;
    #endregion
    #region values
    private float fieldOfView;
    #endregion
    private void Awake()
    {
        cam.transform.localPosition = positionOffset;
    }
    void FixedUpdate()
    {
        HandleView();
        HandlePosition();
        HandleRotation();
    }

    private void HandleView()
    {
        Vector3 direction = follow.transform.position - transform.position;
        float offset = Mathf.Abs(direction.magnitude) * 3.6f * 0.01f * 3f;
        float target = offset * (maxfieldOfView - minfieldOfView) + minfieldOfView;
        target = target > maxfieldOfView
            ? maxfieldOfView : target < minfieldOfView
                ? minfieldOfView : target;

        fieldOfView = Mathf.Lerp(fieldOfView, target, Time.fixedDeltaTime);
        cam.fieldOfView = fieldOfView;
    }

    private void HandlePosition()
    {
        transform.position = Vector3.Lerp(
            transform.position,
            follow.transform.position,
            Time.fixedDeltaTime * moveSpeed
        );
    }
    private void HandleRotation()
    {
        Vector3 direction = follow.transform.position - transform.position;
        if (direction.magnitude < 0.2f) return;

        Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);
        transform.rotation = Quaternion.Lerp(
               transform.rotation,
               targetRotation,
               Time.deltaTime * rotationSpeed
           );
    }
}
