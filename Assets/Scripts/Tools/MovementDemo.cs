using UnityEngine;
using System.Collections;

public class MovementDemo : MonoBehaviour
{

    public float Speed = 10f;
    public float rotateSpeed = 1000f;
    public float scrollSpeed = 100f;
    private Vector3 CameraR;
    public Transform CameraTransform;
    public KeyCode forward = KeyCode.W;
    public KeyCode backward = KeyCode.S;
    public KeyCode left = KeyCode.A;
    public KeyCode right = KeyCode.D;
    public KeyCode down = KeyCode.Q;
    public KeyCode up = KeyCode.E;

    void Start()
    {
        
        //CameraTransform = GameObject.Find("Camera").transform;
        CameraTransform = this.transform;
        CameraR = CameraTransform.rotation.eulerAngles;
    }

    void Update()
    {
        Vector3 Face = CameraTransform.rotation * Vector3.forward;
        Face = Face.normalized;

        Vector3 Left = CameraTransform.rotation * Vector3.left;
        Left = Left.normalized;

        Vector3 Right = CameraTransform.rotation * Vector3.right;
        Right = Right.normalized;

        if (Input.GetMouseButton(1))
        {
            float yRot = Input.GetAxis("Mouse X");
            float xRot = Input.GetAxis("Mouse Y");

            Vector3 R = CameraR + new Vector3(-xRot, yRot, 0f);

            CameraR = Vector3.Slerp(CameraR, R, rotateSpeed * Time.deltaTime);

            CameraTransform.rotation = Quaternion.Euler(CameraR);
        }

        if (Input.GetKeyDown(forward)) 
        {
            CameraTransform.position += Face * Speed * Time.deltaTime;
        }

        if (Input.GetKeyDown(left))
        {
            CameraTransform.position += Left * Speed * Time.deltaTime;
        }

        if (Input.GetKeyDown(right))
        {
            CameraTransform.position += Right * Speed * Time.deltaTime;
        }

        if (Input.GetKeyDown(backward))
        {
            CameraTransform.position -= Face * Speed * Time.deltaTime;
        }

        if (Input.GetKeyDown(up))
        {
            CameraTransform.position -= Vector3.up * Speed * Time.deltaTime;
        }

        if (Input.GetKeyDown(down))
        {
            CameraTransform.position += Vector3.up * Speed * Time.deltaTime;
        }

        float scrollWhell = Input.GetAxis("Mouse ScrollWheel");

        if (scrollWhell!=0)
        {
            CameraTransform.position += Face * scrollWhell * scrollSpeed * Speed * Time.deltaTime;
        }

    }
}
