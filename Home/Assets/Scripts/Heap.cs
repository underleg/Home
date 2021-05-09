using System;
using System.Collections;
using UnityEngine;

public class Heap<T> where  T: IHeapItem<T>
{
    T[] m_items;
    int m_currentCount;

    public Heap(int maxSize)
    {
        m_items = new T[maxSize];
    }

    public void Add(T item)
    {
        item.HeapIndex = m_currentCount;
        m_items[m_currentCount] = item;
        SortUp(item);
        m_currentCount++;
    }

    void SortUp(T item)
    {
        int parentIndex = (item.HeapIndex - 1) / 2;

        while(true)
        {
            T parentItem = m_items[parentIndex];
            if(item.CompareTo(parentItem) > 0)
            {
                Swap(item, parentItem);
            }
            else
            {
                break;
            }
        }
    }

    public T RemoveFirst()
    {
        T firstItem = m_items[0];
        m_currentCount--;
        m_items[0] = m_items[m_currentCount];
        m_items[0].HeapIndex = 0;
        SortDown(m_items[0]);
        return firstItem;
    }

    public int Count
    {
        get { return m_currentCount;  }
    }

    public void UpdateItem(T item)
    {
        SortUp(item);
    }

    public bool Contains(T item)
    {
        return Equals(m_items[item.HeapIndex], item);
    }

    void SortDown(T item)
    {
        while(true)
        {
            int childIndexLeft = item.HeapIndex * 2 + 1;
            int childIndexRight = item.HeapIndex * 2 + 2;
            int swapIndex = 0;
            if(childIndexLeft < m_currentCount)
            {
                swapIndex = childIndexLeft;

                if(childIndexRight < m_currentCount)
                {
                    if(m_items[childIndexLeft].CompareTo(m_items[childIndexRight]) < 0)
                    {
                        swapIndex = childIndexRight;
                    }
                }

                if(item.CompareTo(m_items[swapIndex]) < 0)
                {
                    Swap(item, m_items[swapIndex]);
                }
                else
                {
                    return;
                }
            }
            else
            {
                return;
            }
        }

    }

    void Swap(T itemA, T itemB)
    {
        m_items[itemA.HeapIndex] = itemB;
        m_items[itemB.HeapIndex] = itemA;
        int itemAIndex = itemA.HeapIndex;
        itemA.HeapIndex = itemB.HeapIndex;
        itemB.HeapIndex = itemAIndex;
    }
}

public interface IHeapItem<T> : IComparable<T>
{
    int HeapIndex
    {
        get;
        set;
    }
}