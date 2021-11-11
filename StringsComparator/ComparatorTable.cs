using System.Numerics;

namespace StringsComparator
{
    class ComparatorTable : ComparatorTableSkeleton
    {
        // Members
        protected Info[,] _table;

        // Properties
        protected Vector2 StartIndex { get; set; } = Vector2.Zero;

        // Consturctors
        /// <summary>
        /// ComparatorTable constructor.
        /// </summary>
        /// <param name="str1">First string to compare.</param>
        /// <param name="str2">Second  string to compare.</param>
        public ComparatorTable(string str1, string str2)
            : base (str1, str2)
        {
            if (GetType() != typeof(DiagnoalComparatorTable))
                _table = new Info[_str1.Length, _str2.Length];
        }


        // Methods
        /// <summary>
        /// Calculate the dynamic programin table.
        /// </summary>
        public override void FillTable()
        {
            Initialize();
            for (int x = 1; x < _table.GetLength(0); x++)
                for (int y = 1; y < _table.GetLength(1); y++)
                    DoStep(x, y);
        }

        /// <summary>
        /// Print the calculated dynamic programing table.
        /// </summary>
        public override void PrintTable()
        {
            for (int x = 0; x < _table.GetLength(0); x++)
            {
                for (int y = 0; y < _table.GetLength(1); y++)
                    PrintingMethod(_table[x, y] + " ");
                PrintingMethod(System.Environment.NewLine);
            }

        }

        /// <summary>
        /// Calculate the value and direction of the given cell.
        /// </summary>
        /// <param name="x">X index of the cell.</param>
        /// <param name="y">Y index of the cell.</param>
        protected virtual void DoStep(int x, int y)
        {
            _table[x, y] = _table[x - 1, y - 1];
            _table[x, y].SequenceLength += _str1[x] == _str2[y] ? 1 : 0;
            _table[x, y].Direction = Vector2.One;
            if (_table[x, y].SequenceLength <= _table[x - 1, y].SequenceLength)
            {
                _table[x, y] = _table[x - 1, y];
                _table[x, y].Direction = new Vector2(1, 0);
            }
            if (_table[x, y].SequenceLength <= _table[x, y - 1].SequenceLength)
            {
                _table[x, y] = _table[x, y - 1];
                _table[x, y].Direction = new Vector2(0, 1);
            }
        }

        /// <summary>
        /// Find the longest sequence and build the stack of common chars.
        /// </summary>
        protected override void FindCommonChars()
        {
            Vector2 cell;
            if (_commonChars.Count > 0)
                cell = _commonChars.Pop() - StartIndex;
            else
                cell = GetMaxIndex();
            Info Current = _table[(int)cell.X, (int)cell.Y];
            while (Current.Direction != Vector2.Zero)
            {
                if (Current.Direction == Vector2.One)
                    _commonChars.Push(cell + StartIndex);
                cell -= Current.Direction;
                Current = _table[(int)cell.X, (int)cell.Y];
            } 

            _commonChars.Push(cell+ StartIndex);
        }

        /// <summary>
        /// Initialize cell value and direction.
        /// </summary>
        /// <param name="x">X index of the cell.</param>
        /// <param name="y">Y index of the cell.</param>
        protected override void InitalizeCell(int x, int y)
        {
            _table[x, y].SequenceLength = _str1[x] == _str2[y] ? 1 : 0;
            _table[x, y].Direction = Vector2.Zero;
        }

        /// <summary>
        /// Find the index of highest value in last column\row.
        /// </summary>
        /// <returns>Return the index of the longest sequence.</returns>
        private Vector2 GetMaxIndex()
        {
            int x = _table.GetLength(0) - 1;
            int y = _table.GetLength(1) - 1;
            Vector2 cell = new Vector2(x, y);
            int max = _table[x, y].SequenceLength;
            for (int i = x - 1; i >= 0; i--)
                if (_table[i, y].SequenceLength > max)
                {
                    max = _table[i, y].SequenceLength;
                    cell.X = i;
                }
            for (int j = y - 1; j >= 0; j--)
                if (_table[x, j].SequenceLength > max)
                {
                    max = _table[x, j].SequenceLength;
                    cell = new Vector2(x, j);
                }
            return cell;
        }
    }
}