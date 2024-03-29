﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using WebAppRepositoryWithUOW.EF.IdentityModels;

namespace WebApplication1.Areas.Identity.Pages.Account.Manage
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public IndexModel(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public string Username { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required, MaxLength(50), Display(Name = "First Name")]
            public string FirstName { get; set; }

            [Required, MaxLength(50), Display(Name = "Last Name")]
            public string LastName { get; set; }

            [Phone]
            [Display(Name = "Phone number")]
            public string PhoneNumber { get; set; }

            [Display(Name = "Profile Picture")]
            public byte[] ProfilePicture { get; set; }
        }

        private void LoadAsync(AppUser user)
        {
            Username = user.UserName;

            Input = new InputModel
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber,
                ProfilePicture = user.ProfilePicture,
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(IFormFile file)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                LoadAsync(user);
                return Page();
            }

            if (file is not null)
            {
                var extentions = new string[] { ".jpg", ".png" };

                if (!extentions.Contains(Path.GetExtension(file.FileName).ToLower()))    //check file extention
                {
                    ModelState.AddModelError("imagProfilePicturee", "only .jpg, .png image are allowed");
                    LoadAsync(user);
                    return Page();
                }

                if (file.Length > 2097152)    //check file size
                {
                    ModelState.AddModelError("image", "image can not be more than 2 MB");
                    LoadAsync(user);
                    return Page();
                }

                using var dataStream = new MemoryStream();   //creates streams that have memory as a backing store instead of a disk or a network connection 

                await file.CopyToAsync(dataStream);   //copy image to stream memory as a backing store

                user.ProfilePicture = dataStream.ToArray();    //convert file to byte array and save it

                await _userManager.UpdateAsync(user);
            }


            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            var firstName = user.FirstName;
            var lastName = user.LastName;

            if (Input.FirstName != firstName || Input.LastName != lastName)
            {
                user.FirstName = Input.FirstName;
                user.LastName = Input.LastName;
                user.UserName = $"{Input.FirstName}_{Input.LastName}";
                await _userManager.UpdateAsync(user);
            }

            if (Input.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    StatusMessage = "Unexpected error when trying to set phone number.";
                    return RedirectToPage();
                }
            }

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }
    }
}
