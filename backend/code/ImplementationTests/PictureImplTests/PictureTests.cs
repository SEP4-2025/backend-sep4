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
    public void AddPictureAsync_Throws_WhenDtoIsNull()
    {
        PictureDTO? nullDto = null;
        var exception = Assert.ThrowsAsync<Exception>(
            async () => await _pictureLogic.AddPictureAsync(nullDto!)
        );
        Assert.That(exception.Message, Is.EqualTo("Picture data is invalid."));
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
    public void GetPictureById_Throws_WhenNotFound()
    {
        var exception = Assert.ThrowsAsync<Exception>(
            async () => await _pictureLogic.GetPictureById(9999)
        );
        Assert.That(exception.Message, Is.EqualTo("Picture not found."));
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

        var exception = Assert.ThrowsAsync<Exception>(
            async () => await _pictureLogic.GetPictureById(testPicture.Id)
        );
        Assert.That(exception.Message, Is.EqualTo("Picture not found."));
    }

    [Test]
    public void UpdateNote_Throws_WhenNotFound()
    {
        var ex = Assert.ThrowsAsync<Exception>(
            async () => await _pictureLogic.UpdateNote(9999, "note")
        );
        Assert.That(ex.Message, Is.EqualTo("Sensor with ID 9999 not found."));
    }

    [TearDown]
    public void TearDown()
    {
        _context.Pictures.RemoveRange(_context.Pictures);
        _context.SaveChangesAsync();
    }
}
