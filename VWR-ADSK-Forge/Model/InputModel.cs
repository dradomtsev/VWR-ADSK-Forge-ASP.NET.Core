using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Backend.Model
{
    public class InputModel
    {
        public string bucketKey { get; set; }
        public IFormFile fileToUpload { get; set; }
    }
}
