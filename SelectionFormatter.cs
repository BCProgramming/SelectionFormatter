using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace SelectionFormatter_Test
{
    public class SelectionFormatter
    {
        public delegate FT Succ<FT>(FT Item);

        public static String FormatSelection(String pSeparator, IList<int> pSelectedItems, Func<int, int, String> pRangeFormatter = null)
        {
            return String.Join(pSeparator, GetSelectionRanges(pSelectedItems, i => (int)(i + (int)1), pRangeFormatter));
        }
        public static IEnumerable<SelectionRange<int>> GetSelectionRanges(IList<int> pSelectedItems, Func<int, int, String> pRangeFormatter)
        {
            return GetSelectionRanges(pSelectedItems, i => (int)(i + (int)1), pRangeFormatter);
        }

        public static String FormatSelection(String pSeparator, IList<long> pSelectedItems, Func<long, long, String> pRangeFormatter = null)
        {
            return String.Join(pSeparator, GetSelectionRanges(pSelectedItems, i => (long)(i + (long)1), pRangeFormatter));
        }
        public static IEnumerable<SelectionRange<long>> GetSelectionRanges(IList<long> pSelectedItems, Func<long, long, String> pRangeFormatter)
        {
            return GetSelectionRanges(pSelectedItems, i => (long)(i + (long)1), pRangeFormatter);
        }

        public static String FormatSelection(String pSeparator,IList<char> pSelectedItems, Func<char, char, String> pRangeFormatter=null)
        {
            return String.Join(pSeparator,GetSelectionRanges(pSelectedItems, i => (char) (i + (char) 1),pRangeFormatter));
        }
        public static IEnumerable<SelectionRange<char>>  GetSelectionRanges(IList<char> pSelectedItems,Func<char,char,String> pRangeFormatter)
        {
            return GetSelectionRanges(pSelectedItems, i => (char)(i + (char)1), pRangeFormatter);
        }
        public static String FormatSelection<T>(String pSeparator,IList<T> pSelectedItems,Succ<T> Successor,Func<T,T,String> pRangeFormatter = null)
        {
            return String.Join(",", GetSelectionRanges(pSelectedItems, Successor, pRangeFormatter));
        }
        public static IEnumerable<SelectionRange<T>> GetSelectionRanges<T>(IList<T> pSelectedItems, Succ<T> Successor, Func<T, T, String> pRangeFormatter = null)
        {
            List<T> SelectedItems = (from sel in pSelectedItems orderby sel select sel).ToList();
            List<SelectionRange<T>> ChosenRanges = new List<SelectionRange<T>>();

            for (int i = 0; i < SelectedItems.Count; i++)
            {
                T StartItem = SelectedItems[i];
                T CurrentItem = StartItem;
                int offset = 0;

                while ((i + offset < (SelectedItems.Count - 1)) && (Successor(CurrentItem).Equals(SelectedItems[i + offset])))
                {
                    CurrentItem = Successor(CurrentItem);
                    offset++;
                }

                if (offset > 0) offset--;

                T EndItem = SelectedItems[i + offset];
                if (offset == 1)
                {
                    ChosenRanges.Add(new SelectionRange<T>(StartItem, StartItem));
                    ChosenRanges.Add(new SelectionRange<T>(EndItem, EndItem));
                }
                else
                {
                    SelectionRange<T> BuildRange = new SelectionRange<T>(StartItem, EndItem,pRangeFormatter);
                    ChosenRanges.Add(BuildRange);
                }

                i += offset;
            }
            return ChosenRanges;
        }
        public static String FormatSelection<T>(String pSeparator,IList<T> pSelectedItems,IList<T> pFullListing,Func<T,T,String> pRangeFormatter=null)
        {
            return String.Join(pSeparator, GetSelectionRanges<T>(pSelectedItems, pFullListing, pRangeFormatter));
        }
        /// <summary>
        ///     Given a list of selected entries, and a full listing of those entries, retrieves a series of selection ranges representing the specified selection within that full listing.
        /// </summary>
        /// <param name="SelectedItems">Items that are selected</param>
        /// <param name="FullListing">Full list of selectable items.</param>
        /// <returns></returns>
        public static IEnumerable<SelectionRange<T>> GetSelectionRanges<T>(IList<T> pSelectedItems, IList<T> pFullListing, Func<T, T, String> pRangeFormatter = null)
        {
            //make copies of both listings, sorting them.
            List<T> SelectedItems = (from sel in pSelectedItems orderby sel select sel).ToList();
            List<T> FullListing = (from sel in pFullListing orderby sel select sel).ToList();
            //Validation: Make sure SelectedItems is a subset of FullListing.
            foreach (var verifySelection in SelectedItems)
            {
                if (!FullListing.Contains(verifySelection)) throw new ArgumentException("Selection List contains entries not present in full listing.", nameof(SelectedItems));
            }

            //iterate through every element in SelectedItems.
            List<SelectionRange<T>> ChosenRanges = new List<SelectionRange<T>>();

            for (int i = 0; i < SelectedItems.Count; i++)
            {
                T StartItem = SelectedItems[i];
                //find the index of this item in the full listing.
                int StartFullIndex = FullListing.IndexOf(StartItem);
                int offset = 0;
                while (i + offset < (SelectedItems.Count - 1) && SelectedItems[i + offset].Equals(FullListing[StartFullIndex + offset]))
                {
                    offset++;
                }
                if (offset > 0) offset--;

                T EndItem = SelectedItems[i + offset];
                if (offset == 1)
                {
                    ChosenRanges.Add(new SelectionRange<T>(StartItem, StartItem, pRangeFormatter));
                    ChosenRanges.Add(new SelectionRange<T>(EndItem, EndItem, pRangeFormatter));
                }
                else
                {
                    SelectionRange<T> BuildRange = new SelectionRange<T>(StartItem, EndItem);
                    ChosenRanges.Add(BuildRange);
                }

                i += offset;
            }
            return ChosenRanges;
        }
    }

    public class SelectionRange<T>
    {
        private delegate String FormatRangeFunc(T StartRange, T EndRange);
        private FormatRangeFunc RangeFormatter = DefaultRangeFormat;
        private static String DefaultRangeFormat(T StartRange,T EndRange)
        {
            if (StartRange.Equals(EndRange)) return StartRange.ToString();
            return StartRange + "-" + EndRange;
        }
        public SelectionRange(T pStartRange, T pEndRange,Func<T,T, String> pRangeFormatter)
        {
            StartRange = pStartRange;
            EndRange = pEndRange;
            if (pRangeFormatter != null)
                RangeFormatter = (range, endRange) => pRangeFormatter(range, endRange);
            else
                RangeFormatter = DefaultRangeFormat;
        }
        public SelectionRange(T pStartRange,T pEndRange):this(pStartRange,pEndRange,DefaultRangeFormat)
        {
            
        }
        public T StartRange { get; private set; }
        public T EndRange { get; private set; }

        public override string ToString()
        {
            return RangeFormatter(StartRange, EndRange);
        }
    }
}