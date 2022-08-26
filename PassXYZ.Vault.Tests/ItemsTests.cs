using KPCLib;
using PassXYZLib;
using Moq;
using PassXYZ.Vault.Services;
using PassXYZ.Vault.Pages;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using KeePassLib;
using System.Collections.ObjectModel;
using Svg.ExCSS;

namespace PassXYZ.Vault.Tests
{
    public class ItemsTests : TestContext
    {
        static string[] jsonBrokers = {
        "pxtem://{'IsPxEntry':false,'Strings':{'Title':{'Value':'Fidelity','IsProtected':false},'UserName':{'Value':'test1','IsProtected':false},'Password':{'Value':'12345','IsProtected':true},'Mobile':{'Value':'1234567890','IsProtected':false},'Notes':{'Value':'Bank','IsProtected':false},'PIN':{'Value':'1234','IsProtected':true},'URL':{'Value':'https://www.fidelity.com','IsProtected':false}}}",
        "pxtem://{'IsPxEntry':false,'Strings':{'Title':{'Value':'Ameritrade','IsProtected':false},'UserName':{'Value':'test1','IsProtected':false},'Password':{'Value':'12345','IsProtected':true},'Mobile':{'Value':'1234567890','IsProtected':false},'Notes':{'Value':'Bank','IsProtected':false},'PIN':{'Value':'1234','IsProtected':true},'URL':{'Value':'https://www.tdameritrade.com/','IsProtected':false}}}",
        "pxtem://{'IsPxEntry':false,'Strings':{'Title':{'Value':'E*TRADE','IsProtected':false},'UserName':{'Value':'test1','IsProtected':false},'Password':{'Value':'12345','IsProtected':true},'Mobile':{'Value':'1234567890','IsProtected':false},'Notes':{'Value':'Bank','IsProtected':false},'PIN':{'Value':'1234','IsProtected':true},'URL':{'Value':'https://us.etrade.com/','IsProtected':false}}}"
        };
        PwGroup rootGroup;
        PwGroup childGroup1;
        PxEntry entry1;
        PxEntry entry2;
        PxEntry entry3;
        Mock<IDataStore<Item>> dataStore;
        public ItemsTests() 
        {
            rootGroup = new PwGroup(true, true)
            {
                Name = "ItemsTests Root Group",
                Notes = "This is the root group in ItemsTests."
            };
            entry1 = new PxEntry(jsonBrokers[0]);
            entry2 = new PxEntry(jsonBrokers[1]);
            entry3 = new PxEntry(jsonBrokers[2]);
            childGroup1 = new PwGroup(true, true)
            {
                Name = "Child Group1",
                Notes = "This is a child group in ItemsTests."
            };

            dataStore = new Mock<IDataStore<Item>>();
            dataStore.SetupGet(x => x.RootGroup).Returns(rootGroup);

            Services.AddSingleton<IDataStore<Item>>(dataStore.Object);
        }

        [Fact]
        public void Load_RootGroup_Success()
        {
            // Arrange
            List<Item> items = new List<Item>();
            items.Add(entry1);
            items.Add(childGroup1);
            dataStore.Setup(x => x.GetItemsAsync(true)).Returns(Task.FromResult((IEnumerable<Item>)items));
            dataStore.Setup(x => x.CurrentGroup).Returns(rootGroup);

            // Act
            var cut = RenderComponent<Items>();

            // Assert
            Assert.Equal(rootGroup.Name, cut.Instance.Title);
            Assert.Null(cut.Instance.SelectedItemId);

            Debug.WriteLine($"Load_RootGroup_Success: {cut.Instance.Title}");
        }

        [Fact]
        public void Load_ChildGroup1_Failure() 
        {
            // Arrange
            List<Item> items = new List<Item>();
            items.Add(entry1);
            items.Add(childGroup1);
            dataStore.Setup(x => x.GetItemsAsync(true)).Returns(Task.FromResult((IEnumerable<Item>)items));
            dataStore.Setup(x => x.CurrentGroup).Returns(rootGroup);

            // Act
            var cut = RenderComponent<Items>(parameters => parameters.Add(p => p.SelectedItemId, childGroup1.Id));

            // Assert
            Assert.Null(cut.Instance.Title);
        }

        [Fact]
        public void Load_ChildGroup1_Success() 
        {
            // Arrange
            List<Item> items = new List<Item>();
            items.Add(entry2);
            dataStore.Setup(x => x.GetItemsAsync(true)).Returns(Task.FromResult((IEnumerable<Item>)items));
            dataStore.Setup(x => x.CurrentGroup).Returns(childGroup1);
            dataStore.Setup(x => x.GetItem(It.IsAny<string>(), It.IsAny<bool>())).Returns(childGroup1);

            // Act
            var cut = RenderComponent<Items>(parameters => parameters.Add(p => p.SelectedItemId, childGroup1.Id));

            // Assert
            Assert.Equal(childGroup1.Name, cut.Instance.Title);
            Assert.Equal(childGroup1.Id, cut.Instance.SelectedItemId);

            Debug.WriteLine($"Load_ChildGroup1_Success: {cut.Instance.Title}");
        }
    }
}
