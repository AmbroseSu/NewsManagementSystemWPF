using BusinessObject;
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
    /// Interaction logic for CreateUpdateTag.xaml
    /// </summary>
    public partial class CreateUpdateTag : Window
    {

        private readonly ITagService iTagService;
        private Tag tagToUpdate;
        public CreateUpdateTag()
        {
            InitializeComponent();
            iTagService = new TagService();
            Loaded += Window_Loaded;
        }
        public CreateUpdateTag(Tag selectedTag)
        {
            InitializeComponent();
            iTagService = new TagService();
            Loaded += Window_Loaded;
            tagToUpdate = selectedTag;
            txtTagId.Text = tagToUpdate.TagId.ToString();
            txtTagName.Text = tagToUpdate.TagName;
            txtNote.Text = tagToUpdate.Note;
        }

        private void Window_Loaded(Object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtTagId.Text))
            {
                txtTagId.IsEnabled = false;
                btnCreate.IsEnabled = false;
            }
            else
            {
                btnUpdate.IsEnabled = false;
                txtTagId.IsEnabled = true;
            }
        }

        private void resetInput()
        {
            if (!string.IsNullOrEmpty(txtTagId.Text))
            {
                txtTagName.Text = "";
                txtNote.Text = "";
            }
            else
            {
                txtTagId.Text = "";
                txtTagName.Text = "";
                txtNote.Text = "";
            }
        }

        private void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Tag tag = new Tag();
                tag.TagId = int.Parse(txtTagId.Text);
                tag.TagName = txtTagName.Text;
                tag.Note = txtNote.Text;
                iTagService.SaveTag(tag);


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                this.Close();
            }
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                if (txtTagId.Text.Length > 0)
                {
                    Tag tag = new Tag();
                    tag.TagId = int.Parse(txtTagId.Text);
                    tag.TagName = txtTagName.Text;
                    tag.Note = txtNote.Text;
                    iTagService.UpdateTag(tag);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("You must select a Category !");
            }
            finally
            {
                this.Close();
            }
        }

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            resetInput();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
