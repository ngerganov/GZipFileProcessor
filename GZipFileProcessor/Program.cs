namespace GZipFileProcessor;

class Program
{
    private const int BlockSize = 1024 * 1024;

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
            CompressFile(fileName, new GZipCompress());
        }
        else if (operationChoice == "2")
        {
            Console.WriteLine("Please provide the name of the file you want to decompress (without extension):");
            string fileName = Console.ReadLine();
            Console.WriteLine("Please provide the extension of the file (e.g., pdf, txt, docx):");
            string fileExtension = Console.ReadLine();
            DecompressFile(fileName, fileExtension, new GZipDecompress(BlockSize));
        }
        else
        {
            Console.WriteLine("Invalid operation choice.");
        }
    }

    static void CompressFile(string fileName, ICompressor compressor)
    {
        string currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
        string sourceFilePath = Path.Combine(currentDirectory, fileName);
        string destinationFilePath =
            Path.Combine(currentDirectory, $"{Path.GetFileNameWithoutExtension(fileName)}_compressed.gz");

        using (var inputStream = File.OpenRead(sourceFilePath))
        using (var outputStream = File.Create(destinationFilePath))
        {
            var blockQueue = new ConcurrentBlockQueue();
            var compressorTasks = new List<Task>();

            Task.Run(() =>
            {
                int index = 0;
                while (true)
                {
                    byte[] buffer = new byte[BlockSize];
                    int bytesRead = inputStream.Read(buffer, 0, buffer.Length);
                    if (bytesRead == 0)
                    {
                        blockQueue.Complete();
                        break;
                    }

                    var block = new Block(buffer.Take(bytesRead).ToArray(), index);
                    blockQueue.Add(block);
                    index++;
                }
            });

            for (int i = 0; i < 10; i++)
            {
                var compressorTask = Task.Run(() =>
                {
                    while (true)
                    {
                        var block = blockQueue.Next();
                        if (block == null) break;
                        byte[] compressedBlock = compressor.Process(block.Data);
                        lock (outputStream)
                        {
                            FileManager.WriteInt32(outputStream, compressedBlock.Length);
                            outputStream.Write(compressedBlock, 0, compressedBlock.Length);
                        }
                    }
                });
                compressorTasks.Add(compressorTask);
            }

            Task.WaitAll(compressorTasks.ToArray());
        }

        Console.WriteLine($"Operation completed successfully. Result saved as {destinationFilePath}");
    }

    static void DecompressFile(string fileName, string fileExtension, ICompressor decompressor)
    {
        string currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
        string sourceFilePath = Path.Combine(currentDirectory, $"{fileName}.gz");
        string destinationFilePath = Path.Combine(currentDirectory, $"{fileName}_decompressed.{fileExtension}");

        using (var inputStream = File.OpenRead(sourceFilePath))
        using (var outputStream = File.Create(destinationFilePath))
        {
            var blockQueue = new ConcurrentBlockQueue();
            var decompressorTasks = new List<Task>();

            Task.Run(() =>
            {
                int index = 0;
                while (true)
                {
                    int blockSize = FileManager.ReadInt32(inputStream);
                    if (blockSize == 0) break;

                    byte[] buffer = new byte[blockSize];
                    inputStream.Read(buffer, 0, blockSize);
                    var block = new Block(buffer, index);
                    blockQueue.Add(block);
                    index++;
                }

                blockQueue.Complete();
            });

            for (int i = 0; i < 10; i++)
            {
                var decompressorTask = Task.Run(() =>
                {
                    while (true)
                    {
                        var block = blockQueue.Next();
                        if (block == null) break;
                        byte[] decompressedBlock = decompressor.Process(block.Data);
                        lock (outputStream)
                        {
                            outputStream.Write(decompressedBlock, 0, decompressedBlock.Length);
                        }
                    }
                });
                decompressorTasks.Add(decompressorTask);
            }

            Task.WaitAll(decompressorTasks.ToArray());
        }

        Console.WriteLine($"File decompressed successfully. Result saved as {destinationFilePath}");
    }
}