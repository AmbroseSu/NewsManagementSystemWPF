using Service;
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

namespace FUNewsWPF
{
    /// <summary>
    /// Interaction logic for NewsArticleUI.xaml
    /// </summary>
    public partial class NewsArticleUI : Window
    {
        private readonly INewsArticleService iNewsArticleService;
        public NewsArticleUI()
        {
            InitializeComponent();
            iNewsArticleService = new NewsArticleService();
        }

        public void LoadNewsArticlesList()
        {
            try
            {
                var neArList = iNewsArticleService.GetNewsArticles();
                dgNewsArticles.ItemsSource = neArList;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error on load list of categories");
            }
            finally
            {
                //resetInput();
            }
        }

        private void Window_Loaded(Object sender, RoutedEventArgs e)
        {
            LoadNewsArticlesList();

        }



    }
}
