using Maynard.AlbumArt.Models;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace Maynard.AlbumArt.Utilities;

public static class ImageRgba32Extensions
{
    public static Image<Rgba32> ScaleToMaxDimension(this Image<Rgba32> self)
    {
        Image<Rgba32> output = self.Width == self.Height ? null : self.Clone();
        if (output == null)
            return output;
        
        float scale = (float)Math.Max(output.Width, output.Height) / Math.Min(output.Width, output.Height);
        int width = (int)(output.Width * scale);
        int height = (int)(output.Height * scale);
        output.Mutate(img => img.Resize(width, height));
        return output;
    }
    
    /// <summary>
    /// Creates a scaled copy of the image using the maximum dimension as the new minimum dimension.
    /// For example, if an image is 100x200px, the new image will be 200x400px.
    /// </summary>
    /// <param name="self">The original image.</param>
    /// <param name="scaled">The scaled copy of the original image.</param>
    /// <returns>The scaled copy of the original image for chaining.</returns>
    public static Image<Rgba32> ScaleToMaxDimension(this Image<Rgba32> self, out Image<Rgba32> scaled)
    {
        scaled = self.Width == self.Height ? null : self.Clone();
        if (scaled == null)
            return scaled;
        
        float scale = (float)Math.Max(scaled.Width, scaled.Height) / Math.Min(scaled.Width, scaled.Height);
        int width = (int)(scaled.Width * scale);
        int height = (int)(scaled.Height * scale);
        scaled.Mutate(img => img.Resize(width, height));
        return scaled;
    }
    
    /// <summary>
    /// Blurs an image using linear averages.
    /// </summary>
    /// <param name="self">The original image.</param>
    /// <param name="strength">The pixel radius to consider for averaging.</param>
    /// <param name="blurred">The blurred copy of the original image.</param>
    /// <returns>The blurred copy of the original image for chaining.</returns>
    public static Image<Rgba32> Blur(this Image<Rgba32> self, int strength, out Image<Rgba32> blurred)
    {
        List<Rgba32[]> rows = new();
        self.ProcessPixelRows(image =>
        {
            for (int y = 0; y < image.Height; y++)
                rows.Add(image.GetRowSpan(y).ToArray());
        });

        Queue<Rgba32> data = new();
        Queue<NeighboringPixels> spotlight = new();
        
        for (int y = 0; y < rows.Count; y++)
            for (int x = 0; x < rows[y].Length; x++)
            {
                NeighboringPixels neighbors = y == 0
                    ? new(ref rows, x, y, strength)
                    : spotlight.Dequeue();
                data.Enqueue(neighbors.Average());
                neighbors.ShiftUp(y + strength < rows.Count
                    ? rows[y + strength]
                    : null
                );
                spotlight.Enqueue(neighbors);
            }

        blurred = new(self.Width, self.Height);
        blurred.ProcessPixelRows(image =>
        {
            for (int y = 0; y < image.Height; y++)
            {
                Span<Rgba32> row = image.GetRowSpan(y);
                for (int x = 0; x < row.Length; x++)
                    row[x] = data.Dequeue();
            }
        });
        return blurred;
    }
    
    /// <summary>
    /// Blurs an image using linear averages.
    /// </summary>
    /// <param name="self">The original image.</param>
    /// <param name="strength">The pixel radius to consider for averaging.</param>
    /// <param name="blurred">The blurred copy of the original image.</param>
    /// <returns>The blurred copy of the original image for chaining.</returns>
    public static Image<Rgba32> NewBlur(this Image<Rgba32> self, int strength, out Image<Rgba32> blurred)
    {
        List<Rgba32[]> rows = new();
        self.ProcessPixelRows(image =>
        {
            for (int y = 0; y < image.Height; y++)
                rows.Add(image.GetRowSpan(y).ToArray());
        });

        Queue<Rgba32> data = new();
        Queue<AveragingWindow> spotlight = new();
        
        for (int y = 0; y < rows.Count; y++)
        for (int x = 0; x < rows[y].Length; x++)
        {
            AveragingWindow neighbors = y == 0
                ? new(ref rows, x, y, strength)
                : spotlight.Dequeue();
            data.Enqueue(neighbors.Average());
            neighbors.ShiftUp(y + strength < rows.Count
                ? rows[y + strength]
                : null
            );
            spotlight.Enqueue(neighbors);
        }

        blurred = new(self.Width, self.Height);
        blurred.ProcessPixelRows(image =>
        {
            for (int y = 0; y < image.Height; y++)
            {
                Span<Rgba32> row = image.GetRowSpan(y);
                for (int x = 0; x < row.Length; x++)
                    row[x] = data.Dequeue();
            }
        });
        return blurred;
    }
    
