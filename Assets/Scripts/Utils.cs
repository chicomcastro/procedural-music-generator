using System.Collections;
using System.Collections.Generic;

public static class Utils
{
    public static void PopulateWithSingleValue<T>(this T[] arr, T value)
    {
        for (int i = 0; i < arr.Length; i++)
        {
            arr[i] = value;
        }
    }

    public static void PopulateWithCrescentValues<T>(this T[] arr, T initialValue)
    {
        for (int i = 0; i < arr.Length; i++)
        {
            arr[i] = initialValue;
        }
    }
}
