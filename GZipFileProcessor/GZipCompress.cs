using System.IO.Compression;

namespace GZipFileProcessor;

public class GZipCompress : ICompressor
{
    public byte[] Process(byte[] bytes)
    {
        if (bytes == null) 
            throw new ArgumentNullException(nameof(bytes));

        using (var output = new MemoryStream())
        {
            using (var compress = new GZipStream(output, CompressionMode.Compress))
            {
                compress.Write(bytes, 0, bytes.Length);
            }

            return output.ToArray();
        }
    }
}