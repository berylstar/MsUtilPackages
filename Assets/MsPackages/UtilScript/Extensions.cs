using System;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections.Generic;

public static class Extensions
{
    #region Enumerate
    /// <summary>
    /// 배열 셔플
    /// </summary>
    public static void Shuffle<T>(this T[] array)
    {
        if (array == null || array.Length <= 1)
            return;

        int randomIndex;
        T tempValue;

        // Fisher-Yates 셔플
        for (int i = array.Length - 1; i > 0; i--)
        {
            randomIndex = Utils.RandomRangeInt(0, i + 1);

            tempValue = array[i];
            array[i] = array[randomIndex];
            array[randomIndex] = tempValue;
        }
    }

    /// <summary>
    /// 리스트 셔플
    /// </summary>
    public static void Shuffle<T>(this List<T> list)
    {
        if (list == null || list.Count <= 1)
            return;

        int randomIndex;
        T tempValue;

        // Fisher-Yates 셔플
        for (int i = list.Count - 1; i > 0; i--)
        {
            randomIndex = Utils.RandomRangeInt(0, i + 1);

            tempValue = list[i];
            list[i] = list[randomIndex];
            list[randomIndex] = tempValue;
        }
    }

    /// <summary>
    /// 배열의 무작위 원소 가져오기
    /// </summary>
    public static T GetRandomElement<T>(this T[] array)
    {
        if (array == null || array.Length == 0)
            return default;

        return array[Utils.RandomRangeInt(0, array.Length)];
    }

    /// <summary>
    /// 리스트의 무작위 원소 가져오기
    /// </summary>
    public static T GetRandomElement<T>(this List<T> list)
    {
        if (list == null || list.Count == 0)
            return default;

        return list[Utils.RandomRangeInt(0, list.Count)];
    }

    /// <summary>
    /// 리스트의 무작위 원소들 가져오기
    /// </summary>
    public static T[] GetRandomElements<T>(this T[] sourceArray, int count)
    {
        if (sourceArray == null || sourceArray.Length == 0 || count <= 0)
        {
            return new T[] { };
        }

        // 뽑으려는 개수가 리스트 전체 크기보다 크면 전체 크기로 제한
        int actualCount = Math.Min(count, sourceArray.Length);

        int[] indices = new int[sourceArray.Length];
        for (int i = 0; i < sourceArray.Length; i++)
        {
            indices[i] = i;
        }

        T[] result = new T[actualCount];

        for (int i = 0; i < actualCount; i++)
        {
            int randomIndex = Utils.RandomRangeInt(i, indices.Length);

            (indices[randomIndex], indices[i]) = (indices[i], indices[randomIndex]);

            result[i] = sourceArray[indices[i]];
        }

        return result;
    }

    /// <summary>
    /// 리스트의 무작위 원소들 가져오기
    /// </summary>
    public static List<T> GetRandomElements<T>(this List<T> sourceList, int count)
    {
        if (sourceList == null || sourceList.Count == 0 || count <= 0)
        {
            return new List<T>();
        }

        // 뽑으려는 개수가 리스트 전체 크기보다 크면 전체 크기로 제한
        int actualCount = Math.Min(count, sourceList.Count);

        List<int> indices = new List<int>(sourceList.Count);
        for (int i = 0; i < sourceList.Count; i++)
        {
            indices.Add(i);
        }

        List<T> result = new List<T>(actualCount);

        for (int i = 0; i < actualCount; i++)
        {
            int randomIndex = Utils.RandomRangeInt(i, indices.Count);

            (indices[randomIndex], indices[i]) = (indices[i], indices[randomIndex]);

            result.Add(sourceList[indices[i]]);
        }

        return result;
    }
    #endregion
}
