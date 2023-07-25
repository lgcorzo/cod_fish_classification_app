

using System.Text;
namespace csr.com
{
    public class CSRMessage
    {
        //////////////////////////////////////////////////////////////
        public string DestinyId { get ; set; }
        public string OriginId { get; set; }
        public byte[] Data { get; set; }
        public CSRContentType ContentType { get; set; }

        private System.Text.ASCIIEncoding ascii_encoding = new System.Text.ASCIIEncoding();
        private Encoding enc = new UTF8Encoding(true, true);

        //////////////////////////////////////////////////////////////
        public CSRMessage(string destiny, string data)
        {
            DestinyId   = destiny;
            OriginId    = "undefined";
            //Data        = ascii_encoding.GetBytes(data);
            Data = enc.GetBytes(data);
            ContentType = CSRContentType.Text;
        }

        //////////////////////////////////////////////////////////////
        public CSRMessage(string destiny, byte[] data)
        {
            DestinyId   = destiny;
            OriginId    = "undefined";
            Data        = data;
            ContentType = CSRContentType.Binary;
        }

        //////////////////////////////////////////////////////////////
        public override string ToString()
        {
            //return ascii_encoding.GetString(Data);
            return enc.GetString(Data);
        }

        //////////////////////////////////////////////////////////////
        public byte[] ToByteArray()
        {
            return Data;
        }

        //////////////////////////////////////////////////////////////
    }
}
