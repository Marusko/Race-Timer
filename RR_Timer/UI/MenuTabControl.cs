using System.Collections.Generic;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;

namespace Race_timer.UI
{
    /// <summary>
    /// The left menu of the main window. A plain <see cref="TabControl"/> in every respect except
    /// one: whatever its <see cref="FrameworkElement.Tag"/> holds is rendered under the tab strip
    /// by the template in the theme, and this class is what makes that footer visible to screen
    /// readers and UI automation.
    ///
    /// Without it the footer is drawn and can be clicked, but nothing can find it:
    /// <see cref="TabControlAutomationPeer"/> reports only the tabs and the selected tab's content,
    /// so an element sitting elsewhere in the template is left out of the automation tree
    /// </summary>
    public class MenuTabControl : TabControl
    {
        /// <summary>
        /// Returns the peer that adds the footer to what the base peer reports
        /// </summary>
        /// <returns>Automation peer for this control</returns>
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new MenuTabControlAutomationPeer(this);
        }
    }

    /// <summary>
    /// Automation peer of <see cref="MenuTabControl"/>, reports the tabs as the base peer does and
    /// the footer from the Tag after them
    /// </summary>
    internal class MenuTabControlAutomationPeer : TabControlAutomationPeer
    {
        /// <summary>
        /// Creates the peer for one menu tab control
        /// </summary>
        /// <param name="owner">Control this peer belongs to</param>
        public MenuTabControlAutomationPeer(MenuTabControl owner) : base(owner)
        {
        }

        /// <summary>
        /// The tabs, plus the footer element when there is one
        /// </summary>
        /// <returns>Children of this peer</returns>
        protected override List<AutomationPeer> GetChildrenCore()
        {
            var children = base.GetChildrenCore() ?? new List<AutomationPeer>();

            if (((TabControl)Owner).Tag is UIElement footer)
            {
                var peer = CreatePeerForElement(footer);
                if (peer != null)
                {
                    children.Add(peer);
                }
            }

            return children;
        }
    }
}
