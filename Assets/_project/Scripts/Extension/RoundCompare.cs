using System;
using UnityEngine;

public static class RoundCompare 
{
    public static bool EqualsRounded(this Vector2 a, Vector2 b)
    {
        Vector2 aRounded = new Vector2((float)Math.Round(a.x, 3), (float)Math.Round(a.y, 3));
        Vector2 bRounded = new Vector2((float)Math.Round(b.x, 3), (float)Math.Round(b.y, 3));

        return aRounded.Equals(bRounded);
    }
}
                                                                                                    