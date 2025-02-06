using System.IO;
using System.Threading.Tasks;

namespace NorthwindStore.BL.Services;

public class ImageService
{
    private readonly string applicationPath;

    public ImageService(string applicationPath)
    {
        this.applicationPath = applicationPath;
    }

    public Task<Stream> GetCategoryImage(int categoryId)
    {
        var picturePath = Path.Combine(applicationPath, $"wwwroot/images/categories/{categoryId}.bmp");
        return Task.FromResult<Stream>(File.OpenRead(picturePath));
    }

    public async Task SaveCategoryImage(int categoryId, Stream stream)
    {
        var picturePath = Path.Combine(applicationPath, $"wwwroot/images/categories/{categoryId}.bmp");
        await using var fs = File.OpenWrite(picturePath);
        await stream.CopyToAsync(fs);
    }

}