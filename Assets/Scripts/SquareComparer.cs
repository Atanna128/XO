// This class is not demonstrated in the Main method
// and is provided only to show how to implement
// the interface. It is recommended to derive
// from Comparer<T> instead of implementing IComparer<T>.
using System;
using System.Collections.Generic;

public class SquareComparer : IComparer<Square>
{
    // Compares by Height, Length, and Width.
    public int Compare(Square x, Square y)
    {
       
            return 0;
       
    }
}