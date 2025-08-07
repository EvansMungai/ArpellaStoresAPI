namespace ArpellaStores.Extensions;

public class BulkModelValidator
{
    public int RowNumber { get; set; }
    public Dictionary<string, List<string>> Errors { get; set; } = new Dictionary<string, List<string>>();
}
