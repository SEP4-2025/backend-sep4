using Entities;

namespace ImplementationTests.PictureImplTests;

public static class PictureSeeder
{
    public static async Task<Picture> SeedPictureAsync(int plantId = 1)
    {
        var context = TestSetup.Context;

        var picture = new Picture
        {
            Url = "http://test-url.com/image.jpg",
            Note = "Test picture note",
            TimeStamp = DateTime.Now,
            PlantId = plantId
        };

        await context.Pictures.AddAsync(picture);
        await context.SaveChangesAsync();

        return picture;
    }
}