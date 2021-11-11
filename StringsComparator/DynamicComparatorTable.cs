using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace StringsComparator
{
    class DynamicComparatorTable : ComparatorTableSkeleton
    {
        // Members
        private Dictionary<Vector2, DynamicCell> _table = new Dictionary<Vector2, DynamicCell>();
        private readonly DynamicCell _virtualCell;
        private static int _maxValueFound = -1;

        // Constructors
        /// <summary>
        /// DynamicComparatorTable Constructor.
        /// </summary>
        /// <param name="str1">First string to compare.</param>
        /// <param name="str2">Second  string to compare.</param>
        public DynamicComparatorTable(string str1, string str2)
            : base (str1,str2)
        {
            DynamicCell vc = new DynamicCell(-Vector2.One);
            vc.Value = -2;
            _virtualCell = vc;
            DynamicCell.ReleaseFunction = Release;
        }

        // Methods
        /// <summary>
        /// Print the calculated dynamic programing table.
        /// </summary>
        public override void PrintTable()
        {

            for (int y = 0; y < _str2.Length; y++)
            {
                for (int x = 0; x < _str1.Length; x++)
                {
                    DynamicCell current = TakeCell(x, y);
                    if (current != _virtualCell)
                        PrintingMethod(current.Value + " ");
                    else
                        PrintingMethod("- ");
                }
                PrintingMethod(Environment.NewLine);
            }
        }

        /// <summary>
        /// Initialize cell value and direction.
        /// </summary>
        /// <param name="x">X index of the cell.</param>
        /// <param name="y">Y index of the cell.</param>
        protected override void InitalizeCell(int x, int y)
        {
            ForceTakeCell(x, y).Value = _str1[x] == _str2[y] ? 1 : 0;
        }

        /// <summary>
        /// Calculate the dynamic programin table.
        /// </summary>
        public override void FillTable()
        {
            DiagonalIterator di = new DiagonalIterator(_str1.Length, _str2.Length);
            foreach (Tuple<int, int> index in di)
            {
                int x = index.Item1;
                int y = index.Item2;
                if (TakeCell(x,y) == _virtualCell)
                    DoStep(x,y);
                TryReleaseArea(x, y);
            }
        }

        /// <summary>
        /// Find the longest sequence and build the stack of common chars.
        /// </summary>
        protected override void FindCommonChars()
        {
            Vector2 max = FindMax();
            DynamicCell current = _table[max];
            while (current.BasedOn != null)
            {
                if (current.Value > current.BasedOn.Value)
                    _commonChars.Push(current.Location);
                current = current.BasedOn;
            }

            if (current.Value > 0)
                _commonChars.Push(current.Location);
        }

        /// <summary>
        /// Find the last indices of the longest common sequence.
        /// </summary>
        /// <returns>Return the last indices of the longest common sequence.</returns>
        private Vector2 FindMax()
        {
            Vector2 max = Vector2.Zero;
            int maxVal = 0;
            foreach (Vector2 location in _table.Keys)
                if (_table[location].Value > maxVal)
                {
                    max = location;
                    maxVal = _table[location].Value;
                }
            return max;
        }

        /// <summary>
        /// Calculate the value and direction of the given cell.
        /// </summary>
        /// <param name="index">Tuple of indices in both strings.</param>
        private void DoStep(int x, int y)
        {
            DynamicCell TopLeft = TakeCell(x - 1, y - 1);
            DynamicCell Top = TakeCell(x, y - 1);
            DynamicCell Left = TakeCell(x - 1, y);
            DynamicCell Current = ForceTakeCell(x, y);
            DynamicCell Chosen;
            int MatchAdditionalValue = _str1[x] == _str2[y] ? 1 : 0;
            if (TopLeft.Value + MatchAdditionalValue >= Top.Value)
            {
                if (TopLeft.Value + MatchAdditionalValue > Left.Value)
                {
                    Current.Value = TopLeft.Value + MatchAdditionalValue;
                    Chosen = TopLeft;
                }
                else
                {
                    Current.Value = Left.Value;
                    Chosen = Left;
                }
            }
            else
            {
                if (Left.Value >= Top.Value)
                {
                    Current.Value = Left.Value;
                    Chosen = Left;
                }
                else
                {
                    Current.Value = Top.Value;
                    Chosen = Top;
                }
            }

            if (Current.Value < 0)
                Release(Current);
            else
            {
                Chosen.AddConnection();
                Current.BasedOn = Chosen;
                if (Current.Value > _maxValueFound)
                    _maxValueFound = Current.Value;
            }
        }

        /// <summary>
        /// Get cell in given location or create it if not exist.
        /// </summary>
        /// <param name="x">X index of the cell.</param>
        /// <param name="y">Y index of the cell.</param>
        /// <returns>The cell in the given location.</returns>
        private DynamicCell ForceTakeCell(int x, int y)
        {
            Vector2 Index = new Vector2(x, y);
            if (!_table.Keys.Contains(Index))
                _table[Index] = new DynamicCell(Index);
            return _table[Index];
        }

        /// <summary>
        /// Get cell in given location or virtual cell if not exist.
        /// </summary>
        /// <param name="x">X index of the cell.</param>
        /// <param name="y">Y index of the cell.</param>
        /// <returns>The cell in the given location.</returns>
        private DynamicCell TakeCell(int x, int y)
        {
            Vector2 Index = new Vector2(x, y);
            if (!_table.Keys.Contains(Index))
                return _virtualCell;
            return _table[Index];
        }

        /// <summary>
        /// Try to release the given cell and it lefty neighbor.
        /// </summary>
        /// <param name="x">X index of the cell.</param>
        /// <param name="y">Y index of the cell.</param>
        private void TryReleaseArea(int x, int y)
        {
            Func<int, int, bool> WontReachBetter = (x, y) => TakeCell(x, y).Value + Math.Min(_str1.Length - x, _str2.Length - y) - 1 < _maxValueFound;
            Func<int, int, bool> NotEffective = (x, y) => TakeCell(x, y).Value == 0;
            Func<int, int, bool> ShouldReleased = (x, y) => WontReachBetter(x, y) || NotEffective(x, y);
            if (ShouldReleased(x, y))
                TryRelease(x, y);
            if(ShouldReleased(x-1, y))
                TryRelease(x-1, y);
            if (TakeCell(x - 1, y - 1).Value + 1 <= TakeCell(x, y).Value)
                TryRelease(x - 1, y - 1);
        }

        /// <summary>
        /// Release cell in given indices if and only if it not virtual and can be free.
        /// </summary>
        /// <param name="x">X index of the cell.</param>
        /// <param name="y">Y index of the cell.</param>
        private void TryRelease(int x, int y)
        {
           DynamicCell current = TakeCell(x,y);
            if ((current != _virtualCell) && current.CanBeFree())
                Release(current);
        }

        /// <summary>
        /// Release dynamic cell from dictionary and remove it connection from based dynamic cell.
        /// </summary>
        /// <param name="cell">The cell to release.</param>
        private void Release(DynamicCell cell)
        {
            _table.Remove(cell.Location);
            if (cell.BasedOn != null)
                cell.BasedOn.RemoveConnection();
        }

        // Nested Classes
        private class DynamicCell
        {
            // Members
            private short _nexts = 0;
            public static Action<DynamicCell> ReleaseFunction;

            // Properties
            public DynamicCell BasedOn { get; set; }

            public Vector2 Location { get; private init; }

            public int Value { get; set; }

            // Constructors
            public DynamicCell(Vector2 location)
            {
                Location = location;
            }

            // Methods
            /// <summary>
            /// Update about another dynamic cell that based on this dynamic cell.
            /// </summary>
            public void AddConnection()
            {
                _nexts++;
            }

            /// <summary>
            /// Update about removed dynamic cell that based on this dynamic cell and release this cell if it can be free.
            /// </summary>
            public void RemoveConnection()
            {
                _nexts--;
                if (CanBeFree())
                    ReleaseFunction.Invoke(this);
            }

            /// <summary>
            /// Check if there are some dynamic cell that based on this dynamic cell.
            /// </summary>
            /// <returns>True if no cell based on this cell, otherwise False.</returns>
            public bool CanBeFree()
            {
                return _nexts <= 0;
            }
        }
   }
}

