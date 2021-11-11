using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace StringsComparator
{
    class DiagnoalComparatorTable : ComparatorTable
    {
        // Members
        private DiagnoalComparatorTable _next;
        private bool _isFirst = true;

        // Properties
        protected override List<Vector2> CommonChars
        {
            get
            {
                if (_commonChars != null)
                    return _commonChars.ToList();

                if (_next != null)
                {
                    List<Vector2> temp = _next.CommonChars;
                    _commonChars = _next._commonChars;
                    FindCommonChars();
                }

                return base.CommonChars;
            }
        }

        // Constructors
        public DiagnoalComparatorTable(string str1, string str2, Vector2? proportion = null, Vector2? start = null, ushort maxTableSize = 1000)
            : base(str1, str2)
        {
            StartIndex = start.HasValue ? start.Value : Vector2.Zero;

            if (!proportion.HasValue)
            {
                int amount = (int)Math.Ceiling(Math.Max(str1.Length, str2.Length) / (float) maxTableSize);
                proportion = new Vector2(str1.Length / amount, str2.Length / amount);
            }

            Vector2 step = new Vector2((int)Math.Ceiling(proportion.Value.X / 2), (int)Math.Ceiling(proportion.Value.Y / 2));
            if (str1.Length < proportion.Value.X || str2.Length < proportion.Value.Y)
            {
                _table = new Info[str1.Length, str2.Length];
                _next = null;
            }
            else
            {
                _table = new Info[(int)proportion.Value.X, (int)proportion.Value.Y];
                _next = new DiagnoalComparatorTable(str1.Substring((int)step.X), str2.Substring((int)step.Y), proportion, StartIndex + step);
                _next._isFirst = false;
            }
        }

        // Methods
        /// <summary>
        /// Print the calculated dynamic programing table.
        /// </summary>
        public override void PrintTable()
        {
            for (int i = 0; i < _table.GetLength(0); i++)
            {
                for (int j = 0; j < _table.GetLength(1); j++)
                    PrintingMethod(_table[i, j].SequenceLength + " ");
                PrintingMethod(Environment.NewLine);
            }
            PrintingMethod(string.Format("{0}{0}",Environment.NewLine));
            if (_next != null)
                _next.PrintTable();
        }

        /// <summary>
        /// Calculate the dynamic programin table.
        /// </summary>
        public override void FillTable()
        {
            int halfX = _table.GetLength(0) / 2 - 1;
            int halfY = _table.GetLength(1) / 2 - 1;
            for (int x = 1; x < _table.GetLength(0); x++)
                for (int y = 1; y < _table.GetLength(1); y++)
                    if (_isFirst || (x > halfX) || (y > halfY))
                        DoStep(x, y);
            if (_next != null)
            {
                _next.Initialize();
                ConnectNextTable();
                _next.FillTable();
            }
        }

        /// <summary>
        /// Complete first quarter values from last quarter of previous table.
        /// </summary>
        private void ConnectNextTable()
        {
            int maxX = _table.GetLength(0) / 2;
            int maxY = _table.GetLength(1) / 2;
            int startX = _table.GetLength(0) - maxX;
            int startY = _table.GetLength(1) - maxY;
            for (int x = 0; x < maxX; x++)
                for (int y = 0; y < maxY; y++)
                {
                    _next._table[x, y] = _table[x + startX, y + startY];
                    if ((x == 0) || (y == 0))
                       _next._table[x, y].Direction = Vector2.Zero;
                }
        }
    }
}
