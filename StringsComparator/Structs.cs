using System.Numerics;
using System.Collections.Generic;

namespace StringsComparator
{
    internal struct Info
    {
        public int SequenceLength;
        public Vector2 Direction;
    }

    public struct DiffrencesRanges
    {
        public List<Vector2> Str1;
        public List<Vector2> Str2;
    }
}