using System;
using System.IO;
using System.Collections.Generic;

namespace JSS.IO
{
    public static class JPath
    {
        public static string LocalToWebURI(string uri)
        {
            return uri.Replace("\\", "/");
        }

        public static string WebToLocalURI(string uri)
        {
            return uri.Replace("/", "\\");
        }

        /// <summary>
        /// Retrieves the root of a web address. Example: input - 'ftp://ftp.somesite.com/someFolder/', returns - 'ftp://ftp.somesite.com'
        /// </summary>
        /// <param name="uri">The uri to find the root of.</param>
        /// <returns>Returns an empty string if the uri is incorrect. The base uri if it is correct.</returns>
        public static string GetRootAddress(string uri)
        {
            char[] charArray = uri.ToCharArray();
            int length = charArray.Length;

            //look for the third slash
            int numSlashes = 0;

            for (int i = 0; i < length; i++)
			{
                if (charArray[i] == '/')
                {
                    numSlashes++;

                    if (numSlashes == 3)
                    {
                        //found it!
                        return uri.Substring(0, i);
                    }
                }
			}

            //couldn't find it.
            return "";
        }

        /// <summary>
        /// Retrieves the extension string of an inputted web uri. Example: input - 'ftp://ftp.somesite.com/someFolder/subFolder', output - 'someFolder/subFolder'
        /// </summary>
        /// <param name="uri">The uri to find the root of.</param>
        /// <returns>Returns an empty string if the uri is incorrect. The base uri if it is correct.</returns>
        public static string GetWebExtensionAddress(string uri)
        {
            char[] charArray = uri.ToCharArray();
            int length = charArray.Length;

            //look for the third slash
            int numSlashes = 0;

            for (int i = 0; i < length; i++)
            {
                if (charArray[i] == '/')
                {
                    numSlashes++;

                    if (numSlashes == 3)
                    {
                        //found it!
                        return uri.Substring(i + 1, uri.Length - (i + 1)).Trim('/');
                    }
                }
            }

            //couldn't find it.
            return "";
        }

        /// <summary>
        /// Returns the last folder name for a given directory. Example: 'C:\\Batch\\Hello' returns 'Hello'
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public static string GetLastFolderName(string uri)
        {
            uri = uri.Trim('\\');
            string[] split = uri.Split("\\".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            return split[split.Length - 1];
        }
    }
}
