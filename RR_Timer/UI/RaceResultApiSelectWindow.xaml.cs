using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using Race_timer.API;
using Race_timer.Data;

namespace Race_timer.UI
{
    /// <summary>
    /// Interaction logic for RaceResultApiSelectWindow.xaml
    ///
    /// Asks which of Race Timer's Simple API entries to create on the chosen RaceResult event.
    /// Mandatory ones are ticked and locked, optional ones start ticked and can be turned off.
    /// Nothing is deleted either way, unticking a row simply means it is not created
    /// </summary>
    public partial class RaceResultApiSelectWindow
    {
        private readonly List<ApiSelectionItem> _items;

        /// <summary>
        /// Builds the rows from the catalog and groups them under the headings of the app's own tabs
        /// </summary>
        /// <param name="eventName">Name of the chosen event, shown under the title</param>
        public RaceResultApiSelectWindow(string eventName)
        {
            InitializeComponent();

            EventNameText.Text = eventName;
            _items = RaceResultApiCatalog.Entries.Select(e => new ApiSelectionItem(e)).ToList();

            var view = new CollectionViewSource { Source = _items };
            view.GroupDescriptions.Add(new PropertyGroupDescription(nameof(ApiSelectionItem.Group)));
            ApiList.ItemsSource = view.View;
        }

        /// <summary>
        /// Every row left ticked, the mandatory ones included - what setup is asked to create
        /// </summary>
        public IReadOnlyCollection<RaceResultApiDefinition> Selected =>
            _items.Where(i => i.IsSelected).Select(i => i.Definition).ToList();

        /// <summary>
        /// Method called by the Create button, closes the window and lets setup run
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Create(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        /// <summary>
        /// Method called by the Cancel button, closes the window without creating anything
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Cancel(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
