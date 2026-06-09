using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Core.Base
{
    public class Response<T>
    {
        //عشان التعامل مع ال API بشكل موحد
        //مثلاً لو عايز تبعت Status Code مع ال Response
        public HttpStatusCode StatusCode { get; set; }
        public object? Meta { get; set; }

        public bool Succeeded { get; set; }
        public string? Message { get; set; }
        public List<string>? Errors { get; set; }
        //public Dictionary<string, List<string>> ErrorsBag { get; set; }
        public T? Data { get; set; }
        public Response()
        {

        }
        public Response(T data, string message = "", bool succeeded = true)
        {
            Data = data;
            Message = message;
            Succeeded = succeeded;
            StatusCode = succeeded ? HttpStatusCode.OK : HttpStatusCode.BadRequest;
        }
        public Response(string message)
        {
            Succeeded = false;
            Message = message;
            StatusCode = HttpStatusCode.BadRequest;
        }
        public Response(string message, bool succeeded)
        {
            Succeeded = succeeded;
            Message = message;
            StatusCode = succeeded ? HttpStatusCode.OK : HttpStatusCode.BadRequest;
        }



    }
}