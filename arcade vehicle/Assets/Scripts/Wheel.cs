using UnityEngine;

public class Wheel : MonoBehaviour
{
    #region parameters
    [SerializeField] private bool isLeft;
    [SerializeField] private float angle = 6;
    [SerializeField] private float defaultForwardFrictionValue = 0.5f;
    [SerializeField] private float defaultSidewayFrictionValue = 0.75f;
    [SerializeField] private float driftForwardFrictionValue = 1f;
    [SerializeField] private float driftSidewayFrictionValue = 0.6f;
    #endregion

    #region values
    private WheelCollider wheelCollider;
    private Transform wheelTransform;
    #endregion
    private void Awake()
    {
        wheelCollider = GetComponentInChildren<WheelCollider>();
        wheelTransform = GetComponentInChildren<MeshRenderer>().transform;
    }
    private void Update()
    {
        UpdateTire();
    }
    public void HandleMotor(float verticalInput, float motorForce)
    {
        wheelCollider.motorTorque = verticalInput * motorForce;
    }
    public void HandleSteering(float horizontalInput)
    {
        wheelCollider.steerAngle = AckermanSteering(horizontalInput);
    }
    private float AckermanSteering(float horizontalInput)
    {
        float direction = isLeft ? 1 : -1;
        if (horizontalInput > 0)
        {
            return Mathf.Rad2Deg * Mathf.Atan(2.55f / (angle + (direction * (1.5f / 2)))) * horizontalInput;
        }
        else if (horizontalInput < 0)
        {
            return Mathf.Rad2Deg * Mathf.Atan(2.55f / (angle + (-direction * (1.5f / 2)))) * horizontalInput;
        }
        else
        {
            return 0;
        }
    }
    public void ApplyBreak(bool isBreaking, float breakForce)
    {
        wheelCollider.brakeTorque = isBreaking ? breakForce : 0;
    }
    public void Drift(bool isBreaking, bool chageStiffness)
    {
        //forward friction
        WheelFrictionCurve curve = wheelCollider.forwardFriction;
        curve.asymptoteValue = isBreaking ? driftForwardFrictionValue : defaultForwardFrictionValue;
        curve.stiffness = Mathf.Lerp(curve.stiffness, isBreaking && chageStiffness ? 0.5f : 1, Time.fixedDeltaTime * 3);
        wheelCollider.forwardFriction = curve;

        //sideway friction
        curve = wheelCollider.sidewaysFriction;
        curve.asymptoteValue = isBreaking ? driftSidewayFrictionValue : defaultSidewayFrictionValue;
        curve.stiffness = Mathf.Lerp(curve.stiffness, isBreaking && chageStiffness ? 0.5f : 1, Time.fixedDeltaTime * 3);
        wheelCollider.sidewaysFriction = curve;
    }
    private void UpdateTire()
    {
        Vector3 pos;
        Quaternion rot;
        wheelCollider.GetWorldPose(out pos, out rot);
        wheelTransform.rotation = rot;
        wheelTransform.position = pos;
    }
}
