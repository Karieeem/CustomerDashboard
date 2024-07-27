using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CustomerDashboard.Models.Dtos
{
    public class ResponseDto<T>
    {
        public T Data { get; set; }
        public short StatusCode { get; set; }
        public string Message { get; set; }
        public bool IsSuccess { get; set; }
        public int TotalCount { get; set; }

        public ResponseDto()
        {
            IsSuccess = true;
            StatusCode = (short)HttpStatusCode.OK;
        }

        public ResponseDto(T data, int count = 0)
        {
            IsSuccess = true;
            StatusCode = (short)HttpStatusCode.OK;
            Data = data;
            TotalCount = count;
        }

        public ResponseDto(T data, string message, bool Status = false)
        {
            IsSuccess = true;
            Message = message;
            StatusCode = Status ? (short)HttpStatusCode.OK : (short)HttpStatusCode.BadRequest;
            Data = data;
            if (IsList(data))
                TotalCount = (data as IList).Count;
        }

        public ResponseDto(string message, bool Status = false)
        {
            IsSuccess = Status;
            Message = message;
            StatusCode = Status ? (short)HttpStatusCode.OK : (short)HttpStatusCode.BadRequest;
        }

        private static bool IsList(T t)
        {
            if (t == null) return false;
            return t is IList &&
                   t.GetType().IsGenericType &&
                   t.GetType().GetGenericTypeDefinition().IsAssignableFrom(typeof(List<>));
        }
    }

}
