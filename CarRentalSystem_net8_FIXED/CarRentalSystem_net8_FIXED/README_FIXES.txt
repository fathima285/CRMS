# Applied Fixes

- AccountController.cs: Patched to use BCrypt hashing/verify and removed plain-text compare.
- User.cs: Added unique index, ensured lengths suitable for SQL Server & hashing.
- Views/Account/Login.cshtml: Password field uses type="password".
- Views/Account/Register.cshtml: Password fields use type="password".
- Services/IEmailService.cs: Added interface.
- Services/SmtpEmailService.cs: Added SMTP email implementation.