using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace StringsComparator
{
    abstract class ComparatorTableSkeleton: IComparatorTable
    {
        // Members
        public static Action<string> PrintingMethod = Console.Write;
        private static Action startMarkFunction = () => Console.BackgroundColor = ConsoleColor.DarkYellow;
        private static Action stopMarkFunction = () => Console.BackgroundColor = ConsoleColor.Black;
        protected string _str1, _str2;
        protected Stack<Vector2> _commonChars = null;
        private DiffrencesRanges? _diffrences = null;

        // Properties
        protected virtual List<Vector2> CommonChars
        {
            get
            {
                if (_commonChars == null)
                {
                    _commonChars = new Stack<Vector2>();
                    FindCommonChars();
                }

                return _commonChars.ToList();
            }
        }

        public DiffrencesRanges DiffrencesSequences
        {
            get
            {
                if (!_diffrences.HasValue)
                {
                    List<int> com1 = CommonChars.Select(v => (int)v.X).ToList();
                    List<int> com2 = CommonChars.Select(v => (int)v.Y).ToList();
                    DiffrencesRanges d;
                    d.Str1 = IndicesToRanges(_str1, com1);
                    d.Str2 = IndicesToRanges(_str2, com2);
                    _diffrences = d;
                }
                return _diffrences.Value;
            }
        }

        // Constructors
        public ComparatorTableSkeleton(string str1, string str2)
        {
            _str1 = str1;
            _str2 = str2;
        }

        // Methods
        /// <summary>
        /// Calculate the dynamic programin table.
        /// </summary>
        abstract public void FillTable();

        /// <summary>
        /// Print the calculated dynamic programing table.
        /// </summary>
        abstract public void PrintTable();

        /// <summary>
        /// Find the longest sequence and build the stack of common chars.
        /// </summary>
        abstract protected void FindCommonChars();
        
        /// <summary>
        /// Initialize cell value and direction.
        /// </summary>
        /// <param name="x">X index of the cell.</param>
        /// <param name="y">Y index of the cell.</param>
        abstract protected void InitalizeCell(int x, int y);

        /// <summary>
        /// Initialize first values of the table.
        /// </summary>
        public void Initialize()
        {
            for (int x = 0; x < _str1.Length; x++)
                InitalizeCell(x, 0);
            for (int y = 0; y < _str2.Length; y++)
                InitalizeCell(0, y);
        }

        /// <summary>
        /// Translate common chars list to differences ranges list.
        /// </summary>
        /// <param name="str">Some string.</param>
        /// <param name="indices">Indices of chars to take as a range.</param>
        /// <returns>List of ranges.</returns>
        private List<Vector2> IndicesToRanges(string str, List<int> indices)
        {
            indices = new List<int>(Enumerable.Range(0, str.Length)).Where(x => !indices.Contains(x)).ToList();
            List<Vector2> ranges = new List<Vector2>();
            if (indices.Count > 0)
            {
                indices.Sort();
                int start = indices[0];
                int end = start;
                for (int i = 1; i <= indices.Count; i++)
                    if ((i == indices.Count) || (end + 1 < indices[i]))
                    {
                        ranges.Add(new Vector2(start, end));
                        if (i < indices.Count)
                        {
                            start = indices[i];
                            end = start;
                        }
                    }
                    else
                        end = indices[i];
            }

            return ranges;
        }

        /// <summary>
        /// Print string with marking differences.
        /// </summary>
        /// <param name="str">String to print.</param>
        /// <param name="marks">Mark ranges.</param>
        private void PrintDiffrences(string str, List<Vector2> marks)
        {
            int last = 0;
            for (int i = 0; i < marks.Count; i++)
            {
                PrintingMethod(str.Substring(last, (int)marks[i].X - last));
                startMarkFunction();
                PrintingMethod(str.Substring((int)marks[i].X, (int)(1 + marks[i].Y - marks[i].X)));
                stopMarkFunction();
                last = (int)marks[i].Y + 1;
            }
            if (last < str.Length)
                PrintingMethod(str.Substring(last, str.Length - last));
        }

        /// <summary>
        /// Print differences sequences by a given list.
        /// </summary>
        private void PrintSequences(List<Vector2> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                Vector2 range = list[i];
                PrintingMethod(string.Format("{0}{1}", range, Environment.NewLine));
            }
        }

        /// <summary>
        /// Print both string with marking differences.
        /// </summary>
        public void PrintDiffrences()
        {
            PrintDiffrences(_str1, DiffrencesSequences.Str1);
            PrintingMethod(string.Format("{0}{0}", Environment.NewLine));
            PrintDiffrences(_str2, DiffrencesSequences.Str2);
        }

        /// <summary>
        /// Print differences sequences of both strings.
        /// </summary>
        public void PrintSequences()
        {
            PrintSequences(DiffrencesSequences.Str1);
            PrintingMethod(string.Format("{0}{0}",Environment.NewLine));
            PrintSequences(DiffrencesSequences.Str2);
        }

        /// <summary>
        /// Print common char indices in both strings.
        /// </summary>
        public void PrintCommonChars()
        {
            foreach (Vector2 common in CommonChars)
                PrintingMethod(string.Format("{0}{1}",common,Environment.NewLine));
        }

        /// <summary>
        /// Set the mark start\stop function.
        /// </summary>
        /// <param name="start">Start mark function.</param>
        /// <param name="stop">Stop mark function.</param>
        public static void SetMarkFunctions(Action start, Action stop)
        {
            startMarkFunction = start;
            stopMarkFunction = stop;
        }
    }
}
