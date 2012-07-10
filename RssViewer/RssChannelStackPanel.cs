using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace RssViewer
{
    public class RssChannelStackPanel : StackPanel
    {
        public int Id { get; set; }
        public int ItemCount { get; set; }

        public RssChannelStackPanel(int id)
            :base()
        {
            this.Id = id;
        }
    }
}
