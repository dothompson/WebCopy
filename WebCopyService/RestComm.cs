using NLog;
using System;
using System.Net.Http;
using WebCopy;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace WebCopy
{
    class RestComm : IDisposable
    {
        private HttpClient FileApiServer = new HttpClient();
        private static Logger logger = LogManager.GetCurrentClassLogger();
        ///// <summary>
        ///// Register the client to the log server
        ///// </summary>
        ///// <param name="server"></param>
        ///// <returns></returns>
        //public async Task<Uri> RegisterClientAsync(Service server)
        //{
        //    HttpResponseMessage response = await LogServer.PostAsJsonAsync("api/Servers", server);
        //    response.EnsureSuccessStatusCode();

        //    // return URI of the created resource.
        //    return response.Headers.Location;
        //}

        /// <summary>
        /// query (GET) from the server
        /// </summary>
        /// <returns>Server class ojbect</returns>
        public async Task<FileToCopy> GetFileNameAsync()
        {
            FileToCopy objFile = null;
            try
            {
                HttpResponseMessage response = await FileApiServer.GetAsync("api/Service");
                if (response.IsSuccessStatusCode)
                {
                    objFile = await response.Content.ReadAsAsync<FileToCopy>();
                }
                return objFile;
            }
            catch (Exception e)
            {
                logger.Warn("GetFileAsync Failure: {0}", e.Message.ToString());
                logger.Warn("GetFileAsync Failure: {0}", e.InnerException.ToString());
                objFile.FileHasError = true;
                objFile.FileStatus = e.Message;
                return objFile;
            }

        }

        /// <summary>
        /// query (GET) from the server
        /// </summary>
        /// <returns>Server class ojbect</returns>
        public async Task<FileToCopy> GetFileAsync()
        {
            FileToCopy objFile = null;
            try
            {
                HttpResponseMessage response = await FileApiServer.GetAsync("api/Service");
                if (response.IsSuccessStatusCode)
                {
                    objFile = await response.Content.ReadAsAsync<FileToCopy>();
                }
                return objFile;
            }
            catch (Exception e)
            {
                logger.Warn("GetFileAsync Failure: {0}", e.Message.ToString());
                logger.Warn("GetFileAsync Failure: {0}", e.InnerException.ToString());
                objFile.FileHasError = true;
                objFile.FileStatus = e.Message;
                return objFile;
            }

        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="objFile"></param>
        /// <returns></returns>
        public async Task<FileToCopy> UpdateFileAsync(FileToCopy objFile)
        {
            //debug code
            //dump the current service ojbect
            if (Properties.Settings.Default.LogMode)
            {
                logger.Info("File Has Issue: {0}", objFile.FileHasError);
                logger.Info("File Name: {0}", objFile.FileName);
                logger.Info("File Status Date: {0}", objFile.FilesStatusDate);
                logger.Info("File Status: {0}", objFile.FileStatus);
                logger.Info("LogServerUrl: {0}", FileApiServer.BaseAddress);
            }
            // end debug

            try
            {
                HttpResponseMessage response = await FileApiServer.PutAsJsonAsync($"api/Service/{objFile.FileName}", objFile);
                if (response.IsSuccessStatusCode)
                {
                    // Deserialize the updated product from the response body.
                    objFile = await response.Content.ReadAsAsync<FileToCopy>();
                }
                else
                {
                    logger.Warn("UpdateFileAsync Failure: {0}", response.StatusCode);
                    objFile.FileStatus = response.StatusCode.ToString();
                    objFile.FileHasError = true;

                }
            }
            catch (Exception e)
            {
                objFile.FileStatus = e.Message.ToString();
                objFile.FileHasError = true;
                logger.Warn("UpdateFileAsync Failure: {0}", e.Message.ToString());
                logger.Warn("UpdateFileAsync Failure: {0}", e.InnerException.ToString());

            }

            return objFile;
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        /// <summary>
        /// 
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    FileApiServer.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~ServerComm() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        /// <summary>
        /// 
        /// </summary>
            // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
