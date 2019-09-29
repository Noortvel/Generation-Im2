using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAIController : MonoBehaviour
{
    private PathFinder pathFinder;
    private GameField gameField;
    [SerializeField]
    private EnemyDetectSector detector;
    [SerializeField]
    private MeshRenderer meshRender;
    [SerializeField]
    private Material defaulMat;
    [SerializeField]
    private Material detectedMat;
    public List<Cell> path
    {
        get;
        private set;
    }
    private Cell nextCell;
    public Character controlledPawn
    {
        private set;
        get;
    }

    void Awake()
    {
        controlledPawn = GetComponent<Character>();
        if(controlledPawn == null)
        {
            print("Pawn need Character component");
        }
        controlledPawn.MoveEndEvent += PawnEndMove;
        meshRender.material = defaulMat;

    }

    private bool isInit = false;
    public void Initialize(GameField gameField, Cell mpos, Cell dest)
    {
        this.gameField = gameField;
        controlledPawn.Initialize(gameField, mpos);
        detector.Initialize(gameField.characterInstance, this);

        pathFinder = new PathFinder(gameField.field);
        path = pathFinder.CalcPath(mpos, dest);
        Debug.Log("path " + path.Count);

        meshRender.material = defaulMat;


        ResetPathIndex();
        nextCell = path[GetNextPathIndex()];
        MoveToTargetStep();
        isInit = true;
    }
    public void Initialize(GameField gameField, List<Cell> path)
    {
        if(path.Count <= 1)
        {
            Debug.Log("Path is: " + path.Count);
            Debug.Break();
        }
        this.gameField = gameField;
        this.path = path;
        //DEBUG_DRAW_PATH();
        controlledPawn.Initialize(gameField, path[0]);
        meshRender.material = defaulMat;

        detector.Initialize(gameField.characterInstance, this);
        pathFinder = new PathFinder(gameField.field);

        ResetPathIndex();
        nextCell = path[GetNextPathIndex()];
        MoveToTargetStep();
        isInit = true;
    }
    private bool isRegularPath = true;
    public void PlayerDetected()
    {
        if (isRegularPath)
        {
            gameField.characterInstance.MoveEndEvent += TargetEndMove;

            meshRender.material = detectedMat;

            isRegularPath = false;
            ResetPath();
        }
    }
    private void DEBUG_DRAW_PATH()
    {
        if (gameField.enemyInstance1 == this)
        {
            gameField.DrawColor1Path(path);
        }
        else
        {
            gameField.DrawColor3Path(path);
        }
    }
    private void ResetPath()
    {
        path = pathFinder.CalcPath(controlledPawn.mPosition, gameField.characterInstance.mPosition);
        ResetPathIndex();
        //DEBUG_DRAW_PATH();

    }
    private void TargetEndMove()
    {
        if (!isRegularPath)
        {
            ResetPath();
            MoveOnPath();

        }
    }
    private int indexDirection = 1;
    private int currentPathIndex = 0;
    private int GetNextPathIndex()
    {
        if(path.Count == 1)
        {
            return 0;
        }
        if (isRegularPath)
        {
            currentPathIndex += indexDirection;
            if (currentPathIndex == 0)
            {
                indexDirection = 1;
            }
            if (currentPathIndex == path.Count - 1)
            {
                indexDirection = -1;
            }
        }
        else
        {
            if(currentPathIndex == path.Count - 1)
            {
                ResetPath();
                return currentPathIndex;
            }
            currentPathIndex++;
        }
        return currentPathIndex;
    }
    private void ResetPathIndex()
    {
        indexDirection = 1;
        currentPathIndex = 0;
    }
    private void MoveToTargetStep()
    {
        int hor = (nextCell.j - controlledPawn.mPosition.j);
        int ver = -(nextCell.i - controlledPawn.mPosition.i);
        if (ver > 0)
        {
            controlledPawn.MoveDown();
        }
        if (ver < 0)
        {
            controlledPawn.MoveUp();
        }
        if (hor > 0)
        {
            controlledPawn.MoveRight();
        }
        if (hor < 0)
        {
            controlledPawn.MoveLeft();
        }
    }
    private bool isMoving = false;
    private void MoveOnPath()
    {

        isMoving = true;
        int index = GetNextPathIndex();
        nextCell = path[index];
        MoveToTargetStep();

    }
    private void PawnEndMove()
    {
        if (isRegularPath)
        {
            MoveOnPath();
        }
        else
        {
            ResetPath();
            MoveOnPath();
        }
    }
}
