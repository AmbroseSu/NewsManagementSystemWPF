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
    /// Interaction logic for CreateCategoryUI.xaml
    /// </summary>
    public partial class CreateCategoryUI : Window
    {
        private readonly ICategoryService iCategoryService;
        private Category categoryToUpdate;
        public CreateCategoryUI()
        {
            InitializeComponent();
            iCategoryService = new CategoryService();
            Loaded += Window_Loaded;
        }

        public CreateCategoryUI(Category selectedCategory)
        {
            InitializeComponent();
            iCategoryService = new CategoryService();
            Loaded += Window_Loaded;
            categoryToUpdate = selectedCategory;
            
            txtCategoryId.Text = categoryToUpdate.CategoryId.ToString();
            txtCategoryName.Text = categoryToUpdate.CategoryName;
            txtCategoryDescription.Text = categoryToUpdate.CategoryDesciption;
        }


        private void Window_Loaded(Object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtCategoryId.Text))
            {
                txtCategoryId.IsEnabled = false;
                btnCreateCategory.IsEnabled = false;
            }
            else
            {
                btnUpdateCategory.IsEnabled = false;
            }
        }

        private void resetInput()
        {
            if (!string.IsNullOrEmpty(txtCategoryId.Text))
            {
                txtCategoryName.Text = "";
                txtCategoryDescription.Text = "";
            }
            else
            {
                txtCategoryId.Text = "";
                txtCategoryName.Text = "";
                txtCategoryDescription.Text = "";
            }
        }

        private void btnCreateCategory_Click(object sender, RoutedEventArgs e)
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
                this.Close();
            }
        }

        private void btnUpdateCategory_Click(object sender, RoutedEventArgs e)
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
