using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFinder
{
    public class CellChain
    {
        public Cell cell;
        public CellChain from;
        public CellChain(Cell cur, CellChain from)
        {
            this.cell = cur;
            this.from = from;
        }
    }
    public List<Cell> path
    {
        get;
        private set;
    }
    private Queue<CellChain> queue;
    private PriorityQueue<CellChain> priorQueue;
    private MatrixField field;
    private int[,] weights;
   public PathFinder(MatrixField field)
    {
        this.field = field;
        weights = new int[field.height, field.width];
        queue = new Queue<CellChain>();
        priorQueue = new PriorityQueue<CellChain>();
    }
    public List<Cell> CalcPath(Cell start, Cell end)
    {
        return CalcPath_AStar(start, end);        
    }
    public void SetWeight(Cell cell, int weight)
    {
        weights[cell.i, cell.j] = weight;
    }
    public int GetWeight(Cell cell)
    {
        return (weights[cell.i, cell.j]);
    }
    public void ClearWeights()
    {
        for(int i = 0; i < field.height; i++)
        {
            for(int j = 0; j < field.width; j++)
            {
                weights[i, j] = 0;
            }
        }
    }
    public List<Cell> CalcPath_AStar(Cell start, Cell end)
    {
        priorQueue.Clear();
        ClearWeights();
        CellChain tail = new CellChain(start, null);
        priorQueue.Enqueue(tail, 0);
        SetWeight(start, 1);
        while (priorQueue.Count > 0)
        {
          
            CellChain current = priorQueue.Dequeue();
            Cell curcell = current.cell;
            if (curcell.Equals(end))
            {
                tail = current;
                break;
            }
            List<Cell> neights = field.GetNeight(current.cell);
            for (int i = 0; i < neights.Count; i++) 
            {
                Cell neight = neights[i];
                int newCost = GetWeight(current.cell) + 1;
                if (GetWeight(neight) == 0 || newCost < GetWeight(neight))
                {
                    SetWeight(neight, newCost);
                    CellChain cc = new CellChain(neight, current);
                    int prior = newCost + Cell.GetCityLength(neight, end);
                    priorQueue.Enqueue(cc, prior);
                }
            }
        }
        path = new List<Cell>();
        CellChain root = tail;
      
        while (root != null)
        {
            path.Add(root.cell);
            root = root.from;
        }
        path.Reverse();
        return path;
    }
    public List<Cell> CalcPath_BFS(Cell start, Cell end)
    {

        queue.Clear();

        CellChain tail = new CellChain(start, null);
        queue.Enqueue(tail);

        field.CleanUpAll();
        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            field.SetVisited(current.cell);
            foreach (Cell x in field.GetNeight(current.cell))
            {
                if (!field.isVisited(x))
                {
                    var cc = new CellChain(x, current);
                    if (end.i == x.i && end.j == x.j)
                    {
                        tail = cc;
                        break;
                    }
                    queue.Enqueue(cc);
                }
            }
        }
        path = new List<Cell>();
        CellChain root = tail;
        while (root != null)
        {
            path.Add(root.cell);
            root = root.from;
        }
        path.Reverse();
        return path;
    }

    public static int GetPathsIntersects(List<Cell> p1, List<Cell> p2)
    {
        int count = 0;
        for(int i = 0; i < p1.Count; i++)
        {
            for(int j = 0; j < p2.Count; j++)
            {
                if (p1[i].Equals(p2[j]))
                {
                    count++;
                }
            }
        }
        return count;
    }
    
}
