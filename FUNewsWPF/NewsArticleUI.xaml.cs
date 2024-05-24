using BusinessObject;
using DataAccess;
using Microsoft.EntityFrameworkCore;
using Service;
using System;
using System.Collections.Generic;
using System.DirectoryServices;
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
        private SystemAccount account;
        private Dictionary<string, bool> tagSelectionStatus = new Dictionary<string, bool>();
        
        /*public NewsArticleUI(SystemAccount systemAccount)
        {
            InitializeComponent();
            iNewsArticleService = new NewsArticleService();
            iCategoryService = new CategoryService();
            iTagService = new TagService();
            account = systemAccount;
            if (account != null)
            {
                DisableEditingFeatures();
            }
        }*/

        public NewsArticleUI()
        {
            InitializeComponent();
            iNewsArticleService = new NewsArticleService();
            iCategoryService = new CategoryService();
            iTagService = new TagService();
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
            if (account == null)
            {
                DisableEditingFeatures();
            }
        }

        private void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            NewsArticleManagerUI newsArticleManagerUI = new NewsArticleManagerUI(account);

            // Mở cửa sổ CreateCategoryUI
            newsArticleManagerUI.Show();
            newsArticleManagerUI.Closed += (s, args) => {
                // Load lại danh sách Category khi cửa sổ CreateCategoryUI được đóng
                LoadCategoryList();
                LoadTagList();
                LoadNewsArticlesList();

            };
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (dgNewsArticles.SelectedItem != null)
            {
                // Lấy dữ liệu của dòng được chọn
                NewsArticle selectedNewsArticle = dgNewsArticles.SelectedItem as NewsArticle;

                // Tạo một instance của CreateCategoryUI
                NewsArticleManagerUI newsArticleManagerUI = new NewsArticleManagerUI(selectedNewsArticle);

                // Mở cửa sổ CreateCategoryUI
                newsArticleManagerUI.Show();

                // Đăng ký sự kiện xử lý khi cửa sổ CreateCategoryUI được đóng
                newsArticleManagerUI.Closed += (s, args) => {
                    // Load lại danh sách Category khi cửa sổ CreateCategoryUI được đóng
                    LoadCategoryList();
                    LoadTagList() ;
                    LoadNewsArticlesList() ;
                };
            }
            else
            {
                MessageBox.Show("Please select a category to update.");
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
           
            try
            {
                if (!string.IsNullOrWhiteSpace(txtNewsArticleId.Text))
                {
                    
                    MessageBoxResult result = MessageBox.Show("Are you sure you want to delete this News Article?", "Confirmation", MessageBoxButton.OKCancel, MessageBoxImage.Question);

                    if (result == MessageBoxResult.OK)
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
                        // Nếu người dùng chọn Cancel hoặc đóng hộp thoại
                        return;
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

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            Login login = new Login();
            if (login.ShowDialog() == true)
            {
                if(login.Role == LoginRole.Staff)
                {
                    account = login.LoggedInAccount;
                    EnableEditingFeatures();
                }
                else
                {
                    if(login.Role == LoginRole.Lecturer)
                    {
                        account = login.LoggedInAccount;
                        mnMenu.IsEnabled =false;
                        EnableEditingFeatures();
                    }
                }
                 // Kích hoạt các chức năng chỉnh sửa
            }
            else
            {
                this.Close();
            }
        }

        private void DisableEditingFeatures()
        {
            gbNewsArticleDetail.Visibility = Visibility.Collapsed;
            mnMenu.Visibility = Visibility.Collapsed;
            btnCreate.IsEnabled = false;

            btnUpdate.IsEnabled = false;          

            btnDelete.IsEnabled = false;

            btnReset.IsEnabled = false;

            txtNewsArticleId.IsEnabled = false;
            txtNewsTitle.IsEnabled = false;
            txtNewsContent.IsEnabled = false;
            cboCategory.IsEnabled = false;
            rbNewsStatusTrue.IsEnabled = false;
            rbNewsStatusFalse.IsEnabled = false;
            lstTags.IsEnabled = false;
        }

        private void EnableEditingFeatures()
        {
            gbNewsArticleDetail.Visibility = Visibility.Visible;
            mnMenu.Visibility = Visibility.Visible;
            btnCreate.IsEnabled = true;
            btnUpdate.IsEnabled = true;
            btnDelete.IsEnabled = true;
            btnReset.IsEnabled = true;
            txtNewsArticleId.IsEnabled = false;
            txtNewsTitle.IsEnabled = false;
            txtNewsContent.IsEnabled = false;
            cboCategory.IsEnabled = false;
            rbNewsStatusTrue.IsEnabled = false;
            rbNewsStatusFalse.IsEnabled = false;
            foreach (var item in lstTags.Items)
            {
                if (item is FrameworkElement element)
                {
                    // Tìm kiếm các controls kiểu CheckBox trong mỗi mục của ListBox
                    var checkBox = FindVisualChild<CheckBox>(element);
                    if (checkBox != null)
                    {
                        checkBox.IsEnabled = false;
                    }
                }
            }
        }

        private void miCategory_Click(object sender, RoutedEventArgs e)
        {
            CategoryUI categoryUI = new CategoryUI();
            categoryUI.Show();
            miCategory.IsEnabled = false;
            categoryUI.Closed += (s, args) => {
                miCategory.IsEnabled = true;
                LoadCategoryList();
                LoadTagList();
                LoadNewsArticlesList();
            };
        }
        private void miTag_Click(object sender, RoutedEventArgs e)
        {
            TagUI tagUI = new TagUI();
            tagUI.Show();
            miTag.IsEnabled = false;
            tagUI.Closed += (s, args) => {
                miTag.IsEnabled = true;
                LoadCategoryList();
                LoadTagList();
                LoadNewsArticlesList();
            };
        }
        private void miProfile_Click(object sender, RoutedEventArgs e)
        {
            ProfileUI profileUI = new ProfileUI(account);
            profileUI.Show();
            miProfile.IsEnabled = false;
            profileUI.Closed += (s, args) => {
                miProfile.IsEnabled = true;
                LoadCategoryList();
                LoadTagList();
                LoadNewsArticlesList();
            };
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string searchText = txtSearch.Text.Trim(); // Lấy từ khóa tìm kiếm từ TextBox
                string searchCriterion = (cboSearch.SelectedItem as ComboBoxItem)?.Content.ToString();
                List<NewsArticle> searchResults = new List<NewsArticle>();
                if (string.IsNullOrWhiteSpace(searchText) || string.IsNullOrWhiteSpace(searchCriterion))
                {
                    var neArList = iNewsArticleService.GetNewsArticles();
                    dgNewsArticles.ItemsSource = neArList;
                    //MessageBox.Show("Please enter search text and select a search criterion.", "Search", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                switch (searchCriterion)
                {
                    case "News Article ID":
                        searchResults = iNewsArticleService.GetNewsArticlesById(searchText);
                        break;
                    case "News Title":
                        searchResults = iNewsArticleService.GetNewsArticlesbyTitle(searchText);
                        break;
                    case "Category":
                        if (short.TryParse(searchText, out short categoryId))
                        {
                            searchResults = iNewsArticleService.GetNewsArticlesByCategory(categoryId);
                        }
                        else
                        {
                            MessageBox.Show("Please enter a valid CategoryId.", "Search", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                        }

                        break;
                    case "News Status":
                        if (bool.TryParse(searchText, out bool newsStatus))
                        {
                            searchResults = iNewsArticleService.GetNewsArticlesByStatus(newsStatus);
                        }
                        else
                        {
                            MessageBox.Show("Please enter a valid News Status.", "Search", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                        }
                        break;
                    case "Tags":
                        searchResults = iNewsArticleService.GetNewsArticlesByTag(searchText);
                        break;
                    default:
                        MessageBox.Show("Please select a valid search criterion.", "Search", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                }

                dgNewsArticles.ItemsSource = searchResults;
                LoadCategoryList();
                LoadTagList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while searching: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }



        }
    }
}
