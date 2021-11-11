using System;
using System.Collections;
using System.Collections.Generic;

namespace StringsComparator
{
    public class DiagonalIterator : IEnumerable<Tuple<int, int>>
    {
        // Members
        private int _max1, _max2;

        // Consturctors
        public DiagonalIterator(int max1, int max2)
        {
            _max1 = max1-1;
            _max2 = max2-1;
        }

        // Methods
        /// <summary>
        /// Run through all indices in [max1,max2] in diagonal way.
        /// </summary>
        /// <returns>Tuple of indices.</returns>
        public IEnumerator<Tuple<int, int>> GetEnumerator()
        {
            Func<int,int, int> y = (x,s) => s - x;
            for (int sum = 0; sum <= _max1 + _max2; sum++)
                for (int x = Math.Max(0, sum - _max2); y(x, sum) >= 0 && x <= _max1; x++)
                    yield return new Tuple<int, int>(x, y(x, sum));
        }

        /// <summary>
        /// Get diagonal enumerator
        /// </summary>
        /// <returns>Diagonal enumerator.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
