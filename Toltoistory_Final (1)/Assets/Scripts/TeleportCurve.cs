using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportCurve : MonoBehaviour
{
    public Transform teleportCircleUI; // 텔레포트 UI
    LineRenderer lr; // 선을 그릴 라인 렌더러
    Vector3 originScale = Vector3.one * 0.02f; // 최초 텔레포트 UI 크기
    public int lineSmooth = 40; // 커브의 부드러운 정도
    public float curveLength = 50; // 커브의 길이
    public float gravity = -60; //커브의 중력
    public float simulateTime = 0.02f; //곡선 시뮬레이션의 간격 및 시간
    List<Vector3> lines = new List<Vector3>(); //곡선을 이루는 점들을 기억할 리스트
    // Start is called before the first frame update
    void Start()
    {
        teleportCircleUI.gameObject.SetActive(false); //텔레포트UI 비활성화
        lr = GetComponent<LineRenderer>(); //라인렌더러 컴포넌트
        lr.startWidth = 0.0f; // 라인렌더러 선의 너비 지정
        lr.endWidth = 0.2f;
    }

    // Update is called once per frame
    void Update()
    {
        //왼쪽 컨트롤러의 One 버튼을 누르면
        if (ARAVRInput.GetDown(ARAVRInput.Button.One, ARAVRInput.Controller.LTouch))
        {
            lr.enabled = true;   //라인 렌더러 활성화
        }
        //왼쪽 컨트롤러의 One 버튼에서 손을 떼면
        else if (ARAVRInput.GetUp(ARAVRInput.Button.One, ARAVRInput.Controller.LTouch))
        {
            lr.enabled = false;   //라인 렌더러 비활성화
            if (teleportCircleUI.gameObject.activeSelf)  //텔레포트UI가 활성화 되어있으면
            {
                GetComponent<CharacterController>().enabled = false;
                //텔레포트 UI의 위치로 순간 이동
                transform.position = teleportCircleUI.position + Vector3.up;
                GetComponent<CharacterController>().enabled = true;

            }
            teleportCircleUI.gameObject.SetActive(false);  //텔레포트 UI 비활성화
        }
        //왼쪽 컨트롤러의 One 버튼을 누르고 있다면
        else if (ARAVRInput.Get(ARAVRInput.Button.One, ARAVRInput.Controller.LTouch))
        {
            MakeLines();
        }
    }
    void MakeLines()
    {
        lines.RemoveRange(0, lines.Count);//리스트에 담긴 위치 정보들을 비워준다.
        Vector3 dir = ARAVRInput.LHandDirection * curveLength; // 선이 진행될 방향을 정한다.
        Vector3 pos = ARAVRInput.LHandPosition; //선이 그려질 위치와 초기값을 설정한다.
        lines.Add(pos); //최초 위치를 리스트에 담는다. 

        for (int i = 0; i < lineSmooth; i++) //lineSmooth 개수만큼 반복
        {
            Vector3 lastPos = pos; //현재 위치 기억
            dir.y += gravity * simulateTime; //중력을 적용한 속도 계산, 미래속도 = 현재속도 + 가속도 * 시간
            pos += dir * simulateTime; // 등속운동으로 다음 위치 계산 미래위치=현재위치+속도*시간
            if (CheckHitRay(lastPos, ref pos)) //Ray 충돌체크가 발생되었다면
            {
                lines.Add(pos);
                break;
            }
            else
            {
                teleportCircleUI.gameObject.SetActive(false);
            }
            lines.Add(pos);//구한 위치를 등록
        }
        lr.positionCount = lines.Count; //라인 렌더러가 표현할 점의 개수를 등록된 개수의 크기로 할당 
        lr.SetPositions(lines.ToArray()); // 라인 렌더러에 구해진 점의 정보를 저장(리스트 to 배열로 형변환)
    }
    private bool CheckHitRay(Vector3 lastPos, ref Vector3 pos)  //MakeLines메서드 인자인 pos에 ref를 붙여주어 전달인자를 그대로 가져온다
    {
        //앞 점 lastPos에서 다음 점 pos로 향하는 벡터 계산
        Vector3 rayDir = pos - lastPos;  //방향을 구한다
        Ray ray = new Ray(lastPos, rayDir);
        RaycastHit hitInfo;
        //Raycast 할 때 레이의 크기를 앞 점과 다른 점 사이의 거리로 한정한다
        if (Physics.Raycast(ray, out hitInfo, rayDir.magnitude))
        {
            pos = hitInfo.point;  //다음 점의 위치를 충돌한 지점으로 설정, MakeLines 함수의 CheckHitRay한 값을 pos로 가져온다

            int layer = LayerMask.NameToLayer("Terrain");
            if (hitInfo.transform.gameObject.layer == layer)
            {
                //텔레포트 UI활성화
                teleportCircleUI.gameObject.SetActive(true);
                teleportCircleUI.position = pos;  //텔레포트 UI의 위치 지정
                teleportCircleUI.forward = hitInfo.normal;
                float distance = (pos - ARAVRInput.LHandPosition).magnitude;  //방향설정
                teleportCircleUI.localScale = originScale * Mathf.Max(1, distance);  //거리에 따른 UI크기 설정
            }
            return true;
        }
        return false;
    }
}
