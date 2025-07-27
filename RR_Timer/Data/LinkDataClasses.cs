namespace Race_timer.Data
{
    /// <summary>
    /// Structure for data loaded from event API link
    /// </summary>
    public record Event
    {
        public string? EventName { get; set; }
        public EventType? EventType { get; set; }

        /// <summary>
        /// Method for retrieving localized name of event type based on resource dictionary
        /// </summary>
        /// <returns>Localized event type</returns>
        public string GetFormatedType()
        {
            return EventType.GetValueOrDefault().GetDisplayName();
        }
    }

    /// <summary>
    /// Structure for data loaded from starts API link
    /// </summary>
    public record StartTime
    {
        public string? Bib { get; set; }
        public string? Name { get; set; }
        public string? Time { get; set; }
    }

    /// <summary>
    /// Structure for data loaded from contest API link
    /// </summary>
    public record Contest
    {
        public string? Name { get; set; }
        public int? StartTime { get; set; }
    }

    /// <summary>
    /// Structure for APIs loaded from all API link
    /// </summary>
    public record Api
    {
        public bool? Disabled { get; set; }
        public string? Key { get; set; }
        public string? Label { get; set; }
    }
}
