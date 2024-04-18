using System.Collections.Concurrent;

namespace GZipFileProcessor;

public class ConcurrentBlockQueue
{
    private readonly ConcurrentDictionary<int, Block> _blocks = new ConcurrentDictionary<int, Block>();
    private readonly ManualResetEvent _mre = new ManualResetEvent(false);
    private volatile int _currentIndex = 0;
    private bool _isCompleted = false;

    public void Add(Block block)
    {
        if (_isCompleted)
        {
            throw new InvalidOperationException("Cannot add blocks to a completed queue.");
        }
        
        _blocks.TryAdd(block.Index, block);
        _mre.Set();
    }

    public Block Next()
    {
        _mre.WaitOne();
        Block block;
        while (!_blocks.TryRemove(_currentIndex, out block))
        {
            if (_isCompleted)
            {
                throw new InvalidOperationException("Queue is completed.");
            }
            
            if (_blocks.TryRemove(_currentIndex, out block))
            {
                break;
            }
            
            _mre.Reset();
            _mre.WaitOne();
        }
        
        Interlocked.Increment(ref _currentIndex);
        return block;
    }

    public void Complete()
    {
        _isCompleted = true;
        _mre.Set();
    }
}