using SixLabors.ImageSharp.PixelFormats;

namespace Maynard.ImageManipulator.Client.Utilities;

public class AveragingWindow
{
    private Rgba32[] Data { get; set; }
    private int Dimension { get; set; }
    private int Reach { get; set; }
    private int StartingX { get; set; }

    public AveragingWindow(ref List<Rgba32[]> imageData, int x, int y, int reach)
    {
        // reach 1: 9 pixels
        // reach 2: 25 pixels
        // reach 3: 49 pixels
        
        Reach = reach;
        Dimension = reach * 2 + 1;
        StartingX = x;
        Data = new Rgba32[Dimension];

        int index = 0;
        for (int row = y - reach; row < Math.Min(imageData.Count, y + reach + 1); row++)
            Data[index++] = row < 0
                ? new (0, 0, 0, 255)
                : GetAveragePixel(imageData[row]);
        while (index < Data.Length)
            Data[index++] = new (0, 0, 0, 255);
    }
    
    private Rgba32 GetAveragePixel(Rgba32[] rowData)
    {
        if (rowData == null)
            return new (0, 0, 0, 255);

        int red = 0;
        int green = 0;
        int blue = 0;
        int alpha = 0;
        int processed = 0;

        for (int col = StartingX - Reach; col <= Math.Min(rowData.Length - 1, StartingX + Reach); col++)
        {
            if (col < 0 || col >= rowData.Length)  // Fixing out-of-bounds check
                continue;  // Skip invalid pixels instead of adding 255 to alpha

            Rgba32 pixel = rowData[col];
            red += pixel.R;
            green += pixel.G;
            blue += pixel.B;
            alpha += pixel.A;
            processed++;
        }

        // Avoid division by zero
        if (processed == 0)
            return new(0, 0, 0, 255);

        red /= processed;
        green /= processed;
        blue /= processed;
        alpha /= processed;
        
        return new ((byte)red, (byte)green, (byte)blue, (byte)alpha);
    }

    public void ShiftUp(Rgba32[] row = null)
    {
        for (int i = 0; i < Data.Length - 1; i++)
            Data[i] = Data[i + 1];
        Data[^1] = GetAveragePixel(row);
    }

    public Rgba32 Average() => new()
    {
        R = (byte)(int)Data.Average(x => x.R),
        G = (byte)(int)Data.Average(x => x.G),
        B = (byte)(int)Data.Average(x => x.B),
        A = (byte)(int)Data.Average(x => x.A)
    };
}