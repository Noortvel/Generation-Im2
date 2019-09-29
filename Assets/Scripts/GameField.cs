using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System;

public class GameField : MonoBehaviour
{
    public enum GameEndType
    {
        LOSE,
        WIN
    }

    public readonly int width = 10, height = 10;
    [SerializeField]
    private CellSqare colorCell;
    [SerializeField]
    private CellSqare colorCell2;
    [SerializeField]
    private CellSqare colorCell3;

    [SerializeField]
    private Wall wall;
    [SerializeField]
    private EnemyAIController enemy;
    [SerializeField]
    private Character character;
    [SerializeField]
    private AccusticNoizeDetector acusitcDetector;

    public static bool isGame
    {
        get;
        private set;
    }
    public Character characterInstance
    {
        get;
        private set;
    }
    public EnemyAIController enemyInstance1
    {
        get;
        private set;
    }
    public EnemyAIController enemyInstance2
    {
        get;
        private set;
    }

    public CellSqare baseCell
    {
        get { return colorCell; }
    }

    private float[,] noizeField;

    public float noizePower;
    public float noizeShaperPower;



    private PathFinder pathFinder;
    private NoizeGenerator noizeGenerator;
    private NoizedLabyrintGenerator noizeLabyrintGenerator;

    private List<Wall> walls;
    private List<CellSqare> sqares;
    private List<CellSqare> sqares2;
    private List<CellSqare> sqares3;
    private CellSqare startGrounCell;
    private CellSqare endGroundCell;




    public MatrixField field
    {
        get;
        private set;
    }

    void Awake()
    {
        GameField.isGame = true;
        noizeField = new float[height, width];
        walls = new List<Wall>(100);
        sqares = new List<CellSqare>(20);
        sqares2 = new List<CellSqare>(20);
        sqares3 = new List<CellSqare>(20);

        noizeLabyrintGenerator = new NoizedLabyrintGenerator(width, height);
        noizeGenerator = new NoizeGenerator(noizeField, noizePower, noizeShaperPower);
        pathFinder = new PathFinder(noizeLabyrintGenerator.field);

        Generate();
    }
    private bool isGenerated = false;

    public void ReGeneratField()
    {
        isGenerated = false;
        if (enemyInstance1 != null)
        {
            Destroy(enemyInstance1.gameObject);
        }
        if (enemyInstance2 != null)
        {
            Destroy(enemyInstance2.gameObject);
        }
        if (characterInstance != null)
        {
            Destroy(characterInstance.gameObject);
        }
        if (startGrounCell != null)
        {
            Destroy(startGrounCell.gameObject);
        }
        if (endGroundCell != null)
        {
            Destroy(endGroundCell.gameObject);
        }
        EndGameMenuSceneScript.Hide();
        isGame = true;
        Generate();


    }
    private void Generate()
    {
        foreach(var x in walls)
        {
            Destroy(x.gameObject);
        }
        walls.Clear();

        noizeLabyrintGenerator.Generate(noizeField);
        field = noizeLabyrintGenerator.field;

        CalculateStartPoints();

        startGrounCell = InstacneCell(colorCell2, startCell);
        endGroundCell = InstacneCell(colorCell3, endCell);

        InstacneCharacter(startCell);

        enemyInstance1 = InstanceEnemy(enemy1StartCell, enemyPath1);
        enemyInstance2 = InstanceEnemy(enemy2StartCell, enemyPath2);

        acusitcDetector.Initialize(this);
        InstanceBlocksFromField(field.matrix);

        isGenerated = true;


    }
    private int hevDist = 5;
    private int hevGenMaxCount = 10;
    private int pathIntersectMaxCount = 6;
    private int minEnemyRegPathDistance = 3;

