﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using AtlasTracker.Models;
using AtlasTracker.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AtlasTracker.Areas.Identity.Pages.Account.Manage
{

    public class IndexModel : PageModel
    {
        private readonly UserManager<BTUser> _userManager;
        private readonly SignInManager<BTUser> _signInManager;
        private readonly IBTFileService _fileService;
        private readonly IConfiguration _configuration;
        public IndexModel(
            UserManager<BTUser> userManager,
            SignInManager<BTUser> signInManager,
            IBTFileService fileService,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _fileService = fileService;
            _configuration = configuration;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData]
        public string StatusMessage { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>

            [Display(Name = "First Name")]
            public string FirstName { get; set; }

            [Display(Name = "Last Name")]
            public string LastName { get; set; }
            
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Phone]
            [Display(Name = "Phone number")]
            public string PhoneNumber { get; set; }

            
            [Display(Name = "Avatar")]
            public byte[] AvatarData { get; set; }

            [Display(Name = "My Image")]
            public IFormFile AvatarFormFile { get; set; }
        }
        private async Task LoadAsync(BTUser user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            var firstName = user.FirstName;
            var lastName = user.LastName;
            var avatarPicture = user.AvatarData;
            var Email = user.Email;
            Username = userName;

            Input = new InputModel
            {
                FirstName = firstName,
                LastName = lastName,
                AvatarData = avatarPicture,
                PhoneNumber = phoneNumber,
                Email = Email
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }
            
            //add last name
            
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            if (Input.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    StatusMessage = "Unexpected error when trying to set phone number.";
                    return RedirectToPage();
                }
            }
            user.FirstName = Input.FirstName;
            user.LastName = Input.LastName;
            //user.Email = Input.Email;
            
            //add avatar

            if (Input.AvatarFormFile != null)
            {
                 user.AvatarData = await _fileService.ConvertFileToByteArrayAsync(Input.AvatarFormFile);
            }
            await _userManager.UpdateAsync(user);
            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToAction("Dashboard", "Home");
        }
    }
}
