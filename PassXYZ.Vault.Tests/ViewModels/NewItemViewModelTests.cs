using Microsoft.Extensions.Logging;
using Microsoft.Maui.Controls.Core.UnitTests;
using NSubstitute;
using KPCLib;
using PassXYZLib;
using PassXYZ.Vault.Services;
using PassXYZ.Vault.ViewModels;
using PassXYZ.Vault.Views;

namespace PassXYZ.Vault.Tests.ViewModels
{
    public class NewItemViewModelTests : ShellTestBase
    {
        Microsoft.Maui.Controls.Application app;
        ILogger<NewItemViewModel> logger;
        readonly IDataStore<Item> dataStore;
        TestShell shell;

        public NewItemViewModelTests() 
        {
            using ILoggerFactory loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            logger = loggerFactory.CreateLogger<NewItemViewModel>();
            dataStore = new DataStore();

            Routing.RegisterRoute(nameof(ItemDetailPage), typeof(ContentPage));
            Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));
            shell = new TestShell();
            var aboutPage = new ShellItem { Route = "About" };
            var page = MakeSimpleShellSection("Maui", "content");
            aboutPage.Items.Add(page);
            shell.Items.Add(aboutPage);

            app = Substitute.For<Microsoft.Maui.Controls.Application>();
            app.MainPage = shell;
        }

        [Fact]
        public void CannotAddNewItem() 
        {
            NewItemViewModel vm = new(dataStore, logger);
            Assert.False(vm.SaveCommand.CanExecute(null));
        }

        [Fact]
        public void CanAddNewItem()
        {
            NewItemViewModel vm = new(dataStore, logger);
            vm.Name = "New item";
            vm.Description = "Can add new item";
            Assert.True(vm.SaveCommand.CanExecute(null));
        }

        [Fact]
        public async void CancelNewItem() 
        {
            NewItemViewModel vm = new(dataStore, logger);
            await shell.GoToAsync("//About/Maui/");
            vm.CancelCommand.Execute(null);
            Assert.Equal("//About/Maui/content", Shell.Current.CurrentState.Location.ToString());
        }

        [Fact]
        public async void SaveNewItem()
        {
            // Arrange
            var user = new User();
            user.Username = "test1";
            user.Password = "12345";
            bool result = await dataStore.ConnectAsync(user);
            Assert.True(result);
            dataStore.SetCurrentGroup();
            await shell.GoToAsync("//About/Maui/");
            var items = await dataStore.GetItemsAsync(true);
            int Count1 = items.Count();
            NewItemViewModel vm = new(dataStore, logger);
            vm.Name = "New item";
            vm.Description = "Testing save command";
            // Act
            vm.SaveCommand.Execute(null);
            items = await dataStore.GetItemsAsync(true);
            int Count2 = items.Count();
            // Assert
            Assert.Equal(Count1 + 1, Count2);
        }

        [Fact]
        public void CreateNewItemViewModelSuccessTest()
        {
            // Arrange and Act
            NewItemViewModel vm = new(dataStore, logger);
            vm.Name = "New item";
            vm.Description = "This is a new item";
            // Assert
            Assert.Equal("New item", vm.Name);
            Assert.Equal("This is a new item", vm.Description);
        }

        [Fact]
        public void CreateNewItemViewModelFailureTest()
        {
            IDataStore<Item>? dataStore = null;
            NewItemViewModel viewModel1;
#pragma warning disable CS8604 // Possible null reference argument.
            var ex = Assert.Throws<ArgumentNullException>(() => viewModel1 = new NewItemViewModel(dataStore, logger));
#pragma warning restore CS8604 // Possible null reference argument.
            Assert.Equal("Value cannot be null. (Parameter 'dataStore')", ex.Message);
        }
    }
}
