using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportCurve : MonoBehaviour
{
    public Transform teleportCircleUI; // �ڷ���Ʈ UI
    LineRenderer lr; // ���� �׸� ���� ������
    Vector3 originScale = Vector3.one * 0.02f; // ���� �ڷ���Ʈ UI ũ��
    public int lineSmooth = 40; // Ŀ���� �ε巯�� ����
    public float curveLength = 50; // Ŀ���� ����
    public float gravity = -60; //Ŀ���� �߷�
    public float simulateTime = 0.02f; //� �ùķ��̼��� ���� �� �ð�
    List<Vector3> lines = new List<Vector3>(); //��� �̷�� ������ ����� ����Ʈ
    // Start is called before the first frame update
    void Start()
    {
        teleportCircleUI.gameObject.SetActive(false); //�ڷ���ƮUI ��Ȱ��ȭ
        lr = GetComponent<LineRenderer>(); //���η����� ������Ʈ
        lr.startWidth = 0.0f; // ���η����� ���� �ʺ� ����
        lr.endWidth = 0.2f;
    }

    // Update is called once per frame
    void Update()
    {
        //���� ��Ʈ�ѷ��� One ��ư�� ������
        if (ARAVRInput.GetDown(ARAVRInput.Button.One, ARAVRInput.Controller.LTouch))
        {
            lr.enabled = true;   //���� ������ Ȱ��ȭ
        }
        //���� ��Ʈ�ѷ��� One ��ư���� ���� ����
        else if (ARAVRInput.GetUp(ARAVRInput.Button.One, ARAVRInput.Controller.LTouch))
        {
            lr.enabled = false;   //���� ������ ��Ȱ��ȭ
            if (teleportCircleUI.gameObject.activeSelf)  //�ڷ���ƮUI�� Ȱ��ȭ �Ǿ�������
            {
                GetComponent<CharacterController>().enabled = false;
                //�ڷ���Ʈ UI�� ��ġ�� ���� �̵�
                transform.position = teleportCircleUI.position + Vector3.up;
                GetComponent<CharacterController>().enabled = true;

            }
            teleportCircleUI.gameObject.SetActive(false);  //�ڷ���Ʈ UI ��Ȱ��ȭ
        }
        //���� ��Ʈ�ѷ��� One ��ư�� ������ �ִٸ�
        else if (ARAVRInput.Get(ARAVRInput.Button.One, ARAVRInput.Controller.LTouch))
        {
            MakeLines();
        }
    }
    void MakeLines()
    {
        lines.RemoveRange(0, lines.Count);//����Ʈ�� ��� ��ġ �������� ����ش�.
        Vector3 dir = ARAVRInput.LHandDirection * curveLength; // ���� ����� ������ ���Ѵ�.
        Vector3 pos = ARAVRInput.LHandPosition; //���� �׷��� ��ġ�� �ʱⰪ�� �����Ѵ�.
        lines.Add(pos); //���� ��ġ�� ����Ʈ�� ��´�. 

        for (int i = 0; i < lineSmooth; i++) //lineSmooth ������ŭ �ݺ�
        {
            Vector3 lastPos = pos; //���� ��ġ ���
            dir.y += gravity * simulateTime; //�߷��� ������ �ӵ� ���, �̷��ӵ� = ����ӵ� + ���ӵ� * �ð�
            pos += dir * simulateTime; // ��ӿ���� ���� ��ġ ��� �̷���ġ=������ġ+�ӵ�*�ð�
            if (CheckHitRay(lastPos, ref pos)) //Ray �浹üũ�� �߻��Ǿ��ٸ�
            {
                lines.Add(pos);
                break;
            }
            else
            {
                teleportCircleUI.gameObject.SetActive(false);
            }
            lines.Add(pos);//���� ��ġ�� ���
        }
        lr.positionCount = lines.Count; //���� �������� ǥ���� ���� ������ ��ϵ� ������ ũ��� �Ҵ� 
        lr.SetPositions(lines.ToArray()); // ���� �������� ������ ���� ������ ����(����Ʈ to �迭�� ����ȯ)
    }
    private bool CheckHitRay(Vector3 lastPos, ref Vector3 pos)  //MakeLines�޼��� ������ pos�� ref�� �ٿ��־� �������ڸ� �״�� �����´�
    {
        //�� �� lastPos���� ���� �� pos�� ���ϴ� ���� ���
        Vector3 rayDir = pos - lastPos;  //������ ���Ѵ�
        Ray ray = new Ray(lastPos, rayDir);
        RaycastHit hitInfo;
        //Raycast �� �� ������ ũ�⸦ �� ���� �ٸ� �� ������ �Ÿ��� �����Ѵ�
        if (Physics.Raycast(ray, out hitInfo, rayDir.magnitude))
        {
            pos = hitInfo.point;  //���� ���� ��ġ�� �浹�� �������� ����, MakeLines �Լ��� CheckHitRay�� ���� pos�� �����´�

            int layer = LayerMask.NameToLayer("Terrain");
            if (hitInfo.transform.gameObject.layer == layer)
            {
                //�ڷ���Ʈ UIȰ��ȭ
                teleportCircleUI.gameObject.SetActive(true);
                teleportCircleUI.position = pos;  //�ڷ���Ʈ UI�� ��ġ ����
                teleportCircleUI.forward = hitInfo.normal;
                float distance = (pos - ARAVRInput.LHandPosition).magnitude;  //���⼳��
                teleportCircleUI.localScale = originScale * Mathf.Max(1, distance);  //�Ÿ��� ���� UIũ�� ����
            }
            return true;
        }
        return false;
    }
}