    private Cell startCell, endCell;
    private List<Cell> enemyPath1;
    private List<Cell> enemyPath2;
    private Cell enemy1StartCell, enemy2StartCell, enemy1EndCell, enemy2EndCell;
    private void CalculateStartPoints()
    {
        for(int i = 0; i < hevGenMaxCount; i++)
        {
            Cell sc = field.GetRandomCell();
            Cell ec = field.GetRandomCell();
            if((sc - ec).GetLength() >= hevDist)
            {
                startCell = sc;
                endCell = ec;
                break;
            }
        }
        var path1 = pathFinder.CalcPath(startCell, endCell);

        List<Cell> blackList = new List<Cell>(2);
        //field.GetNeight(startCell);
        //blackList.AddRange(field.GetNeight(endCell));
        blackList.Add(startCell);
        blackList.Add(endCell);
        blackList.AddRange(field.GetNeight(endCell));
        blackList.AddRange(field.GetNeight(startCell));

        Cell c1 = field.GetRandomCellNotInList(blackList);
        Cell c2 = field.GetRandomCellNotInList(blackList);
        Cell c3 = field.GetRandomCellNotInList(blackList);
        Cell c4 = field.GetRandomCellNotInList(blackList);

        //Cell c1 = field.GetRandomCell();
        //Cell c2 = field.GetRandomCell();
        //Cell c3 = field.GetRandomCell();
        //Cell c4 = field.GetRandomCell();

        bool isBad(Cell x1, Cell x2, Cell x3, Cell x4) => Cell.GetCityLength(x1, x2) <= minEnemyRegPathDistance || Cell.GetCityLength(x3, x4) <= minEnemyRegPathDistance;
        for (int i = 0; isBad(c1, c2, c3, c4) && i < hevGenMaxCount * hevGenMaxCount; i++)
        {
            c1 = field.GetRandomCellNotInList(blackList);
            c2 = field.GetRandomCellNotInList(blackList);
            c3 = field.GetRandomCellNotInList(blackList);
            c4 = field.GetRandomCellNotInList(blackList);

        }
        if (isBad(c1, c2, c3, c4))
        {
            ReGeneratField();
        }
        //Debug.Log(c1 + " " + c2);
        //Debug.Log(c3 + " " + c4);

        enemy1StartCell = c1;
        enemy2StartCell = c3;
        enemyPath1 = pathFinder.CalcPath(c1, c2);
        enemyPath2 = pathFinder.CalcPath(c3, c4);
        //Debug.Log(enemyPath1.Count);
        //Debug.Log(enemyPath2.Count);


        //DrawColor1Path(path1);
        //DrawColor2Path(enemyPath1);
        //DrawColor1Path(enemyPath2);
    }
    private EnemyAIController InstanceEnemy(Cell mpos, Cell dest)
    {
        var obj = Instantiate(enemy, new Vector3(mpos.j * baseCell.size, 0, mpos.i * baseCell.size), transform.rotation);
        obj.Initialize(this, mpos, dest);
        return obj;
    }
    private EnemyAIController InstanceEnemy(Cell mpos, List<Cell> path)
    {
        //var mpos = path[0];
        var obj = Instantiate(enemy, new Vector3(mpos.j * baseCell.size, 0, mpos.i * baseCell.size), transform.rotation);
        obj.Initialize(this, path);
        return obj;
    }
    private void InstacneCharacter(Cell mpos)
    {
        characterInstance = Instantiate(character, new Vector3(mpos.j * baseCell.size, 0, mpos.i * baseCell.size), transform.rotation);
        characterInstance.Initialize(this, mpos);
    }
    public void InstanceBlocksFromField(int[,] field)
    {
        for (int i = 0; i < height ; i++)
        {
            for (int j = 0; j < width ; j++)
            {
                if(field[i, j] == 1)
                {
                    walls.Add(Instantiate(wall, new Vector3(j * wall.size, 0, i * wall.size), transform.rotation));
                }
            }
        }
    }
    private CellSqare InstacneCell(CellSqare prefab, Cell pos)
    {
        var obj = Instantiate(prefab, new Vector3(pos.j * prefab.size, 0, pos.i * prefab.size), transform.rotation);
        return obj;
    }
    public void DrawColor1Path(List<Cell> path)
    {
        foreach (var x in sqares)
        {
            Destroy(x.gameObject);
        }
        sqares.Clear();

        foreach (var x in path)
        {

            sqares.Add(Instantiate(colorCell, new Vector3(x.j * colorCell.size, 0, x.i * colorCell.size), transform.rotation));
        }
    }
    public void DrawColor2Path(List<Cell> path)
    {
        foreach (var x in sqares2)
        {
            Destroy(x.gameObject);
        }
        sqares2.Clear();

        foreach (var x in path)
        {

            sqares2.Add(Instantiate(colorCell2, new Vector3(x.j * colorCell2.size, 0, x.i * colorCell2.size), transform.rotation));
        }
    }
    public void DrawColor3Path(List<Cell> path)
    {
        foreach (var x in sqares2)
        {
            Destroy(x.gameObject);
        }
        sqares2.Clear();

        foreach (var x in path)
        {

            sqares2.Add(Instantiate(colorCell3, new Vector3(x.j * colorCell3.size, 0, x.i * colorCell3.size), transform.rotation));
        }
    }
    public void EndGame(GameEndType type)
    {
        //Time.timeScale = 0;
        if (isGame)
        {
            GameField.isGame = false;
            string text = "";
            if (type == GameEndType.LOSE)
            {
                text = "You Lose";
            }
            else
            {
                text = "You Win";
            }
            EndGameMenuSceneScript.Show(text);
            //Debug.Break();
        }


    }
    //private float _time = 0;
    //private void _TimeTick()
    //{
    //    ReGeneratField();
    //}
    void Update()
    {
        //_time -= Time.deltaTime;
        //if (_time < 0)
        //{
        //    _time = 0.1f;
        //    _TimeTick();
        //}
        if (isGenerated)
        {
            if (characterInstance.mPosition.Equals(endCell) && GameField.isGame)
            {
                EndGame(GameEndType.WIN);
            }
        }
    }
}
