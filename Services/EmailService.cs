using SendGrid;
using SendGrid.Helpers.Mail;

namespace VerseCraft.Services
{
    public class EmailService
    {
        private readonly string _apiKey;
        private readonly EmailAddress _from;

        public EmailService(IConfiguration config)
        {
            _apiKey = Environment.GetEnvironmentVariable("SENDGRID_API_KEY")
                ?? throw new InvalidOperationException("SendGrid API key is missing.");
            var fromEmail = config["EmailSettings:FromEmail"] ?? "no-reply@versecraft.com";
            var fromName = config["EmailSettings:FromName"] ?? "VerseCraft";
            _from = new EmailAddress(fromEmail, fromName);
        }

        public async Task SendOtpAsync(string toEmail, string otp)
        {
            var client = new SendGridClient(_apiKey);
            var to = new EmailAddress(toEmail);
            var subject = "Your VerseCraft OTP Code";

            // HTML content with the OTP and a confirmation link
            var htmlContent = $@"
                <html>
                    <body style='font-family: Arial, sans-serif;'>
                        <div style='background-color: #f4f4f9; padding: 20px;'>
                            <div style='background-color: #fff; border-radius: 10px; padding: 20px;'>
                                <h2 style='color: #FF6F91;'>Welcome to VerseCraft!</h2>
                                <p style='font-size: 16px; color: #555;'>Thank you for registering. To verify your account, please use the OTP code below:</p>
                                <h3 style='font-size: 24px; color: #333;'>{otp}</h3>
                                <p style='font-size: 16px; color: #555;'>The OTP code is valid for 10 minutes.</p>
                                <p style='font-size: 16px; color: #555;'>If you did not request this verification, please ignore this email.</p>
                                <p style='margin-top: 20px; text-align: center;'>
                                    <a href='https://yourdomain.com/confirm-email?userId={toEmail}' 
                                       style='padding: 10px 20px; background-color: #FF6F91; color: white; text-decoration: none; border-radius: 5px;'>
                                        Click here to confirm your email
                                    </a>
                                </p>
                                <p style='font-size: 14px; color: #999; text-align: center;'>If the button doesn't work, copy and paste this URL into your browser:</p>
                                <p style='font-size: 14px; color: #999; text-align: center;'>https://yourdomain.com/confirm-email?userId={toEmail}</p>
                            </div>
                        </div>
                    </body>
                </html>";

            var plainTextContent = $"Your OTP code is: {otp}. It will expire in 10 minutes.";

            var msg = MailHelper.CreateSingleEmail(_from, to, subject, plainTextContent, htmlContent);
            var response = await client.SendEmailAsync(msg);

            if (!response.IsSuccessStatusCode)
            {
                var errorDetails = await response.Body.ReadAsStringAsync();
                throw new Exception($"Failed to send email. Status: {response.StatusCode}, Details: {errorDetails}");
            }
        }

        public async Task SendPasswordResetAsync(string toEmail, string resetLink)
        {
            var client = new SendGridClient(_apiKey);
            var to = new EmailAddress(toEmail);
            var subject = "VerseCraft Password Reset Request";

            var htmlContent = $@"
        <html>
            <body style='font-family: Arial, sans-serif;'>
                <div style='background-color: #f4f4f9; padding: 20px;'>
                    <div style='background-color: #fff; border-radius: 10px; padding: 20px;'>
                        <h2 style='color: #FF6F91;'>Reset Your VerseCraft Password</h2>
                        <p style='font-size: 16px; color: #555;'>We received a request to reset your password. Click the button below to continue:</p>
                        <p style='margin-top: 20px; text-align: center;'>
                            <a href='{resetLink}' 
                               style='padding: 10px 20px; background-color: #FF6F91; color: white; text-decoration: none; border-radius: 5px;'>
                                Reset Password
                            </a>
                        </p>
                        <p style='font-size: 14px; color: #999; text-align: center;'>If the button doesn't work, copy and paste this URL into your browser:</p>
                        <p style='font-size: 14px; color: #999; text-align: center;'>{resetLink}</p>
                        <p style='font-size: 16px; color: #555;'>This link will expire in 10 minutes.</p>
                        <p style='font-size: 16px; color: #555;'>If you didn't request a password reset, you can safely ignore this email.</p>
                    </div>
                </div>
            </body>
        </html>";

            var plainTextContent = $"Click the following link to reset your VerseCraft password: {resetLink}";

            var msg = MailHelper.CreateSingleEmail(_from, to, subject, plainTextContent, htmlContent);
            var response = await client.SendEmailAsync(msg);

            if (!response.IsSuccessStatusCode)
            {
                var errorDetails = await response.Body.ReadAsStringAsync();
                throw new Exception($"Failed to send password reset email. Status: {response.StatusCode}, Details: {errorDetails}");
            }
        }

        public async Task SendContactMessageAsync(string senderName, string senderEmail, string messageContent)
        {
            var client = new SendGridClient(_apiKey);
            var to = new EmailAddress("apixnull@gmail.com", "VerseCraft Admin"); // Replace with your real email
            var subject = $"New Contact Form Message from {senderName}";

            var plainTextContent = $"From: {senderName} ({senderEmail})\n\nMessage:\n{messageContent}";

            var htmlContent = $@"
        <html>
            <body style='font-family: Arial, sans-serif;'>
                <div style='background-color: #f4f4f9; padding: 20px;'>
                    <div style='background-color: #fff; border-radius: 10px; padding: 20px;'>
                        <h2 style='color: #FF6F91;'>New Contact Message</h2>
                        <p><strong>Name:</strong> {senderName}</p>
                        <p><strong>Email:</strong> {senderEmail}</p>
                        <h4>Message:</h4>
                        <p>{messageContent}</p>
                    </div>
                </div>
            </body>
        </html>";

            var msg = MailHelper.CreateSingleEmail(_from, to, subject, plainTextContent, htmlContent);
            var response = await client.SendEmailAsync(msg);

            if (!response.IsSuccessStatusCode)
            {
                var errorDetails = await response.Body.ReadAsStringAsync();
                throw new Exception($"Failed to send contact message. Status: {response.StatusCode}, Details: {errorDetails}");
            }
        }

    }
}
