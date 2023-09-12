using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    // 싱글톤
    #region 싱글톤
    public static BoardManager Instance { set; get; }

    private void Awake()
    {
        Instance = this;
    }
    #endregion

    #region 타일 사이즈
    // 타일 사이즈는 1로 고정
    private const float TILE_SIZE = 1.0f;
    // 타일의 중앙을 계산할 때 사용, 위치 보정.
    private const float TILE_OFFSET = TILE_SIZE / 2;
    #endregion

    // 체스 말 
    public List<GameObject> ChessmanPrefabs;

    // 보드 위에 있는 체스말들 업데이트 하기 위한 체스말.
    private List<GameObject> ActiveChessmans;

    // cam
    Camera cam;


    // 체스 기물.
    public Chessman[,] Chessmans { set; get; }
    // Currently Selected Chessman
    public Chessman SelectedChessman;

    // 허용된 움직임 2차원 배열
    public bool[,] allowedMoves;

    // Select하기 위한 x, y 값.
    private int selectionX = -1;
    private int selectionY = -1;

    int outline_selectpieceX;
    int outline_selectpieceY;

    // Turn System
    public bool isWhiteTurn = true;

    public Chessman WhiteKing;
    public Chessman BlackKing;

    private void Start()
    {
        // 체스 기물.
        ActiveChessmans = new List<GameObject>();
        Chessmans = new Chessman[8, 8];
        cam = Camera.main;
        SpawnAllChessmans();
    }

    private void Update()
    {
        SelectMouseChessman();
        if (Input.GetMouseButtonDown(0))
        {
            if (selectionX >= 0 && selectionY >= 0 && selectionX <= 7 && selectionY <= 7)
            {
                // 만약 선택된 체스맨이 없다면 먼저 선택해야 함
                if (SelectedChessman == null)
                {
                    // Chess를 선택
                    SelectChessman();

                }
                // 이미 체스맨이 선택된 경우 이동해야 함
                else
                {
                    // 체스의 움직임 가능한 위치를 renderer에 띄워주고 움직임.
                    MoveChessman(selectionX, selectionY);

                }
            }
        }
        // AI turn
        else if (!isWhiteTurn)
        {
            // NPC가 움직임을 수행
            ChessAI.Instance.NPCMove();
        }

    }

    // 마우스 클릭으로 체스 기물 선택.
    void SelectMouseChessman()
    {
        // 카메라에서 화면상의 마우스 클릭 지점까지의 레이를 쏨
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // 만약 레이캐스트가 무언가와 충돌하면
        if (Physics.Raycast(ray, out hit, 25.0f, LayerMask.GetMask("ChessBoard")))
        {
            // 전에 했던 값들  Outline 설정해줘야함.


            // 선택된 위치의 x와 y 좌표를 계산하여 저장
            // (hit.point.x, hit.point.z)의 소수점 아래 자리를 반올림해서 정수로 변환
            selectionX = (int)(hit.point.x + 0.5f);
            selectionY = (int)(hit.point.z + 0.5f);

        }
        else
        {
            // 만약 아무 것도 충돌하지 않으면 (-1, -1)로 설정하여 선택이 없음을 표시
            selectionX = -1;
            selectionY = -1;
        }

    }

    //  체스 기물 선택함.
    private void SelectChessman()
    {

        // 만약 클릭한 타일에 체스맨이 없다면
        if (Chessmans[selectionX, selectionY] == null) return;

        // 선택된 체스맨의 팀의 턴이 아니라면
        if (Chessmans[selectionX, selectionY].isWhite != isWhiteTurn) return;

        // 임의로 렌더러로 함.
        // 노란색으로 강조된 체스맨 선택
        SelectedChessman = Chessmans[selectionX, selectionY];

        // Outline 
        Chessmans[selectionX, selectionY].outline.enabled = true;

        // 위치값 저장.
        outline_selectpieceX = selectionX;
        outline_selectpieceY = selectionY;

        // 허용된 움직임.
        allowedMoves = SelectedChessman.PossibleMoves();

        // 허용된 움직임 UI 띄워주기.
        BoardHighlight.Instance.HighlightPossibleMoves(allowedMoves, !isWhiteTurn);

    }

    // 체스 기물 Spawn
    private void SpawnChessman(int index, Vector3 position)
    {
        // 체스맨 게임 오브젝트를 생성하고 위치와 회전을 설정합니다.
        GameObject ChessmanObject = Instantiate(ChessmanPrefabs[index], position, ChessmanPrefabs[index].transform.rotation) as GameObject;

        // 생성된 체스맨을 이 스크립트의 자식으로 설정합니다.
        ChessmanObject.transform.SetParent(this.transform);

        // 활성화된 체스맨 목록에 추가합니다.
        ActiveChessmans.Add(ChessmanObject);

        // 생성된 체스맨의 위치를 정수로 변환하여 x와 y에 저장합니다.
        int x = (int)(position.x);
        int y = (int)(position.z);

        // Chessmans 배열에 생성된 체스맨을 추가하고 현재 위치를 설정합니다.
        Chessmans[x, y] = ChessmanObject.GetComponent<Chessman>();
        Chessmans[x, y].SetPosition(x, y);
    }

    // 보드에 체스 스폰.
    private void SpawnAllChessmans()
    {
        // Spawn White Pieces
        // Rook1
        SpawnChessman(0, new Vector3(0, 0, 7));
        // Knight1
        SpawnChessman(1, new Vector3(1, 0, 7));
        // Bishop1
        SpawnChessman(2, new Vector3(2, 0, 7));
        // King
        SpawnChessman(3, new Vector3(3, 0, 7));
        // Queen
        SpawnChessman(4, new Vector3(4, 0, 7));
        // Bishop2
        SpawnChessman(2, new Vector3(5, 0, 7));
        // Knight2
        SpawnChessman(1, new Vector3(6, 0, 7));
        // Rook2
        SpawnChessman(0, new Vector3(7, 0, 7));

        // Pawns
        for (int i = 0; i < 8; i++)
        {
            SpawnChessman(5, new Vector3(i, 0, 6));
        }

        // Spawn Black Pieces
        // Rook1
        SpawnChessman(6, new Vector3(0, 0, 0));
        // Knight1
        SpawnChessman(7, new Vector3(1, 0, 0));
        // Bishop1
        SpawnChessman(8, new Vector3(2, 0, 0));
        // King
        SpawnChessman(9, new Vector3(3, 0, 0));
        // Queen
        SpawnChessman(10, new Vector3(4, 0, 0));
        // Bishop2
        SpawnChessman(8, new Vector3(5, 0, 0));
        // Knight2
        SpawnChessman(7, new Vector3(6, 0, 0));
        // Rook2
        SpawnChessman(6, new Vector3(7, 0, 0));

        // Pawns
        for (int i = 0; i < 8; i++)
        {
            SpawnChessman(11, new Vector3(i, 0, 1));
        }
    }

    bool ispromotion = false;
    // 체스 기물 이동
   public void MoveChessman(int x, int y)
    {
        // 갈 수 있는 곳이라면.
        if (allowedMoves[x, y])
        {
            Chessman opponent = Chessmans[x, y];

            // 상대 말을 잡는 코드.
            if (opponent != null)
            {
                // 상대 말을 잡음
                ActiveChessmans.Remove(opponent.gameObject);
                Destroy(opponent.gameObject);
            }

            // Pawn 프로모션.
            if (SelectedChessman.GetType() == typeof(Pawn))
            {

                //-------프로모션 이동 관리------------
                // AI
                if (y == 7)
                {
                    ActiveChessmans.Remove(SelectedChessman.gameObject);
                    Destroy(SelectedChessman.gameObject);
                    // 보통 퀸으로 소환하기 때문에 퀸으로 소환.
                    SpawnChessman(10, new Vector3(x, 0, y));
                    SelectedChessman = Chessmans[x, y];

                }
                // Player
                if (y == 0)
                {
                    ActiveChessmans.Remove(SelectedChessman.gameObject);
                    Destroy(SelectedChessman.gameObject);
                    SpawnChessman(4, new Vector3(x, 0, y));
                    SelectedChessman = Chessmans[x, y];

                    ispromotion = true;
                }
                //-------프로모션 이동 관리 끝-------
            }

            // 가고자 하는 위치가 null이라면
            Chessmans[SelectedChessman.currentX, SelectedChessman.currentY] = null;
            Chessmans[x, y] = SelectedChessman;

            // 선택된 체스말 위치 업데이트
            SelectedChessman.SetPosition(x, y);

            SelectedChessman.transform.position = new Vector3(x, 0, y);
            SelectedChessman.isMoved = true;

            // 상대 턴으로 넘김.
            //isWhiteTurn = !isWhiteTurn;

            // Outline 해제와 SelectedChessman 해제 해주고 return
            if(!ispromotion)
            Chessmans[x, y].outline.enabled = false;
            else ispromotion = false;
            SelectedChessman = null;
            Debug.Log(ispromotion);
            return;
        }

        // 체스 기물에 대한 Outline 해제.
        if (Chessmans[outline_selectpieceX, outline_selectpieceY].GetComponent<Outline>() != null)
            Chessmans[outline_selectpieceX, outline_selectpieceY].outline.enabled = false;
        // 선택된 체스맨 해제
        SelectedChessman = null;

        isWhiteTurn = !isWhiteTurn;
    }
}