using NLog;
using RestSharp;
using System;
using System.IO;
using System.Reflection;
using System.ServiceProcess;
using System.Threading.Tasks;
using System.Timers;
using WebCopy.Framework;

namespace WebCopy
{
    /// <summary>
    /// The actual implementation of the windows service goes here...
    /// </summary>
    [WindowsService("WebCopyService",
        DisplayName = "WebCopyService",
        Description = "The description of the WebCopyService service.",
        EventLogSource = "WebCopyService",
        StartMode = ServiceStartMode.Automatic)]
    public class ServiceImplementation : IWindowsService
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private Timer _timer = null;

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // dispose managed resources
                _timer.Dispose();
                logger.Info("Dispose called!");
                GC.SuppressFinalize(this);
            }
            // free native resources
        }

        /// <summary>
        /// This method is called when the service gets a request to start.
        /// </summary>
        /// <param name="args">Any command line arguments</param>
        public void OnStart(string[] args)
        {
            logger.Info("Service Start");

            _timer = new Timer();
            _timer.Elapsed += new ElapsedEventHandler(DoTimerTasks);

            //set timer to 10 minutes (60 seconds * 1000 miliseconds)
            double intInterval = Convert.ToDouble(Properties.Settings.Default.PollingInterval * 60 * 1000);
            _timer.Interval = intInterval; //10000; //10 seconds
            _timer.AutoReset = true;

            //leave timer down as it is checked in CheckForFilesAsync() and enabled at the end.
            CheckForFilesAsync();
        }

        /// <summary>
        /// This method is called when the service gets a request to stop.
        /// </summary>
        public void OnStop()
        {
        }

        /// <summary>
        /// This method is called when a service gets a request to pause,
        /// but not stop completely.
        /// </summary>
        public void OnPause()
        {
        }

        /// <summary>
        /// This method is called when a service gets a request to resume 
        /// after a pause is issued.
        /// </summary>
        public void OnContinue()
        {
        }

        /// <summary>
        /// This method is called when the machine the service is running on
        /// is being shutdown.
        /// </summary>
        public void OnShutdown()
        {
        }

        /// <summary>
        /// This method is called when a custom command is issued to the service.
        /// </summary>
        /// <param name="command">The command identifier to execute.</param >
        public void OnCustomCommand(int command)
        {
        }




        /// <summary>
        /// purge log files older that [param] to prevent overgrown logs.
        /// </summary>
        private void PurgeOldLogFiles()
        {
            try
            {
                DateTime dtNowMinusCheckinParam = DateTime.Now;
                //set the checkin param to a date/time
                TimeSpan tsChangeFromNow = new TimeSpan(Properties.Settings.Default.LogRetain, 0, 0, 0);
                dtNowMinusCheckinParam = DateTime.Now.Subtract(tsChangeFromNow);

                string dirName = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                string[] files = Directory.GetFiles(dirName + @"\logs");

                foreach (string file in files)
                {
                    FileInfo fi = new FileInfo(file);
                    if (fi.LastAccessTime < dtNowMinusCheckinParam)
                    {
                        fi.Delete();
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Warn("Failure in PurgeOldLogFiles: {0}", ex.Message);
            }
        }

        /// <summary>
        /// Get the next file or list of files
        /// </summary>
        protected async Task CheckForFilesAsync()
        {
            //shut down the timer
            _timer.Enabled = false;

            //foreach file found, download it
            RestComm objRest = new RestComm();
            //get a file to act on
            FileToCopy objFile = await objRest.GetFileNameAsync();

            //check if file exists locally already
            if (!File.Exists(objFile.FileName))
            {
                //if not, download file
                //if (GetFile(objFile.FileName)) //regular download
                if (GetStreamFromServer(objFile.FileName)) //stream download
                {
                    if (Properties.Settings.Default.LogMode)
                    {
                        logger.Info("Success: {0}", objFile.FileName);
                    }

                    //set the file parameters
                    objFile.FileHasError = false;
                    objFile.FileStatus = "";
                    objFile.FilesStatusDate = DateTime.Now;
                    //update the status on the server to complete
                    await objRest.UpdateFileAsync(objFile);
                }
                else
                {
                    logger.Error("Failure Name: {0}", objFile.FileName);
                    //logger.Error("Failure Name: {0}", objFile.FileStatus);
                    //logger.Error("Failure Name: {0}", objFile.FileName);
                }
            }
            //enable timer
            _timer.Enabled = true;

        }


        /// <summary>
        /// Do the actual copy work
        /// direct download
        /// </summary>
        /// <param name="strFileName"></param>
        /// <returns></returns>
        //public bool GetFile(string strFileName)
        //{
        //    string strUrlForSource;
        //    strUrlForSource = Properties.Settings.Default.FileServerURL + strFileName;
        //    try
        //    {
        //        string strDestDirectory = Properties.Settings.Default.FileDestFolder;
        //        WebClient Client = new WebClient();
        //        Client.DownloadFile(new Uri(strUrlForSource), strDestDirectory);

        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        logger.Warn("Failure to get update from server: {0}", ex.Message);
        //        logger.Warn("Attempted address: {0}", strUrlForSource);
        //        return false;
        //    }

        //}

        /// <summary>
        /// Check for a new file
        /// </summary>
        public void DoTimerTasks(object sender, ElapsedEventArgs args)
        {
            try
            {
                //remove old log files
                PurgeOldLogFiles();
                //start downloading by checking for new files
                CheckForFilesAsync();
            }
            catch (Exception ex)
            {
                logger.Warn("DoTimerTasks error: {0}", ex.Message);
            }
        }

        /// <summary>
        /// RestSharp example
        /// </summary>
        public bool GetStreamFromServer(string FileToGet)
        {
            try
            {
                //string tempFile = Path.GetTempFileName();
                using (var writer = File.OpenWrite(FileToGet))
                {
                    var client = new RestClient(Properties.Settings.Default.FileServerURL);
                    var request = new RestRequest(Properties.Settings.Default.FileDestFolder + FileToGet)
                    {
                        ResponseWriter = (responseStream) => responseStream.CopyTo(writer)
                    };
                    var response = client.DownloadData(request);
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }

        /// <summary>
        /// sample code from web
        /// </summary>
        /// <param name="wavStream"></param>
        //private void UploadDirectly(Stream wavStream)
        //{
        //    string serviceUri = "http://localhost:4349/files/test.wav";
        //    HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(serviceUri);
        //    request.Method = "POST";
        //    request.BeginGetRequestStream(result =>
        //    {
        //        Stream requestStream = request.EndGetRequestStream(result);
        //        wavStream.CopyTo(requestStream);
        //        requestStream.Close();
        //        request.BeginGetResponse(result2 =>
        //        {
        //            try
        //            {
        //                HttpWebResponse response = (HttpWebResponse)request.EndGetResponse(result2);
        //                if (response.StatusCode == HttpStatusCode.Created)
        //                {
        //                    this.Dispatcher.BeginInvoke(() =>
        //                    {
        //                        MessageBox.Show("Upload completed.");
        //                    });
        //                }
        //                else
        //                {
        //                    this.Dispatcher.BeginInvoke(() =>
        //                    {
        //                        MessageBox.Show("An error occured during uploading. Please try again later.");
        //                    });
        //                }
        //            }
        //            catch
        //            {
        //                this.Dispatcher.BeginInvoke(() =>
        //                {
        //                    MessageBox.Show("An error occured during uploading. Please try again later.");
        //                });
        //            }
        //            wavStream.Close();
        //        }, null);
        //    }, null);
        //}



        #region ServiceActions

        /// <summary>
        /// Convert service status to string
        /// </summary>
        /// <param name="strServiceName">service to examine</param>
        /// <returns>current service status</returns>
        private string GetServiceStatus(string strServiceName)
        {
            try
            {
                //change status from var to string
                ServiceController sc = new ServiceController(strServiceName);

                switch (sc.Status)
                {
                    case ServiceControllerStatus.Running:
                        return "Running";
                    case ServiceControllerStatus.Stopped:
                        return "Stopped";
                    case ServiceControllerStatus.Paused:
                        return "Paused";
                    case ServiceControllerStatus.StopPending:
                        return "Stopping";
                    case ServiceControllerStatus.StartPending:
                        return "Starting";
                    default:
                        return "Status Changing";
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        private string RestartService(string serviceName, int timeoutMilliseconds)
        {
            ServiceController service = new ServiceController(serviceName);
            try
            {
                int millisec1 = Environment.TickCount;
                TimeSpan timeout = TimeSpan.FromMilliseconds(timeoutMilliseconds);

                service.Stop();
                service.WaitForStatus(ServiceControllerStatus.Stopped, timeout);

                // count the rest of the timeout
                int millisec2 = Environment.TickCount;
                timeout = TimeSpan.FromMilliseconds(timeoutMilliseconds - (millisec2 - millisec1));

                service.Start();
                service.WaitForStatus(ServiceControllerStatus.Running, timeout);

                return GetServiceStatus(serviceName);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        private string StartService(string serviceName, int timeoutMilliseconds)
        {
            ServiceController service = new ServiceController(serviceName);
            try
            {
                TimeSpan timeout = TimeSpan.FromMilliseconds(timeoutMilliseconds);

                service.Start();
                service.WaitForStatus(ServiceControllerStatus.Running, timeout);

                return GetServiceStatus(serviceName);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        private string StopService(string serviceName, int timeoutMilliseconds)
        {
            ServiceController service = new ServiceController(serviceName);
            try
            {
                TimeSpan timeout = TimeSpan.FromMilliseconds(timeoutMilliseconds);

                service.Stop();
                service.WaitForStatus(ServiceControllerStatus.Stopped, timeout);

                return GetServiceStatus(serviceName);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        #endregion ServiceActions

    }
}
