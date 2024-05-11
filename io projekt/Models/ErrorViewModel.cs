using Microsoft.Extensions.Hosting;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.InteropServices;

namespace io_projekt.Models
{

    public class ErrorViewModel
    {
        public string? RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}