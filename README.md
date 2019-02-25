# EmailOrPhoneAttribute
==================================

Modified from Microsoft source code. Merged EmailAddressAttribute and Phone Attribute. Allow users to login with email or phone. The default is email only.

**The latest official release** can be found on [NuGet](https://www.nuget.org/packages/Ezfx.DataAnnotations.EmailOrPhoneAttribute)


[![Build Status](https://travis-ci.org/juwikuang/EmailOrPhoneAttribute.svg?branch=master)](https://travis-ci.org/juwikuang/EmailOrPhoneAttribute)
[![Build status](https://ci.appveyor.com/api/projects/status/1w54o1nv2jotqpbt?svg=true)](https://ci.appveyor.com/project/juwikuang/emailorphoneattribute)
[![Build Status](https://dev.azure.com/juwikuang/EmailOrPhoneAttribute/_apis/build/status/EmailOrPhoneAttribute?branchName=master)](https://dev.azure.com/juwikuang/EmailOrPhoneAttribute/_build/latest?definitionId=2?branchName=master)

## How to use:
Replace the EmailAttribute to EmailOrPhoneAttribute. Change the ErrorMessage to your own language.
```
    public class LoginViewModel
    {
        [Required]
        [EmailOrPhone(ErrorMessage ="Phone or Email not in the right format.")]
        [Display(Name = "Email/Phone")]
        public string EmailOrPhone { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }
```
