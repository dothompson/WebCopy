using System;

namespace WebCopy.Models
{
    public class FileToCopy
    {
        public FileToCopy()
        {

        }

        /// <summary>
        /// name of file to download
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Size, so client knows if the download was successful
        /// </summary>
        public long FileSize { get; set; }

        /// <summary>
        ///  status, to help manage multi-download clients
        /// </summary>
        public string FileStatus { get; set; }

        /// <summary>
        /// Date/Time to evaluate stalled or failed download of files
        /// </summary>
        public DateTime FilesStatusDate { get; set; }

        /// <summary>
        /// If there is an error, flag to true then put error in FileStatus
        /// </summary>
        public bool FileHasError { get; set; }

    }
}
