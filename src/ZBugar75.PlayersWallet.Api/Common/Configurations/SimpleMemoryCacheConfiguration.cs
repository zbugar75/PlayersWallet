using System;
using System.ComponentModel.DataAnnotations;
using NetEscapades.Configuration.Validation;

namespace Zbugar75.PlayersWallet.Api.Common.Configurations
{
    public class SimpleMemoryCacheConfiguration : IValidatable
    {
        [Range(1, int.MaxValue, ErrorMessage = "Configuration " + nameof(SimpleMemoryCacheConfiguration) + "." + nameof(SizeLimit) + " is required with a value > 0")]
        public int SizeLimit { get; set; }

        [Required]
        public TimeSpan SlidingExpiration { get; set; }

        [Required]
        public TimeSpan AbsoluteExpiration { get; set; }

        public void Validate()
        {
            Validator.ValidateObject(this, new ValidationContext(this), validateAllProperties: true);
        }
    }

}
