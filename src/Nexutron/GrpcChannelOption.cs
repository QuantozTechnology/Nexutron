﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nexutron
{
    public class GrpcChannelOption
    {
        public string Host { get; set; } = "grpc.shasta.trongrid.io";
        public int Port { get; set; } = 50051;
    }
}