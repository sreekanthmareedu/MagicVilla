using System.Net;

namespace MagicVilla_Web.Models
{
    public class APIResponse
    {
        public HttpStatusCode StatusCodes{set;get;}

        public bool isSuccess { set; get; } = true;

        public List<String> ErrorMessage { get; set; }
        public object Result { set; get; }

    }
}
