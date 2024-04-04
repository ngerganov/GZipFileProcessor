using System.IO.Compression;

namespace GZipFileProcessor;

public class GZipDecompress : ICompressor
{
    private readonly int _bufferSize;

    public GZipDecompress(int bufferSize)
    {
        _bufferSize = bufferSize;
    }

    public byte[] Process(byte[] bytes)
    {
        if (bytes == null) 
            throw new ArgumentNullException(nameof(bytes));

        using (var output = new MemoryStream())
        {
            using (var input = new MemoryStream(bytes))
            {
                using (var decompress = new GZipStream(input, CompressionMode.Decompress))
                {
                    decompress.Copy(output, _bufferSize);
                }
            }

            return output.ToArray();
        }
    }
}