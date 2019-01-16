using System;
using System.Data;

namespace WebCopy.Models
{
    public class FileListingStruc
    {
        public DataSet FileListingDs()
        {

            DataSet ds = new DataSet();
            DataTable dt = ds.Tables.Add();

            dt.Columns.Add("FileName", typeof(string));
            dt.Columns.Add("FileSize", typeof(long));
            dt.Columns.Add("FileStatus", typeof(string));
            dt.Columns.Add("FilesStatusDate", typeof(DateTime));
            dt.Columns.Add("FileHasError", typeof(bool));

            return ds;
        }

    }
}