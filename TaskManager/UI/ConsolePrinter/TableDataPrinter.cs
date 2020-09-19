using System;
using System.Collections.Generic;
using System.Text;

namespace UI.ConsolePrinter
{
    internal class TableDataStringBuilder
    {
        private int[] mColumnsLengths;
        private readonly List<string> mDataToPrintCollector = new List<string>();
        public int SpacesBetweenColumns { get; set; } = 3;
        public int MinimalColumnLength { get; set; } = 3;

        public TableDataStringBuilder(string[] headers)
        {
            InitializeColumnsLength(headers.Length);
            InnerAppend(headers);
        }

        private void InitializeColumnsLength(int size)
        {
            mColumnsLengths = new int[size];
            for(int i = 0; i < mColumnsLengths.Length; ++i)
            {
                mColumnsLengths[i] = MinimalColumnLength;
            }
        }

        public void AppandRow(params string[] rowData)
        {
            if(rowData.Length != mColumnsLengths.Length)
            {
                Console.WriteLine(
                    $"Row columns number {rowData.Length} is not the same as headers columns number {mColumnsLengths.Length}");
                return;
            }

            InnerAppend(rowData);
        }

        private void InnerAppend(IEnumerable<string> rowData)
        {
            int index = 0;
            foreach (string data in rowData)
            {
                if(data != null)
                {
                    mDataToPrintCollector.Add(data);
                    if (mColumnsLengths[index] < data.Length)
                        mColumnsLengths[index] = data.Length;
                }
                else
                {
                    mDataToPrintCollector.Add(string.Empty);
                }

                index++;
            }
        }

        public string Build()
        {
            int rowIndex = 0;
            int columnIndex = 0;
            StringBuilder stringBuilder = new StringBuilder();

            foreach(string data in mDataToPrintCollector)
            {
                if(columnIndex == mColumnsLengths.Length)
                {
                    columnIndex = 0;
                    rowIndex++;
                    stringBuilder.AppendLine();
                }

                AppendColumnToGivenStringBuilder(stringBuilder, data, mColumnsLengths[columnIndex]);
                stringBuilder.Append(' ', SpacesBetweenColumns);
                columnIndex++;
            }

            return stringBuilder.ToString();
        }

        private void AppendColumnToGivenStringBuilder(StringBuilder stringBuilder, string data, int maximalColumnSize)
        {
            stringBuilder.Append(data);
            stringBuilder.Append(' ', maximalColumnSize - data.Length);
        }
    }
}