using Database;
using DTOs;
using Entities;
using LogicImplements;
using LogicInterfaces;

namespace ImplementationTests.GardenerImplTests;

public class GardenerTests
{
    private AppDbContext _context;
    private IGardenerInterface _gardenerLogic;

    [SetUp]
    public void Setup()
    {
        _context = TestSetup.Context;
        _gardenerLogic = new GardenerLogic(_context);
    }

    [Test]
    public async Task GetGardenersAsync_Success_ReturnsAllGardenersCorrectly()
    {
        await GardenerSeeder.SeedGardenerAsync();

        var gardenerList = await _gardenerLogic.GetGardeners();

        Assert.IsNotNull(gardenerList);
        Assert.That(gardenerList.Count, Is.EqualTo(1));
    }

    [Test]
    public async Task GetGardenersByIdAsync_Success_ReturnsCorrectGardener()
    {
        var testGardener = await GardenerSeeder.SeedGardenerAsync();

        var addedGardener = await _gardenerLogic.GetGardenerByIdAsync(testGardener.Id);

        Assert.IsNotNull(addedGardener);
        Assert.That(addedGardener.Username, Is.EqualTo(testGardener.Username));
    }

    [Test]
    public void GetGardenerByIdAsync_Throws_WhenNotFound()
    {
        var exception = Assert.ThrowsAsync<KeyNotFoundException>(
            async () => await _gardenerLogic.GetGardenerByIdAsync(9999)
        );
        Assert.That(exception.Message, Is.EqualTo("Gardener with id 9999 not found."));
    }

    [Test]
    public async Task AddGardenerAsync_Success_AddsUserCorrectly()
    {
        var gardenerDto = new GardenerDTO { Username = "newGardener", Password = "newPassword" };

        var testGardener = await _gardenerLogic.AddGardenerAsync(gardenerDto);
        var addedGardener = await _gardenerLogic.GetGardenerByIdAsync(testGardener.Id);

        Assert.IsNotNull(addedGardener);
        Assert.That(gardenerDto.Username, Is.EqualTo(addedGardener.Username));
    }

    [Test]
    public async Task UpdateGardenerAsync_Success_UpdatesUsernameCorrectly()
    {
        var oldGardener = await GardenerSeeder.SeedGardenerAsync();
        var newGardener = new GardenerDTO { Username = "newGardener", Password = "123456" };

        await _gardenerLogic.UpdateGardenerAsync(oldGardener.Id, newGardener);

        var updatedGardener = await _gardenerLogic.GetGardenerByIdAsync(oldGardener.Id);

        Assert.IsNotNull(updatedGardener);
        Assert.That(updatedGardener.Username, Is.EqualTo(newGardener.Username));
    }

    [Test]
    public void UpdateGardenerAsync_Throws_WhenNotFound()
    {
        var updateGardener = new GardenerDTO { Username = "doesnotexist" };

        var exception = Assert.ThrowsAsync<KeyNotFoundException>(
            async () => await _gardenerLogic.UpdateGardenerAsync(9999, updateGardener)
        );
        Assert.That(exception.Message, Is.EqualTo("Gardener with id 9999 not found."));
    }

    [Test]
    public async Task DeleteGardenerAsync_Success_DeletesUser()
    {
        var testGardener = await GardenerSeeder.SeedGardenerAsync();

        await _gardenerLogic.DeleteGardenerAsync(testGardener.Id);
        var gardenerList = await _gardenerLogic.GetGardeners();

        Assert.IsEmpty(gardenerList);
    }

    [TearDown]
    public void TearDown()
    {
        _context.Gardeners.RemoveRange(_context.Gardeners.ToList());
        _context.SaveChanges();
    }
}
