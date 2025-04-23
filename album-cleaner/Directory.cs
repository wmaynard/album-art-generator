namespace album_cleaner;

public class Directory
{
    private const string ALBUM_ART_PATH = "__Album Art";
    private const string BOX_ART_PATH = "__Box Art";
    private const string OTHER_FILES_PATH = "__Extras";
    
    public static readonly string USER_HOME = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
    public bool Exists => SysDir.Exists(Path);
    public bool IsEmpty => TotalFileCount == 0;
    public string Path { get; set; }
    public string Name { get; set; }
    public Directory Parent { get; set; }
    public Directory[] Directories { get; set; }
    public File[] Files { get; set; }
    
    public File[] AudioFiles => AllFiles
        .Where(x => x.IsAudio)
        .Where(x => 
            !x.Path.Contains(ALBUM_ART_PATH)
            && !x.Path.Contains(BOX_ART_PATH)
            && !x.Path.Contains(OTHER_FILES_PATH)
        )
        .ToArray();
    public File[] NonAudioFiles => AllFiles
        .Where(x => !x.IsAudio)
        .Where(x => 
            !x.Path.Contains(ALBUM_ART_PATH)
            && !x.Path.Contains(BOX_ART_PATH)
            && !x.Path.Contains(OTHER_FILES_PATH)
        )
        .ToArray();
    public File[] ImageFiles => AllFiles
        .Where(x => x.IsImage)
        .Where(x => 
            !x.Path.Contains(ALBUM_ART_PATH)
            && !x.Path.Contains(BOX_ART_PATH)
            && !x.Path.Contains(OTHER_FILES_PATH)
        )
        .ToArray();
    public File[] Extras => AllFiles
        .Where(x => !x.IsAudio)
        .Where(x => 
            !x.Path.Contains(ALBUM_ART_PATH)
            && !x.Path.Contains(BOX_ART_PATH)
            && !x.Path.Contains(OTHER_FILES_PATH)
        )
        .ToArray();

    public File[] AllFiles
    {
        get
        {
            List<File> output = new();
            if (Directories.Any())
                output.AddRange(Directories.SelectMany(dir => dir.AllFiles));
            output.AddRange(Files);
            return output.ToArray();
        }
    }

    public Directory[] AllDirectories
    {
        get
        {
            List<Directory> output = new();
            if (Directories.Any())
                output.AddRange(Directories.SelectMany(dir => dir.AllDirectories));
            output.AddRange(Directories);
            return output.ToArray();
        }
    }
    
    public int ImmediateFileCount { get; set; }
    public int ImmediateDirectoryCount { get; set; }
    public int TotalFileCount { get; set; }
    public int TotalDirectoryCount { get; set; }

    public Directory(string path)
    {
        Path = path;
        Name = System.IO.Path.GetFileName(path);
        Update();
    }

    private void Update()
    {
        if (!Exists)
        {
            Directories = Array.Empty<Directory>();
            Files = Array.Empty<File>();
            return;
        }
        Directories = SysDir
            .GetDirectories(Path)
            .Select(sysdir => new Directory(sysdir))
            .Where(dir => dir.Name != ".DS_Store")
            .ToArray();
        foreach (Directory dir in Directories)
            dir.Parent = this;
        Files = SysDir
            .GetFiles(Path)
            .Select(sysfile => new File(sysfile))
            .ToArray();

        ImmediateDirectoryCount = Directories.Length;
        ImmediateFileCount = Files.Length;
        TotalDirectoryCount = ImmediateDirectoryCount > 0
            ? Directories.Sum(dir => dir.TotalDirectoryCount) + ImmediateDirectoryCount
            : ImmediateDirectoryCount;
        TotalFileCount = ImmediateDirectoryCount > 0
            ? Directories.Sum(dir => dir.TotalFileCount) + ImmediateFileCount
            : ImmediateFileCount;
    }

    private void Create() => SysDir.CreateDirectory(Path);
    public Directory Create(string relativePath)
    {
        Directory target = FromRelative(relativePath);
        target.Create();
        return target;
    }

    public void CreateSpecialDirectories(out Directory albumArt, out Directory boxArt, out Directory extras)
    {
        albumArt = Create(ALBUM_ART_PATH);
        boxArt = Create(BOX_ART_PATH);
        extras = Create(OTHER_FILES_PATH);
    }

    private Directory[] GetAllDirectories()
    {
        List<Directory> output = new();
        foreach (Directory dir in Directories)
            output.AddRange(dir.GetAllDirectories());
        if (Directories.Any())
            output.AddRange(Directories);
        return output.ToArray();
    }

    public Directory FromRelative(string relativePath)
    {
        string path = System.IO.Path.Combine(Path, relativePath);
        Directory[] subs = GetAllDirectories();

        return subs.FirstOrDefault(dir => dir.Path == path)
            ?? new Directory(path) { Parent = this };
    }
    public static Directory FromRelativeToUserDirectory(string relativePath) => new (System.IO.Path.Combine(USER_HOME, relativePath));

    
    private string ResolveFileConflict(string target)
    {
        if (!SysFile.Exists(target))
            return target;

        string directory = System.IO.Path.GetDirectoryName(target);
        string filename = System.IO.Path.GetFileNameWithoutExtension(target);
        string extension = System.IO.Path.GetExtension(target);
        int counter = 1;

        while (SysFile.Exists(target))
            target = System.IO.Path.Combine(directory, $"{filename} ({++counter}){extension}");

        return target;
    }
    public File[] CollectImagesFrom(Directory album)
    {
        List<File> output = new();
        File[] targets = album.ImageFiles;
        foreach (File file in targets)
        {
            string newFilename = file.Path.Replace($"{album.Path}/", string.Empty).Replace('/', '-');
            string newPath = ResolveFileConflict(System.IO.Path.Combine(Path, newFilename));
            
            SysFile.Copy(file.Path, newPath);
            output.Add(new(newPath));
        }
        album.Update();
        return output.ToArray();
    }

    public File[] CollectExtrasFrom(Directory album, string replacePath = null)
    {
        replacePath ??= album.Path;
        List<File> output = new();
        File[] extras = album.Extras;
        foreach (File file in extras)
        {
            string relativePath = file.Path.Replace($"{replacePath}/", string.Empty);
            string newPath = System.IO.Path.Combine(Path, relativePath);
            string targetDir = new FileInfo(newPath).Directory.FullName;
            if (!SysDir.Exists(targetDir))
                SysDir.CreateDirectory(targetDir);
            if (!SysFile.Exists(newPath))
                try
                {
                    SysFile.Move(file.Path, newPath);
                }
                catch (IOException e)
                {
                    if (!e.Message.Contains("already exists"))
                        throw;
                }
        }
        album.Update();
        return output.ToArray();
    }

    public int DeleteEmptyDirectories()
    {
        Directory[] toDelete = AllDirectories
            .Where(dir => dir.IsEmpty)
            .Where(dir => !dir.Path.Contains(ALBUM_ART_PATH))
            .OrderByDescending(dir => dir.Path.Length)
            .ToArray();
        foreach (Directory dir in toDelete)
            if (dir.Exists)
                SysDir.Delete(dir.Path);
        Update();
        return toDelete.Length;
    }

    public int DeleteTxtDsStoreFiles()
    {
        File[] files = AllFiles.Where(file => file.Path.EndsWith(".DS_Store") || file.Path.EndsWith(".txt")).ToArray();
        foreach (File file in files)
            SysFile.Delete(file.Path);
        Update();
        return files.Length;
    }

    public override string ToString() => Path.Replace(USER_HOME, string.Empty);
}