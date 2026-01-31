namespace EmployeeManagement.Services
{
    public static class EmailTemplates
    {
        public static string GetHtmlTemplate(string title, string message, string buttonText, string buttonUrl)
        {
            return $@"
                <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; border: 1px solid #e9ecef; border-radius: 8px; overflow: hidden;'>
                    <div style='background-color: #0d6efd; padding: 20px; text-align: center;'>
                        <h1 style='color: #ffffff; margin: 0;'>Team Manager</h1>
                    </div>
                    <div style='padding: 30px; background-color: #ffffff;'>
                        <h2 style='color: #212529;'>{title}</h2>
                        <p style='color: #495057; line-height: 1.6;'>{message}</p>
                        <div style='text-align: center; margin-top: 30px;'>
                            <a href='{buttonUrl}' style='background-color: #0d6efd; color: #ffffff; padding: 12px 25px; text-decoration: none; border-radius: 5px; font-weight: bold; display: inline-block;'>
                                {buttonText}
                            </a>
                        </div>
                        <p style='color: #adb5bd; font-size: 12px; margin-top: 30px; text-align: center;'>
                            If the button doesn't work, copy and paste this link into your browser:<br>
                            <a href='{buttonUrl}' style='color: #0d6efd;'>{buttonUrl}</a>
                        </p>
                    </div>
                    <div style='background-color: #f8f9fa; padding: 15px; text-align: center; color: #6c757d; font-size: 12px;'>
                        &copy; {DateTime.Now.Year} Team Manager. All rights reserved.
                    </div>
                </div>";
        }
    }
}
