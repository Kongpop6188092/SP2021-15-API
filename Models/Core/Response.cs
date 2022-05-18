using System.Collections.Generic;

namespace MUDENT_API.Models.Core
{
    public class Response<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
    }
}
