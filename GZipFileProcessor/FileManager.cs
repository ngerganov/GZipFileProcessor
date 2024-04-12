namespace GZipFileProcessor;

public static class FileManager
{
    public static int ReadInt32(Stream stream)
    {
        byte[] bytes = new byte[4];
        stream.Read(bytes, 0, bytes.Length);
        return BitConverter.ToInt32(bytes, 0);
    }

    public static void WriteInt32(Stream stream, int value)
    {
        byte[] bytes = BitConverter.GetBytes(value);
        stream.Write(bytes, 0, bytes.Length);
    }
}