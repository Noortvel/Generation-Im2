using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Cell
{
    public Cell(int i, int j)
    {
        this.i = i;
        this.j = j;
    }
    public int i, j;
    public override string ToString()
    {
        return ("i: " + i + " j: " + j);
    }
    public static Cell operator -(Cell a, Cell b)
    {
        int di = a.i - b.i;
        int dj = a.j - b.j;
        return new Cell(di, dj);
    }
    public float GetLength()
    {
        return Mathf.Sqrt(i * i + j * j);
    }
    public static int GetCityLength(Cell a, Cell b)
    {
        return Mathf.Abs(a.i - b.i) + Mathf.Abs(a.j - b.j);
    }

    public bool Equals(Cell obj)
    {
        return (obj.i == i && obj.j == j);
    }
    public override bool Equals(object o)
    {
        if (o == null || o.GetType() != GetType())
        {
            return false;
        }
        Cell obj = (Cell)o;
        return (obj.i == i && obj.j == j);
    }
    public static bool operator ==(Cell a, Cell b)
    {
        return a.Equals(b);
    }
    public static bool operator !=(Cell a, Cell b)
    {
        return !(a.Equals(b));
    }
}

public class NoizedLabirint
{


    private System.Random random;
    public int[,] matrix
    {
        get;
        set;
    }
    public int width
    {
        get;
    }
    public int height
    {
        get;
    }
    public NoizedLabirint(int[,] matrix)
    {
        this.matrix = matrix;
        height = matrix.GetLength(0);
        width = matrix.GetLength(1);
        neigh = new List<Cell>(4);
        random = new System.Random();
    }
    private List<Cell> neigh;
    public Cell GetRandomCell()
    {
        int i = random.Next(0, height);
        int j = random.Next(0, width);
        while(matrix[i,j] == MatrixField.WALL)
        {
            i = random.Next(0, height);
            j = random.Next(0, width);
        }
        return new Cell(i, j);
    }
    public void CleanUpWeight()
    {
        for(int i = 0; i < height; i++)
        {
            for(int j = 0; j < width; j++)
            {
                if(matrix[i,j] != MatrixField.WALL)
                {
                    matrix[i, j] = MatrixField.UNVISITED;
                }
            }
        }
    }
    public bool isVisited(Cell cell)
    {
        if(matrix[cell.i, cell.j] == MatrixField.UNVISITED)
        {
            return false;
        }
        return true;
    }
    public void SetVisited(Cell cell)
    {
        matrix[cell.i, cell.j] = MatrixField.VISITED;
    }
    public List<Cell> GetNeight(Cell cell)
    {
        int i = cell.i;
        int j = cell.j;
        int u = i + 1;
        int d = i - 1;
        int r = j + 1;
        int l = j - 1;
        neigh.Clear();
        if (u < height && matrix[u, j] == MatrixField.UNVISITED)
        {
            neigh.Add(new Cell(u, j));
        }
        if (d >= 0 && matrix[d, j] == MatrixField.UNVISITED)
        {
            neigh.Add(new Cell(d, j));
        }
        if (r < width && matrix[i, r] == MatrixField.UNVISITED)
        {
            neigh.Add(new Cell(i, r));
        }
        if (l >= 0 && matrix[i, l] == MatrixField.UNVISITED)
        {
            neigh.Add(new Cell(i, l));
        }
        return neigh;
    }
}

public class NoizedLabyrintGenerator
{
  

    private List<Cell> currNeight;
    private int width, height;
    private System.Random random;
    private Stack<Cell> stack;

    public int offsetX
    {
        private set;
        get;
    }
    public int offsetY
    {
        private set;
        get;
    }

    public int[,] matrix
    {
        get
        {
            return field.matrix;
        }
    }
    public MatrixField field
    {
        get;
    }