    /// <summary>
    /// Crops an image to a square area.  The resulting image will trim from two sides of the original image, leaving a
    /// window focused on the central pixel.
    /// </summary>
    /// <param name="self">The original image.</param>
    /// <param name="cropped">The cropped copy of the original image.</param>
    /// <returns>The cropped copy of the original image for chaining.</returns>
    public static Image<Rgba32> CropToSquare(this Image<Rgba32> self, out Image<Rgba32> cropped)
    {
        cropped = self.Width == self.Height ? self.Clone() : null;
        if (cropped != null)
            return cropped;

        int dimension = Math.Min(self.Width, self.Height);
        cropped = new(dimension, dimension);

        Queue<Rgba32> data = new();
        self.ProcessPixelRows(image =>
        {
            int remainingRows = dimension;
            
            bool landscape = image.Width > image.Height;
            bool portrait = !landscape;
            
            int y = landscape ? 0 : (image.Height - image.Width) / 2;

            while (remainingRows-- > 0)
            {
                Span<Rgba32> row = image.GetRowSpan(y++);
                int x = portrait ? 0 : (image.Width - image.Height) / 2;

                for (int remaining = dimension; remaining > 0; remaining--)
                    data.Enqueue(row[x++]);
            }
        });
        
        cropped.ProcessPixelRows(image =>
        {
            for (int y = 0; y < image.Height; y++)
            {
                Span<Rgba32> row = image.GetRowSpan(y);
                for (int x = 0; x < row.Length; x++)
                    row[x] = data.Dequeue();
            }
        });
        return cropped;
    }
    
    /// <summary>
    /// Adds a radial dim to an image, strongest on the outside.
    /// </summary>
    /// <param name="self">The original image.</param>
    /// <param name="strength">The percentage of darkening you want to occur, values 0-200.  At a value of 200,
    /// the entire image will be black.</param>
    /// <param name="dimmed">The dimmed copy of the image.</param>
    /// <returns>The dimmed copy of the image for chaining.</returns>
    public static Image<Rgba32> Dim(this Image<Rgba32> self, int strength, out Image<Rgba32> dimmed)
    {
        dimmed = self.Clone();
        int maxX = dimmed.Width / 2;
        int maxY = dimmed.Height / 2;
        double maxDistance = Math.Sqrt(maxX * maxX + maxY * maxY);
        
        dimmed.ProcessPixelRows(image =>
        {
            for (int row = 0; row < image.Height; row++)
            {
                Span<Rgba32> pixels = image.GetRowSpan(row);
                for (int col = 0; col < pixels.Length; col++)
                {
                    int x = Math.Abs(image.Width / 2 - row);
                    int y = Math.Abs(image.Height / 2 - col);
                    double distance = Math.Sqrt(x * x + y * y);
                    
                    double dimPercentage = Math.Max(0, 1 - (strength / 100.0) * (distance / maxDistance));

                    Rgba32 pixel = pixels[col];
                    Rgba32 replacement = new()
                    {
                        R = (byte)(int)(pixel.R * dimPercentage),
                        G = (byte)(int)(pixel.G * dimPercentage),
                        B = (byte)(int)(pixel.B * dimPercentage),
                        A = pixel.A
                    };
                    pixels[col] = replacement;
                }
            }
        });
        return dimmed;
    }
    /// <summary>
    /// Layers an image on top of the current one.  The image will be centered vertically or horizontally.
    /// TODO: Test what happens when superimpose is used with an image larger than the source.
    /// </summary>
    /// <param name="self">The original image.</param>
    /// <param name="other">The image to be superimposed.</param>
    /// <param name="superimposed">The combined image.</param>
    /// <returns>The combined image for chaining.</returns>
    public static Image<Rgba32> Superimpose(this Image<Rgba32> self, Image<Rgba32> other, out Image<Rgba32> superimposed)
    {
        bool sameDimensions = other.Width == self.Width && other.Height == self.Height;
        superimposed = (sameDimensions ? other : self).Clone();
        if (sameDimensions)
            return superimposed;

        Queue<Rgba32> data = other.Enqueue();
        superimposed.ProcessPixelRows(image =>
        {
            int startX = Math.Abs(self.Width - other.Width) / 2;
            int startY = Math.Abs(self.Height - other.Height) / 2;

            for (int y = startY; y < Math.Min(image.Height, startY + other.Height); y++)
            {
                Span<Rgba32> row = image.GetRowSpan(y);
                int remaining = other.Width;
                for (int x = startX; x < row.Length && remaining-- > 0; x++)
                {
                    if (data.Count == 0)
                        System.Diagnostics.Debugger.Break();
                    row[x] = data.Dequeue(); // TODO
                }
            }
        });

        return superimposed;
    }

