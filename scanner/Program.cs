bool IsImage(string path) => new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".tiff", ".webp" }
    .Contains(Path.GetExtension(path).ToLower());

const string PLATFORM = "Sega Genesis";
string home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
string SOURCE_DIRECTORY = $"{home}/_TODO/{PLATFORM}";
string TARGET_DIRECTORY = $"{home}/Pictures/Box Art/Raw/{PLATFORM}";

List<string> Scan(string path)
{
    List<string> output = Directory.GetFiles(path).ToList();
    foreach (string dir in Directory.GetDirectories(path))
        output.AddRange(Scan(dir));
    return output.Where(IsImage).Distinct().ToList();
}

foreach (string image in Scan(TARGET_DIRECTORY))
    File.Delete(image);

string[] images = Scan(SOURCE_DIRECTORY).ToArray();
foreach (string image in images)
{
    string target = image[(image.IndexOf(PLATFORM, StringComparison.Ordinal) + PLATFORM.Length + 1)..];
    target = $"{TARGET_DIRECTORY}/{target.Replace('/', '-')}";

    string directory = Path.GetDirectoryName(target) ?? TARGET_DIRECTORY;
    string filename = Path.GetFileNameWithoutExtension(target);
    string extension = Path.GetExtension(target);
    int counter = 1;

    while (File.Exists(target))
        target = Path.Combine(directory, $"{filename} ({++counter}){extension}");

    File.Copy(image, target);
}

Console.Write($"{images.Length} images found and copied to {TARGET_DIRECTORY}.");