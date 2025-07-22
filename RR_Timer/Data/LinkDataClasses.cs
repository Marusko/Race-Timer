namespace Race_timer.Data
{
    public record Event
    {
        public string? EventName { get; set; }
        public EventType? EventType { get; set; }

        public string GetFormatedType()
        {
            return EventType.GetValueOrDefault().GetDisplayName();
        }
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

    public record Api
    {
        public bool? Disabled { get; set; }
        public string? Key { get; set; }
        public string? Label { get; set; }
    }
}
