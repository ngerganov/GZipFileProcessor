namespace GZipFileProcessor;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Welcome to GZipFileProcessor!");

        Console.WriteLine("Select operation:");
        Console.WriteLine("1. Compress");
        Console.WriteLine("2. Decompress");
        Console.Write("Enter operation number: ");
        string operationChoice = Console.ReadLine();

        if (operationChoice == "1")
        {
            Console.WriteLine("Please provide the name of the file you want to compress (with extension):");
            string fileName = Console.ReadLine();
            CompressFile(fileName);
        }
        else if (operationChoice == "2")
        {
            Console.WriteLine("Please provide the name of the file you want to decompress (without extension):");
            string fileName = Console.ReadLine();
            Console.WriteLine("Please provide the extension of the file (e.g., pdf, txt, docx):");
            string fileExtension = Console.ReadLine();
            DecompressFile(fileName, fileExtension);
        }
        else
        {
            Console.WriteLine("Invalid operation choice.");
        }
    }

    static void CompressFile(string fileName)
    {
        string currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
        string sourceFilePath = Path.Combine(currentDirectory, fileName);
        string destinationFilePath = Path.Combine(currentDirectory, $"{Path.GetFileNameWithoutExtension(fileName)}_compressed.gz");

        byte[] sourceBytes = FileManager.ReadAllBytes(sourceFilePath);
        if (sourceBytes == null)
        {
            Console.WriteLine("File not found or unable to read file.");
            return;
        }

        ICompressor compressor = new GZipCompressor(bufferSize: 1024);

        byte[] resultBytes = compressor.Compress(sourceBytes);

        FileManager.WriteAllBytes(destinationFilePath, resultBytes);
        Console.WriteLine($"File compressed successfully. Result saved as {destinationFilePath}");
    }

    static void DecompressFile(string fileName, string fileExtension)
    {
        string currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
        string sourceFilePath = Path.Combine(currentDirectory, $"{fileName}.gz");
        string destinationFilePath = Path.Combine(currentDirectory, $"{fileName}_decompressed.{fileExtension}");

        byte[] sourceBytes = FileManager.ReadAllBytes(sourceFilePath);
        if (sourceBytes == null)
        {
            Console.WriteLine("File not found or unable to read file.");
            return;
        }

        ICompressor compressor = new GZipCompressor(bufferSize: 1024);

        byte[] resultBytes = compressor.Decompress(sourceBytes);

        FileManager.WriteAllBytes(destinationFilePath, resultBytes);
        Console.WriteLine($"File decompressed successfully. Result saved as {destinationFilePath}");
    }
}