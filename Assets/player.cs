using UnityEngine;

public class player : MonoBehaviour
{
    float playerHorizontalSpeed = 0,
          playerVerticalSpeed = 0,
          walkSpeed = 2f,
          runSpeed = 10f,
          jumpHeight = 4f;
    Vector2 playerState = new Vector2(0f, 0f);
    float gravity = -10f;
    float cameraOffsetFromPlayer = 3f;
    [SerializeField] Vector2 cameraMovementSensitivity = new Vector2(2f,1f);
    Animator animator;
    GameObject mainCamera;
    CharacterController characterController;

    void Start()
    {
        animator = this.GetComponent<Animator>();
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        mainCamera.transform.localPosition = new Vector3(0f, 1.5f, -1 * cameraOffsetFromPlayer);
        characterController = this.GetComponent<CharacterController>();
    }

    void Update()
    {
        PlayerMovement();
        CameraMovement();
    }

    void PlayerMovement()
    {
        print(characterController.isGrounded);
        Vector2 playerInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        float inputMagnitude = playerInput.magnitude;
        playerHorizontalSpeed = 0;
        // walk and run
        if (playerInput.magnitude > Mathf.Epsilon)
        {
            AlignPlayerWithCamera();
            playerHorizontalSpeed = walkSpeed;
            playerState.x = playerInput.y / 2;
            if (Input.GetKey(KeyCode.LeftShift))
            {
                playerState.x = playerState.x * 2;
                playerHorizontalSpeed = runSpeed;
            }
            if(!characterController.isGrounded) {
                playerState.x = 0f;
            }
        }

        playerVerticalSpeed += gravity * Time.deltaTime;
        if(characterController.isGrounded)
        {
            playerState.y = 0f;
        }

        // jump
        if (Input.GetKey(KeyCode.Space) && characterController.isGrounded && animator.GetFloat("InputY") < Mathf.Epsilon)
        {
           playerState.y = 1f;
           playerVerticalSpeed = Mathf.Sqrt(-2 * gravity * jumpHeight);
        }

        // slide
        if (Input.GetKey(KeyCode.LeftControl) && characterController.isGrounded && animator.GetFloat("InputY") < Mathf.Epsilon)
        {
            playerState.y = -1f;
        }

        Vector3 totalVelocity = this.transform.forward * playerHorizontalSpeed + this.transform.up * playerVerticalSpeed;
        characterController.Move(totalVelocity * Time.deltaTime);
        animator.SetFloat("InputX", playerState.x);
        animator.SetFloat("InputY", playerState.y);
    }

    void CameraMovement()
    {
        Vector2 userCameraInput = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")) * cameraMovementSensitivity;
        mainCamera.transform.RotateAround(this.transform.position, this.transform.up, userCameraInput.x);
        float cameraRotationX = mainCamera.transform.localRotation.eulerAngles.x;
        cameraRotationX = cameraRotationX > 180 ? cameraRotationX - 360 : cameraRotationX;
        if ((cameraRotationX > 45f && userCameraInput.y > 0) || (cameraRotationX < -20f && userCameraInput.y < 0)) { return; }
        mainCamera.transform.RotateAround(this.transform.position, mainCamera.transform.right, userCameraInput.y);
    }

    void AlignPlayerWithCamera()
    {
        Vector3 mainCameraRotation = mainCamera.transform.rotation.eulerAngles;
        Vector3 mainCameraPosition = mainCamera.transform.position;
        Vector3 playerRotation = this.transform.localRotation.eulerAngles;
        this.transform.rotation = Quaternion.Euler(new Vector3(playerRotation.x, mainCameraRotation.y, playerRotation.z));
        mainCamera.transform.position = mainCameraPosition;
        mainCamera.transform.rotation = Quaternion.Euler(mainCameraRotation);
    }
}
