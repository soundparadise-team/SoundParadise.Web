using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using SoundParadise.Api.Options;

namespace SoundParadise.Api.Services;

/// <summary>
///     Smtp service.
/// </summary>
public class SmtpService
{
    private readonly IOptions<SmtpOptions> _options;

    /// <summary>
    ///     Constructor for smtp service.
    /// </summary>
    /// <param name="options">Smtp option.</param>
    public SmtpService(IOptions<SmtpOptions> options)
    {
        _options = options;
    }

    /// <summary>
    ///     Confirmation email.
    /// </summary>
    /// <param name="recipientEmail">Email.</param>
    /// <param name="confirmationLink">Confirmation link.</param>
    /// <returns>Task object.</returns>
    public async Task SendConfirmationEmail(string recipientEmail, string confirmationLink)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(_options.Value.Name, _options.Value.Email));
        message.To.Add(new MailboxAddress("", recipientEmail));
        message.Subject = "Підтвердження аккаунту";

        var bodyBuilder = new BodyBuilder
        {
            HtmlBody = @"
        <!DOCTYPE html>
        <html>
        <head>
            <meta charset='UTF-8'>
            <title>Підтвердження аккаунту</title>
        </head>
        <body>
            <h2>Підтвердження аккаунту</h2>
            <h4>Вітаю козаче, ти тепер зареєстрований на нашому сайті!</h1>
            <p>Будь-ласка натисніть цю кнопку щоб підтвердити аккаунт:</p>
            <p>
                <a href='" + confirmationLink +
                       @"' style='display: inline-block; padding: 10px 20px; background-color: #007bff; color: #fff; text-decoration: none; border-radius: 4px;'>Підтвердити аккаунт</a>
            </p>
        </body>
        </html>"
        };

        message.Body = bodyBuilder.ToMessageBody();

        using var client = new SmtpClient();
        await client.ConnectAsync(_options.Value.Host, _options.Value.Port, SecureSocketOptions.StartTls);
        await client.AuthenticateAsync(_options.Value.Email, _options.Value.Password);
        await client.SendAsync(message);
        await client.DisconnectAsync(true);
    }

    /// <summary>
    ///     Send password change email.
    /// </summary>
    /// <param name="recipientEmail">Email.</param>
    /// <param name="confirmationLink">Confirmation link.</param>
    /// <returns>Task object.</returns>
    public async Task SendPasswordChangeEmail(string recipientEmail, string confirmationLink)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(_options.Value.Name, _options.Value.Email));
        message.To.Add(new MailboxAddress("", recipientEmail));
        message.Subject = "Зміна паролю";

        var bodyBuilder = new BodyBuilder
        {
            HtmlBody = @"
        <!DOCTYPE html>
        <html>
        <head>
            <meta charset='UTF-8'>
            <title>Зміна паролю</title>
        </head>
        <body>
            <h2>Зміна паролю</h2>
            <h4>Вітаю козаче, ти змінив пароль на нашому сайті!</h1>
            <p>Будь-ласка натисніть цю кнопку щоб підтвердити зміну паролю:</p>
            <p>
                <a href='" + confirmationLink +
                       @"' style='display: inline-block; padding: 10px 20px; background-color: #007bff; color: #fff; text-decoration: none; border-radius: 4px;'>Підтвердити зміну паролю</a>
            </p>
        </body>
        </html>"
        };

        message.Body = bodyBuilder.ToMessageBody();

        using var client = new SmtpClient();
        await client.ConnectAsync(_options.Value.Host, _options.Value.Port, SecureSocketOptions.StartTls);
        await client.AuthenticateAsync(_options.Value.Email, _options.Value.Password);
        await client.SendAsync(message);
        await client.DisconnectAsync(true);
    }

    /// <summary>
    ///     Send order confirmation url.
    /// </summary>
    /// <param name="recipientEmail">Email.</param>
    /// <param name="orderNumber">Order nubmer.</param>
    /// <returns>Task object.</returns>
    public async Task SendOrderConfirmationEmail(string recipientEmail, string orderNumber)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(_options.Value.Name, _options.Value.Email));
        message.To.Add(new MailboxAddress("", recipientEmail));

        var bodyBuilder = new BodyBuilder
        {
            HtmlBody = $@"
        <!DOCTYPE html>
        <html>
        <head>
            <meta charset='UTF-8'>
            <title>Замовлення {orderNumber}</title>
        </head>
        <body>
            <h2>Замовлення {orderNumber}</h2>
            <h4>Дякуемо за те що зробили замовлення на нашому сайті!</h1>
            <p>Номер замовлення: " + orderNumber + @"</p>
        </body>
        </html>"
        };

        message.Body = bodyBuilder.ToMessageBody();

        using var client = new SmtpClient();
        await client.ConnectAsync(_options.Value.Host, _options.Value.Port, SecureSocketOptions.StartTls);
        await client.AuthenticateAsync(_options.Value.Email, _options.Value.Password);
        await client.SendAsync(message);
        await client.DisconnectAsync(true);
    }
}