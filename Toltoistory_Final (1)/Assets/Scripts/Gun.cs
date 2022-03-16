using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public Transform crosshair; //crosshair �Ӽ�
    public Transform bulletImpact; //�Ѿ� ���� ȿ��
    ParticleSystem bulletEffect; // �Ѿ� ���� ��ƼŬ �ý���
    AudioSource bulletAudio; // �Ѿ� �߻� ����
    public float rayDistance = 100f;
    

    void Start()
    {
        bulletEffect = bulletImpact.GetComponent<ParticleSystem>();
        bulletAudio = bulletImpact.GetComponent<AudioSource>();
        
    }

    void Update()
    {
        ARAVRInput.DrawCrosshair(crosshair);
        //����ڰ� IndexTrigger ��ư�� ������
        if (ARAVRInput.GetDown(ARAVRInput.Button.IndexTrigger) || Input.GetMouseButtonDown(0))
        {          
            ARAVRInput.PlayVibration(ARAVRInput.Controller.RTouch); //�� �� �� ����
            //ARAVRInput.PlayVibration(1.0f, 2.0f, 2.0f, ARAVRInput.Controller.RTouch); //�� �� �� ���� ���� ����
            bulletAudio.Stop();
            bulletAudio.Play();  //�Ѿ� ����� ���
            //Ray�� ī�޶��� ��ġ�κ��� �������� �����.
            Ray ray = new Ray(ARAVRInput.RHandPosition, ARAVRInput.RHandDirection);
            //Ray�� �浹������ �����ϱ� ���� ���� ����
            RaycastHit hitInfo;
            RaycastHit hit;
            int playerLayer = 1 << LayerMask.NameToLayer("Player"); // �÷��̾� ���̾� ������
            int enemyLayer = 1 << LayerMask.NameToLayer("Enemy");// Ÿ�� ���̾� ������
            int layerMask = playerLayer | enemyLayer;
            if (Physics.Raycast(ray, out hitInfo, 200, ~layerMask))
            {
                //�Ѿ� ���� ȿ�� ó��
                bulletEffect.Stop();
                bulletEffect.Play();  //�Ѿ� ����Ʈ ����ǰ� ������ ���߰� ���
                bulletImpact.position = hitInfo.point;  //�ε��� �������� ��ġ�� ���� ����
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