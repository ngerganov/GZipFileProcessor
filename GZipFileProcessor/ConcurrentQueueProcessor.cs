using System.Collections.Concurrent;

namespace GZipFileProcessor;

public class ConcurrentQueueProcessor : IBlockQueue
{
    private readonly BlockingCollection<(byte[] Block, int Order)> _queue = new BlockingCollection<(byte[] Block, int Order)>();
    private int _blockCounter;
    private const int MaxThreads = 10;

    public void Add(byte[] block)
    {
        _queue.Add((block, _blockCounter++));
    }

    public void CompleteAdding()
    {
        _queue.CompleteAdding();
    }

    public Task ProcessQueueAndSaveAsync(ICompressor compressor, string destinationFilePath)
    {
        var results = new ConcurrentDictionary<int, byte[]>();
        Parallel.ForEach(_queue.GetConsumingEnumerable(), 
            new ParallelOptions { MaxDegreeOfParallelism = MaxThreads }, item =>
            {
                byte[] processedBlock = compressor.Process(item.Block);
                results[item.Order] = processedBlock;
            });

        var orderedBlocks = results.OrderBy(kvp => kvp.Key).SelectMany(kvp => kvp.Value).ToArray();
        File.WriteAllBytes(destinationFilePath, orderedBlocks);
        return Task.CompletedTask;
    }
}