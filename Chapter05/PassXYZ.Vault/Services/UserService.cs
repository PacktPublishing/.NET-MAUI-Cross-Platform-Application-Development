using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using KeePassLib;
using KPCLib;
using KeePassLib.Collections;
using KeePassLib.Keys;
using KeePassLib.Security;
using KeePassLib.Serialization;
using PassXYZLib;

namespace PassXYZ.Vault.Services
{
    public class UserService : IUserService<User>
    {
        public ObservableCollection<User> Users { get; private set; }
        private static readonly object _sync = new object();
        private static bool _isBusyToLoadUsers = false;
        public bool IsBusyToLoadUsers
        {
            get => _isBusyToLoadUsers;
            private set
            {
                lock (_sync)
                {
                    _isBusyToLoadUsers = value;
                }
            }
        }
        private User? _user = default;
        private readonly PasswordDb db = default!;
        public UserService() 
        {
            db = PasswordDb.Instance;
            Users = new ObservableCollection<User>();
            SynchronizeUsersAsync();
        }

        public User CurrentUser
        {
            get => _user;
        }

        public User GetUser(string username)
        {
            return new User();
        }

        public async Task UpdateUserAsync(User user)
        {
            Debug.WriteLine("UpdateUserAsync");
        }

        public async Task DeleteUserAsync(User user)
        {
            Debug.WriteLine("DeleteUserAsync");
        }

        public List<string> GetUsersList()
        {
            List<string> userList = new List<string>();

            if (Users != null)
            {
                foreach (User user in Users)
                {
                    userList.Add(user.Username);
                }
            }
            return userList;
        }

        public async Task AddUserAsync(PassXYZLib.User user)
        {
            if (user == null) { Debug.Assert(false); throw new ArgumentNullException("user"); }

            var logger = new KPCLibLogger();
            await Task.Run(() => {
                db.New(user);
                // Create a PassXYZ Usage note entry
                PwEntry pe = new PwEntry(true, true);
                pe.Strings.Set(PxDefs.TitleField, new ProtectedString(false, Properties.Resources.entry_id_passxyz_usage));
                pe.Strings.Set(PxDefs.NotesField, new ProtectedString(false, Properties.Resources.about_passxyz_usage));
                //pe.CustomData.Set(Item.TemplateType, ItemSubType.Notes.ToString());
                //pe.CustomData.Set(Item.PxIconName, "ic_entry_passxyz.png");
                pe.SetType(ItemSubType.Notes);
                db.RootGroup.AddEntry(pe, true);

                try
                {
                    logger.StartLogging("Saving database ...", true);
                    db.DescriptionChanged = DateTime.UtcNow;
                    db.Save(logger);
                    logger.EndLogging();
                }
                catch (Exception e)
                {
                    Debug.WriteLine("Failed to save database." + e.Message);
                }
            });

        }

        public async Task<bool> SynchronizeUsersAsync()
        {
            IEnumerable<PxUser> pxUsers = null;

#if PASSXYZ_CLOUD_SERVICE
            if (PxCloudConfig.IsConfigured && PxCloudConfig.IsEnabled)
            {
                if (PassXYZ.Vault.App.IsSshOperationTimeout)
                {
                    // If the last connection is timeout, we load local users first.
                    pxUsers = await PxUser.LoadLocalUsersAsync();
                }
                else 
                {
                    ICloudServices<PxUser> sftp = PxCloudConfig.GetCloudServices();
                    pxUsers = await sftp.SynchronizeUsersAsync();
                }
            }
            else
#endif // PASSXYZ_CLOUD_SERVICE
            {
                pxUsers = await PxUser.LoadLocalUsersAsync(IsBusyToLoadUsers);
            }

            if (pxUsers != null && Users != null)
            {
                IsBusyToLoadUsers = true;
                Users.Clear();
                foreach (PxUser pxUser in pxUsers)
                {
                    Users.Add(pxUser);
                }
                IsBusyToLoadUsers = false;
                if (Users.Count > 0)
                {
                    // We need to check whether the current user at App level exist
                    if (!Users.Contains(App.CurrentUser) && !string.IsNullOrEmpty(App.CurrentUser.Username))
                    {
                        Debug.WriteLine($"LoginViewModel: Username={App.CurrentUser.Username} doesn't existed.");
                        App.CurrentUser.Username = string.Empty;
                    }
                }

                return true;
            }
            else
            {
                Debug.WriteLine("LoginViewModel: SynchronizeUsersAsync failed");
                return false;
            }
        }

        public async Task<bool> LoginAsync(User user)
        {
            if (user == null) { Debug.Assert(false); throw new ArgumentNullException("user"); }
            _user = user;

            return true;
        }

        public void Logout()
        {
            if (db.IsOpen) { db.Close(); }
            Debug.WriteLine("DataStore.Logout(done)");
        }

        public string GetMasterPassword()
        {
            var userKey = db.MasterKey.GetUserKey(typeof(KcpPassword)) as KcpPassword;
            return userKey.Password.ReadString();
        }

        public async Task<bool> ChangeMasterPassword(string newPassword)
        {
            bool result = db.ChangeMasterPassword(newPassword, _user);
            if (result)
            {
                db.MasterKeyChanged = DateTime.UtcNow;
                // Save the database to take effect
                await SaveAsync();
            }
            return result;
        }

        public string GetDeviceLockData()
        {
            return db.GetDeviceLockData(_user);
        }

        /// <summary>
        /// Recreate a key file from a PxKeyData
        /// </summary>
        /// <param name="data">PxKeyData source</param>
        /// <param name="username">username inside PxKeyData source</param>
        /// <returns>true - created key file, false - failed to create key file.</returns>
        public bool CreateKeyFile(string data, string username)
        {
            return db.CreateKeyFile(data, username);
        }

        private async Task SaveAsync()
        {
            Debug.WriteLine($"UserService: SaveAsync");
        }
    }
}
