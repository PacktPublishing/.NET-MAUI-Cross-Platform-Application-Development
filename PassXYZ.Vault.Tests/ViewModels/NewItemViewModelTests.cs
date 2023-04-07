using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Core.UnitTests;
using Microsoft.Maui.Controls.PlatformConfiguration.iOSSpecific;
using Moq;
using PassXYZ.Vault.Models;
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
        NewItemViewModel viewModel;
        string itemName = "Item Name";
        string itemDescription = "Item Description";
        TestShell shell;
        public NewItemViewModelTests() 
        {
            logger = LoggerFactory.Create(options => { }).CreateLogger<NewItemViewModel>();
            dataStore = new MockDataStore();
            viewModel = new(dataStore, logger);
            viewModel.Name = itemName;
            viewModel.Description = itemDescription;

            Routing.RegisterRoute("RelativeGoTo_Page1", typeof(ContentPage));
            Routing.RegisterRoute("RelativeGoTo_Page2", typeof(ContentPage));

            Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));

            shell = new TestShell();

            var one = new ShellItem { Route = "one" };
            var two = new ShellItem { Route = "two" };

            var tab11 = MakeSimpleShellSection("tab11", "content");
            var tab21 = MakeSimpleShellSection("tab21", "content");

            one.Items.Add(tab11);
            two.Items.Add(tab21);

            shell.Items.Add(one);
            shell.Items.Add(two);

            var mockApp = new Mock<Microsoft.Maui.Controls.Application>();
            app = mockApp.Object;
            app.MainPage = shell;
        }

        [Fact]
        public void CannotAddNewItem() 
        {
            viewModel.Name = null;
            Assert.False(viewModel.SaveCommand.CanExecute(null));
        }

        [Fact]
        public void CanAddNewItem()
        {
            Assert.True(viewModel.SaveCommand.CanExecute(null));
        }

        [Fact]
        public async void CancelNewItem() 
        {
            await shell.GoToAsync("//two/tab21/");
            viewModel.CancelCommand.Execute(null);
            Assert.Equal("//two/tab21/content", Shell.Current.CurrentState.Location.ToString());
        }

        [Fact]
        public async void SaveNewItem()
        {
            await shell.GoToAsync("//two/tab21/");
            viewModel.SaveCommand.Execute(null);
            Assert.Equal("//two/tab21/content", Shell.Current.CurrentState.Location.ToString());
        }

        [Fact]
        public void CreateNewItemViewModelSuccessTest()
        {
            Assert.Equal(itemName, viewModel.Name);
            Assert.Equal(itemDescription, viewModel.Description);
        }

        [Fact]
        public void CreateNewItemViewModelFailureTest()
        {
            IDataStore<Item> dataStore = null;
            NewItemViewModel viewModel1;
            var ex = Assert.Throws<ArgumentNullException>(() => viewModel1 = new NewItemViewModel(dataStore, logger));
            Assert.Equal("Value cannot be null. (Parameter 'dataStore')", ex.Message);
        }
    }
}
