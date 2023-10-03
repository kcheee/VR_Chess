using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bishop : Chessman
{
    public Bishop()
    {
        value = 30;
    }

    public override bool[,] PossibleMoves()
    {
        bool[,] moves = new bool[8, 8];
        int x = currentX; // 현재 폰의 x 위치
        int y = currentY; // 현재 폰의 y 위치

        // Rook 움직임 체크

        // 상하 좌우 이동 체크를 해야됌.
        // if x < 0 && y < 0 && y>7 && x>7

        // UpRight
        while (y++ < 7&& x++<7)
        {
            if (!BishopMove(x, y, ref moves)) break;
        }

        x = currentX;
        y = currentY;

        // UpLeft
        while (y++ < 7&& x-->0)
        {
            if (!BishopMove(x, y, ref moves)) break;
        }

        x = currentX;
        y = currentY;

        // DownRight
        while (y-- > 0 && x++ <7)
        {
            if (!BishopMove(x, y, ref moves)) break;
        }

        x = currentX;
        y = currentY;

        // DownLeft
        while (y-- > 0 && x-- > 0)
        {
            if (!BishopMove(x, y, ref moves)) break;
        }

        return moves;
    }

    // 메모리 참조를 위해 ref를 사용
    bool BishopMove(int x, int y, ref bool[,] moves)
    {
        // 기물을 가져옴
        Chessman piece = BoardManager.Instance.Chessmans[x, y];

        // 기물이 없으면 계속 실행.
        if (piece == null)
        {
            moves[x, y] = true;
            return true;
        }
        // if 적팀 기물이 있으면 그 자리에서 멈춤.
        if (piece.isWhite != isWhite)
        {
            // 그 자리까지 허용.
            // 먹는 것 체크해야함.
            moves[x, y] = true;
        }

        // if 우리팀 기물이 있던가
        return false;
    }

}
