using UnityEngine;

public class SpriteBillboard : MonoBehaviour
{
    [SerializeField]
    private bool changeSpriteBaseOnRotation;
    [SerializeField]
    float backAngle = 65f, sideAngle = 155f;
    [SerializeField]
    private Animator animator;
    [SerializeField]
    private SpriteRenderer spriteRenderer;
    [SerializeField]
    private Transform mainTransform;
    private Transform cameraTransform;


    private void Start()
    {
        cameraTransform = Camera.main.transform;
    }

    private void LateUpdate()
    {
        mainTransform.rotation = Quaternion.Euler(0f, cameraTransform.eulerAngles.y, 0f);

        if (changeSpriteBaseOnRotation)
        {
            // Note: this is the vector between the location of this object to the camera
            Vector3 camForwardVector = new Vector3(transform.position.x - cameraTransform.position.x, 0f, transform.position.z - cameraTransform.position.z);;
            float signedAngle = Vector3.SignedAngle(transform.forward, camForwardVector, Vector3.up);

            Vector2 animationDirection;

            float angle = Mathf.Abs(signedAngle);

            spriteRenderer.flipX = false; 
            if (angle < backAngle)
            {
                animationDirection = new Vector2(0f, -1f);
            }
            else if (angle < sideAngle)
            {
                if (signedAngle < 0)
                {
                    animationDirection = new Vector2(-1f, 0f);
                }
                else
                {
                    animationDirection = new Vector2(1f, 0f);
                    spriteRenderer.flipX = true; 
                }
            }
            else
            {
                animationDirection = new Vector2(0f, 1f);
            }

            animator.SetFloat("moveX", animationDirection.x);
            animator.SetFloat("moveY", animationDirection.y);
        }
    }
}
