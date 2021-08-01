using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ObjectMananger : MonoBehaviour
{

    public static SortedList ObjectDic=  new SortedList();

    public static int time = 0;

    public static int MAXSIZE = 100;

    public static int DELETESIZE = 50;

    public static bool AddObject(GameObject newItem)
    {
        if (ObjectDic.Count >= DELETESIZE)
        {
            Clean(5);
        }
        if (ObjectDic.Count >= MAXSIZE)
        {
            return false;
        }
        ObjectDic.Add(time, newItem);
        time += 1;
        return true;
    }

    public static bool Clean(int count)
    {
        int index = 0;
        int size = ObjectDic.Count;
        for (int i = 0; i < size; i++)
        {
            if (((GameObject)ObjectDic.GetByIndex(i)).activeSelf == false)
            {
                GameObject deleteGo = (GameObject)ObjectDic.GetByIndex(i);
                ObjectDic.RemoveAt(i);
                Destroy(deleteGo);
                size--;
                i--;
                index++;
            }
            if (index >= count)
            {
                break;
            }
        }
        if (index == 0)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
}
