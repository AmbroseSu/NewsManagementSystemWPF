using BusinessObject;
using Microsoft.Identity.Client.NativeInterop;
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
    /// Interaction logic for TagUI.xaml
    /// </summary>
    public partial class TagUI : Window
    {
        private readonly ITagService iTagService;
        public TagUI()
        {
            InitializeComponent();
            iTagService = new TagService();
        }

        public void LoadTagList()
        {
            try
            {
                var tagList = iTagService.GetTags();
                dgTags.ItemsSource = tagList;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error on load list of tags");
            }
            finally
            {

            }
        }

        private void Window_Loaded(Object sender, RoutedEventArgs e)
        {
            LoadTagList();
            txtTagName.IsEnabled = false;
            txtNote.IsEnabled = false;
        }

        private void resetInput()
        {
            txtTagID.Text = "";
            txtTagName.Text = "";
            txtNote.Text = "";
        }

        private void dgTags_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (dgTags.SelectedIndex >= 0)
                {
                    Tag selectTag = dgTags.SelectedItem as Tag;
                    if (selectTag != null)
                    {
                        txtTagID.Text = selectTag.TagId.ToString();
                        txtTagName.Text = selectTag.TagName;
                        txtNote.Text = selectTag.Note;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
        }
    }
}
