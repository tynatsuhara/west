using System.Collections.Generic;

// Because SortedLists don't by default store values with duplicate keys

[System.Serializable]
public class DuplicateKeyComparer<TKey> : IComparer<TKey> where TKey : System.IComparable {
    public int Compare(TKey x, TKey y) {
        int result = x.CompareTo(y);
        return result == 0 ? 1 : result;
    }
}