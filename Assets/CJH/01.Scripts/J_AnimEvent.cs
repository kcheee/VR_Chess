using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class J_AnimEvent : MonoBehaviour
{
    //이벤트함수를 제작하고싶다
    // Hit, AttackFinished
    //J_AttackTest attackTest;
    J_PieceMove pieceMove;

    //사운드
    public AudioSource audioSource;
    //public AudioClip attackSound;
    public AudioClip[] attackSound;

    public enum ChessType //체스 종류
    {
        KING, QUEEN, BISHOP, KNIGHT, ROOK, PAWN,
    }
    public ChessType chessType;
    // Start is called before the first frame update
    void Awake()
    {
        pieceMove = GetComponentInParent<J_PieceMove>();
        audioSource = gameObject.AddComponent<AudioSource>();

        #region 각 기물별 오디오클립
        if (chessType == ChessType.KING)
        {
            audioSource.clip = attackSound[0];
        }
        else if(chessType == ChessType.QUEEN)
        {
            audioSource.clip = attackSound[1];
        }
        else if (chessType == ChessType.BISHOP)
        {
            audioSource.clip = attackSound[2];
        }
        else if (chessType == ChessType.KNIGHT)
        {
            audioSource.clip = attackSound[3];
        }
        else if (chessType == ChessType.ROOK)
        {
            audioSource.clip = attackSound[4];
        }
        else if (chessType == ChessType.PAWN)
        {
            audioSource.clip = attackSound[5];
        }
        #endregion

    }

    // Update is called once per frame
    void Update()
    {

    }

   public void OnAttack_Hit()
    {
        pieceMove.OnAttack_Hit();
        audioSource.Play();
        
    }
    public void OnAttack_Finished()
    {
        pieceMove.OnAttack_Finished();
    }
}
