using BusinessObject;
using DataAccess;
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
            if (cboSearch.Items.Count > 0)
            {
                ComboBoxItem firstItem = cboSearch.Items[0] as ComboBoxItem;
                if (firstItem != null)
                {
                    cboSearch.Text = firstItem.Content.ToString();
                }
            }
        }

        private void resetInput()
        {
            txtTagID.Text = "";
            txtTagName.Text = "";
            txtNote.Text = "";
            dgTags.SelectedIndex = -1;
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

        private void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            CreateUpdateTag createUpdateTag = new CreateUpdateTag();

            createUpdateTag.Show();
            createUpdateTag.Closed += (s, args) => {
                LoadTagList();
            };
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if(dgTags.SelectedItem != null)
            {
                Tag selectedTag = dgTags.SelectedItem as Tag;
                CreateUpdateTag createUpdateTag = new CreateUpdateTag(selectedTag);
                createUpdateTag.Show();
                createUpdateTag.Closed += (s, args) => {
                    LoadTagList();
                };

            }
            else
            {
                MessageBox.Show("Please choose Tag update");
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (txtTagID.Text.Length > 0)
                {
                    MessageBoxResult result = MessageBox.Show("Are you sure you want to delete this category?", "Confirmation", MessageBoxButton.OKCancel, MessageBoxImage.Question);

                    if (result == MessageBoxResult.OK)
                    {
                        Tag tag = new Tag();
                        tag.TagId = int.Parse(txtTagID.Text);
                        tag.TagName = txtTagName.Text;
                        tag.Note = txtNote.Text;
                        iTagService.DeleteTag(tag);
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    MessageBox.Show("You must select a category.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                LoadTagList();
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

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string searchText = txtSearch.Text.ToLower();
                string searchCriterion = (cboSearch.SelectedItem as ComboBoxItem)?.Content.ToString();
                List<Tag> searchResults = new List<Tag>();
                if (string.IsNullOrWhiteSpace(searchText) || string.IsNullOrWhiteSpace(searchCriterion))
                {
                    var tagList = iTagService.GetTags();
                    dgTags.ItemsSource = tagList;
                    return;
                }

                
                    

                    switch (searchCriterion)
                    {
                        case "Tag ID":
                            if (short.TryParse(searchText, out short tagId))
                            {
                            Tag account = iTagService.GetTagById(tagId);
                                if(account != null)
                                {
                                   searchResults.Add(account);
                                }
                            }
                            else
                            {
                                MessageBox.Show("Please enter a valid Tag ID.", "Search", MessageBoxButton.OK, MessageBoxImage.Warning);
                                return;
                            }
                            break;

                        case "Tag Name":
                            searchResults = iTagService.GetTagsByName(searchText);
                            break;

                        case "Note":
                            searchResults = iTagService.GetTagsByNote(searchText);
                            break;

                        default:
                            MessageBox.Show("Please select a valid search criterion.", "Search", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                    }

                    dgTags.ItemsSource = searchResults;
                
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while searching: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
