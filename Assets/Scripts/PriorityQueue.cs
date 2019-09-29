using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pair<G, K> : System.Tuple<G, K>
{
    public Pair(G item1, K item2) : base(item1, item2)
    {
    }
}
public class PriorityQueue<T>
{
   
    private List<Pair<T, int>> list;

    public PriorityQueue()
    {
        list = new List<Pair<T, int>>();
    }
    public void Enqueue(T elem, int prior)
    {
        list.Add(new Pair<T, int>(elem, prior));
        list.Sort(ComparatorMin);
    }
    public T Dequeue()
    {
        var c = list[0];
        list.RemoveAt(0);
        return c.Item1;
    }
    public void Clear()
    {
        list.Clear();
    }
    private int ComparatorMin(Pair<T,int> a, Pair<T, int> b)
    {
        if(a.Item2 < b.Item2)
        {
            return -1;
        }
        if(a.Item2 > b.Item2)
        {
            return 1;
        }
        return 0;
    }
    public int Count
    {
        get { return list.Count; }
    }
    
}
