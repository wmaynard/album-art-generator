using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

bool IsImage(string path) => new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".tiff", ".webp" }
    .Contains(Path.GetExtension(path).ToLower());

bool IsAudio(string path) => new[] { ".mp3", ".wav", ".flac", ".ogg", ".aac", ".m4a" }
    .Contains(Path.GetExtension(path).ToLower());
    
const string PLATFORM = "Sega Genesis";
string home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
string SOURCE_DIRECTORY = $"{home}/_TODO/{PLATFORM}";
string TARGET_DIRECTORY = $"{home}/_TODO/{PLATFORM}/__Non-Audio Files";

List<string> Scan(string path)
{
    List<string> output = new();

    foreach (string file in Directory.GetFiles(path))
        if (!IsAudio(file))
            output.Add(file);
    
    // Ignore special directories
    foreach (string dir in Directory.GetDirectories(path))
        if (!Path.GetFileName(dir).StartsWith("__"))
            output.AddRange(Scan(dir));

    return output.Distinct().ToList();
}

void MoveFiles()
{
    string[] files = Scan(SOURCE_DIRECTORY).ToArray();

    foreach (string file in files)
    {
        // Get relative path from SOURCE_DIRECTORY
        string relativePath = Path.GetRelativePath(SOURCE_DIRECTORY, file);
        string targetPath = Path.Combine(TARGET_DIRECTORY, relativePath);

        // Ensure target directory exists
        string targetDir = Path.GetDirectoryName(targetPath);
        if (targetDir != null && !Directory.Exists(targetDir))
            Directory.CreateDirectory(targetDir);

        // Move the file, resolving conflicts
        string finalTarget = ResolveFileConflict(targetPath);
        if (!finalTarget.EndsWith(".DS_Store"))
            File.Move(file, finalTarget);
    }
}

string ResolveFileConflict(string target)
{
    if (!File.Exists(target))
        return target;

    string directory = Path.GetDirectoryName(target) ?? TARGET_DIRECTORY;
    string filename = Path.GetFileNameWithoutExtension(target);
    string extension = Path.GetExtension(target);
    int counter = 1;

    while (File.Exists(target))
        target = Path.Combine(directory, $"{filename} ({++counter}){extension}");

    return target;
}

List<string> FindEmptyDirectories(string path)
{
    List<string> output = new ();

    foreach (string directory in Directory.GetDirectories(path))
    {
        List<string> subdirectories = FindEmptyDirectories(directory);

        // If the directory is empty or contains only empty subdirectories, add it to the list
        if (!Directory.EnumerateFileSystemEntries(directory).Any() || subdirectories.Contains(directory))
            output.Add(directory);

        output.AddRange(subdirectories);
    }

    return output.Distinct().ToList();
}

MoveFiles();
foreach (string file in Scan(SOURCE_DIRECTORY))
    if (file.EndsWith(".DS_Store"))
        File.Delete(file);
foreach (string directory in FindEmptyDirectories(SOURCE_DIRECTORY))
    Directory.Delete(directory, true);

// TODO: Delete .DS_Store files
// TODO: Delete empty directories
Console.WriteLine("Files moved successfully!");