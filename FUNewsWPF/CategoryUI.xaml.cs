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
    /// Interaction logic for CategoryUI.xaml
    /// </summary>
    public partial class CategoryUI : Window
    {

        private readonly ICategoryService iCategoryService;
        public CategoryUI()
        {
            InitializeComponent();
            iCategoryService = new CategoryService();
        }

        public void LoadCategoryList()
        {
            try
            {
                var cateList = iCategoryService.GetCategories();
                dgCategories.ItemsSource = cateList;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message,"Error on load list of categories");
            }
            finally
            {
                resetInput();
            }
        }

        private void Window_Loaded(Object sender, RoutedEventArgs e)
        {
            LoadCategoryList();
            
        }

        private void resetInput()
        {
            txtCategoryId.Text = "";
            txtCategoryName.Text = "";
            txtCategoryDescription.Text = "";
        }

        private void btnCreate_Click(object sender, RoutedEventArgs e)
        {
           
            CreateCategoryUI createCategoryUI = new CreateCategoryUI();

            // Mở cửa sổ CreateCategoryUI
            createCategoryUI.Show();
            createCategoryUI.Closed += (s, args) => {
                // Load lại danh sách Category khi cửa sổ CreateCategoryUI được đóng
                LoadCategoryList();
            };
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            
            if (dgCategories.SelectedItem != null)
            {
                // Lấy dữ liệu của dòng được chọn
                Category selectedCategory = dgCategories.SelectedItem as Category;

                // Tạo một instance của CreateCategoryUI
                CreateCategoryUI createCategoryUI = new CreateCategoryUI(selectedCategory);

                // Mở cửa sổ CreateCategoryUI
                createCategoryUI.Show();

                // Đăng ký sự kiện xử lý khi cửa sổ CreateCategoryUI được đóng
                createCategoryUI.Closed += (s, args) => {
                    // Load lại danh sách Category khi cửa sổ CreateCategoryUI được đóng
                    LoadCategoryList();
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
                if (txtCategoryId.Text.Length > 0)
                {
                    // Hiển thị hộp thoại xác nhận
                    MessageBoxResult result = MessageBox.Show("Are you sure you want to delete this category?", "Confirmation", MessageBoxButton.OKCancel, MessageBoxImage.Question);

                    if (result == MessageBoxResult.OK)
                    {
                        // Nếu người dùng chọn OK
                        Category category = new Category();
                        category.CategoryId = short.Parse(txtCategoryId.Text);
                        category.CategoryName = txtCategoryName.Text;
                        category.CategoryDesciption = txtCategoryName.Text;
                        iCategoryService.DeleteCategory(category);
                    }
                    else
                    {
                        // Nếu người dùng chọn Cancel hoặc đóng hộp thoại
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
                LoadCategoryList();
            }
        }

        private void dgCategories_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                /*DataGrid dataGrid = sender as DataGrid;
                DataGridRow row =
                    (DataGridRow)dataGrid.ItemContainerGenerator
                    .ContainerFromIndex(dataGrid.SelectedIndex);
                DataGridCell RowColumn =
                    dataGrid.Columns[0].GetCellContent(row).Parent as DataGridCell;
                String id = ((TextBlock)RowColumn.Content).Text;
                if (id.Equals(""))
                {
                    MessageBox.Show("Data null");
                }
                else
                {
                    Category category = iCategoryService.GetCategoryById(short.Parse(id));
                    txtCategoryId.Text = category.CategoryId.ToString();
                    txtCategoryName.Text = category.CategoryName;
                    txtCategoryDescription.Text = category.CategoryDesciption;
                }*/
                if (dgCategories.SelectedIndex >= 0)
                {
                    Category selectedCategory = dgCategories.SelectedItem as Category;
                    if (selectedCategory != null)
                    {
                        txtCategoryId.Text = selectedCategory.CategoryId.ToString();
                        txtCategoryName.Text = selectedCategory.CategoryName;
                        txtCategoryDescription.Text = selectedCategory.CategoryDesciption;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
        }

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            resetInput();
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            string searchTerm = txtSearch.Text.Trim(); // Lấy từ khóa tìm kiếm từ TextBox
            string searchType = (cboSearch.SelectedItem as ComboBoxItem)?.Content.ToString(); // Lấy loại tìm kiếm từ ComboBox

            // Kiểm tra xem người dùng đã nhập từ khóa và chọn loại tìm kiếm chưa
            if (!string.IsNullOrEmpty(searchTerm) && !string.IsNullOrEmpty(searchType))
            {
                try
                {
                    List<Category> searchResults = new List<Category>();


                    // Tùy thuộc vào loại tìm kiếm được chọn, thực hiện tìm kiếm trong danh sách categories
                    switch (searchType)
                    {
                        case "Category ID":
                            if (short.TryParse(searchTerm, out short categoryId))
                            {
                                Category category = iCategoryService.GetCategoryById(categoryId);
                                if (category != null)
                                    searchResults.Add(category);
                            }
                            else
                            {
                                MessageBox.Show("Please enter a valid Tag ID.", "Search", MessageBoxButton.OK, MessageBoxImage.Warning);
                                return;
                            }
                            break;
                        case "Category Name":
                            searchResults = iCategoryService.GetCategoryByName(searchTerm);
                            break;
                        case "Category Description":
                            searchResults = iCategoryService.GetCategoryByDescription(searchTerm);
                            break;
                    }

                    // Hiển thị kết quả tìm kiếm trên DataGrid
                    dgCategories.ItemsSource = searchResults;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error occurred while searching: " + ex.Message);
                }
            }
            else
            {
                var cateList = iCategoryService.GetCategories();
                dgCategories.ItemsSource = cateList;
            }
        }
    }
    
}
