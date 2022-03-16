using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public float speed = 5;
    CharacterController cc;
    public float gravity = -20;  //중력 가속도의 크기
    float yVelocity = 0;  //수직 속도
    public float jumpPower = 5;
    private bool doubleJump = false;  //2단 점프를 구현하기 위한 플래그
    public OVRCameraRig oVRCamera;
    // Start is called before the first frame update
    void Start()
    {
        cc = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        float h = ARAVRInput.GetAxis("Horizontal");  //사용자 입력을 받는다
        float v = ARAVRInput.GetAxis("Vertical");  //캐릭터 이동

        float Rh = ARAVRInput.GetAxis("Horizontal", ARAVRInput.Controller.RTouch); //사용자 입력을 받는다
        float Rv = ARAVRInput.GetAxis("Vertical", ARAVRInput.Controller.RTouch);  //카메라 회전
        Vector3 dir = new Vector3(h, 0, v);  //방향을 만든다
        

        //dir = Camera.main.transform.TransformDirection(dir); //pc 카메라 회전
        dir = oVRCamera.centerEyeAnchor.transform.TransformDirection(dir);  //오큘러스 카메라 회전

        yVelocity += gravity * Time.deltaTime;  //중력을 표현한 등가속도 공식
        if (cc.isGrounded)
        {
            yVelocity = 0;
        }
        //사용자가 점프 버튼을 누르면 속도에 점프 크기를 할당한다. 
        if (ARAVRInput.GetDown(ARAVRInput.Button.Two, ARAVRInput.Controller.RTouch))
        {
            if (cc.isGrounded)  //캐릭터가 땅에 붙어있다면
            {
                yVelocity = jumpPower;
                doubleJump = true;
            }
            else if (doubleJump)  //2단 점프
            {
                yVelocity = jumpPower;
                doubleJump = false;
            }
        }
        dir.y = yVelocity;

        cc.Move(dir * speed * Time.deltaTime);  //이동을 한다
        float h_r = ARAVRInput.GetAxis("Horizontal", ARAVRInput.Controller.RTouch);
        transform.Rotate(0, h_r * speed, 0);
        //오른쪽 컨트롤러 스틱으로 이동방향 조정(카메라가 보는 방향)
    }
}
