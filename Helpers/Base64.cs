using System.Drawing;
using System.IO;
using System.Text;

namespace EasyTwoJuetengBackend.Helpers
{
    public class Base64
    {
        public static string WebRootBase { get; set; }
        public static string Base64Encode(byte[] data)
        {
            return System.Convert.ToBase64String(data);
        }

        public static string Base64Encode(string data)
        {
            return System.Convert.ToBase64String(Encoding.ASCII.GetBytes(data));
        }
        public static byte[] Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return base64EncodedBytes;
        }

     
    }
}
