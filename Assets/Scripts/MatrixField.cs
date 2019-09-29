using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatrixField
{
    public const int VISITED = 3;
    public const int UNVISITED = 0;
    public const int WALL = 1;
    public const int NOT_WALL = 2;
    public const int RESERVED = 100;

    private System.Random random;
    private List<Cell> neigh;

    public int[,] matrix
    {
        get;
    }
    public int width
    {
        get;
    }
    public int height
    {
        get;
    }
    private int[,] chachedMatrix;
    public MatrixField(int height, int width)
    {
        matrix = new int[height, width];
        this.height = height;
        this.width = width;
        neigh = new List<Cell>(4);
        random = new System.Random();
        openList = new List<Cell>(height + width);
    }
  
    public Cell GetRandomCell()
    {
        CleanUpAll();
        var cells = CalcOpenCells();
        return cells[random.Next(0, cells.Count)];
        //int i = random.Next(0, height);
        //int j = random.Next(0, width);
        //while (matrix[i, j] == WALL)
        //{
        //    i = random.Next(0, height);
        //    j = random.Next(0, width);
        //}
        //return new Cell(i, j);
    }
    public Cell GetRandomCellNotInList(List<Cell> blacklist)
    {
        CleanUpAll();
        foreach(Cell x in blacklist)
        {
            matrix[x.i, x.j] = RESERVED;
        }
        var cells = CalcOpenCells();
        CleanUpAll();      
        return cells[random.Next(0, cells.Count)];
    }
    private List<Cell> openList;
    public List<Cell> CalcOpenCells()
    {
        openList.Clear();
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                if (matrix[i, j] != WALL && matrix[i, j] != RESERVED)
                {
                    openList.Add(new Cell(i, j));
                }
            }
        }
        return openList;
    }
    public void CleanUpAll()
    {
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                if (matrix[i, j] != WALL)
                {
                    matrix[i, j] = UNVISITED;
                }
            }
        }
    }
    public bool isVisited(Cell cell)
    {
        if (matrix[cell.i, cell.j] == UNVISITED)
        {
            return false;
        }
        return true;
    }

    public void SetVisited(Cell cell)
    {
        matrix[cell.i, cell.j] = VISITED;
    }
    public bool isUpNeightFree(Cell position)
    {
        int i = position.i;
        int j = position.j;
        int u = i + 1;
        if (u < height && matrix[u, j] != WALL)
        {
            return true;
        }
        return false;
    }
    public bool isDownNeightFree(Cell position)
    {
        int i = position.i;
        int j = position.j;
        int d = i - 1;
        if (d >= 0 && matrix[d, j] != WALL)
        {
            return true;
        }
        return false;
    }
    public bool isRightNeightFree(Cell position)
    {
        int i = position.i;
        int j = position.j;
        int r = j + 1;
        if (r < width && matrix[i, r] != WALL)
        {
            return true;
        }
        return false;
    }
    public bool isLeftNeightFree(Cell position)
    {
        int i = position.i;
        int j = position.j;
        int l = j - 1;
        if (l >= 0 && matrix[i, l] != WALL)
        {
            return true;
        }
        return false;
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
        if (u < height && matrix[u, j] != WALL)
        {
            neigh.Add(new Cell(u, j));
        }
        if (d >= 0 && matrix[d, j] != WALL)
        {
            neigh.Add(new Cell(d, j));
        }
        if (r < width && matrix[i, r] != WALL)
        {
            neigh.Add(new Cell(i, r));
        }
        if (l >= 0 && matrix[i, l] != WALL)
        {
            neigh.Add(new Cell(i, l));
        }
        return neigh;
    }
}
