using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Salesforce.Force.UnitTests
{
    public class XmlContent : HttpContent
    {
        private readonly MemoryStream _stream = new MemoryStream();

        public XmlContent(object value)
        {
            Headers.ContentType = new MediaTypeHeaderValue("application/xml");
            var xmlSerializer = new XmlSerializer(value.GetType());
            using (var writer = XmlWriter.Create(_stream))
            {
                xmlSerializer.Serialize(writer, value);
            }
            _stream.Position = 0;
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
