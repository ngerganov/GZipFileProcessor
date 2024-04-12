namespace GZipFileProcessor;

public class Block
{
     public Block(byte[] data, int index)
     {
          Data = data ?? throw new ArgumentNullException(nameof(data));
          if (index < 0)
          {
               throw new ArgumentOutOfRangeException(nameof(index));
          }
          Index = index;
     }

     public int Index { get; set; }
     public byte[] Data { get; set; }
}