using SixLabors.ImageSharp.PixelFormats;

namespace Maynard.Imaging.Models;

public class NeighboringPixels
{
    private Rgba32[] Data { get; set; }
    private int Dimension { get; set; }
    private int Reach { get; set; }
    private int StartingX { get; set; }

    public NeighboringPixels(ref List<Rgba32[]> imageData, int x, int y, int reach)
    {
        // reach 1: 9 pixels
        // reach 2: 25 pixels
        // reach 3: 49 pixels
        
        // (2*reach + 1)^2
        Reach = reach;
        Dimension = reach * 2 + 1;
        StartingX = x;
        Data = new Rgba32[Dimension * Dimension];

        int index = 0;
        for (int row = y - reach; row < Math.Min(imageData.Count, y + reach + 1); row++)
        {
            if (row < 0)
            {
                index += Dimension;
                continue;
            }

            for (int col = x - reach; col <= Math.Min(imageData[row].Length - 1, x + reach); col++)
                if (col < 0 || col > imageData[0].Length)
                    index++;
                else
                    Data[index++] = imageData[row][col];
        }
    }

    public void ShiftUp(Rgba32[] row = null)
    {
        int i = 0;
        while (i < Data.Length - Dimension)
            Data[i] = Data[i++ + Dimension];
        if (StartingX - Reach < 0)
            i += (StartingX - Reach) * -1;
        
        if (row == null)
            while (i < Data.Length)
                Data[i++] = default;
        else
            for (int next = Math.Max(StartingX - Reach, 0); next <= Math.Min(StartingX + Reach, row.Length - 1); next++)
                Data[i++] = row[next];
    }

    public Rgba32 Average() => new()
    {
        R = (byte)(int)Data.Average(x => x.R),
        G = (byte)(int)Data.Average(x => x.G),
        B = (byte)(int)Data.Average(x => x.B),
        A = (byte)(int)Data.Average(x => x.A)
    };
}