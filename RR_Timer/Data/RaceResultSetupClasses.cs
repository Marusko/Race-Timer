using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace Race_timer.Data
{
    /// <summary>
    /// What the Create API window needs to open a RaceResult session: which server, over which
    /// scheme, and one of the two ways RaceResult accepts a login - a user and password, or an
    /// API key. Nothing here is stored anywhere, it lives only for the length of one login
    /// </summary>
    public record RaceResultCredentials
    {
        /// <summary>The server events are administered on, e.g. <c>events.raceresult.com</c></summary>
        public string Server { get; init; } = "";

        /// <summary>
        /// Off only for an on-premise server without a certificate. The Simple API key travels in
        /// the path, so a link built from a plain-HTTP login is one the app then warns about
        /// </summary>
        public bool UseHttps { get; init; } = true;

        public string? User { get; init; }

        public string? Password { get; init; }

        public string? ApiKey { get; init; }

        /// <summary>An API key wins when both are filled in, the same way the API Creator does it</summary>
        public bool HasApiKey => !string.IsNullOrWhiteSpace(ApiKey);

        public bool HasUserPassword => !string.IsNullOrWhiteSpace(User) && !string.IsNullOrEmpty(Password);
    }

    /// <summary>
    /// One Simple API entry on the RaceResult event
    /// </summary>
    /// <param name="Label">
    /// Short name, and the key everything is matched by: <see cref="API.LinkHandler.LoadApi"/>
    /// switches on it, and setup finds an entry already on the event by it
    /// </param>
    /// <param name="Url">The endpoint definition, exactly as RaceResult stores it</param>
    public record RaceResultApiEndpoint(string Label, string Url);

    /// <summary>
    /// One row in the API selection dialog. A row is one Simple API entry today, but it carries a
    /// list so a feature needing two of them could be offered as a single switch
    /// </summary>
    /// <param name="Group">Heading the row is listed under, so the rows follow the app's own tabs</param>
    /// <param name="Title">What the entry is called in the UI</param>
    /// <param name="Mandatory">
    /// Mandatory entries are always created - the API timer cannot work without them - so their
    /// checkbox is shown ticked and locked. Optional ones can be turned off
    /// </param>
    /// <param name="Endpoints">The Simple API entries this row creates</param>
    /// <param name="Hint">One line saying what turning the row off costs</param>
    public record RaceResultApiDefinition(
        string Group,
        string Title,
        bool Mandatory,
        IReadOnlyList<RaceResultApiEndpoint> Endpoints,
        string? Hint = null)
    {
        /// <summary>The labels this row covers, as RaceResult shows them</summary>
        public string LabelText => string.Join(", ", Endpoints.Select(e => e.Label));
    }

    /// <summary>
    /// The outcome of one run of setup: the links to copy out, and what it took to get there
    /// </summary>
    public class RaceResultSetupResult
    {
        /// <summary>Link of the <c>api</c> entry - the one pasted into the API Link box</summary>
        public string ApiLink { get; init; } = "";

        /// <summary>
        /// Link of the <c>starts</c> entry, or <c>null</c> when individual starts were not part of
        /// this run. Starts are loaded from their own link, not from the shared one
        /// </summary>
        public string? StartsLink { get; init; }

        public int Created { get; init; }

        public int Corrected { get; init; }

        public int Unchanged { get; init; }

        public bool NothingChanged => Created == 0 && Corrected == 0;

        /// <summary>A sentence saying what happened, shown above the links</summary>
        public string Describe(string eventName)
        {
            if (NothingChanged)
            {
                return $"Event '{eventName}' was already set up, all {Unchanged} API(s) were left as they were.";
            }

            var parts = new List<string>();
            if (Created > 0)
            {
                parts.Add($"{Created} created");
            }
            if (Corrected > 0)
            {
                parts.Add($"{Corrected} corrected");
            }
            if (Unchanged > 0)
            {
                parts.Add($"{Unchanged} unchanged");
            }

            return $"Event '{eventName}': {string.Join(", ", parts)}. "
                   + "The other APIs of the event were left untouched.";
        }
    }

    /// <summary>
    /// One RaceResult event in the picker. The name alone does not tell two events apart - the
    /// same race is run every year, and a test copy sits beside the real one - so the date and
    /// the place are shown under it
    /// </summary>
    public class RaceResultEventItem
    {
        public RaceResultEventItem(string id, string name, string date, string place)
        {
            Id = id;
            Name = string.IsNullOrWhiteSpace(name) ? id : name;
            Date = date ?? "";
            Place = place ?? "";
        }

        /// <summary>The RaceResult event id - what setup is run against</summary>
        public string Id { get; }

        public string Name { get; }

        public string Date { get; }

        public string Place { get; }

        /// <summary>
        /// The line under the name. Not every event names a place, and an empty one should not
        /// leave a stray separator behind
        /// </summary>
        public string Caption => string.IsNullOrWhiteSpace(Place) ? Date : $"{Date} · {Place}";

        /// <summary>Whether this event matches what was typed into the picker's search box</summary>
        public bool Matches(string filter)
        {
            return string.IsNullOrWhiteSpace(filter)
                   || Name.Contains(filter, StringComparison.OrdinalIgnoreCase)
                   || Place.Contains(filter, StringComparison.OrdinalIgnoreCase);
        }
    }

    /// <summary>
    /// One checkbox row in the API selection dialog. Mandatory entries are ticked and locked -
    /// the API timer cannot work without them - so only optional rows can be toggled
    /// </summary>
    public class ApiSelectionItem
    {
        public ApiSelectionItem(RaceResultApiDefinition definition)
        {
            Definition = definition;
            //Everything starts selected, the user unticks what they do not want
            IsSelected = true;
        }

        /// <summary>The catalog row this represents, what setup is handed back for the ticked ones</summary>
        public RaceResultApiDefinition Definition { get; }

        public string Group => Definition.Group;

        public string Title => Definition.Title;

        /// <summary>The Simple API labels this row covers, as RaceResult shows them</summary>
        public string LabelText => Definition.LabelText;

        public string? Hint => Definition.Hint;

        public Visibility HintVisibility =>
            string.IsNullOrWhiteSpace(Hint) ? Visibility.Collapsed : Visibility.Visible;

        public bool IsMandatory => Definition.Mandatory;

        /// <summary>Mandatory rows keep their checkbox ticked but greyed out</summary>
        public bool IsToggleable => !IsMandatory;

        /// <summary>The "Required" badge, shown on the rows that cannot be turned off</summary>
        public Visibility MandatoryVisibility =>
            IsMandatory ? Visibility.Visible : Visibility.Collapsed;

        /// <summary>
        /// Bound two-way from the checkbox. Mandatory rows are disabled in the UI, so this only
        /// ever changes on an optional one
        /// </summary>
        public bool IsSelected { get; set; }
    }
}
