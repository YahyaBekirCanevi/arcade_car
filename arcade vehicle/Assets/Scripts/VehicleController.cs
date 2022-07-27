using UnityEngine;

public class VehicleController : MonoBehaviour
{
    #region parameters
    [Header("Core")]
    [SerializeField] private float motorForce;
    [SerializeField] private float breakForce;
    [SerializeField] private float downForce = 50;
    [SerializeField] private float minWorldLevel;
    [SerializeField] private Wheel[] frontWheels;
    [SerializeField] private Wheel[] backWheels;
    [SerializeField] private Transform centerOfMass;

    [Header("UI")]
    [SerializeField] private TMPro.TMP_Text text;
    #endregion

    #region values
    private const string HORIZONTAL = "Horizontal", VERTICAL = "Vertical";
    private float horizontalInput, verticalInput;
    private bool isBreaking;
    private Rigidbody rb;
    #endregion
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = centerOfMass.localPosition;
    }
    private void FixedUpdate()
    {
        GetInput();
        HandleWheels();
        AddDownForce();
    }
    private void GetInput()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (transform.position.y < minWorldLevel)
            {
                transform.position = Vector3.zero;
            }
            Vector3 euler = transform.eulerAngles;
            euler.x = 0;
            euler.z = 0;
            transform.eulerAngles = euler;
        }
        horizontalInput = Input.GetAxis(HORIZONTAL);
        verticalInput = Input.GetAxis(VERTICAL);
        isBreaking = Input.GetKey(KeyCode.Space);
    }
    private void HandleWheels()
    {
        int KPH = ((int)(rb.velocity.magnitude * 3.6f));
        text.SetText(KPH.ToString());
        foreach (Wheel w in frontWheels)
        {
            w.HandleSteering(horizontalInput: horizontalInput);
            w.ApplyBreak(isBreaking: isBreaking, breakForce: breakForce);
            //w.Drift(isBreaking: isBreaking, chageStiffness: false);
        }
        foreach (Wheel w in backWheels)
        {
            w.HandleMotor(verticalInput: verticalInput, motorForce: motorForce);
            w.ApplyBreak(isBreaking: isBreaking, breakForce: breakForce);
            w.Drift(isBreaking: isBreaking, chageStiffness: true);
        }
    }
    private void AddDownForce()
    {
        rb.AddForce(Vector3.down * downForce * rb.velocity.magnitude);
    }
}
