using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace wp13_FestivalSearch
{
    /// <summary>
    /// LinkWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class LinkWindow : MetroWindow
    {
        public LinkWindow()
        {
            InitializeComponent();
        }

        public LinkWindow(string Homepage_Url) : this()
        {
            BrsLink.Address = $"{Homepage_Url}";
        }
    }
}
