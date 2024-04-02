namespace GZipFileProcessor;

public static class FileManager
{
    public static byte[] ReadAllBytes(string filePath)
    {
        try
        {
            return File.ReadAllBytes(filePath);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error reading file: {ex.Message}");
            return null;
        }
    }

    public static void WriteAllBytes(string filePath, byte[] bytes)
    {
        try
        {
            File.WriteAllBytes(filePath, bytes);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error writing file: {ex.Message}");
        }
    }
}