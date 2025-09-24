public class Image
{
    public byte[] Data { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }

    public Image(byte[] data, int w = 0, int h = 0)
    {
        Data = data;
        Width = w;
        Height = h;
    }
}

public class Account
{
    public string Id { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
}

// для каждой задачаи свой интерфейс
public interface IImageDownloader
{
    byte[] Download(string url);
}

public interface IImageSaver
{
    void Save(Image image, string path);
}

public interface IImageEditor
{
    Image Resize(Image image, int width, int height);
    Image InvertColors(Image image);
}

public interface IImageRepository
{
    void SetAccountPicture(Image image, Account account);
}

public interface IDuplicateRemover
{
    int DeleteDuplicates(string directory);
}

// реализации-заглущки

public class SimpleDownloader : IImageDownloader
{
    public byte[] Download(string url)
    {
        Console.WriteLine($"[Downloader] Downloading from {url} (simulated).");
        return new byte[] { 1, 2, 3 };
    }
}

public class FileImageSaver : IImageSaver
{
    public void Save(Image image, string path)
    {
        Console.WriteLine($"[Saver] Saving image to {path} (simulated).");
    }
}

public class BasicEditor : IImageEditor
{
    public Image Resize(Image image, int width, int height)
    {
        Console.WriteLine($"[Resizer] Resizing image {image.Width}x{image.Height} -> {width}x{height} (simulated).");
        return new Image(image.Data, width, height);
    }

    public Image InvertColors(Image image)
    {
        Console.WriteLine("[Processor] Inverting colors (simulated).");
        return new Image((byte[])image.Data.Clone(), image.Width, image.Height);
    }
}

public class DummyRepository : IImageRepository
{
    public void SetAccountPicture(Image image, Account account)
    {
        Console.WriteLine($"[Repository] Set image as account picture for account {account.Id} (simulated).");
    }
}

public class SimpleDuplicateRemover : IDuplicateRemover
{
    public int DeleteDuplicates(string directory)
    {
        Console.WriteLine($"[DuplicateRemover] Scan directory {directory} and delete duplicates (simulated).");
        return 0;
    }
}

// паттерн Фасад — комбинируем мелкие сервисы без выполнения логики
public class ImageService
{
    private readonly IImageDownloader _downloader;
    private readonly IImageSaver _saver;
    private readonly IImageEditor _editor;
    private readonly IImageRepository _repository;
    private readonly IDuplicateRemover _duplicateRemover;

    public ImageService(
        IImageDownloader downloader,
        IImageSaver saver,
        IImageEditor editor,
        IImageRepository repository,
        IDuplicateRemover duplicateRemover)
    {
        _downloader = downloader;
        _saver = saver;
        _editor = editor;
        _repository = repository;
        _duplicateRemover = duplicateRemover;
    }

    public void DownloadResizeSaveAndAssign(string url, int w, int h, string savePath, Account account)
    {
        var bytes = _downloader.Download(url);
        var image = new Image(bytes);
        var resized = _editor.Resize(image, w, h);
        _saver.Save(resized, savePath);
        _repository.SetAccountPicture(resized, account);
    }

    public int CleanupDuplicates(string directory)
    {
        return _duplicateRemover.DeleteDuplicates(directory);
    }

    public Image InvertAndSave(Image image, string path)
    {
        var inverted = _editor.InvertColors(image);
        _saver.Save(inverted, path);
        return inverted;
    }
}

class Program
{
    static void Main()
    {
        var downloader = new SimpleDownloader();
        var saver = new FileImageSaver();
        var editor = new BasicEditor();
        var repo = new DummyRepository();
        var duplicateRemover = new SimpleDuplicateRemover();

        var service = new ImageService(downloader, saver, editor, repo, duplicateRemover);

        var account = new Account { Id = "user123", DisplayName = "Ivan" };
        service.DownloadResizeSaveAndAssign("http://example.com/photo.jpg", 200, 200, "/tmp/photo.jpg", account);

        int removed = service.CleanupDuplicates("/var/images");
        Console.WriteLine($"Removed duplicates: {removed}");

        var img = new Image(new byte[] { 9, 9, 9 }, 100, 100);
        service.InvertAndSave(img, "/tmp/inverted.jpg");
    }
}