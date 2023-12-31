using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class J_PieceMove : MonoBehaviour
{
    public enum ChessType //체스 종류
    {
        KING, QUEEN, BISHOP, KNIGHT, ROOK, PAWN,
    }
    [SerializeField]
    private float moveSpeed = 3f;
    public bool isMoving = false; // 움직이는지
    public bool hasAttacked = false; //공격해야되는경우가 아니라면

    Animator anim;
    public int currentX;
    public int currentY;
    private Vector3 targetPosition;
    Chessman[,] ch = new Chessman[8, 8];
    public ChessType chessType;

    int PosX, PosY; //타겟위치
    float preTargetX, preTargetZ; // 공격하기 전 위치
    //float currTime = 0;

    float myAngle; //내가 움직였던 각도

    bool EndRot = false;  // 마지막 앞을 바라보는 회전

    public GameObject particleObject; //파티클 시스템
    public Transform particlePos; //파티클 생성 위치
    [SerializeField]
    public float rotationSpeed = 2.5f; //회전속도 조정값

    private void Start()
    {
        myAngle = transform.eulerAngles.y;
        anim = GetComponentInChildren<Animator>();
        anim.Play("Idle");
    }

    public void UpdateRotate1(int x, int y)
    {
        //비숍일 때
        //targetX = 4;   //0
        //targetZ = 3;   //6
        PosX = x;
        PosY = y;

        Vector3 pos = transform.position;// (transform.position + (Vector3.right + Vector3.up) * 0.05f) * 10;
        pos.x += 0.05f;
        pos.z += 0.05f;
        pos *= 10;

        int reCal_X = (int)pos.x;
        int reCal_Y = (int)pos.z;

        #region 비숍
        if (chessType == ChessType.BISHOP)
        {
            CalculateBishop(reCal_X, reCal_Y);
        }

        #endregion
        #region 룩
        if (chessType == ChessType.ROOK)
        {
            CalculateRook(reCal_X, reCal_Y);           
        }

        #endregion
        #region 폰
        if (chessType == ChessType.PAWN)
        {

            CalculateBishop(reCal_X, reCal_Y);
            //직선이동일때는 직선이동한다.
            if (isPawn(reCal_X,reCal_Y))
            {
                CalculateRook(x, y);
            }

        }
        #endregion
        #region 퀸
        if (chessType == ChessType.QUEEN)
        {
            //비숍처럼
            if(isBishopOrRook(reCal_X, reCal_Y))
            {
                CalculateBishop(reCal_X, reCal_Y);

            }
            //룩처럼
            else
            {
                CalculateRook(reCal_X, reCal_Y);
            }
        }

        #endregion
        #region 킹
        if (chessType == ChessType.KING)
        {
            //비숍처럼
            if (isBishopOrRook(reCal_X, reCal_Y))
            {
                CalculateBishop(reCal_X, reCal_Y);
            }
            //룩처럼
            else
            {
                CalculateRook(reCal_X, reCal_Y);
            }
        }

        #endregion
        //Debug.Log(PosX);
        //Debug.Log(preTargetX );
        PieceMove(PosX, PosY);
    }
    //true면 직선이동 
    bool isPawn(int x, int y)
    {
        return x == PosX;
    }

    // true 면 비숍 false 룩
    bool isBishopOrRook(int x, int y)
    {
        return (x != PosX && y != PosY);
    }
    //비숍 계산법
    void CalculateBishop(int x, int y)
    {

        if (x - PosX < 0)
        {
            //Debug.Log(PosX);
            preTargetX = PosX - 1;
        }
        else if (x - PosX > 0)
        {
            preTargetX = PosX + 1;
        }

        if (y - PosY < 0)
        {
            preTargetZ = PosY - 1;
        }
        else if (y - PosY > 0)
        {
            preTargetZ = PosY + 1;
        }
    }
    //룩 계산법
    void CalculateRook(int x, int y)
    {
        //룩처럼
        if (x > PosX)
        {
            preTargetX = PosX + 1;
        }
        else if (x < PosX)
        {
            preTargetX = PosX - 1;
        }
        else
        {
            preTargetX = PosX;
        }

        if (y > PosY)
        {
            preTargetZ = PosY + 1;
        }
        else if (y < PosY)
        {
            preTargetZ = PosY - 1;
        }
        else
        {
            preTargetZ = PosY;
        }
    }

    float posOffset = 0.222f;
    public void OnAttack_Hit()
    {
        if (chessType == ChessType.PAWN)
        {
            posOffset = 0.05f;
        }
        else if (chessType == ChessType.BISHOP)
        {
            posOffset = 0.25f;
        }
        else if(chessType == ChessType.ROOK)
        {
            posOffset = 0.15f;
        }
        Vector3 spawnPos = transform.position + transform.forward * posOffset;
        GameObject newParticle = Instantiate(particleObject,spawnPos, transform.rotation);
        //newParticle.transform.parent = transform;
        //audioSource.Play();

        Destroy(newParticle,5);
        BoardManager.Instance.deletePiece.gameObject.GetComponentInChildren<Animator>().CrossFade("Hit", 0, 0);
    }
    public void OnAttack_Finished()
    {

        BoardManager.Instance.deletePiece.gameObject.GetComponentInChildren<Animator>().CrossFade("Die", 0, 0);
        Destroy(BoardManager.Instance.deletePiece, 2);
        //Debug.Log("삭제");
    }
    public void OnDie_Finish()
    {

    }

    private void Update()
    {
        currentX = (int)transform.position.x;
        currentY = (int)transform.position.y;

        if (Input.GetKeyDown(KeyCode.K))
        {
            //PieceMove(3, 3);
            UpdateRotate1(5, 4);
        }
        
    }
    float angle = 0;
    int nDir = 0;
    private bool isend;

    //모든 말들의 움직임을 계산하는 함수 
    public void PieceMove(int targetX, int targetY)
    {
        EndRot = false;

        Vector3 targetPos = new Vector3(targetX, 0, targetY) - transform.position * 10;
        float dot = Vector3.Dot(transform.right, targetPos);
        nDir = (dot > 0) ? 1 : (dot < 0) ? -1 : (Vector3.Dot(transform.forward, targetPos) < 0) ? 1 : 0;
        #region 1if
        //if (dot > 0)
        //{
        //    nDir = 1;
        //    print("오른쪽에 회전해야해");
        //}
        //else if (dot < 0)
        //{
        //    nDir = -1;
        //    print("왼쪽에 회전해야해");
        //}
        //else // dot = 0 앞/뒤
        //{
        //    float d = Vector3.Dot(transform.forward, targetPos);
        //    if (d < 0) //뒤
        //    {
        //        nDir = 1;
        //        print("뒤로 돌아라");
        //    }
        //    else //앞
        //    {
        //        nDir = 0;
        //        print("회전 하지 말아라");
        //    }
        //}
        #endregion
        //상대방과 나와의 각도를 잰다
        angle = Vector3.Angle(transform.forward, targetPos);
        //StartCoroutine(Attack(targetX, targetY));
        StartCoroutine(RotatePiece(angle * nDir, (1.0f / 45) * angle));
        //ChangeState(PieceState.Rotate1);
        return;
    }

    //직선이동(완료)
    IEnumerator StraightMove(float targetX, float targetY, bool Enemy = false, bool rot = false)
    {
        //초기 위치
        Vector3 currentPos = transform.position * 10;
        //타겟의 위치
        targetPosition = new Vector3(targetX, 0, targetY);

        //흐른시간 체크
        float elapsedTime = 0;
        //거리잰다
        float dist = Vector3.Distance(transform.position * 10, targetPosition);
        //시간
        float duration = dist / moveSpeed;
        //타겟의 방향
        Vector3 dir = transform.forward;
        anim.CrossFade("Move", 0, 0);
        J_SoundManager.Instance.MoveSound((int)chessType);
        //audioSource.Play();


        while (elapsedTime / duration < 1 /* 적이 없으면 */)
        {
            elapsedTime += Time.deltaTime;
            transform.position = Vector3.Lerp(currentPos * 0.1f, targetPosition * 0.1f, elapsedTime / duration);
            yield return null;// new WaitForSeconds(0.05f);
            //print("움직인다");
        }

        transform.position = targetPosition * 0.1f;
        //yield return new WaitForSeconds(0.01f);

        
        // move에서 idle로 전환
        //anim.CrossFade("Idle", 0, 0);
        // 적이있다면 공격
        if (Enemy)
        {
            // 코루틴으로 딜레이주고 실행.
            StartCoroutine(Co_Attack());
            J_SoundManager.Instance.MoveSound((int)chessType);
        }
        if (rot)
        {
            //Debug.Log("이거 한번만 실행해야함.");
            EndRot = true;
            // 앞에 보게 회전.
            
            StartCoroutine(RotatePiece(-angle * nDir, (1f / 45) * angle));
            J_SoundManager.Instance.MoveSound((int)chessType);
            // 한번만 싫행해야함.
            // ----------- 턴넘김----------------
            //턴넘기기전 딜레이 3초
            yield return new WaitForSeconds(1f);
            BoardManager.Instance.PieceIsMove = false;

            // ----------- 턴넘김----------------
        }

    }
    //회전 공식(완료)
    private IEnumerator RotatePiece(float targetAngle, float duration)
    {
        if (duration > 0)
        {
            //자신의 각도
            float currentAngle = 0;// transform.eulerAngles.y;
            float elapsedTime = 0f; // 시간
                                    //시간이 흐르면
            while (elapsedTime < duration)
            {
                //회전을 Lerp값으로 회전
                float angle = Mathf.LerpAngle(currentAngle, targetAngle, elapsedTime / duration);

                transform.rotation = Quaternion.Euler(0, myAngle + angle, 0);

                elapsedTime += Time.deltaTime * rotationSpeed ; //회전속도 조정
                yield return null;
            }
        }

        myAngle = myAngle + targetAngle;
        transform.rotation = Quaternion.Euler(0f, myAngle, 0f);
        //yield return new WaitForSeconds(0.1f);
        anim.CrossFade("Idle",0,0);
        // 움직임
        // 적이 있는지 판별

        if (!EndRot)
        {
            //StartCoroutine(StraightMove(preTargetX, preTargetZ, true));
            //1.적이없다면 바로감
            if (BoardManager.Instance.deletePiece == null)
            {
                Debug.Log("적 발견");
                StartCoroutine(StraightMove(PosX, PosY, false, true));
            }
            //2.적이 있다면 pretarget
            else
            {
                Debug.Log(preTargetX + " " + preTargetZ);
                StartCoroutine(StraightMove(preTargetX, preTargetZ, true));
            }
        }
    }
    //공격 함수
    IEnumerator Co_Attack()
    {
        //yield return null;
        anim.CrossFade("Attack", 0, 0);
        //자신의 attackClip에 맞는 SoundEffcet가 실행된다.
        //audioSource.PlayOneShot(attackSound, 1);

        //Debug.Log(PosX + " " + PosY);
        //yield return new WaitForSeconds(3);
        yield return new WaitForSeconds(3f);
        StartCoroutine(StraightMove(PosX, PosY, false, true));
    }


    // 

}