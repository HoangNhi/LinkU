﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MODELS.BASE
{
    public class BaseResponse<T>
    {
        public T Data { get; set; }
        public bool Error { get; set; } = false;
        public string? Message { get; set; }
    }

    public class BaseResponse
    {
        public bool Error { get; set; } = false;
        public string? Message { get; set; }
    }
}
