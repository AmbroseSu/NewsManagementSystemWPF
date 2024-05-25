using BusinessObject;
using DataAccess;
using Microsoft.EntityFrameworkCore;
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
    /// Interaction logic for NewsArticleManagerUI.xaml
    /// </summary>
    public partial class NewsArticleManagerUI : Window
    {
        private readonly INewsArticleService iNewsArticleService;
        private readonly ICategoryService iCategoryService;
        private readonly ITagService iTagService;
        private SystemAccount account;
        private NewsArticle newsArticleToUpdate = null;
        private Dictionary<string, bool> tagSelectionStatus = new Dictionary<string, bool>();
        public NewsArticleManagerUI()
        {
            InitializeComponent();
            iNewsArticleService = new NewsArticleService();
            iCategoryService = new CategoryService();
            iTagService = new TagService();
        }
        public NewsArticleManagerUI(SystemAccount systemAccount)
        {
            InitializeComponent();
            iNewsArticleService = new NewsArticleService();
            iCategoryService = new CategoryService();
            iTagService = new TagService();
            account = systemAccount;
        }
        public NewsArticleManagerUI(NewsArticle selectedNewsArticle)
        {
            InitializeComponent();
            iNewsArticleService = new NewsArticleService();
            iCategoryService = new CategoryService();
            iTagService = new TagService();
            Loaded += Window_Loaded;
            newsArticleToUpdate = selectedNewsArticle;
           
            
        }

        private void Window_Loaded(Object sender, RoutedEventArgs e)
        {
            LoadTagList(); 
            LoadCategoryList(); 

            Dispatcher.InvokeAsync(() =>
            {
                if (newsArticleToUpdate != null)
                {
                    txtNewsArticleId.Text = newsArticleToUpdate.NewsArticleId.ToString();
                    txtNewsArticleId.IsEnabled = false;
                    btnCreate.IsEnabled = false;
                    txtNewsTitle.Text = newsArticleToUpdate.NewsTitle;
                    txtNewsContent.Text = newsArticleToUpdate.NewsContent;
                    cboCategory.SelectedValue = newsArticleToUpdate.CategoryId;
                    if (newsArticleToUpdate.NewsStatus == true)
                    {
                        rbNewsStatusTrue.IsChecked = true;
                    }
                    else if (newsArticleToUpdate.NewsStatus == false)
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
                    foreach (var articleTag in newsArticleToUpdate.Tags)
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
                else
                {
                    btnUpdate.IsEnabled = false;
                }
            });

            

        }

        public void LoadCategoryList()
        {
            var cateList = iCategoryService.GetCategories();
            cboCategory.ItemsSource = cateList;
            cboCategory.DisplayMemberPath = "CategoryName";
            cboCategory.SelectedValuePath = "CategoryId";
        }

        public void LoadTagList()
        {
            var tags = iTagService.GetTags(); 
            foreach (var tag in tags)
            {
                if (!tagSelectionStatus.ContainsKey(tag.TagName))
                {
                    tagSelectionStatus[tag.TagName] = false; 
                }
            }
            lstTags.ItemsSource = tags;
        }
        private void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var neArList = iNewsArticleService.GetNewsArticles();
                NewsArticle newsArticle = new NewsArticle();

                
                foreach (var article in neArList)
                {
                    if (txtNewsArticleId.Text.Equals(article.NewsArticleId.ToString()))
                    {
                        MessageBox.Show("ID exists");
                        return;
                    }
                }
                if (string.IsNullOrWhiteSpace(txtNewsArticleId.Text) ||
                    string.IsNullOrWhiteSpace(txtNewsTitle.Text) ||
                    string.IsNullOrWhiteSpace(txtNewsContent.Text) ||
                    cboCategory.SelectedValue == null ||
                    account == null)
                {
                    MessageBox.Show("All fields must be filled in and a category must be selected.");
                    return;
                }

                newsArticle.NewsArticleId = txtNewsArticleId.Text;
                newsArticle.NewsTitle = txtNewsTitle.Text;
                newsArticle.CreatedDate = DateTime.Now;
                newsArticle.NewsContent = txtNewsContent.Text;
                newsArticle.CategoryId = short.Parse(cboCategory.SelectedValue.ToString());
                newsArticle.NewsStatus = rbNewsStatusTrue.IsChecked == true;
                newsArticle.CreatedById = account.AccountId;
                newsArticle.ModifiedDate = DateTime.Now;

                
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
                    this.Close();
                    
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                //LoadCategoryList();
                //this.Close();
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
                            existingArticle.NewsTitle = txtNewsTitle.Text;
                            existingArticle.NewsContent = txtNewsContent.Text;
                            existingArticle.CategoryId = short.Parse(cboCategory.SelectedValue.ToString());
                            existingArticle.NewsStatus = rbNewsStatusTrue.IsChecked == true;
                            existingArticle.ModifiedDate = DateTime.Now;

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
                            this.Close();
                            //ResetInput();
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
                //this.Close();
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

        public void ResetInput()
        {
            if (!string.IsNullOrEmpty(txtNewsArticleId.Text))
            {
                       
                btnCreate.IsEnabled = false;
                txtNewsArticleId.IsEnabled = false;
                txtNewsTitle.Text = "";
                txtNewsContent.Text = "";
                cboCategory.SelectedIndex = -1;
                rbNewsStatusTrue.IsChecked = false; 

                
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
            else
            {
                btnUpdate.IsEnabled = false;
                txtNewsArticleId.Text = "";
                txtNewsTitle.Text = "";
                txtNewsContent.Text = "";
                cboCategory.SelectedIndex = -1;
                rbNewsStatusTrue.IsChecked = false; 

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
}
