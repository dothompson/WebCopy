using NLog;
using RestSharp;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace WebCopy
{
    public partial class Form1 : Form
    {
        private HttpClient FileApiServer = new HttpClient();
        private static Logger logger = LogManager.GetCurrentClassLogger();
        //private Timer _timer = null;

        public Form1()
        {
            InitializeComponent();
            if (!txtFromURL.Text.EndsWith("/"))
            {
                txtFromURL.Text = txtFromURL.Text + "/";
            }
            FileApiServer.BaseAddress = new Uri(txtFromURL.Text);
            FileApiServer.DefaultRequestHeaders.Accept.Clear();
            FileApiServer.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            //we may not be on the same domain so we need to send UID & PWD
            //since we are using HTTPS via TLS, we can safely send basic authentication data
            //byte[] cred = UTF8Encoding.UTF8.GetBytes("UID:PWD");
            //FileApiServer.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(cred));
        }

        private void btnStartDownload_Click(object sender, EventArgs e)
        {
            //refresh the endpoint
            if (!txtFromURL.Text.EndsWith("/"))
            {
                txtFromURL.Text = txtFromURL.Text + "/";
            }
            FileApiServer.BaseAddress = new Uri(txtFromURL.Text);
            lbLog.Items.Add("Starting App");
            logger.Info("Starting App");
            CheckForFilesAsync();
        }

        protected async Task CheckForFilesAsync()
        {
            //shut down the timer
            //_timer.Enabled = false;

            //foreach file found, download it

            //get a file to act on
            lbLog.Items.Add("pre-get file name");
            logger.Info("pre-get file name");
            FileToCopy objFile = await GetFileNameAsync();
            lbLog.Items.Add("post-get file name" + objFile.FileName);
            logger.Info("post-get file name" + objFile.FileName);

            //check if file exists locally already
            if (!File.Exists(objFile.FileName))
            {
                //if not, download file
                //if (GetFile(objFile.FileName)) //regular download
                lbLog.Items.Add("pre-do actual download");
                logger.Info("pre-do actual download");
                if (GetStreamFromServer(objFile.FileName)) //stream download
                {
                    if (chkLogging.Checked)
                    {
                        logger.Info("Success: {0}", objFile.FileName);
                    }

                    //set the file parameters
                    objFile.FileHasError = false;
                    objFile.FileStatus = "";
                    objFile.FilesStatusDate = DateTime.Now;
                    //update the status on the server to complete
                    await UpdateFileAsync(objFile);
                }
                else
                {
                    logger.Error("Failure Name: {0}", objFile.FileName);
                }
            }
            //enable timer
            //_timer.Enabled = true;

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
                    var client = new RestClient(txtFromURL.Text);
                    var request = new RestRequest(txtDestinationFolder.Text + FileToGet)
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
        /// query (GET) from the server
        /// </summary>
        /// <returns>Server class ojbect</returns>
        public async Task<FileToCopy> GetFileNameAsync()
        {
            FileToCopy objFile = null;
            try
            {
                HttpResponseMessage response = await FileApiServer.GetAsync("api/status");
                if (response.IsSuccessStatusCode)
                {
                    objFile = await response.Content.ReadAsAsync<FileToCopy>();
                }
                else
                {
                    lbLog.Items.Add("GetFileNameAsync Failure: " + response.ToString());
                    logger.Error("GetFileNameAsync Failure: {0}", response);
                }
                return objFile;
            }
            catch (Exception e)
            {
                lbLog.Items.Add("GetFileNameAsync Failure: " + e.Message.ToString());
                lbLog.Items.Add("GetFileNameAsync Failure: " + e.InnerException.ToString());
                logger.Warn("GetFileNameAsync Failure: {0}", e.Message.ToString());
                logger.Warn("GetFileNameAsync Failure: {0}", e.InnerException.ToString());
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
                HttpResponseMessage response = await FileApiServer.GetAsync("api/fc");
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
            if (chkLogging.Checked)
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
                HttpResponseMessage response = await FileApiServer.PostAsJsonAsync("api/status", objFile);
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

        private void txtDestinationFolder_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnPickFolder_Click(object sender, EventArgs e)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                DialogResult result = fbd.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    txtDestinationFolder.Text = fbd.SelectedPath;
                }
            }
        }
    }
}
