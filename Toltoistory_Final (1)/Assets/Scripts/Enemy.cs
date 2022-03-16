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
        //target이 감지거리안에 들어오면 Move로 전이한다.
        //1. 나와 target의 거리를 구해서
        //Distance(나의 거리, 찾는 대상의 거리)
        float distance = Vector3.Distance(transform.position, target.transform.position);
        //2. 만약 그 거리가 감지거리보다 작으면
        if (distance < findDistance)
        {
            //3. Move상태로 전이하다
            state = State.Z_Run;
        }
    }
 
    public float speed = 1f;
    public float attackDistance = 1f;// 공격 가능 거리
    private void UpdateMove()
    {
        //target방향으로 이동하다가 target이 공격거리안에 들어오면 Attack으로 전이하다.
        //1. target방향으로 이동한다. P = P0 + vt
        Vector3 dir = target.transform.position - transform.position;
        dir.Normalize();
        transform.position += dir * speed * Time.deltaTime;
        //2. 나와 target의 거리를 구한다.
        //Distance(나의 거리, 찾는 대상의 거리)
        float distance = Vector3.Distance(transform.position, target.transform.position);
        //3. 만약 그 거리가 공격거리보다 작으면
        if (distance < attackDistance)
        {
            //4. Attack상태로 전이한다.
            state = State.Z_Attack;
        }
    }
 
    float currentTime;
    float attackTime = 1f;
    private void UpdateAttack()
    {
        //일정시간 마다 공격 하되 공격시점에 target이 공격거리 밖에 있으면 Move상태로 전이한다. ,그렇지 않으면 계속 반복해서 공격!
        //1. 시간이 흐르다가
        currentTime += Time.deltaTime;
        //2. 현재시간이 공격시간이 되면
        if (currentTime > attackTime)
        {
            //3. 현재시간을 초기화하고
            currentTime = 0f;
 
            //4. 플레이어를 공격하고
            //target.AddDamage(); - 추후 구현 예정
 
            //5. 만약 target이 공격거리 밖에 있으면 Move상태로 전이한다.(공격하는데 플레이어가 도망갔는지 안갔는지 확인)
            //5-1. 나와 target의 거리를 구한다.
            float distance = Vector3.Distance(transform.position, target.transform.position);
            //5-2. 만약 그 거리가 공격거리보다 크거나 같다면
            if(distance >= attackDistance)
            {
                //5-3. Move상태로 전이한다.
                state = State.Z_Run;
            }
        }
    }
 
    private void UpdateDie()
    {
 
    }
 
    //데미지 주는 함수
    public void AddDamage(int damage)
    {
        //죽고 사라질때
        Destroy(gameObject);
    }
}