    /// <summary>
    /// Chops up an image into a collection of tiles.  This is used for special effects such as the Spotify() method.
    /// </summary>
    /// <param name="self">The original image.</param>
    /// <param name="tileSize">The size, in pixels, of each tile's side.</param>
    /// <returns>A Queue of rows of tiled images.</returns>
    private static Queue<List<Image<Rgba32>>> SliceAndDice(this Image<Rgba32> self, int tileSize)
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
    private static Image<Rgba32> Stitch(Queue<List<Image<Rgba32>>> tileRows)
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
            {
                foreach (Image<Rgba32> tile in row)
                {
                    tile.ProcessPixelRows(image =>
                    {
                        foreach (Rgba32 pixel in image.GetRowSpan(y))
                        {
                            data.Enqueue(pixel);
                            finalData.Enqueue(pixel);
                        }
                    });
                }
            }
            bar.Load(data);
        }
        Image<Rgba32> output = new(width, totalHeight);
        output.Load(finalData);

        return output;
    }
    
    /// <summary>
    /// Processes "tiles" of an image and adds a radial dim to each one before stitching them back together.
    /// </summary>
    /// <param name="self">The original image.</param>
    /// <param name="tileSize">The size of each tile in pixels.  Must be greater than 1 to have any effect.</param>
    /// <param name="spotted">The processed image.</param>
    /// <returns>The processed image for chaining.</returns>
    public static Image<Rgba32> Spotify(this Image<Rgba32> self, int tileSize, out Image<Rgba32> spotted)
        => spotted = self.ProcessTiles(tileSize, image => image.Dim(150, out _), out _);

    /// <summary>
    /// Splits an image into tiles, performs a specific action on each as a separate image, then stitches the tiles
    /// back together into a composite image.
    /// </summary>
    /// <param name="self">The original image.</param>
    /// <param name="tileSize">The size of each tile in pixels.  Must be greater than 1 to have any effect.</param>
    /// <param name="action">The processing to take place on each tile.</param>
    /// <param name="processed">The processed image.</param>
    /// <returns>The processed image for chaining.</returns>
    private static Image<Rgba32> ProcessTiles(this Image<Rgba32> self, int tileSize, Func<Image<Rgba32>, Image<Rgba32>> action, out Image<Rgba32> processed)
    {
        if (tileSize <= 1)
            return processed = self.Clone();
        Queue<List<Image<Rgba32>>> rows = self.SliceAndDice(tileSize);

        for (int i = 0; i < rows.Count; i++)
        {
            List<Image<Rgba32>> row = rows.Dequeue();
            for (int j = 0; j < row.Count; j++)
                row[j] = action(row[j]);
            rows.Enqueue(row);
        }

        return processed = Stitch(rows);
    }
    
    /// <summary>
    /// Converts an image into a queue of individual pixels.  It's up to the consumer of this method to repack them
    /// into an image of correct dimensions.
    /// </summary>
    /// <param name="self">The original image.</param>
    /// <returns>A queue of individual pixels starting from the top left.</returns>
    private static Queue<Rgba32> Enqueue(this Image<Rgba32> self)
    {
        Queue<Rgba32> output = new();
        self.ProcessPixelRows(image =>
        {
            for (int y = 0; y < image.Height; y++)
            {
                Span<Rgba32> row = image.GetRowSpan(y);
                foreach (Rgba32 pixel in row)
                    output.Enqueue(pixel);
            }
        });
        return output;
    }

    /// <summary>
    /// Counterpart to Enqueue(), this will take an image and load pixels data into it.
    /// </summary>
    /// <param name="self">The image to load pixels into.</param>
    /// <param name="pixels">The pixels to load.</param>
    private static void Load(this Image<Rgba32> self, Queue<Rgba32> pixels)
    {
        self.ProcessPixelRows(image =>
        {
            for (int y = 0; y < image.Height; y++)
            {
                Span<Rgba32> row = image.GetRowSpan(y);
                for (int x = 0; x < row.Length; x++)
                    row[x] = pixels.Dequeue();
            }
        });
    }
}