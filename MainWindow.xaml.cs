using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Assignment11._1._1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private AppDbContext _ctx = new AppDbContext();

        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
            Closed += (s, e) => _ctx.Dispose();
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Create DB/table if missing; for real apps prefer migrations
            _ctx.Database.EnsureCreated();

            // Load data into context's local view (observable)
            _ctx.Books.Load();

            // Bind DataGrid to EF Core local collection (observable)
            BooksGrid.ItemsSource = _ctx.Books.Local.ToObservableCollection();

            // (Optional) seed one record if table is empty
            if (!_ctx.Books.Any())
            {
                _ctx.Books.Add(new Book
                {
                    ISBN = "9780131103627",
                    Name = "The C Programming Language",
                    Author = "Kernighan & Ritchie",
                    Description = "Classic on C programming."
                });
                _ctx.SaveChanges();
            }
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            var isbn = txtISBN.Text.Trim();
            var name = txtName.Text.Trim();
            var author = txtAuthor.Text.Trim();
            var desc = txtDescription.Text.Trim();

            // Basic validation
            if (string.IsNullOrWhiteSpace(isbn))
            {
                MessageBox.Show("ISBN is required.");
                txtISBN.Focus();
                return;
            }
            if (string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show("Name is required.");
                txtName.Focus();
                return;
            }
            if (string.IsNullOrWhiteSpace(author))
            {
                MessageBox.Show("Author is required.");
                txtAuthor.Focus();
                return;
            }

            var book = new Book
            {
                ISBN = isbn,
                Name = name,
                Author = author,
                Description = desc
            };

            _ctx.Books.Add(book);

            try
            {
                _ctx.SaveChanges();
                ClearInputs();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Add failed: " + ex.Message);
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (BooksGrid.SelectedItem is Book book)
            {
                if (MessageBox.Show($"Delete '{book.Name}' ({book.ISBN})?",
                                    "Confirm", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    _ctx.Books.Remove(book);
                    try { _ctx.SaveChanges(); }
                    catch (Exception ex) { MessageBox.Show("Delete failed: " + ex.Message); }
                }
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _ctx.SaveChanges();
                MessageBox.Show("Changes saved.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Save failed: " + ex.Message);
            }
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            _ctx.Dispose();
            _ctx = new AppDbContext();
            _ctx.Database.EnsureCreated();
            _ctx.Books.Load();
            BooksGrid.ItemsSource = _ctx.Books.Local.ToObservableCollection();
        }

        private void ClearInputs()
        {
            txtISBN.Clear();
            txtName.Clear();
            txtAuthor.Clear();
            txtDescription.Clear();
            txtISBN.Focus();
        }
    }
}