    public NoizedLabyrintGenerator(int height, int width)
    {
        this.height = height;
        this.width = width;

        field = new MatrixField(height, width);

        random = new System.Random();
        stack = new Stack<Cell>();

        currNeight = new List<Cell>(4);

    }
    private void AddSubctractiveNoize(float[,] noize)
    {
        {
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    float val = noize[y, x];
                    if (val >= 1)
                    {
                        matrix[y, x] = 0;
                    }
                }
            }
            switch (offsetY)
            {
                case 1:
                    rowAddRow(0, noize);
                    break;
                case 0:
                    rowAddRow(height - 1, noize);
                    break;

            }
            switch (offsetX)
            {
                case 0:
                    colAddCol(width - 1, noize);
                    break;
                case 1:
                    colAddCol(0, noize);
                    break;
            }
        }

    }
    public void Generate(float[,] noize)
    {
        if (!(noize.GetLength(0) == height && noize.GetLength(1) == width))
        {
            Debug.LogError("|Noize| != |Labyrint|");
            Debug.Break();
        }
            for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                matrix[y, x] = MatrixField.WALL;
            }
        }
        offsetY = random.Next(0, 2);
        offsetX = random.Next(0, 2);
        int cellCount = 0;
        for (int y = offsetY; y < height; y += 2)
        {
            for (int x = offsetX; x < width; x += 2)
            {
                matrix[y, x] = MatrixField.UNVISITED;
                cellCount++;
            }
        }
        Cell curCell = FindUnVisited();
        matrix[curCell.i, curCell.j] = MatrixField.VISITED;
        cellCount--;
        while (cellCount > 0)
        {
            if (isHasUnvisitNeig2Step(curCell.i, curCell.j))
            {
                stack.Push(curCell);
                Cell k = currNeight[random.Next(0, currNeight.Count)];
                int ti = curCell.i + (k.i - curCell.i) / 2;
                int tj = curCell.j + (k.j - curCell.j) / 2;
                matrix[ti, tj] = MatrixField.NOT_WALL;
                curCell = k;
                matrix[k.i, k.j] = MatrixField.VISITED;
                cellCount--;
            }
            else if (stack.Count > 0)
            {
                curCell = stack.Pop();
            }
            else if (cellCount > 0)
            {
                curCell = FindUnVisited();
                if (curCell.i == -1 && curCell.j == -1)
                {
                    Debug.Log("NotUnvisisted, but cellCount not 0 : " + cellCount);
                    break;
                }
            }
        }
        ClearBound();
        AddSubctractiveNoize(noize);
        __generateCount++;
        if(__generateCount > 100)
        {
            Debug.Log("GENERATION LOOPED");
            Debug.Break();
            return;
        }
        if (CheckHasClosedCells())
        {
            //Debug.Log("Bad noize, Regenerate all");
            Generate(noize);
        }
        else
        {
            __generateCount = 0;
        }
    }
    private int __generateCount = 0;
    private void ClearBound()
    {
        switch (offsetY)
        {
            case 1:
                rowSetVal(0, MatrixField.NOT_WALL);
                break;
            case 0:
                rowSetVal(height - 1, MatrixField.NOT_WALL);
                break;

        }
        switch (offsetX)
        {
            case 0:
                colSetVal(width - 1, MatrixField.NOT_WALL);
                break;
            case 1:
                colSetVal(0, MatrixField.NOT_WALL);
                break;
        }
    }
    private void rowAddRow(int row, float[,] mat)
    {
        for (int i = 0; i < width; i++)
        {
            if(mat[row, i] >= 1)
            {
                matrix[row, i] = MatrixField.WALL;
            }
        }
    }
    private void colAddCol(int col, float[,] mat)
    {
        for (int i = 0; i < height; i++)
        {
            if (mat[i, col] >= 1)
            {
                matrix[i, col] = MatrixField.WALL;
            }
        }
    }
    private void rowSetVal(int row, int val)
    {
        for (int i = 0; i < width; i++)
        {
            matrix[row, i] = val;
        }
    }
    private void colSetVal(int col, int val)
    {
        for (int i = 0; i < height; i++)
        {
            matrix[i, col] = val;
        }
    }
    private Cell FindUnVisited()
    {
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (matrix[y, x] == MatrixField.UNVISITED)
                {
                    return new Cell(y, x);
                }
            }
        }
        return new Cell(-1, -1);
    }
    private bool CheckHasClosedCells()
    {
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                int u = i + 1;
                int d = i - 1;
                int r = j + 1;
                int l = j - 1;

                bool isClosed = true;

                //Если есть сосед, то проверем что он не стенка
                if (u < height)
                {
                    if (matrix[u, j] != MatrixField.WALL)
                    {
                        isClosed = false;
                    }
                }
                if (d >= 0)
                {
                    if (matrix[d, j] != MatrixField.WALL)
                    {
                        isClosed = false;
                    }
                }
                if (r < width)
                {
                    if (matrix[i, r] != MatrixField.WALL)
                    {
                        isClosed = false;
                    }
                }
                if (l >= 0)
                {
                    if(matrix[i,l] != MatrixField.WALL)
                    {
                        isClosed = false;
                    }
                }
                if (isClosed)
                {
                    return true;
                }
            }
        }
        return false;
    }
    private bool isHasUnvisitNeig2Step(int i, int j)
    {
        bool isFinded = false;
        int u = i + 2;
        int d = i - 2;
        int r = j + 2;
        int l = j - 2;
        currNeight.Clear();
        if (u < height && matrix[u, j] == MatrixField.UNVISITED)
        {
            currNeight.Add(new Cell(u, j));
            isFinded = true;
        }
        if (d >= 0 && matrix[d, j] == MatrixField.UNVISITED)
        {
            currNeight.Add(new Cell(d, j));

            isFinded = true;

        }
        if (r < width && matrix[i, r] == MatrixField.UNVISITED)
        {
            currNeight.Add(new Cell(i, r));

            isFinded = true;

        }
        if (l >= 0 && matrix[i, l] == MatrixField.UNVISITED)
        {
            currNeight.Add(new Cell(i, l));
            isFinded = true;
        }
        return isFinded;
    }
}
