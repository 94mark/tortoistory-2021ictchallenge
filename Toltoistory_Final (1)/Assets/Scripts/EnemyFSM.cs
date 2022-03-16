using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyFSM : MonoBehaviour
{
    enum EnemyState
    {
        Idle,
        Move,
        Attack,
        Return,
        Damaged,
        Die
    }
    EnemyState m_State; //���ʹ� ���� ����
    public float findDistance = 8f;
    Transform player;
    public float attackDistance = 2f;
    public float moveSpeed = 5f;
    CharacterController cc;
    float currentTime = 0; //���� �ð�
    float attackDelay = 2f; //���� ������ �ð�
    public int attackPower = 3;
    Vector3 originPos; //�ʱ� ��ġ 
    Quaternion originRot;
    public float moveDistance = 20f; //�̵� ���� ����
    public int hp = 15; //���ʹ� ü��
    Animator anim;
    NavMeshAgent smith;

    void Start()
    {
        m_State = EnemyState.Idle; //���ʹ� ������ ���´� Idle
        player = GameObject.Find("Player").transform;
        cc = GetComponent<CharacterController>();
        originPos = transform.position;
        originRot = transform.rotation;
        anim = transform.GetComponentInChildren<Animator>();
        smith = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        //���º� ������ ��� ����
        switch (m_State)
        {
            case EnemyState.Idle:
                Idle();
                break;
            case EnemyState.Move:
                Move();
                break;
            case EnemyState.Attack:
                Attack();
                break;
            case EnemyState.Return:
                Return();
                break;
            case EnemyState.Damaged:
                //Damaged();
                break;
            case EnemyState.Die:
                //Die();
                break;
        }
    }
    void Idle()
    {
        //���� �÷��̾���� �Ÿ��� �׼� ���� ���� �̳���� Move���·� ��ȯ�Ѵ�.
        if (Vector3.Distance(transform.position, player.position) < findDistance)
        {
            m_State = EnemyState.Move;
            anim.SetTrigger("IdleToMove");  //�̵� �ִϸ��̼� ��ȯ
        }
    }
    void Move()
    {
        //���� ��ġ�� �ʱ� ��ġ���� �̵� ���� ������ �Ѿ�ٸ�
        if(Vector3.Distance(transform.position, originPos) > moveDistance)
        {
            m_State = EnemyState.Return;
        }
        //�÷��̾���� �Ÿ��� ���� ���� ���̶�� �÷��̾ ���� �̵�
        else if (Vector3.Distance(transform.position, player.position) > attackDistance)
        {
            //�̵����� ����
            //Vector3 dir = (player.position - transform.position).normalized;
            //ĳ���� ��Ʈ�ѷ��� �̿��� �̵�
            //cc.Move(dir * moveSpeed * Time.deltaTime);
            //transform.forward = dir;
            smith.isStopped = true;
            smith.ResetPath();
            smith.stoppingDistance = attackDistance;
            smith.destination = player.position;
        }
        //�׷��� �ʴٸ� ���� ���¸� �������� ��ȯ
        else
        {
            m_State = EnemyState.Attack;
            currentTime = attackDelay;
            anim.SetTrigger("MoveToAttackDelay"); //���� ��� �ִϸ��̼� �÷���
        }
    }
    void Attack()
    {
        //�÷��̾ ���� ���� �̳��� �ִٸ� �÷��̾� ����
        if (Vector3.Distance(transform.position, player.position) < attackDistance)
        {
            //������ �ð����� �÷��̾� ����
            currentTime += Time.deltaTime;
            if (currentTime > attackDelay)
            {
                //player.GetComponent<PlayerMove>().DamageAction(attackPower);
                currentTime = 0;
                anim.SetTrigger("StartAttack");
            }
        }
        //�׷��� �ʴٸ� ���� ���¸� �̵����� ��ȯ
        else
        {
            m_State = EnemyState.Move;
            currentTime = 0;
            anim.SetTrigger("AttackToMove");
        }
    }
    public void AttackAction()
    {
        //player.GetComponent<PlayerMove>().DamageAction(attackPower);
    }
    void Return()
    {
        //�ʱ� ��ġ������ �Ÿ��� 0.1f �̻��̶�� �ʱ� ��ġ ������ �̵�
        if (Vector3.Distance(transform.position, originPos) > 0.1f)
        {
            //Vector3 dir = (originPos - transform.position).normalized;
            //cc.Move(dir * moveSpeed * Time.deltaTime);
            //transform.forward = dir; //������ ���� �������� ��ȯ
            smith.destination = originPos;
            smith.stoppingDistance = 0;
        }
        else
        {
            smith.isStopped = true;
            smith.ResetPath();
            transform.position = originPos;
            transform.rotation = originRot;
            //hp = maxHp;
            m_State = EnemyState.Idle;
            anim.SetTrigger("MoveToIdle"); //��� �ִϸ��̼����� ��ȯ�ϴ� Ʈ������ ȣ��
        }
    }
    //������ ���� �Լ�
    public void HitEnemy(int hitPower)
    {
        //�̹� �ǰ� �����̰ų� ��� ���� �Ǵ� ���� ���¶�� �ƹ��� ó������ �ʰ�, �Լ� ����
        if (m_State == EnemyState.Damaged || m_State == EnemyState.Die || m_State == EnemyState.Return)
        {
            return;
        }
        //�÷��̾��� ���ݷ¸�ŭ ���ʹ��� ü���� ����
        hp -= hitPower;
        smith.isStopped = true;
        smith.ResetPath();
        //���ʹ��� ü���� 0���� ũ�� �ǰ� ���·� ��ȯ
        if (hp > 0)
        {
            m_State = EnemyState.Damaged;
            Damaged();
        }
        //�׷��� �ʴٸ� ���� ���·� ��ȯ
        else
        {
            m_State = EnemyState.Die;
            anim.SetTrigger("Die");
            Die();
        }
    }
    void Damaged()
    {
        //�ǰ� ���¸� ó���ϱ� ���� �ڷ�ƾ ����
        StartCoroutine(DamageProcess());
    }
    IEnumerator DamageProcess()
    {
        //�ǰ� ��� �ð���ŭ ��ٸ�
        yield return new WaitForSeconds(0.5f);
        //���� ���¸� �̵� ���·� ��ȯ
        m_State = EnemyState.Move;
    }
    //���� ����
    void Die()
    {
        //���� ���� �ǰ� �ڷ�ƾ ����
        StopAllCoroutines();
        //���� ���¸� ó���ϱ� ���� �ڷ�ƾ ����
        StartCoroutine(DieProcess());
    }
    IEnumerator DieProcess()
    {
        //ĳ���� ��Ʈ�ѷ� ������Ʈ�� ��Ȱ��ȭ
        cc.enabled = false;
        //2�� ���� ��ٸ� �Ŀ� �ڱ� �ڽ��� ����
        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
    }
}
