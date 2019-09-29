using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{

    public GameField gameField
    {
        private set;
        get;
    }
    private MatrixField matrixField;

    [SerializeField]
    private Transform _center;
    [SerializeField]
    private float _radius;
    [SerializeField]
    private float speedCellPerSec;
    [SerializeField]
    private float rotationSpeed;

    public Vector3 toMoveRotation
    {
        private set;
        get;
    }
    public float radius
    {
        get { return _radius; }
    }
    public Transform center
    {
        get { return _center; }
    }
    public Cell mPosition
    {
        get;
        private set;
    }
    public bool isMoving
    {
        private set;
        get;
    }

    private float moveDistance = 0;
 
    private Vector3 moveDirect;

    public event System.Action MoveStartEvent = delegate { };
    public event System.Action MoveEndEvent = delegate { };


    private float step;

    private bool isInit = false;
    public void Initialize(GameField gameField, Cell position)
    {
        this.gameField = gameField;
        matrixField = gameField.field;
        mPosition = new Cell(position.i, position.j);
        //step = gameField.baseCell.size;
        isMoving = false;
        isInit = true;

    }
       
    void Update()
    {
        if (isInit && GameField.isGame)
        {
            MoveUpdate();
        }
    }
    public void MoveUp()
    {
        if (matrixField.isUpNeightFree(mPosition))
        {
            MoveVert(1);
        }

    }
    public void MoveDown()
    {
        if (matrixField.isDownNeightFree(mPosition))
        {
            MoveVert(-1);
        }
    }
    public void MoveRight()
    {
        if(matrixField.isRightNeightFree(mPosition))
        {
            MoveHoriz(1);

        }
    }
    public void MoveLeft()
    {
        if (matrixField.isLeftNeightFree(mPosition))
        {
            MoveHoriz(-1);
        }
    }
    private void MoveUpdate()
    {
        if (isMoving)
        {
            if (moveDistance > 0)
            {
                float val = speedCellPerSec * Time.deltaTime;
                moveDistance -= val;
                transform.position += moveDirect * val;
            }
            else
            {
                isMoving = false;
                MoveEndEvent();
            }
        }
    }
    

    //z
    private void MoveVert(int dir)
    {
        if (!isMoving)
        {
            float angle = dir > 0 ? 0 : 180;
            toMoveRotation = new Vector3(0, angle, 0);

            moveDistance = speedCellPerSec;

            mPosition = new Cell(mPosition.i + dir, mPosition.j);
            //mPosition.i += dir;
            moveDirect = new Vector3(0, 0, dir);
            isMoving = true;
            MoveStartEvent();
        }

    }
    //x
    private void MoveHoriz(int dir)
    {
        if (!isMoving)
        {
            float angle = dir > 0 ? 90 : -90;
            toMoveRotation = new Vector3(0, angle, 0);


            moveDistance = speedCellPerSec;

            mPosition = new Cell(mPosition.i , mPosition.j + dir);

            moveDirect = new Vector3(dir, 0, 0);
            isMoving = true;
            MoveStartEvent();
        }
    }
    
}
