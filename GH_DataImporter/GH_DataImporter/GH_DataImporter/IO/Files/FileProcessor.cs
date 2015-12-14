using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace JSS.IO.Files
{
    /// <summary>
    /// Provides a quick and easy framework for importing, processing, and outputting a simple text file.
    /// </summary>
    public class FileProcessor
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public FileProcessor()
        {
            
        }

        /// <summary>
        /// Outputs the given data to a given file.
        /// </summary>
        /// <param name="outputFileName">The name of the file to output to.</param>
        /// <param name="linesToOutput">The lines to output to the file.</param>
        public void OutputDataToFile(string outputFileName, IEnumerable<string> linesToOutput)
        {
            //Load in file processing resources
            using (FileStream fileStream = new FileStream(outputFileName, FileMode.Create))
            using (StreamWriter fileWriter = new StreamWriter(fileStream))
            {
                foreach (string line in linesToOutput)
                {
                    fileWriter.WriteLine(line);
                }
            }
        }

        /// <summary>
        /// Outputs the given data to the Console.
        /// </summary>
        /// <param name="linesToOutput">The lines to output to the file.</param>
        public void OutputDataToConsole(IEnumerable<string> linesToOutput)
        {
            foreach (string line in linesToOutput)
            {
                Console.WriteLine(line);
            }
        }

        /// <summary>
        /// Loads a file and calls the lineProcessingFunction for each line.
        /// </summary>
        /// <param name="fileName">The name of the file to process.</param>
        /// <param name="lineProcessingFunction">The function to process the lines.</param>
        public void ProcessFile(string fileName, Action<string> lineProcessingFunction)
        {
            ProcessFile(fileName, lineProcessingFunction, 1);
        }

        /// <summary>
        /// Loads a file and calls the lineProcessingFunction for each line.
        /// </summary>
        /// <param name="fileName">The name of the file to process.</param>
        /// <param name="lineProcessingFunction">The function to process the lines.</param>
        /// <param name="startingRowNumber">The row number that this should start processing the file on. This is assuming 1 based row numbers.</param>
        public void ProcessFile(string fileName, Action<string> lineProcessingFunction, int startingRowNumber)
        {
            //Load in file processing resources
            using (FileStream fileStream = new FileStream(fileName, FileMode.Open))
            using (StreamReader fileReader = new StreamReader(fileStream))
            {
                for (int i = 1; i < startingRowNumber; i++)
                {
                    if (fileReader.EndOfStream == false)
                    {
                        //take another row...
                        fileReader.ReadLine();
                    }
                }

                while (fileReader.EndOfStream == false)
                {
                    //keep loading in lines
                    lineProcessingFunction(fileReader.ReadLine());
                }
            }
        }
    }
}
