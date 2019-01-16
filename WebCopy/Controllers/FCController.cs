using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;

namespace WebCopy.Controllers
{
    public class FCController : ApiController
    {
        //// GET api/<controller>/5
        //public FileToCopy Get()
        //{
        //    Retriever objServerRetriever = new Retriever();
        //    return objServerRetriever.GetServiceTask(id);
        //}
        
        //// POST api/<controller>
        //public bool Post([FromBody]FileToCopy inFileStatus)
        //{
        //    LogRecorder objServerMonitor = new LogRecorder();
        //    return objServerMonitor.SaveServiceLog(inFileStatus);
        //}

        /// <summary>
        /// sample code from web
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public HttpResponseMessage Get([FromUri]string filename)
        {
            

            string path = HttpContext.Current.Server.MapPath("~/ToDownload/" + filename);
            if (!File.Exists(path))
            {
                //throw new HttpResponseException("The file does not exist.", HttpStatusCode.NotFound);
                throw new HttpResponseException( HttpStatusCode.NotFound);
            }

            try
            {
                MemoryStream responseStream = new MemoryStream();
                Stream fileStream = File.Open(path, FileMode.Open);
                bool fullContent = true;
                if (this.Request.Headers.Range != null)
                {
                    fullContent = false;

                    // Currently we only support a single range.
                    RangeItemHeaderValue range = this.Request.Headers.Range.Ranges.First();


                    // From specified, so seek to the requested position.
                    if (range.From != null)
                    {
                        fileStream.Seek(range.From.Value, SeekOrigin.Begin);

                        // In this case, actually the complete file will be returned.
                        if (range.From == 0 && (range.To == null || range.To >= fileStream.Length))
                        {
                            fileStream.CopyTo(responseStream);
                            fullContent = true;
                        }
                    }
                    if (range.To != null)
                    {
                        // 10-20, return the range.
                        if (range.From != null)
                        {
                            long? rangeLength = range.To - range.From;
                            int length = (int)Math.Min(rangeLength.Value, fileStream.Length - range.From.Value);
                            byte[] buffer = new byte[length];
                            fileStream.Read(buffer, 0, length);
                            responseStream.Write(buffer, 0, length);
                        }
                        // -20, return the bytes from beginning to the specified value.
                        else
                        {
                            int length = (int)Math.Min(range.To.Value, fileStream.Length);
                            byte[] buffer = new byte[length];
                            fileStream.Read(buffer, 0, length);
                            responseStream.Write(buffer, 0, length);
                        }
                    }
                    // No Range.To
                    else
                    {
                        // 10-, return from the specified value to the end of file.
                        if (range.From != null)
                        {
                            if (range.From < fileStream.Length)
                            {
                                int length = (int)(fileStream.Length - range.From.Value);
                                byte[] buffer = new byte[length];
                                fileStream.Read(buffer, 0, length);
                                responseStream.Write(buffer, 0, length);
                            }
                        }
                    }
                }
                // No Range header. Return the complete file.
                else
                {
                    fileStream.CopyTo(responseStream);
                }
                fileStream.Close();
                responseStream.Position = 0;

                HttpResponseMessage response = new HttpResponseMessage
                {
                    StatusCode = fullContent ? HttpStatusCode.OK : HttpStatusCode.PartialContent,
                    Content = new StreamContent(responseStream)
                };
                return response;
            }
            catch (IOException)
            {
                //throw new HttpResponseException("A generic error occured. Please try again later.", HttpStatusCode.InternalServerError);
                throw new HttpResponseException( HttpStatusCode.InternalServerError);
            }
        }

        /// <summary>
        /// sample code from web
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public HttpResponseMessage Post([FromUri]string filename)
        {
            var task = this.Request.Content.ReadAsStreamAsync();
            task.Wait();
            Stream requestStream = task.Result;

            try
            {
                Stream fileStream = File.Create(HttpContext.Current.Server.MapPath("~/" + filename));
                requestStream.CopyTo(fileStream);
                fileStream.Close();
                requestStream.Close();
            }
            catch (IOException)
            {
                //throw new HttpResponseException("A generic error occured. Please try again later.", HttpStatusCode.InternalServerError);
                throw new HttpResponseException( HttpStatusCode.InternalServerError);
            }

            HttpResponseMessage response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.Created
            };
            return response;
        }
    }
}
