using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace Runner.UserProperties
{
    public class UserProperty
    {
        [Category("Writing")]
        [DisplayName("Writing Font Size")]
        [Description("This property uses the DoubleUpDown as the default editor.")]
        [ItemsSource(typeof(FontSizeItemsSource))]
        public double WritingFontSize { get;}
    }

    public class FontSizeItemsSource : IItemsSource
    {
        public ItemCollection GetValues()
        {
            ItemCollection sizes = new ItemCollection();
            sizes.Add(5.0, "Five");
            sizes.Add(5.5);
            sizes.Add(6.0, "Six");
            sizes.Add(6.5);
            sizes.Add(7.0, "Seven");
            sizes.Add(7.5);
            sizes.Add(8.0, "Eight");
            sizes.Add(8.5);
            sizes.Add(9.0, "Nine");
            sizes.Add(9.5);
            sizes.Add(10.0);
            sizes.Add(12.0, "Twelve");
            sizes.Add(14.0);
            sizes.Add(16.0);
            sizes.Add(18.0);
            sizes.Add(20.0);
            return sizes;
        }
    }
}
