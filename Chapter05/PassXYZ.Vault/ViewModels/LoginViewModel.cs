using System.Collections.ObjectModel;
using System.Diagnostics;

using KPCLib;
using PassXYZLib;
using PassXYZ.Vault.Properties;
using PassXYZ.Vault.Services;
using PassXYZ.Vault.Views;

namespace PassXYZ.Vault.ViewModels;

public class LoginViewModel : BaseViewModel
{
    readonly IUserService<User> userService = LoginUser.UserService;
    private Action<string> _signUpAction;
    public Command LoginCommand { get; }
    public Command SignUpCommand { get; }
    public Command CancelCommand { get; }
    public LoginUser CurrentUser
    {
        get
        {
            return LoginUser.Instance;
        }
    }
    public ObservableCollection<User>? Users 
    {
        get
        {
            return userService.Users;
        }
    }

    public LoginViewModel()
    {
        LoginCommand = new Command(OnLoginClicked, ValidateLogin);
        SignUpCommand = new Command(OnSignUpClicked, ValidateSignUp);
        CancelCommand = new Command(OnCancelClicked);
        CurrentUser.PropertyChanged +=
            (_, __) => LoginCommand.ChangeCanExecute();

        CurrentUser.PropertyChanged +=
            (_, __) => SignUpCommand.ChangeCanExecute();

        Debug.WriteLine($"data_path={PxDataFile.DataFilePath}");
    }

    public LoginViewModel(Action<string> signUpAction) : this()
    {
        _signUpAction = signUpAction;
    }

    private bool ValidateLogin()
    {
        Debug.WriteLine($"LoginViewModel: ValidateLogin username={CurrentUser.Username}, password={CurrentUser.Password}");
        return !string.IsNullOrWhiteSpace(CurrentUser.Username)
#if PASSXYZ_PRIVACYNOTICE_REQUIRED
            && LoginUser.IsPrivacyNoticeAccepted
#endif
            && !string.IsNullOrWhiteSpace(CurrentUser.Password);
    }

    private bool ValidateSignUp()
    {
        return !string.IsNullOrWhiteSpace(CurrentUser.Username)
            && !string.IsNullOrWhiteSpace(CurrentUser.Password)
            && !string.IsNullOrWhiteSpace(CurrentUser.Password2)
#if PASSXYZ_PRIVACYNOTICE_REQUIRED
            && LoginUser.IsPrivacyNoticeAccepted
#endif
            && CurrentUser.Password.Equals(CurrentUser.Password2);
    }

    public void OnAppearing()
    {
        IsBusy = false;
    }

    public async void OnLoginClicked()
    {
        try
        {
            IsBusy = true;

            if (string.IsNullOrWhiteSpace(CurrentUser.Password))
            {
                await Shell.Current.DisplayAlert("", Properties.Resources.settings_empty_password, Properties.Resources.alert_id_ok);
                IsBusy = false;
                return;
            }

            bool status = await userService.LoginAsync(CurrentUser);

            if (status)
            {
                if (AppShell.CurrentAppShell != null)
                {
                    AppShell.CurrentAppShell.SetRootPageTitle(DataStore.RootGroup.Name);

                    string path = Path.Combine(PxDataFile.TmpFilePath, CurrentUser.FileName);
                    if (File.Exists(path))
                    {
                        // If there is file to merge, we merge it first.
                        bool result = await DataStore.MergeAsync(path);
                    }

                    await Shell.Current.GoToAsync($"//RootPage");
                }
                else
                {
                    throw (new NullReferenceException("CurrentAppShell is null"));
                }
            }
            IsBusy = false;
        }
        catch (Exception ex)
        {
            IsBusy = false;
            string msg = ex.Message;
            if (ex is System.IO.IOException ioException)
            {
                Debug.WriteLine("LoginViewModel: Need to recover");
                msg = Properties.Resources.message_id_recover_datafile;
            }
            await Shell.Current.DisplayAlert(Properties.Resources.LoginErrorMessage, msg, Properties.Resources.alert_id_ok);
        }
    }

    private async void OnSignUpClicked()
    {
        if (CurrentUser.IsUserExist)
        {
            await Shell.Current.DisplayAlert(Properties.Resources.SignUpPageTitle, Properties.Resources.SignUpErrorMessage1, Properties.Resources.alert_id_ok);
            return;
        }

        try
        {
            await userService.AddUserAsync(CurrentUser);
            _signUpAction?.Invoke(CurrentUser.Username);
            _ = await Shell.Current.Navigation.PopModalAsync();
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert(Properties.Resources.SignUpPageTitle, ex.Message, Properties.Resources.alert_id_ok);
        }
        Debug.WriteLine($"LoginViewModel: OnSignUpClicked {CurrentUser.Username}, DeviceLock: {CurrentUser.IsDeviceLockEnabled}");
    }

    private async void OnCancelClicked()
    {
        _ = await Shell.Current.Navigation.PopModalAsync();
    }

    public async void ImportKeyFile()
    {
        var options = new PickOptions
        {
            PickerTitle = Properties.Resources.import_message1,
            //FileTypes = customFileType,
        };

        try
        {
            var result = await FilePicker.PickAsync(options);
            if (result != null)
            {
                var stream = await result.OpenReadAsync();
                var fileStream = File.Create(CurrentUser.KeyFilePath);
                stream.Seek(0, SeekOrigin.Begin);
                stream.CopyTo(fileStream);
                fileStream.Close();
            }
            else
            {
                await Shell.Current.DisplayAlert(Properties.Resources.action_id_import, Properties.Resources.import_error_msg, Properties.Resources.alert_id_ok);
            }
        }
        catch (Exception ex)
        {
            // The user canceled or something went wrong
            Debug.WriteLine($"LoginViewModel: ImportKeyFile, {ex}");
        }
    }

    public string GetMasterPassword()
    {
        return userService.GetMasterPassword();
    }

    public string GetDeviceLockData()
    {
        return userService.GetDeviceLockData();
    }

    public bool CreateKeyFile(string data)
    {
        return userService.CreateKeyFile(data, CurrentUser.Username);
    }

    public List<string> GetUsersList()
    {
        return userService.GetUsersList();
    }
}
