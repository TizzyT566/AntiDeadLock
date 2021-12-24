using System.Collections;

namespace System.Threading;

/// <summary>
/// Locks multiple objects in an order that prevents deadlocking.
/// </summary>
public class AntiDeadLock
{
    private class ObjectHashCode64Comparer : IComparer
    {
        public int Compare(object x, object y) => x.GetHashCode64().CompareTo(y.GetHashCode64());
    }

    private readonly object[] _objects;

    /// <summary>
    /// Creates a new AntiDeadLock object and locks the objects specified.
    /// </summary>
    /// <param name="objects">The list of objects to lock.</param>
    public AntiDeadLock(params object[] objects)
    {
        try { }
        finally
        {
            if (objects != null && objects.Length > 1)
            {
                for (int i = 0; i < objects.Length - 1; i++)
                    for (int j = i + 1; j < objects.Length; j++)
                        if (objects[i] == objects[j])
                            throw new ArgumentException("Cannot lock multiple times of the same object.");
                foreach (object obj in objects)
                    if (obj == null || obj.GetType().IsValueType)
                        throw new Exception("Cannot lock null or value types.");
                Array.Sort(objects, new ObjectHashCode64Comparer());
                _objects = objects;
                foreach (object obj in objects)
                {
                    if (!Monitor.IsEntered(obj))
                        Monitor.Enter(obj);
                }
            }
            else
                throw new ArgumentException("Must provide 2 or more non-null objects for locking.");
        }
    }

    /// <summary>
    /// Release all locked objects.
    /// </summary>
    public void ReleaseLocks()
    {
        try { }
        finally
        {
            foreach (object obj in _objects)
                while (Monitor.IsEntered(obj))
                    Monitor.Exit(obj);
        }
    }
}

/// <summary>
/// Exposes GetHashCode64() method.
/// </summary>
public static class ObjectExtenstions
{
    /// <summary>
    /// Generates a 64bit hashcode.
    /// </summary>
    /// <param name="this">The object to generate the hashcode for.</param>
    /// <returns>A long representing the 64 bit hashcode.</returns>
    public static long GetHashCode64(this object @this)
    {
        long result = @this.GetType().GetHashCode();
        result <<= 32;
        return result + @this.GetHashCode();
    }
}