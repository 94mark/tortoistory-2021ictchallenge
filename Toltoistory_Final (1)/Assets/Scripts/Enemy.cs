using UnityEngine;
using System.Collections;
using System;
using UnityEngine.AI;
 
public class Enemy : MonoBehaviour
{
    public ParticleSystem hitEffect;
    public AudioClip deathSound;
    public AudioClip hitSound;
    private Animator enemyAnimator;
    private AudioSource enemyAudioPlayer;
    private Renderer enemyRenderer;

    public float damage = 20f;
    public float timeBetAttack = 0.5f;
    private float lastAttackTime;

    public enum State
    {
        Z_Idle,
        Z_Run,
        Z_Attack,
        Z_FallingForward
    }
    public State state;
    GameObject target;
    void Start()
    {
        state = State.Z_Idle;
        target = GameObject.Find("Player");
    }
 
    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case State.Z_Idle:
                UpdateIdle();
                break;
            case State.Z_Run:
                UpdateMove();
                break;
            case State.Z_Attack:
                UpdateAttack();
                break;
            case State.Z_FallingForward:
                UpdateDie();
                break;
        }
    }
 
    public float findDistance = 5f;
    private void UpdateIdle()
    {
        //target�� �����Ÿ��ȿ� ������ Move�� �����Ѵ�.
        //1. ���� target�� �Ÿ��� ���ؼ�
        //Distance(���� �Ÿ�, ã�� ����� �Ÿ�)
        float distance = Vector3.Distance(transform.position, target.transform.position);
        //2. ���� �� �Ÿ��� �����Ÿ����� ������
        if (distance < findDistance)
        {
            //3. Move���·� �����ϴ�
            state = State.Z_Run;
        }
    }
 
    public float speed = 1f;
    public float attackDistance = 1f;// ���� ���� �Ÿ�
    private void UpdateMove()
    {
        //target�������� �̵��ϴٰ� target�� ���ݰŸ��ȿ� ������ Attack���� �����ϴ�.
        //1. target�������� �̵��Ѵ�. P = P0 + vt
        Vector3 dir = target.transform.position - transform.position;
        dir.Normalize();
        transform.position += dir * speed * Time.deltaTime;
        //2. ���� target�� �Ÿ��� ���Ѵ�.
        //Distance(���� �Ÿ�, ã�� ����� �Ÿ�)
        float distance = Vector3.Distance(transform.position, target.transform.position);
        //3. ���� �� �Ÿ��� ���ݰŸ����� ������
        if (distance < attackDistance)
        {
            //4. Attack���·� �����Ѵ�.
            state = State.Z_Attack;
        }
    }
 
    float currentTime;
    float attackTime = 1f;
    private void UpdateAttack()
    {
        //�����ð� ���� ���� �ϵ� ���ݽ����� target�� ���ݰŸ� �ۿ� ������ Move���·� �����Ѵ�. ,�׷��� ������ ��� �ݺ��ؼ� ����!
        //1. �ð��� �帣�ٰ�
        currentTime += Time.deltaTime;
        //2. ����ð��� ���ݽð��� �Ǹ�
        if (currentTime > attackTime)
        {
            //3. ����ð��� �ʱ�ȭ�ϰ�
            currentTime = 0f;
 
            //4. �÷��̾ �����ϰ�
            //target.AddDamage(); - ���� ���� ����
 
            //5. ���� target�� ���ݰŸ� �ۿ� ������ Move���·� �����Ѵ�.(�����ϴµ� �÷��̾ ���������� �Ȱ����� Ȯ��)
            //5-1. ���� target�� �Ÿ��� ���Ѵ�.
            float distance = Vector3.Distance(transform.position, target.transform.position);
            //5-2. ���� �� �Ÿ��� ���ݰŸ����� ũ�ų� ���ٸ�
            if(distance >= attackDistance)
            {
                //5-3. Move���·� �����Ѵ�.
                state = State.Z_Run;
            }
        }
    }
 
    private void UpdateDie()
    {
 
    }
 
    //������ �ִ� �Լ�
    public void AddDamage(int damage)
    {
        //�װ� �������
        Destroy(gameObject);
    }
}
