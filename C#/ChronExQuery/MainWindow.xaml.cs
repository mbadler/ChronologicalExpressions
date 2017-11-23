using ChronEx.Models;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ChronExQuery
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        List<ChronologicalEvent> ChronEvents = null;

        private List<ChronologicalEvent> GetEvts()
        {
            if(ChronEvents == null)
            {
                var a = inputTbox.Text.Split('\n');
                ChronEvents = a.Select(y=>y.Split(',')).Select(x => new ChronologicalEvent()
                {
                    EventName=x[0]
                }).ToList();
            }
            return ChronEvents;
        }

        private void concatPattern()
        {
            resultTB.AppendText("--- For: " +String.Join(",", patternTB.Text.Split('\n'))+'\n');
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var evts = GetEvts();
            concatPattern();
            resultTB.AppendText(String.Format("IsMatch: {0}\n", ChronEx.ChronEx.IsMatch(patternTB.Text, evts)));
            resultTB.ScrollToEnd();

        }

       

        private void inputTbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ChronEvents = null;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var evts = GetEvts();
            concatPattern();
            resultTB.AppendText(String.Format("MatchCount: {0}\n", ChronEx.ChronEx.MatchCount(patternTB.Text, evts)));
            resultTB.ScrollToEnd();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            var evts = GetEvts();
            concatPattern();
            var res = ChronEx.ChronEx.Matches(patternTB.Text, evts);
            for (int i = 0; i < res.Count; i++)
            {
                resultTB.AppendText(string.Format("Result #{0}\n", i));
                var resl = res[i];
                foreach (var item in resl.CapturedEvents)
                {
                    resultTB.AppendText("   " + item.EventName+"\n");
                }
            }
            
            resultTB.ScrollToEnd();
        }
    }
}
