using BusinessObject;
using DataAccess;
using Microsoft.EntityFrameworkCore;
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
        private readonly ICategoryService iCategoryService;
        private readonly ITagService iTagService;
        private readonly SystemAccount account;
        private Dictionary<string, bool> tagSelectionStatus = new Dictionary<string, bool>();
        public NewsArticleUI(SystemAccount systemAccount)
        {
            InitializeComponent();
            iNewsArticleService = new NewsArticleService();
            iCategoryService = new CategoryService();
            iTagService = new TagService();
            account = systemAccount;
        }

        public void LoadNewsArticlesList()
        {
            try
            {
                var neArList = iNewsArticleService.GetNewsArticles();
                dgNewsArticles.ItemsSource = neArList;
/*                var cateList = iCategoryService.GetCategories();
                cboCategory.ItemsSource = cateList;
*/
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

        public void LoadCategoryList()
        {
            var  cateList = iCategoryService.GetCategories();
            cboCategory.ItemsSource = cateList;
            cboCategory.DisplayMemberPath = "CategoryName";
            cboCategory.SelectedValuePath = "CategoryId";
        }

        public void LoadTagList()
        {
            var tags = iTagService.GetTags(); // Giả sử iTagService.GetTags() trả về List<Tag>
            foreach (var tag in tags)
            {
                if (!tagSelectionStatus.ContainsKey(tag.TagName))
                {
                    tagSelectionStatus[tag.TagName] = false; // Khởi tạo trạng thái không chọn
                }
            }
            lstTags.ItemsSource = tags;
        }

        private void Window_Loaded(Object sender, RoutedEventArgs e)
        {
            LoadNewsArticlesList();
            LoadCategoryList();
            LoadTagList();
        }

        private void btnCreate_Click(object sender, RoutedEventArgs e)
        {
          
            try
            {
                var neArList = iNewsArticleService.GetNewsArticles();
                NewsArticle newsArticle = new NewsArticle();

                // Check if ID already exists
                foreach (var article in neArList)
                {
                    if (txtNewsArticleId.Text.Equals(article.NewsArticleId.ToString()))
                    {
                        MessageBox.Show("ID exists");
                        return;
                    }
                }

                newsArticle.NewsArticleId = txtNewsArticleId.Text;
                newsArticle.NewsTitle = txtNewsTitle.Text;
                newsArticle.CreatedDate = DateTime.Now;
                newsArticle.NewsContent = txtNewsContent.Text;
                newsArticle.CategoryId = short.Parse(cboCategory.SelectedValue.ToString());
                newsArticle.NewsStatus = rbNewsStatusTrue.IsChecked == true;
                newsArticle.CreatedById = account.AccountId;
                newsArticle.ModifiedDate = DateTime.Now;

                // Save NewsArticle first to get the generated ID
                iNewsArticleService.SaveNewsArticle(newsArticle);

                using (var context = new FunewsManagementDbContext())
                {
                    var selectedTags = new List<Dictionary<string, object>>();
                    foreach (var item in lstTags.Items)
                    {
                        var listBoxItem = lstTags.ItemContainerGenerator.ContainerFromItem(item) as ListBoxItem;
                        var checkBox = FindVisualChild<CheckBox>(listBoxItem);
                        if (checkBox != null && checkBox.IsChecked == true)
                        {
                            var tag = item as Tag;
                            if (tag != null)
                            {
                                var tagFromDb = context.Tags.FirstOrDefault(t => t.TagName == tag.TagName);
                                if (tagFromDb != null)
                                {
                                    selectedTags.Add(new Dictionary<string, object>
                            {
                                { "NewsArticleId", newsArticle.NewsArticleId },
                                { "TagId", tagFromDb.TagId }
                            });
                                }
                            }
                        }
                    }

                    foreach (var nt in selectedTags)
                    {
                        context.Set<Dictionary<string, object>>("NewsTag").Add(nt);
                    }
                    context.SaveChanges();
                    MessageBox.Show("News article updated successfully.");
                    ResetInput();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                //LoadCategoryList();
                LoadNewsArticlesList();
                //LoadTagList();
                
            }
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                
                if (!string.IsNullOrWhiteSpace(txtNewsArticleId.Text))
                {
                    var newsArticleId = txtNewsArticleId.Text;

                    using (var context = new FunewsManagementDbContext())
                    {
                        var existingArticle = context.NewsArticles
                            .Include(na => na.Tags)
                            .FirstOrDefault(na => na.NewsArticleId == newsArticleId);

                        if (existingArticle != null)
                        {
                            // Update properties of the existing article
                            existingArticle.NewsTitle = txtNewsTitle.Text;
                            existingArticle.NewsContent = txtNewsContent.Text;
                            existingArticle.CategoryId = short.Parse(cboCategory.SelectedValue.ToString());
                            existingArticle.NewsStatus = rbNewsStatusTrue.IsChecked == true;
                            existingArticle.ModifiedDate = DateTime.Now;

                            // Update associated tags
                            var selectedTags = new List<Tag>();
                            foreach (var item in lstTags.Items)
                            {
                                var listBoxItem = lstTags.ItemContainerGenerator.ContainerFromItem(item) as ListBoxItem;
                                var checkBox = FindVisualChild<CheckBox>(listBoxItem);
                                if (checkBox != null && checkBox.IsChecked == true)
                                {
                                    var tag = item as Tag;
                                    if (tag != null)
                                    {
                                        selectedTags.Add(tag);
                                    }
                                }
                            }

                            existingArticle.Tags.Clear();
                            foreach (var tag in selectedTags)
                            {
                                var existingTag = context.Tags.FirstOrDefault(t => t.TagId == tag.TagId);
                                if (existingTag != null)
                                {
                                    existingArticle.Tags.Add(existingTag);
                                }
                            }

                            context.SaveChanges();
                            MessageBox.Show("News article updated successfully.");
                            ResetInput();
                        }
                        else
                        {
                            MessageBox.Show("News article not found.");
                        }
                    }
                }
                else
                {
                    MessageBox.Show("You must select a news article to update.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                LoadCategoryList();
                LoadNewsArticlesList();
                LoadTagList();
                
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            /*try
            {
                if(txtNewsArticleId.Text.Length > 0)
                {
                    NewsArticle newsArticle = new NewsArticle();
                    newsArticle.NewsArticleId = txtNewsArticleId.Text;
                    newsArticle.NewsTitle = txtNewsTitle.Text;
                    newsArticle.NewsContent = txtNewsContent.Text;
                    newsArticle.CategoryId = short.Parse(cboCategory.SelectedValue.ToString());
                    newsArticle.NewsStatus = rbNewsStatusTrue.IsChecked == true;
                    newsArticle.CreatedById = account.AccountId;
                    iNewsArticleService.DeleteNewsArticle(newsArticle);
                }
                else
                {
                    MessageBox.Show("You must select");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                LoadCategoryList();
                LoadNewsArticlesList() ; 
                LoadTagList();
            }*/

            try
            {
                if (!string.IsNullOrWhiteSpace(txtNewsArticleId.Text))
                {
                    var newsArticleId = txtNewsArticleId.Text;

                    using (var context = new FunewsManagementDbContext())
                    {
                        var newsArticle = context.NewsArticles.Include(na => na.Tags)
                            .FirstOrDefault(na => na.NewsArticleId == newsArticleId);

                        if (newsArticle != null)
                        {
                            // Remove associated tags from the NewsTag table
                            var newsTags = context.Set<Dictionary<string, object>>("NewsTag")
                                .Where(nt => EF.Property<string>(nt, "NewsArticleId") == newsArticleId).ToList();
                            context.Set<Dictionary<string, object>>("NewsTag").RemoveRange(newsTags);

                            // Remove the NewsArticle
                            context.NewsArticles.Remove(newsArticle);

                            context.SaveChanges();
                            MessageBox.Show("News article deleted successfully.");
                            ResetInput();
                        }
                        else
                        {
                            MessageBox.Show("News article not found.");
                        }
                    }
                }
                else
                {
                    MessageBox.Show("You must select a news article to delete.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                LoadCategoryList();
                LoadNewsArticlesList();
                LoadTagList();
                
            }
        
    }

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            ResetInput();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void dgNewsArticles_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                txtNewsArticleId.IsEnabled = false;

                if (dgNewsArticles.SelectedIndex >= 0)
                {
                    NewsArticle selectedNewsArticle = dgNewsArticles.SelectedItem as NewsArticle;
                    if (selectedNewsArticle != null)
                    {
                        txtNewsArticleId.Text = selectedNewsArticle.NewsArticleId.ToString();
                        txtNewsTitle.Text = selectedNewsArticle.NewsTitle;
                        txtNewsContent.Text = selectedNewsArticle.NewsContent;
                        cboCategory.SelectedValue = selectedNewsArticle.CategoryId;
                        if (selectedNewsArticle.NewsStatus == true)
                        {
                            rbNewsStatusTrue.IsChecked = true;
                        }
                        else if (selectedNewsArticle.NewsStatus == false)
                        {
                            rbNewsStatusFalse.IsChecked = true;
                        }
                        else
                        {
                            // If NewsStatus is null or invalid, reset both RadioButtons
                            rbNewsStatusTrue.IsChecked = false;
                            rbNewsStatusFalse.IsChecked = false;
                        }

                        foreach (var item in lstTags.Items)
                        {
                            var listBoxItem = lstTags.ItemContainerGenerator.ContainerFromItem(item) as ListBoxItem;
                            var checkBox = FindVisualChild<CheckBox>(listBoxItem);
                            if (checkBox != null)
                            {
                                checkBox.IsChecked = false;
                            }
                        }

                        // Đặt trạng thái các CheckBox đã chọn
                        foreach (var articleTag in selectedNewsArticle.Tags)
                        {
                            foreach (var item in lstTags.Items)
                            {
                                var listBoxItem = lstTags.ItemContainerGenerator.ContainerFromItem(item) as ListBoxItem;
                                var checkBox = FindVisualChild<CheckBox>(listBoxItem);
                                if (checkBox != null && checkBox.Content.ToString() == articleTag.TagName)
                                {
                                    checkBox.IsChecked = true;
                                }
                            }
                        }




                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private T FindVisualChild<T>(DependencyObject obj) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                if (child != null && child is T)
                {
                    return (T)child;
                }
                else
                {
                    T childOfChild = FindVisualChild<T>(child);
                    if (childOfChild != null)
                    {
                        return childOfChild;
                    }
                }
            }
            return null;
        }

        private void ResetInput()
        {
            txtNewsArticleId.Text = "";
            txtNewsTitle.Text = "";
            txtNewsContent.Text = "";
            cboCategory.SelectedIndex = -1;
            rbNewsStatusTrue.IsChecked = false; // Assuming rbNewsStatusTrue is the RadioButton for true status

            // Clear tag selection
            foreach (var item in lstTags.Items)
            {
                var listBoxItem = lstTags.ItemContainerGenerator.ContainerFromItem(item) as ListBoxItem;
                var checkBox = FindVisualChild<CheckBox>(listBoxItem);
                if (checkBox != null)
                {
                    checkBox.IsChecked = false;
                }
            }
        }
    }
}
