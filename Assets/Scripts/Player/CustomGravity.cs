using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CustomGravity : MonoBehaviour
{
    [SerializeField]
    private float gravityScale = 1.0f;
    public static float globalGravity = -9.8f;
    private Rigidbody rb;

    void OnEnable()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
    }

    void FixedUpdate()
    {
        rb.AddForce(globalGravity * gravityScale * Vector3.up, ForceMode.Acceleration); 
    }
}
