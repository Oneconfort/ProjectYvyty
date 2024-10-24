using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class CameraController : MonoBehaviour
{
    public static CameraController cameraController;
    public Vector3 offset;

    Vector3 offsetCamCaverna = new Vector3(-1.36f, 12.69f, 14.31f);
    public Camera cam;
    bool useOffsetValues, invertY;
    public float rotateSpeed, maxViewAngle, minViewAngle;
    public Transform pivot;
   public bool cave;
    void Awake()
    {
        if (cameraController == null)
        {
            cameraController = this;
        }
    }

    void LateUpdate()
    {
        MoveMainCamera();
        MoveCameraCaverna();
    }



    void Start()
    {
        if (!useOffsetValues)
        {
            offset = GameController.controller.Player.transform.position - transform.position;
        }
        pivot.transform.position = GameController.controller.Player.transform.position;

        pivot.transform.parent = null;

        cam = GetComponent<Camera>();
    }




    private void MoveMainCamera()
    {
        if (GameController.controller.uiController.visivelpause || GameController.controller.Player.caverna == false || cave == true) return;
        cam.orthographic = false;
        pivot.transform.position = GameController.controller.Player.transform.position;

        float h = Input.GetAxis("Mouse X") * rotateSpeed;
        pivot.transform.Rotate(0, h, 0);
        float v = Input.GetAxis("Mouse Y") * rotateSpeed;

        if (invertY)
        {
            pivot.transform.Rotate(v, 0, 0);
        }
        else
        {
            pivot.transform.Rotate(-v, 0, 0);
        }

        if (pivot.rotation.eulerAngles.x > maxViewAngle && pivot.rotation.eulerAngles.x < 180f)
        {
            pivot.rotation = Quaternion.Euler(maxViewAngle, pivot.rotation.eulerAngles.y, pivot.rotation.eulerAngles.z);
        }
        if (pivot.rotation.eulerAngles.x > 180f && pivot.rotation.eulerAngles.x < 360f + minViewAngle)
        {
            pivot.rotation = Quaternion.Euler(360f + minViewAngle, pivot.rotation.eulerAngles.y, pivot.rotation.eulerAngles.z);
        }

        float desiredYAngle = pivot.transform.eulerAngles.y;
        float desiredXAngle = pivot.transform.eulerAngles.x;

        Quaternion rotation = Quaternion.Euler(desiredXAngle, desiredYAngle, 0);
        transform.position = GameController.controller.Player.transform.position - (rotation * offset);

        if (transform.position.y < GameController.controller.Player.transform.position.y)
        {
            transform.position = new Vector3(transform.position.x, GameController.controller.Player.transform.position.y - 0.5f, transform.position.z);
        }

        transform.LookAt(GameController.controller.Player.transform.position);
    }

    void MoveCameraCaverna()
    {
        if (GameController.controller.uiController.visivelpause || GameController.controller.Player.caverna == true || cave == true) return;

        Vector3 novaPosicao = GameController.controller.Player.transform.position + offsetCamCaverna;
        novaPosicao.y = 7.489f;
        transform.position = novaPosicao;
    }
   
}