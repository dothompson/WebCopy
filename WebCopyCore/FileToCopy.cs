using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace WebCopyService
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
