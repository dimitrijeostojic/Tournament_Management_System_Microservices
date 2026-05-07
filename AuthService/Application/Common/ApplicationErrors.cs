using Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common;

public static class ApplicationErrors
{
    public static readonly Error NotFound = new("Auth.NotFound", "The requested resource was not found.");
    public static readonly Error InvalidCredentials = new("Auth.InvalidCredentials", "Invalid email or password.");
    public static readonly Error EmailAlreadyExists = new("Auth.EmailAlreadyExists", "A user with this email already exists.");
    public static readonly Error RegistrationFailed = new("Auth.RegistrationFailed", "User registration failed.");
    public static readonly Error InvalidRefreshToken = new("Auth.InvalidRefreshToken", "Refresh token is invalid or has expired.");
    public static readonly Error InvalidTwoFactorCode = new("Auth.InvalidTwoFactorCode", "The two-factor authentication code is invalid.");
    public static readonly Error TwoFactorNotEnabled = new("Auth.TwoFactorNotEnabled", "Two-factor authentication is not enabled for this user.");
    public static readonly Error UnauthorizedRole = new("Auth.UnauthorizedRole", "Access is restricted to administrators and managers only.");
}
