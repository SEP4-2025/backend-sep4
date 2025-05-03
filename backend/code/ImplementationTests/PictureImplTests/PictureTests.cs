using Database;
using DTOs;
using Entities;
using LogicImplements;
using LogicInterfaces;

namespace ImplementationTests.PictureImplTests;

public class PictureTests
{
    private AppDbContext _context;
    private IPictureInterface _pictureLogic;

    [SetUp]
    public void Setup()
    {
        _context = TestSetup.Context;
        _pictureLogic = new PictureLogic(_context);
    }

    [Test]
    public async Task GetPictureById_Success_ReturnsCorrectPicture()
    {
        var testPicture = await PictureSeeder.SeedPictureAsync();

        var result = await _pictureLogic.GetPictureById(testPicture.Id);

        Assert.IsNotNull(result);
        Assert.That(result.Url, Is.EqualTo(testPicture.Url));
        Assert.That(result.Note, Is.EqualTo(testPicture.Note));
    }

    [Test]
    public async Task GetPictureByPlantIdAsync_Success_ReturnsCorrectPictures()
    {
        const int plantId = 5;
        await PictureSeeder.SeedPictureAsync(plantId);
        await PictureSeeder.SeedPictureAsync(plantId);

        var result = await _pictureLogic.GetPictureByPlantIdAsync(plantId);

        Assert.IsNotNull(result);
        Assert.That(result.Count, Is.EqualTo(2));
        Assert.That(result.All(p => p.PlantId == plantId), Is.True);
    }

    [Test]
    public async Task AddPictureAsync_Success_AddsPictureCorrectly()
    {
        var picture = new PictureDTO
        {
            Url = "http://new-image.com/test.jpg",
            Note = "New test picture",
            PlantId = 3
        };

        var result = await _pictureLogic.AddPictureAsync(picture);

        Assert.IsNotNull(result);
        Assert.That(result.Id, Is.GreaterThan(0));
        Assert.That(result.Url, Is.EqualTo(picture.Url));
        Assert.That(result.Note, Is.EqualTo(picture.Note));
        Assert.That(result.PlantId, Is.EqualTo(picture.PlantId));
    }

    [Test]
    public async Task UpdateNote_Success_UpdatesNoteCorrectly()
    {
        var testPicture = await PictureSeeder.SeedPictureAsync();
        var newNote = "Updated picture note";

        var result = await _pictureLogic.UpdateNote(testPicture.Id, newNote);

        Assert.IsNotNull(result);
        Assert.That(result.Note, Is.EqualTo(newNote));
    }

    [Test]
    public async Task DeletePictureAsync_Success_DeletesPicture()
    {
        var testPicture = await PictureSeeder.SeedPictureAsync();

        await _pictureLogic.DeletePictureAsync(testPicture.Id);

        var result = await _pictureLogic.GetPictureById(testPicture.Id);
        Assert.IsNull(result);
    }

    [TearDown]
    public void TearDown()
    {
        _context.Pictures.RemoveRange(_context.Pictures);
        _context.SaveChangesAsync();
    }
}