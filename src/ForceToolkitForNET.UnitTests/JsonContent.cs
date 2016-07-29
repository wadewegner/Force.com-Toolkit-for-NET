using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Salesforce.Force.UnitTests
{
    public class JsonContent : HttpContent
    {
        private readonly MemoryStream _stream = new MemoryStream();
        
        public JsonContent(object value)
        {
            Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var jw = new JsonTextWriter(new StreamWriter(_stream)) { Formatting = Formatting.Indented };
            var serializer = new JsonSerializer();
            serializer.Serialize(jw, value);
            jw.Flush();
            _stream.Position = 0;
        }

        protected JsonContent(string content)
        {
            Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var sw = new StreamWriter(_stream);
            sw.Write(content);
            sw.Flush();
            _stream.Position = 0;
        }

        public static HttpContent FromFile(string filepath)
        {
            string content = File.ReadAllText(filepath);
            return new JsonContent(content);
        }

        protected override Task SerializeToStreamAsync(Stream stream, TransportContext context)
        {
            return _stream.CopyToAsync(stream);
        }

        protected override bool TryComputeLength(out long length)
        {
            length = _stream.Length;
            return true;
        }
    }
}