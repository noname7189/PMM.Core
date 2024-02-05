using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace PMM.Core.Provider.DataClass.Rest
{
    public record Response<T>
    {
        public HttpStatusCode StatusCode { get; set; }
        public string? Msg { get; set; }
        public T? Data { get; set; }
    }
}
