using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public Transform crosshair; //crosshair 속성
    public Transform bulletImpact; //총알 파펀 효과
    ParticleSystem bulletEffect; // 총알 파편 파티클 시스템
    AudioSource bulletAudio; // 총알 발사 사운드
    public float rayDistance = 100f;
    

    void Start()
    {
        bulletEffect = bulletImpact.GetComponent<ParticleSystem>();
        bulletAudio = bulletImpact.GetComponent<AudioSource>();
        
    }

    void Update()
    {
        ARAVRInput.DrawCrosshair(crosshair);
        //사용자가 IndexTrigger 버튼을 누르면
        if (ARAVRInput.GetDown(ARAVRInput.Button.IndexTrigger) || Input.GetMouseButtonDown(0))
        {          
            ARAVRInput.PlayVibration(ARAVRInput.Controller.RTouch); //총 쏠 때 진동
            //ARAVRInput.PlayVibration(1.0f, 2.0f, 2.0f, ARAVRInput.Controller.RTouch); //총 쏠 때 진동 세부 조정
            bulletAudio.Stop();
            bulletAudio.Play();  //총알 오디오 재생
            //Ray를 카메라의 위치로부터 나가도록 만든다.
            Ray ray = new Ray(ARAVRInput.RHandPosition, ARAVRInput.RHandDirection);
            //Ray의 충돌정보를 저장하기 위한 변수 지정
            RaycastHit hitInfo;
            RaycastHit hit;
            int playerLayer = 1 << LayerMask.NameToLayer("Player"); // 플레이어 레이어 얻어오기
            int enemyLayer = 1 << LayerMask.NameToLayer("Enemy");// 타워 레이어 얻어오기
            int layerMask = playerLayer | enemyLayer;
            if (Physics.Raycast(ray, out hitInfo, 200, ~layerMask))
            {
                //총알 파편 효과 처리
                bulletEffect.Stop();
                bulletEffect.Play();  //총알 이펙트 진행되고 있으면 멈추고 재생
                bulletImpact.position = hitInfo.point;  //부딪힌 지점에서 위치와 방향 설정
                bulletImpact.forward = hitInfo.normal;
            }
            if (Physics.Raycast(ray, out hit, rayDistance))
            {
                if (hit.transform.tag == "Enemy")
                {
                    Destroy(hit.transform.gameObject);
                    
                }
            }
        }
    }
}