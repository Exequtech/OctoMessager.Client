using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace OctoMessager.client.Configuration
{
    public class OctoClientConfig
    {
        public string BaseUrl { get; set; } = "https://octo-messager.exequtech.com";
        public JsonSerializerOptions JsonSerializerOptions { get; set; } = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }
}
