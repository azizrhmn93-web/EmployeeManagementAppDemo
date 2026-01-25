using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.Utilities
{
    public class EmailDomainAttribute :
        ValidationAttribute
    {
        private readonly string allowedDomain;

        public EmailDomainAttribute(string allowedDomain)
        {
            this.allowedDomain = allowedDomain;
        }

        public override bool IsValid(object value)
        {
            string[] strings = value.ToString().Split('@');
            return strings[1].ToUpper() == allowedDomain.ToUpper();
        }
    }
}
