using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;


namespace Salesforce.Common
{
    /// <summary>
    /// Extension methods for <see cref="HttpContent"/>.
    /// </summary>
    public static class HttpContentCompressedDataExtensions
    {
        /// <summary>
        /// Content-Encoding default value 
        /// </summary>
        public const string GZipEncoding = "gzip";

        /// <summary>
        /// Returns the response content to string. It decompresses the content if needed
        /// </summary>
        /// <param name="responseContent">Http Response Message</param>
        /// <returns>Http Response content as string</returns>
        public static async Task<string> ReadAsDecompressedStringAsync(this HttpContent responseContent)
        {
            string content;

            if (responseContent == null)
            {
                return string.Empty;
            }

            // Check if the response content is gzip encoded. Gzipped response length is less than the actual and thereby less 
            // payload over the network.
            if (responseContent.Headers.ContentEncoding.Contains(GZipEncoding,StringComparer.OrdinalIgnoreCase))
            {
                var responseStream = await responseContent.ReadAsStreamAsync().ConfigureAwait(false);
                var unzippedContent = new GZipStream(responseStream, CompressionMode.Decompress);
                content = await(new StreamReader(unzippedContent)).ReadToEndAsync();
            }
            else
            {
                content = await responseContent.ReadAsStringAsync().ConfigureAwait(false);
            }

            return content;
        }
    }
}
