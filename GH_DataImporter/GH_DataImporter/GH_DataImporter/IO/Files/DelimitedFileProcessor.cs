using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSS.IO.Files
{
    /// <summary>
    /// Extends the FileProcessor class to add the capacity to quickly edit delimited files.
    /// </summary>
    public class DelimitedFileProcessor : FileProcessor
    {
        private List<string[]> _rows = new List<string[]>();

        private bool _hasHeaderRow = false;

        /// <summary>
        /// True if this DelimitedFileProcessor contains a header row.
        /// </summary>
        public bool HasHeaderRow
        {
            get
            {
                return _hasHeaderRow;
            }
        }

        /// <summary>
        /// True if this DelimitedFileProcessor has a file loaded in.
        /// </summary>
        public bool FileIsCurrentlyInMemory
        {
            get
            {
                return _rows.Count > 0;
            }
        }

        /// <summary>
        /// The delimiter used to split the lines of the file.
        /// </summary>
        private char _delimiter;

        /// <summary>
        /// Gets the string value associated with the provided 0-based indexed row and column.
        /// </summary>
        public string this[int row, int column]
        {
            get
            {
                return _rows[row][column];
            }
        }

        /// <summary>
        /// Gets the string value associated with the provided 0-based indexed row and column name string.
        /// Can only use if the file has a header row as defined in LoadInDelimitedFile().
        /// <exception cref="InvalidOperationException" />
        /// </summary>
        public string this[int row, string columnName]
        {
            get
            {
                return _rows[row][this.ConvertColumnNameToIndex(columnName)];
            }
        }

        /// <summary>
        /// Takes a given columnName and converts it to the proper column index for any given row.
        /// </summary>
        /// <param name="columnName">The name of the column to retrieve the index for.</param>
        /// <returns>An integer that represents a zero based index for that given columnName.</returns>
        /// <exception cref="InvalidOperationException" />
        private int ConvertColumnNameToIndex(string columnName)
        {
            if (this._hasHeaderRow == false)
            {
                throw new InvalidOperationException("There is not a header row for this delimited file!");
            }

            for (int i = 0; i < _rows[0].Length; i++)
            {
                if (this._rows[0][i] == columnName)
                {
                    return i;
                }
            }

            //No column with the inputted columnName so throw an error.
            throw new InvalidOperationException("There is no column with that name!");
        }

        /// <summary>
        /// Takes a the given information to load in a file. Unloads the previous file.
        /// </summary>
        /// <param name="fileName">The full file path of the delimited text file you wish to load.</param>
        /// <param name="delimeter">The delimiter that separates fields in the text file.</param>
        /// <param name="hasHeaderRow">True if the first row is a header row.</param>
        public void LoadInDelimitedFile(string fileName, char delimeter, bool hasHeaderRow)
        {
            UnloadFile();

            _hasHeaderRow = hasHeaderRow;
            _delimiter = delimeter;

            //Take each line and split them up!
            ProcessFile(fileName, this.ProcessLine);
        }

        /// <summary>
        /// Outputs the given file name to the desktop.
        /// </summary>
        /// <param name="fileName">The full path and name of the file to output to.</param>
        /// <param name="delimiter">The delimeter to use to separate the fields.</param>
        public void OutputDataToDelimitedFile(string fileName, char delimiter)
        {
            //Create single lines
            List<string> lines = new List<string>();

            foreach (string[] row in _rows)
            {
                StringBuilder sBuilder = new StringBuilder();

                foreach (string field in row)
                {
                    sBuilder.AppendFormat("{0}{1}", field, delimiter);
                }

                //remove last delimiter character
                sBuilder.Remove(sBuilder.Length - 1, 1);

                //append
                lines.Add(sBuilder.ToString());
            }

            //Save single lines to a file
            OutputDataToFile(fileName, lines);
        }

        /// <summary>
        /// Outputs the given file name to the desktop.
        /// </summary>
        /// <param name="fileName">The full path and name of the file to output to.</param>
        /// <param name="delimiter">The delimeter to use to separate the fields.</param>
        /// <param name="fieldEncloser">The character used to enclose individual fields.</param>
        public void OutputDataToDelimitedFile(string fileName, char delimiter, char fieldEncloser)
        {
            //Create single lines
            List<string> lines = new List<string>();

            foreach (string[] row in _rows)
            {
                StringBuilder sBuilder = new StringBuilder();

                foreach (string field in row)
                {
                    sBuilder.AppendFormat("{0}{1}{2}{3}", fieldEncloser, field, fieldEncloser, delimiter);
                }

                //remove last delimiter character
                sBuilder.Remove(sBuilder.Length - 1, 1);

                //append
                lines.Add(sBuilder.ToString());
            }

            //Save single lines to a file
            OutputDataToFile(fileName, lines);
        }

        /// <summary>
        /// Unloads all loaded in rows.
        /// </summary>
        public void UnloadFile()
        {
            this._rows.Clear();
        }

        /// <summary>
        /// Processes a column and edits the information in a column with the given columnName based on the return value of the columnProcessingFunction.
        /// </summary>
        /// <param name="columnName">The header name of the column to process.</param>
        /// <param name="columnProcessingFunction">The function to process the column fields with.</param>
        public void ProcessColumn(string columnName, Func<string, string> columnProcessingFunction)
        {
            ProcessColumn(ConvertColumnNameToIndex(columnName), columnProcessingFunction);
        }

        /// <summary>
        /// Processes a column and edits the information in a zero-based index column based on the return value of the columnProcessingFunction.
        /// </summary>
        /// <param name="columnIndex">The zero-based index of the column to process through.</param>
        /// <param name="columnProcessingFunction">The function to process the file with. It takes a string (which is the starting field value) and returns a string.</param>
        public void ProcessColumn(int columnIndex, Func<string, string> columnProcessingFunction)
        {
            if (this.FileIsCurrentlyInMemory == false)
            {
                throw new InvalidOperationException("Must have a file loaded into memory before a column can be processed!");
            }

            //Figure out starting row
            //If the current file has a header row, skip it. Else, count the first row...
            int r = this.HasHeaderRow ? 1 : 0;

            for ( ; r < _rows.Count; r++)
            {
                _rows[r][columnIndex] = columnProcessingFunction(_rows[r][columnIndex]);
            }
        }

        private void ProcessLine(string line)
        {
            this._rows.Add(line.Split(_delimiter));
        }
    }
}
