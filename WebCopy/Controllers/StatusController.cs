using System.Web.Http;
using WebCopy.Models;

namespace WebCopy.Controllers
{
    public class StatusController : ApiController
    {

        // GET api/<controller>/5
        public FileToCopy Get()
        {
            FileData objFileActions = new FileData();
            return objFileActions.GetNextFile();
        }

        // POST api/<controller>
        public bool Post([FromBody] FileToCopy inFileStatus)
        {
            FileData objFileActions = new FileData();
            return objFileActions.ReportComplete(inFileStatus);
        }

        //// GET: api/Status
        //public IEnumerable<string> Get()
        //{
        //    return new string[] { "value1", "value2" };
        //}

        //// GET: api/Status/5
        //public string Get(int id)
        //{
        //    return "value";
        //}

        //// POST: api/Status
        //public void Post([FromBody]string value)
        //{
        //}

        //// PUT: api/Status/5
        //public void Put(int id, [FromBody]string value)
        //{
        //}

        //// DELETE: api/Status/5
        //public void Delete(int id)
        //{
        //}
    }
}
