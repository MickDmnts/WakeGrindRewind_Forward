using System.Runtime.CompilerServices;
using UnityEngine;

public static class Utils
{
    /// <summary>
    /// Call to print a Debug.Warning at the Unity console
    /// </summary>
    /// <param name="searchedFor">The thing you searched for</param>
    /// <param name="scriptRef">The script in which you searched for</param>
    public static void MissingComponent(string searchedFor, object scriptRef, [CallerMemberName] string callerRef = "", [CallerLineNumber] int lineRef = 0)
    {
        Debug.LogWarning($"NullReferenceException!\n{searchedFor} not found at {scriptRef} Line: {lineRef} Called from: {callerRef}");
    }
}