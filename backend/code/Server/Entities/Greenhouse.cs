namespace Entities;

public class Greenhouse
{
    public int Id { get; set; }
    public string Name { get; set; }

    public Plant Plant { get; set; }
    public Gardener Gardener { get; set; }
    public Log HistoryLog { get; set; }
}
