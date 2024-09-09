namespace Services;

public class LdapService(Config config)
{
    private readonly Config _config = config;
    private const int SCOPE_SUB = 1;

    public async Task<List<Dictionary<string, string>>> SearchAsync(string searchBase, string[] attributes, string searchFilter)
    {
        LdapConnection? connection = ConnectAsync();
        List<Dictionary<string, string>> list = [];

        await Task.Run(() =>
        {
            if (connection == null)
                return;

            LdapSearchConstraints searchConstraints = connection.SearchConstraints;
            searchConstraints.ReferralFollowing = true;

            ILdapSearchResults search = connection.Search(
                searchBase,
                SCOPE_SUB,
                searchFilter,
                attributes,
                false,
                searchConstraints
            );

            while (search.HasMore())
            {
                var searchResults = new Dictionary<string, string>();
                try
                {
                    LdapEntry entry = search.Next();
                    LdapAttributeSet attributeSet = entry.GetAttributeSet();

                    foreach (LdapAttribute attr in attributeSet)
                    {
                        searchResults[attr.Name.ToLower()] = attr.StringValue;
                    }
                    list.Add(searchResults);
                }
                catch (LdapException e)
                {
                    Console.WriteLine("Error: " + e.LdapErrorMessage);
                }
            }
        });
        return list;
    }

    public LdapConnection? ConnectAsync(string username = "", string password = "")
    {
        LdapConnection ldapConnection = new()
        {
            SecureSocketLayer = false,
            ConnectionTimeout = 10000,
        };
        try
        {
            string host = _config.AD_ADDRESS;
            string domainName = _config.AD_DOMAIN;

            ldapConnection.Connect(host, 389);

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                username = _config.AD_USER;
                password = _config.AD_PASSWORD;
            }

            string formattedName = FormatUsername(username, domainName);
            ldapConnection.Bind(formattedName, password);
            return ldapConnection;
        }
        catch (LdapException)
        {
            return null;
        }
    }

    public bool ValidateAsync(string username, string password)
    {
        LdapConnection? connection = ConnectAsync(username, password);
        connection?.Disconnect();
        return connection != null;
    }

    private static string FormatUsername(string username, string domainName)
    {
        username = username.ToLower();
        if (!username.Contains(domainName))
        {
            username = $"{username}@{domainName}";
        }
        return username;
    }

    public async Task<User?> GetStudentByEmailAsync(string email)
    {
        List<User> users = await GetStudentsAsync();
        return users.FirstOrDefault(u => u.Email.ToLower() == email.ToLower());
    }

    public async Task<List<User>> GetStudentsAsync()
    {
        string searchBase = "OU=HOT,OU=UMS,OU=20_Elever,OU=10_Mercantec,DC=global,DC=local";
        string[] attributes = ["cn", "pager"];
        string searchFilter = "(objectClass=person)";

        List<Dictionary<string, string>> usersData = await SearchAsync(searchBase, attributes, searchFilter);
        List<User> users = [];

        foreach (Dictionary<string, string> userData in usersData)
        {
            string[] username = userData.TryGetValue("cn", out string? name) ? name.Split(" ") : ["", ""]; ;
            User user = new()
            {
                Name = username[0],
                Surname = string.Join(" ", username.Skip(1)),
                Email = userData.TryGetValue("pager", out string? email) ? email : "",
                Password = ""
            };

            users.Add(user);
        }
        return users;
    }

    public async Task<List<User>> GetTeachersAsync()
    {
        string searchBase = "OU=undervisere,OU=Brugere Normale,OU=10_Personale,OU=10_Mercantec,DC=global,DC=local";
        string[] attributes = ["cn", "title", "mail"];
        string searchFilter = "(objectClass=person)";
        string[] titleNeedsToContans = ["Timelærer", "Underviser"];

        List<Dictionary<string, string>> usersData = await SearchAsync(searchBase, attributes, searchFilter);
        List<User> users = [];

        foreach (Dictionary<string, string> userData in usersData)
        {
            if (!userData.TryGetValue("title", out string? title))
                continue;

            if (!title.Contains("Data"))
                continue;

            foreach (string item in titleNeedsToContans)
            {
                if (title.Contains(item))
                {
                    string[] username = userData.TryGetValue("cn", out string? name) ? name.Split(" ") : ["", ""];
                    User user = new()
                    {
                        Name = username[0],
                        Surname = string.Join(" ", username.Skip(1)),
                        Email = userData.TryGetValue("mail", out string? email) ? email : "",
                        Password = "",
                        Role = User.UserRoles.Teacher.ToString(),
                    };

                    users.Add(user);
                    break;
                }
            }
        }
        return users;
    }

    // public async Task<List<StudentGroupCreateDto>> GetGroupsAsync()
    // {
    //     string searchBase = "OU=grupper,OU=HOT,OU=UMS,OU=20_Elever,OU=10_Mercantec,DC=global,DC=local";
    //     string[] attributes = ["cn", "member"];
    //     string searchFilter = "(objectClass=group)";
    //     string[] classNameNeedToContans = ["dt", "it"];

    //     List<Dictionary<string, string>> groupsData = await SearchAsync(searchBase, attributes, searchFilter);
    //     List<StudentGroupCreateDto> groups = [];

    //     foreach (Dictionary<string, string> groupData in groupsData)
    //     {
    //         if (!groupData.TryGetValue("cn", out string? className) && string.IsNullOrEmpty(className))
    //             continue;

    //         if (!groupData.TryGetValue("member", out string? students) && string.IsNullOrEmpty(students))
    //             continue;

    //         if (!classNameNeedToContans.Any(className.Contains))
    //             continue;

    //         StudentGroupCreateDto group = new()
    //         {
    //             StudentsInClass = students,
    //             Class = className.ToLower(),
    //         };

    //         groups.Add(group);
    //     }
    //     return groups;
    // }
}
