using System;
using System.Collections.Generic;
using System.Text;

namespace ClusterManager.Model.APIModels.ResponseModel
{
    public class ResponseModel<T>
    {
        //public int code { get; set; }
        public String message { get; set; }
        public T data { get; set; }
        public ResponseModel (String _message, T _data)
        {
            //this.code = _code;
            this.message = _message;
            this.data = _data;
        }
    }
}
