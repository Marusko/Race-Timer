namespace Race_timer.Data
{
    public record Event
    {
        public string? EventName { get; set; }
        public string? EventType { get; set; }
    }

    public record Racer
    {
        public string? DisplayName { get; set; }
        public string? Bib { get; set; }
    }

    public record Contest
    {
        public string? Name { get; set; }
        public int? StartTime { get; set; }
    }
}
