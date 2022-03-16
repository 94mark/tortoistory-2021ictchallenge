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
    EnemyState m_State; //에너미 상태 변수
    public float findDistance = 8f;
    Transform player;
    public float attackDistance = 2f;
    public float moveSpeed = 5f;
    CharacterController cc;
    float currentTime = 0; //누적 시간
    float attackDelay = 2f; //공격 딜레이 시간
    public int attackPower = 3;
    Vector3 originPos; //초기 위치 
    Quaternion originRot;
    public float moveDistance = 20f; //이동 가능 범위
    public int hp = 15; //에너미 체력
    Animator anim;
    NavMeshAgent smith;

    void Start()
    {
        m_State = EnemyState.Idle; //에너미 최초의 상태는 Idle
        player = GameObject.Find("Player").transform;
        cc = GetComponent<CharacterController>();
        originPos = transform.position;
        originRot = transform.rotation;
        anim = transform.GetComponentInChildren<Animator>();
        smith = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        //상태별 정해진 기능 수행
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
        //만일 플레이어와의 거리가 액션 시작 범위 이내라면 Move상태로 전환한다.
        if (Vector3.Distance(transform.position, player.position) < findDistance)
        {
            m_State = EnemyState.Move;
            anim.SetTrigger("IdleToMove");  //이동 애니메이션 전환
        }
    }
    void Move()
    {
        //현재 위치가 초기 위치에서 이동 가능 범위를 넘어간다면
        if(Vector3.Distance(transform.position, originPos) > moveDistance)
        {
            m_State = EnemyState.Return;
        }
        //플레이어와의 거리가 공격 범위 밖이라면 플레이어를 향해 이동
        else if (Vector3.Distance(transform.position, player.position) > attackDistance)
        {
            //이동방향 설정
            //Vector3 dir = (player.position - transform.position).normalized;
            //캐릭터 콘트롤러를 이용해 이동
            //cc.Move(dir * moveSpeed * Time.deltaTime);
            //transform.forward = dir;
            smith.isStopped = true;
            smith.ResetPath();
            smith.stoppingDistance = attackDistance;
            smith.destination = player.position;
        }
        //그렇지 않다면 현재 상태를 공격으로 전환
        else
        {
            m_State = EnemyState.Attack;
            currentTime = attackDelay;
            anim.SetTrigger("MoveToAttackDelay"); //공격 대기 애니메이션 플레이
        }
    }
    void Attack()
    {
        //플레이어가 공격 범위 이내에 있다면 플레이어 공격
        if (Vector3.Distance(transform.position, player.position) < attackDistance)
        {
            //일저한 시간마다 플레이어 공격
            currentTime += Time.deltaTime;
            if (currentTime > attackDelay)
            {
                //player.GetComponent<PlayerMove>().DamageAction(attackPower);
                currentTime = 0;
                anim.SetTrigger("StartAttack");
            }
        }
        //그렇지 않다면 현재 상태를 이동으로 전환
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
        //초기 위치에서의 거리가 0.1f 이상이라면 초기 위치 쪽으로 이동
        if (Vector3.Distance(transform.position, originPos) > 0.1f)
        {
            //Vector3 dir = (originPos - transform.position).normalized;
            //cc.Move(dir * moveSpeed * Time.deltaTime);
            //transform.forward = dir; //방향을 복귀 지점으로 전환
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
            anim.SetTrigger("MoveToIdle"); //대기 애니메이션으로 전환하는 트랜지션 호출
        }
    }
    //데미지 실행 함수
    public void HitEnemy(int hitPower)
    {
        //이미 피격 상태이거나 사망 상태 또는 복귀 상태라면 아무런 처리하지 않고, 함수 종료
        if (m_State == EnemyState.Damaged || m_State == EnemyState.Die || m_State == EnemyState.Return)
        {
            return;
        }
        //플레이어의 공격력만큼 에너미의 체력을 감소
        hp -= hitPower;
        smith.isStopped = true;
        smith.ResetPath();
        //에너미의 체력이 0보다 크면 피격 상태로 전환
        if (hp > 0)
        {
            m_State = EnemyState.Damaged;
            Damaged();
        }
        //그렇지 않다면 죽음 상태로 전환
        else
        {
            m_State = EnemyState.Die;
            anim.SetTrigger("Die");
            Die();
        }
    }
    void Damaged()
    {
        //피격 상태를 처리하기 위한 코루틴 실행
        StartCoroutine(DamageProcess());
    }
    IEnumerator DamageProcess()
    {
        //피격 모션 시간만큼 기다림
        yield return new WaitForSeconds(0.5f);
        //현재 상태를 이동 상태로 전환
        m_State = EnemyState.Move;
    }
    //죽음 상태
    void Die()
    {
        //진행 중인 피격 코루틴 중지
        StopAllCoroutines();
        //죽음 상태를 처리하기 위한 코루틴 실행
        StartCoroutine(DieProcess());
    }
    IEnumerator DieProcess()
    {
        //캐릭터 콘트롤러 컴포넌트를 비활성화
        cc.enabled = false;
        //2초 동안 기다린 후에 자기 자신을 제거
        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
    }
}
