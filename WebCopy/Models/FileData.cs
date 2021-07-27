using NLog;
using System;
using System.Data;
using System.IO;
using System.Web;

namespace WebCopy.Models
{
    public class FileData
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        public FileToCopy GetNextFile()
        {
            //get a fresh list of the directory
            RefreshDirectoryListing();
            //setup return object
            FileToCopy objFileToCopy = new FileToCopy();
            //get the list of files
            //load tracking file
            DataSet dsWorkingList = new DataSet();
            dsWorkingList.ReadXml(HttpContext.Current.Server.MapPath("~/Data/wl.xml"));

            foreach (DataRow data2 in dsWorkingList.Tables[0].Rows)
            {
                if (data2["FileStatus"].ToString() == "N")
                {
                    objFileToCopy.FileName = data2["FileName"].ToString();
                    objFileToCopy.FileSize = Convert.ToInt64(data2["FileSize"]);
                    objFileToCopy.FileStatus = data2["FileStatus"].ToString();
                    objFileToCopy.FilesStatusDate = Convert.ToDateTime(data2["FilesStatusDate"]);
                    objFileToCopy.FileHasError = false;
                    //get out of foreach loop
                    return objFileToCopy;
                }
            }

            //if we have not exited already, then there is no new file to copy
            objFileToCopy.FileName = "None";
            objFileToCopy.FileSize = 0;
            objFileToCopy.FileStatus = "C";
            objFileToCopy.FilesStatusDate = DateTime.Now;
            objFileToCopy.FileHasError = true;
            return objFileToCopy;
        }

        public void RefreshDirectoryListing()
        {
            FileListingStruc objFileList = new FileListingStruc();
            //setup DataSet for 
            DataSet dsDirList = new DataSet();
            DataTable dtDirLilst = dsDirList.Tables.Add();

            dtDirLilst.Columns.Add("FileName", typeof(string));
            dtDirLilst.Columns.Add("FileSize", typeof(long));
            dtDirLilst.Columns.Add("FileStatus", typeof(string));
            dtDirLilst.Columns.Add("FilesStatusDate", typeof(DateTime));
            dtDirLilst.Columns.Add("FileHasError", typeof(bool));

            //status indicator
            //"N" New, not downloaded
            //"D" Downloading, should be downloaded
            //"C" Complete, move to complete folder
            //"E" Error

            //get a list of the directory
            //System.IO.DriveInfo di = new System.IO.DriveInfo(HttpContext.Current.Server.MapPath("~/ToDownload/"));
            DirectoryInfo dirInfo = new DirectoryInfo(HttpContext.Current.Server.MapPath("~/ToDownload/"));
            FileInfo[] fileNames = dirInfo.GetFiles("*.*");
            //string[] filePaths = Directory.GetFiles(HttpContext.Current.Server.MapPath("~/ToDownload/"), "*.*",SearchOption.AllDirectories);
            foreach (System.IO.FileInfo fi in fileNames)
            {
                dsDirList.Tables[0].Rows.Add(fi.Name, fi.Length, "N", DateTime.Now, false);
                //Console.WriteLine("{0}: {1}: {2}", fi.Name, fi.Length, fi.LastAccessTime);
            }

            //if there is no working list file, then skip the compare and save a new working list
            if (File.Exists(HttpContext.Current.Server.MapPath("~/Data/wl.xml")))
            {
                //load tracking file
                DataSet dsWorkingList = new DataSet();
                dsWorkingList.ReadXml(HttpContext.Current.Server.MapPath("~/Data/wl.xml"));
                try
                {
                    //merge the data
                    foreach (DataRow data1 in dsDirList.Tables[0].Rows)
                    {
                        foreach (DataRow data2 in dsWorkingList.Tables[0].Rows)
                        {
                            if (!(data1["FileName"] == data2["FileName"]))
                            {
                                dsWorkingList.Tables[0].Rows.Add(data1.Field<string>("FileName"), data1.Field<long>("FileSize"), "N", data1.Field<DateTime>("FilesStatusDate"), false);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    logger.Error(e);
                }


                //save the tracking file
                //dsWorkingList.WriteXml(HttpContext.Current.Server.MapPath("./Data/wl.xml"));
                dsWorkingList.WriteXml(HttpContext.Current.Server.MapPath("~/Data/wl.xml"));

            }
            else
            {
                dsDirList.WriteXml(HttpContext.Current.Server.MapPath("~/Data/wl.xml"));
            }
        }

        public bool ReportComplete(FileToCopy InFileCopied)
        {
            //load tracking file
            DataSet dsWorkingList = new DataSet();
            dsWorkingList.ReadXml(HttpContext.Current.Server.MapPath("~/Data/wl.xml"));

            //go through the workling list and find the file being reported
            foreach (DataRow data2 in dsWorkingList.Tables[0].Rows)
            {
                //match on name
                if (data2["FileName"].ToString() == InFileCopied.FileName)
                {
                    //verify size to validate complete download
                    if (Convert.ToInt64(data2["FileSize"]) == InFileCopied.FileSize)
                    {
                        //size matches
                        //mark as complete
                        data2["FileStatus"] = "C";

                    }
                    //status indicator
                    //"N" New, not downloaded
                    //"D" Downloading, should be downloaded
                    //"C" Complete, move to complete folder
                    //"E" Error

                }
                else
                {
                    data2["FileStatus"] = "E";
                    data2["FileHasError"] = true;
                }
            }

            //move the file


            //update the working file
            dsWorkingList.WriteXml(HttpContext.Current.Server.MapPath("~/Data/wl.xml"));


            return true;

        }
    }
}