using CodingLibrary.JSS.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSS.IO
{
    public class JDirectories
    {
        /// <summary>
        /// Takes the given directory, moves the files and subdirectories up one, and deletes the given directory.
        /// </summary>
        /// <param name="directory">The directory to move from and delete.</param>
        /// <exception cref="ArgumentException">Throws this when the directory is invalid.</exception>
        public static void MoveFolderUpAndReplace(string directory)
        {
            if (!Directory.Exists(directory))
            {
                throw new ArgumentException("Supplied directory doesn't exist!");
            }

            string[] folders = Directory.GetDirectories(directory, "*", SearchOption.TopDirectoryOnly);
            string[] files = Directory.GetFiles(directory, "*", SearchOption.TopDirectoryOnly);
            string[] combined = new string[folders.Length + files.Length];

            foreach (string file in files)
            {
                //move up one directory
                string newDirectory = Directory.GetParent(Directory.GetParent(file).ToString()).ToString();
                string newName = newDirectory + "\\" + Path.GetFileName(file);

                //delete the new file name if it already exists
                if (File.Exists(newName))
	            {
                    File.Delete(newName);
	            }

                Directory.Move(file, newName);
            }

            foreach (string folder in folders)
            {
                //move up one directory
                string newDirectory = Directory.GetParent(Directory.GetParent(folder).ToString()).ToString();
                string newMoveURL = newDirectory + "\\" + JPath.GetLastFolderName(folder);

                if (Directory.Exists(newMoveURL))
                {
                    Directory.Delete(newMoveURL, true);
                }

                Directory.Move(folder, newMoveURL);
            }

            Directory.Delete(directory);
        }
    }
}
