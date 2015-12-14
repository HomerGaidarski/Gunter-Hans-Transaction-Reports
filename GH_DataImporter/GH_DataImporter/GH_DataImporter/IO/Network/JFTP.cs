using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace JSS.IO.Network
{
    /// <summary>
    /// Holds possible settings for how dropped messages should be handled.
    /// </summary>
    public enum DropMessageModes
    {
        /// <summary>
        /// Writes the messages to the System.Console class
        /// </summary>
        WriteToConsole,

        /// <summary>
        /// Calls the MessageDropped event with the message attached.
        /// </summary>
        CallEvent
    }

    /// <summary>
    /// Provides several functions to allow for downloading from and uploading to FTP servers.
    /// </summary>
    public static class JFTP
    {
        private static NetworkCredential _credentials = new NetworkCredential();
        private static DropMessageModes _dropMessageMode = DropMessageModes.WriteToConsole;
        private static bool _dropMessages = false;

        /// <summary>
        /// Called when a message is released and DropMessageModes is set to 'CallEvent'. DropMessages must also be set to 'True'. Dispatches an event with the message attached.
        /// </summary>
        public static event EventHandler<string> MessageDropped;

        /// <summary>
        /// When set, these credentials will be used when interacting with the web server.
        /// </summary>
        public static NetworkCredential Credentials
        {
            get { return _credentials; }
            set { _credentials = value; }
        }

        /// <summary>
        /// Set this to configure how FTPHelper will release messages. FTPHelper will only release messages if DropMessages is set to True.
        /// </summary>
        public static DropMessageModes DropMessageMode
        {
            get
            {
                return _dropMessageMode;
            }
            set
            {
                _dropMessageMode = value;
            }
        }

        /// <summary>
        /// When set to True, FTPHelper will drop messages to either a Console or Call an Event with the string attached.
        /// It will only call the event if DropMessageMods is set to CallEvent as opposed to WriteToConsole.
        /// </summary>
        public static bool DropMessages 
        {
            get
            {
                return _dropMessages;
            }
            set
            {
                _dropMessages = value;
            }
        }

        /// <summary>
        /// Change a file's address.
        /// </summary>
        /// <param name="oldFileAddress">The full address of the old file name.</param>
        /// <param name="newFileAddress">The full address of the new file name.</param>
        public static void RenameFTPFile(string oldFileAddress, string newFileAddress)
        {
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(oldFileAddress);
            request.Method = WebRequestMethods.Ftp.Rename;
            request.Credentials = Credentials;
            request.RenameTo = newFileAddress;

            request.GetResponse();
        }

        /// <summary>
        /// Creates an FTP Directory.
        /// </summary>
        /// <param name="directoryToAddTo">Folder to add the directory to. Must exist! End slash independant entry.</param>
        /// <param name="newDirectoryName">The folder (or series of folders) to be created within baseDir.
        /// Example: 'dir1/dir2/dir3/'</param>
        public static void CreateFTPDirectory(string directoryToAddTo, string newDirectoryName)
        {
            directoryToAddTo = directoryToAddTo.Trim('/');
            newDirectoryName = newDirectoryName.Trim('/');

            string[] directories = newDirectoryName.Split("/".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            try
            {
                foreach (string folder in directories)
                {
                    //check if folder already exists
                    if (!DirectoryExists(directoryToAddTo + "/" + folder))
                    {
                        //create folder, because it doesn't exist
                        FtpWebRequest request = (FtpWebRequest)WebRequest.Create(directoryToAddTo + "/" + folder);
                        request.Credentials = Credentials;
                        request.Method = WebRequestMethods.Ftp.MakeDirectory;
                        request.GetResponse();
                    }

                    //else, folder already exists - no action needed

                    //add to base directory to continue building
                    directoryToAddTo += "/" + folder;
                }
            }
            catch (Exception ex)
            {
                switch (ex.Message)
                {
                    case "Invalid URI: The format of the URI could not be determined.":
                        //bad URI detected
                        throw new ArgumentException("Invalid URI Inputted.");
                    case "The remote server returned an error: (550) File unavailable (e.g., file not found, no access).":
                        throw new Exception("Either the file could not be found, or you don't have access.");
                    default:
                        throw;
                }
            }
        }

        /// <summary>
        /// Detects whether a given sub directory exists.
        /// </summary>
        /// <param name="fullWebDirectoryAddress">The URL to check</param>
        /// <returns>True if the Directory Exists, False if it doesn't.</returns>
        public static bool DirectoryExists(string fullWebDirectoryAddress)
        {
            fullWebDirectoryAddress = fullWebDirectoryAddress.Trim('/');
            string baseDirectoryAddress = JPath.GetRootAddress(fullWebDirectoryAddress);
            string webFolderExtensionNoFile = JPath.GetWebExtensionAddress(fullWebDirectoryAddress);

            //grab folders from root
            string[] folderList = webFolderExtensionNoFile.Split("/".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            foreach (string folder in folderList)
            {
                //check each folder one by one
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(baseDirectoryAddress);
                request.Credentials = Credentials;
                request.Method = WebRequestMethods.Ftp.ListDirectory;

                using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
                using (StreamReader sr = new StreamReader(response.GetResponseStream()))
                {
                    string[] foldersOnWeb = sr.ReadToEnd().Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

                    //test each folder to make sure
                    bool found = false;

                    foreach (string webFolder in foldersOnWeb)
                    {
                        if (webFolder == folder)
                        {
                            found = true;
                            break;
                        }
                    }

                    if (!found)
                    {
                        //couldn't find current folder in location. Directory does not exist.
                        return false;
                    }
                }

                //found folder in list so still good
                //still need to expand folder to subfolders if necessary however
                baseDirectoryAddress += "/" + folder;
            }

            //made it this far, so all folders must exist in the order given
            //this means that the directory exists!
            return true;
        }

        /// <summary>
        /// Downloads a file from the specified webURL to the specified localURL.
        /// </summary>
        /// <param name="webURL">The URL of the file to download. Includes file name.</param>
        /// <param name="localURL">The URL of the location to put the file. This should end with a directory. NOT a file name!!!</param>
        public static void DownloadFile(string webURL, string localURL)
        {
            DownloadFile(webURL, localURL, "/");
        }

        /// <summary>
        /// Downloads the specified file. Mirrors the folders to the baseLocalURL location.
        /// </summary>
        /// <param name="baseWebURL">The root URL for the FTP server.</param>
        /// <param name="baseLocalURL">The root URL for the download location locally</param>
        /// <param name="localURL">The local URL of the file to download from the FTP server (web style /)</param>
        private static void DownloadFile(string baseWebURL, string baseLocalURL, string localURL)
        {
            using (WebClient client = new WebClient())
            {
                string webURLFull = baseWebURL + localURL;
                string localURLFull = baseLocalURL + localURL.Replace("/", "\\");

                //create directory if it does not exist
                if (Directory.Exists(Path.GetDirectoryName(localURLFull)) == false)
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(localURLFull));
                }
                else
                {
                    //skip if the file already exists in directory

                    //grab the out of the provided URL
                    string fileNameToCheck = Path.GetFileName(localURLFull);

                    foreach (string fileName in Directory.GetFiles(Path.GetDirectoryName(localURLFull)))
                    {
                        if (Path.GetFileName(fileName) == fileNameToCheck)
                        {
                            //file already exists locally, no need to download it so return
                            return;
                        }
                    }
                }

                try
                {
                    client.Credentials = Credentials;
                    client.DownloadFile(new Uri(webURLFull), localURLFull);
                }
                catch (WebException)
                {
                    DropMessage("Couldn't Download: " + webURLFull);
                }
            }
        }

        /// <summary>
        /// Downloads the specified folder from the inputted FTP server to the specied local file.
        /// </summary>
        /// <param name="webURL">The base FTP web directory. Example: "ftp://ftp.myserver.com/baseFolder". Do NOT end with a '/'!</param>
        /// <param name="localURL">The base local URL to mirror download the files to.</param>
        /// <param name="recursive">If set to True, then DownloadFolder() will download recursively.</param>
        public static void DownloadFolder(string webURL, string localURL, bool recursive)
        {
            DownloadFolder(webURL, localURL, "/", recursive, false);
        }

        /// <summary>
        /// Downloads the specified folder (baseWebURL) from a web server to a local folder (baseLocalURL).
        /// webExtDir is merely for recursive purposes. Just insert a "/" for the function call.
        /// </summary>
        /// <param name="baseWebURL">The base FTP web directory. Example: "ftp://ftp.myserver.com/baseFolder". Do NOT end with a '/'!</param>
        /// <param name="baseLocalURL">The base local URL to mirror download the files to.</param>
        /// <param name="webExtDir">The extension directory of  in web style ('/'). Example: "/myLocalDirectory/".
        /// Leave as "/" if there is no additional folders you want to dive</param>
        /// <param name="writeMessages">True / False setting (default False). When set to true, the function will write the currently downloading image in the Console window.</param>
        private static void DownloadFolder(string baseWebURL, string baseLocalURL, string webExtDir, bool recursive, bool writeMessages)
        {
            FtpWebRequest request = (FtpWebRequest)(WebRequest.Create(baseWebURL + webExtDir));
            request.Method = WebRequestMethods.Ftp.ListDirectory;
            request.Credentials = Credentials;
            //request.Credentials = new NetworkCredential("customerfiles@goldcrestdistributing.com", "files2100");

            //////////////////////////
            //Get the root directories

            List<String> directories;

            try
            {
                using (WebResponse response = request.GetResponse())
                using (StreamReader sr = new StreamReader(response.GetResponseStream()))
                {
                    directories = new List<String>(sr.ReadToEnd().Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries));
                    directories.RemoveAll((str) => str.StartsWith("_"));
                }
            }
            catch (WebException)
            {
                //couldn't find the file
                DropMessage("Couldn't find " + request.RequestUri.AbsolutePath + "!");
                directories = new List<string>();
            }

            //////////////////////////////////////
            //Seperate directories into files and folders
            List<String> files = new List<string>(), folders = new List<string>();

            foreach (String item in directories)
            {
                if (item.Length > 2)
                {
                    if (Path.GetExtension(item).Length == 4)
                    {
                        files.Add(item);
                    }
                    else
                    {
                        folders.Add(item);
                    }
                }
            }

            ////////////////////////////////////////////////
            //Download each file individually to the desktop
            foreach (String item in files)
            {
                if (writeMessages == true)
                {
                    DropMessage("Downloading: " + baseWebURL + webExtDir + item);
                }

                DownloadFile(baseWebURL, baseLocalURL, webExtDir + item);
            }

            if (recursive == true)
            {
                //////////////////////////////////////////////////
                //Download each subfolder individually to the desktop
                foreach (String folder in folders)
                {
                    DownloadFolder(baseWebURL, baseLocalURL, webExtDir + folder + "/", true, writeMessages);
                }
            }
        }

        /// <summary>
        /// Downloads the specified folder from the inputted FTP server to the specified local folder.
        /// Uploads the specified local folder to the specified web directory.
        /// </summary>
        /// <param name="webURL">The base FTP web directory. Example: "ftp://ftp.myserver.com/baseFolder". Do NOT end with a '/'!</param>
        /// <param name="localURL">The base local URL to mirror download the files to.</param>
        /// <param name="recursive">If set to True, then DownloadFolder() will download recursively.</param>
        /// <param name="writeMessages">If set to True, then DownloadFolder() will drop messages.</param>
        public static void DownloadFolder(string webURL, string localURL, bool recursive, bool writeMessages)
        {
            DownloadFolder(webURL, localURL, "/", recursive, writeMessages);
        }

        /// <summary>
        /// Call this to message what is going on with all of the Web transactions.
        /// </summary>
        /// <param name="message"></param>
        private static void DropMessage(string message)
        {
            if (DropMessages == true)
            {
                if (DropMessageMode == DropMessageModes.WriteToConsole)
                {
                    Console.WriteLine(message);
                }
                else if (DropMessageMode == DropMessageModes.CallEvent)
                {
                    //TODO: Call event here...
                    MessageDropped(null, message);
                }
            }
        }

        /// <summary>
        /// Uploads the specified local file to the specified FTP location in multiple chunks. Overwrites conflicts.
        /// </summary>
        /// <param name="localPath">Local path to file including the file name.</param>
        /// <param name="ftpPath">FTP path including the file name.</param>
        /// <param name="maxChunkSize">The threshold size of each chunk [in bytes].</param>
        public static void UploadFileInChunks(string localPath, string ftpPath, int maxChunkSize)
        {
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(ftpPath);
            request.UseBinary = true;
            request.KeepAlive = true;
            request.Credentials = Credentials;

            //Delete File not requested
            request.Method = WebRequestMethods.Ftp.UploadFile;

            // Copy the contents of the file to the request stream.
            FileStream sourceStream = File.OpenRead(localPath);
            byte[] buffer = new byte[sourceStream.Length];
            sourceStream.Read(buffer, 0, buffer.Length);
            sourceStream.Close();

            //will need to transfer the file in multiple chunks because it is larger than the MAX_CHUNK_SIZE
            int numStreams = 1;

            if (buffer.Length > maxChunkSize)
            {
                while (numStreams * maxChunkSize < buffer.Length)
                {
                    numStreams += 1;
                }
            }

            if (numStreams > 1)
            {
                var numUploadedStreams = 0;

                //write first
                request.ContentLength = maxChunkSize;

                Stream requestStream = request.GetRequestStream();
                requestStream.Write(buffer, 0, maxChunkSize);
                requestStream.Close();
                request.GetResponse();

                numUploadedStreams++;


                while (numUploadedStreams < numStreams)
                {
                    //generate new request
                    request = (FtpWebRequest)WebRequest.Create(ftpPath);
                    request.Credentials = Credentials;
                    request.UseBinary = true;
                    request.KeepAlive = true;

                    //first chunk uploading, so overwrite
                    request.Method = WebRequestMethods.Ftp.AppendFile;

                    if (numUploadedStreams == numStreams - 1)
                    {
                        request.ContentLength = buffer.Length - maxChunkSize * numUploadedStreams;
                        requestStream = request.GetRequestStream();

                        //on last stream so don't upload MAX_CHUNK_SIZE. Calculate leftover bytes and write those.
                        requestStream.Write(buffer, maxChunkSize * numUploadedStreams, (int)request.ContentLength);
                    }
                    else
                    {
                        requestStream = request.GetRequestStream();
                        requestStream.Write(buffer, maxChunkSize * numUploadedStreams, maxChunkSize);
                    }

                    requestStream.Close();
                    request.GetResponse();
                    numUploadedStreams++;
                }
            }
            else
            {
                //numStreams = 1
                request.ContentLength = buffer.Length;

                Stream requestStream = request.GetRequestStream();
                requestStream.Write(buffer, 0, buffer.Length);
                requestStream.Close();

                var response = (FtpWebResponse)request.GetResponse();

                response.Close();
            }
        }

        /// <summary>
        /// Uploads the specified local folder to the specified web directory.
        /// </summary>
        /// <param name="localFolderURL">This is the file to copy to the FTP server.</param>
        /// <param name="webFolderURL">This is the root FTP directory to copy to.</param>
        /// <param name="recursive">Whether or not to open subfolders and copy them too.</param>
        public static void UploadFolder(string localFolderURL, string webFolderURL, bool recursive)
        {
            UploadFolder(localFolderURL, webFolderURL, recursive, false);
        }

        /// <summary>
        /// Uploads the specified local folder to the specified web directory.
        /// </summary>
        /// <param name="localFolderURL">This is the file to copy to the FTP server.</param>
        /// <param name="webFolderURL">This is the root FTP directory to copy to.</param>
        /// <param name="recursive">Whether or not to open subfolders and copy them too.</param>
        /// <param name="writeMessages">Default is 'False'.</param>
        public static void UploadFolder(string localFolderURL, string webFolderURL, bool recursive, bool writeMessages)
        {
            //ensure argument format
            webFolderURL = webFolderURL.TrimEnd('/');

            string[] fileNames; //holds the files to upload
            List<string> failedFiles = new List<string>(); //holds all files that throw exception when the upload was attempted

            if (recursive == true)
            {
                fileNames = Directory.GetFiles(localFolderURL, "*", SearchOption.AllDirectories);
            }
            else
            {
                fileNames = Directory.GetFiles(localFolderURL);
            }

            //for each file
            int fileNamesLength = fileNames.Length;

            for (int i = 0; i < fileNamesLength; i++)
            {
                string file = fileNames[i];

                //find local address for web server
                string localWebAddressWithFile = file.Remove(0, localFolderURL.Length).Replace("\\", "/").Trim('/');
                string localWebAddressWithoutFile = Path.GetDirectoryName(localWebAddressWithFile).Replace("\\", "/");

                if (i > 0)
                {
                    string lastLocalWebAddressWithoutFile = JPath.GetLastFolderName(Directory.GetParent(fileNames[i - 1]).ToString());

                    if (localWebAddressWithoutFile != lastLocalWebAddressWithoutFile)
                    {
                        //there is a new folder to be inputted into, so you need to create a new directory.
                        CreateFTPDirectory(webFolderURL, localWebAddressWithoutFile);
                    }

                    //else, no action required
                }
                else
                {
                    //first file accessed, so create root first
                    string rootDir = JPath.GetRootAddress(webFolderURL);
                    string extDir = JPath.GetWebExtensionAddress(webFolderURL);
                    CreateFTPDirectory(rootDir, extDir);

                    //now create the extended directory
                    CreateFTPDirectory(webFolderURL, localWebAddressWithoutFile);
                }

                //upload file
                using (WebClient client = new WebClient())
                {
                    client.Credentials = Credentials;

                    string fileName = Path.GetFileName(file);

                    if (writeMessages == true)
                    {
                        DropMessage("Next Upload to: " + webFolderURL + "/" + localWebAddressWithoutFile + "/" + fileName);
                    }

                    try
                    {
                        //client.UploadFile(webFolderURL + "/" + localWebAddressWithoutFile + "/" + fileName, file);
                        UploadFileInChunks(file, webFolderURL + "/" + localWebAddressWithFile, 800000);
                    }
                    catch (WebException)
                    {
                        //upload failed, add to the list of failed uploads and continue
                        failedFiles.Add(file);
                    }
                }

                if (writeMessages == true)
                {
                    double percent = Math.Round(((double)i / fileNamesLength) * 100, 4);
                    Console.Clear();
                    DropMessage("Current Progress: " + percent.ToString("F2") + "%");
                    DropMessage("Number completed: " + i.ToString() + "/" + fileNamesLength.ToString());
                }
            }

            //Write all the files that failed to load
            if (writeMessages == true)
            {
                if (failedFiles.Count > 0)
                {
                    DropMessage("These files failed to upload: ");

                    foreach (string file in failedFiles)
                    {
                        DropMessage(file);
                    }
                }
                else
                {
                    DropMessage("All files uploaded successfully!");
                }
            }
        }
    }
}