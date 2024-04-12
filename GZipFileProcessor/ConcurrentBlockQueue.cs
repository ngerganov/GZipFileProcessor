using System.Collections.Concurrent;

namespace GZipFileProcessor;

public class ConcurrentBlockQueue
{
    private readonly ConcurrentDictionary<int, Block> _blocks = new ConcurrentDictionary<int, Block>();
    private readonly ManualResetEvent _mre = new ManualResetEvent(false);
    private int _currentIndex = 0;

    public void Add(Block block)
    {
        _blocks.TryAdd(block.Index, block);
        _mre.Set();
    }

    public Block Next()
    {
        _mre.WaitOne();
        while (_blocks.ContainsKey(_currentIndex))
        {
            Block block;
            if (_blocks.TryRemove(_currentIndex, out block))
            {
                _currentIndex++;
                return block;
            }
        }

        return null;
    }

    public void Complete()
    {
        _mre.Set();
    }
}