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
            try
            {
                Category category = new Category();
                category.CategoryName = txtCategoryName.Text;
                category.CategoryDesciption = txtCategoryDescription.Text;
                iCategoryService.SaveCategory(category);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                LoadCategoryList() ;
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if(txtCategoryId.Text.Length > 0)
                {
                    Category category = new Category();
                    category.CategoryId = short.Parse(txtCategoryId.Text);
                    category.CategoryName = txtCategoryName.Text;
                    category.CategoryDesciption = txtCategoryDescription.Text ;
                    iCategoryService.UpdateCategory(category);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("You must select a Category !");
            }
            finally
            {
                LoadCategoryList();
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (txtCategoryId.Text.Length > 0)
                {
                    Category category = new Category();
                    category.CategoryId = short.Parse(txtCategoryId.Text);
                    category.CategoryName = txtCategoryName.Text;
                    category.CategoryDesciption = txtCategoryName.Text;
                    iCategoryService.DeleteCategory(category);
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
    }
    
}
