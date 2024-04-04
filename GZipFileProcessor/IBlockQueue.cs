namespace GZipFileProcessor;

public interface IBlockQueue
{
    void Add(byte[] block);
    void CompleteAdding();
}