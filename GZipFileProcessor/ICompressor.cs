namespace GZipFileProcessor;

public interface ICompressor
{
    byte[] Process(byte[] bytes);
}