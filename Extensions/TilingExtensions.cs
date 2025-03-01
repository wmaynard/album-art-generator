using Maynard.AlbumArt.Utilities;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Maynard.AlbumArt.Extensions;

public static class TilingExtensions
{
    /// <summary>
    /// Chops up an image into a collection of tiles.  This is used for special effects such as the Spotify() method.
    /// </summary>
    /// <param name="self">The original image.</param>
    /// <param name="tileSize">The size, in pixels, of each tile's side.</param>
    /// <returns>A Queue of rows of tiled images.</returns>
    internal static Queue<List<Image<Rgba32>>> SliceAndDice(this Image<Rgba32> self, int tileSize)
    {
        Queue<List<Image<Rgba32>>> sliced = new();
        
        self.ProcessPixelRows(image =>
        {
            int processedX = 0;
            int processedY = 0;
            
            List<Image<Rgba32>> tileRow = new();
            while (processedX < self.Width && processedY < self.Height)
            {
                Image<Rgba32> tile = new(Math.Min(tileSize, image.Width - processedX), Math.Min(tileSize, image.Height - processedY));

                Queue<Rgba32> data = new();
                for (int y = processedY; y < Math.Min(image.Height, processedY + tileSize); y++)
                {
                    Span<Rgba32> row = image.GetRowSpan(y);
                    for (int x = processedX; x < Math.Min(row.Length, processedX + tileSize); x++)
                        data.Enqueue(row[x]);
                }
                tile.Load(data);
                tileRow.Add(tile);
                processedX += tileSize;
                if (processedX >= self.Width)
                {
                    processedX = 0;
                    processedY += tileSize;
                    sliced.Enqueue(tileRow);
                    tileRow = new();
                }
            }
        });
        
        return sliced;
    }

    /// <summary>
    /// The counterpart to SliceAndDice().  This method reverses the slicing and stitches all the tiles together again
    /// to recreate an image.
    /// </summary>
    /// <param name="tileRows">The output from SliceAndDice() after whatever processing was performed.</param>
    /// <returns>The modified image after processing each tile.</returns>
    internal static Image<Rgba32> Stitch(Queue<List<Image<Rgba32>>> tileRows)
    {
        int width = 0;
        int totalHeight = 0;
        Queue<Rgba32> data;
        Queue<Rgba32> finalData = new();
        while (tileRows.Any())
        {
            List<Image<Rgba32>> row = tileRows.Dequeue();
            width = row.Sum(image => image.Width);
            int height = row.First().Height;
            totalHeight += height;
            Image<Rgba32> bar = new(width, height);
            data = new();
            for (int y = 0; y < height; y++)
                foreach (Image<Rgba32> tile in row)
                    tile.ProcessPixelRows(image =>
                    {
                        foreach (Rgba32 pixel in image.GetRowSpan(y))
                        {
                            data.Enqueue(pixel);
                            finalData.Enqueue(pixel);
                        }
                    });
            bar.Load(data);
        }
        Image<Rgba32> output = new(width, totalHeight);
        output.Load(finalData);

        return output;
    }

    /// <summary>
    /// Splits an image into tiles, performs a specific action on each as a separate image, then stitches the tiles
    /// back together into a composite image.
    /// </summary>
    /// <param name="self">The original image.</param>
    /// <param name="tileSize">The size of each tile in pixels.  Must be greater than 1 to have any effect.</param>
    /// <param name="action">The processing to take place on each tile.</param>
    /// <returns>The processed image for chaining.</returns>
    internal static Image<Rgba32> ProcessTiles(this Image<Rgba32> self, int tileSize, Func<Image<Rgba32>, Image<Rgba32>> action)
    {
        if (tileSize <= 1)
        {
            Log.Warn("Tile size is less than or equal to 1; values of 1 pixel are not allowed.");
            return self;
        }

        Queue<List<Image<Rgba32>>> rows = self.SliceAndDice(tileSize);

        for (int i = 0; i < rows.Count; i++)
        {
            List<Image<Rgba32>> row = rows.Dequeue();
            for (int j = 0; j < row.Count; j++)
                row[j] = action(row[j]);
            rows.Enqueue(row);
        }

        return self = Stitch(rows);
    }
    
    /// <summary>
    /// Copies an image, splits it into tiles, performs a specific action on each as a separate image, then stitches the
    /// tiles back together into a composite image.
    /// </summary>
    /// <param name="self">The original image.</param>
    /// <param name="tileSize">The size of each tile in pixels.  Must be greater than 1 to have any effect.</param>
    /// <param name="action">The processing to take place on each tile.</param>
    /// <param name="processed">The processed image.</param>
    /// <returns>The processed copy of an image for chaining.</returns>
    internal static Image<Rgba32> ProcessTiles(this Image<Rgba32> self, int tileSize, Func<Image<Rgba32>, Image<Rgba32>> action, out Image<Rgba32> processed)
        => processed = self.Clone().ProcessTiles(tileSize, action);
}