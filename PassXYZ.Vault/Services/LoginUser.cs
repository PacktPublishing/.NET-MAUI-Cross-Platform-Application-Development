using System.Diagnostics;

using PassXYZLib;

namespace PassXYZ.Vault.Services;

/// <summary>
/// Current user who is a valid user. The name is stored in the preference. It should be a singleton instance.
/// </summary>
public class LoginUser : PxUser
{
    private const string PrivacyNotice = "Privacy Notice";

    public static bool IsPrivacyNoticeAccepted
    {
        get => Preferences.Get(PrivacyNotice, false);

        set => Preferences.Set(PrivacyNotice, value);
    }

    private bool _isFingerprintEnabled = false;
    public bool IsFingerprintEnabled => _isFingerprintEnabled;

    public static IUserService<User> UserService => DependencyService.Get<IUserService<User>>();

    public override void Logout() 
    {
        UserService.Logout();
    }

    /// <summary>
    /// Get password in secure storage
    /// </summary>
    public async Task<string> GetSecurityAsync()
    {
        if (string.IsNullOrWhiteSpace(Username)) { return string.Empty; }

        string data = await SecureStorage.GetAsync(Username);
        if (string.IsNullOrEmpty(data))
        {
            _isFingerprintEnabled = true;
        }
        return data;
    }

    /// <summary>
    /// Store password in secure storage
    /// </summary>
    public async Task SetSecurityAsync(string passwd)
    {
        if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(passwd)) { return; }

        await SecureStorage.SetAsync(Username, passwd);
    }

    public async Task<bool> DisableSecurityAsync()
    {
        if (string.IsNullOrWhiteSpace(Username)) { return false; }

        try
        {
            string data = await SecureStorage.GetAsync(Username);
            if (data != null)
            {
                return SecureStorage.Remove(Username);
            }
            else
            {
                return false;
            }
        }
        catch (Exception ex)
        {
            // Possible that device doesn't support secure storage on device.
            Debug.WriteLine($"{ex}");
            return false;
        }
    }

    private static LoginUser? instance = null;
    public static LoginUser Instance 
    {
        get 
        {
            if (instance == null) { instance = new LoginUser(); }
            return instance;
        }
    }

    private LoginUser() { }
}
