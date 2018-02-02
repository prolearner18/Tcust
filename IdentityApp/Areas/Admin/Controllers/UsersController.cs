﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using IdentityApp.Models;
using IdentityApp.Models.ManageViewModels;
using IdentityApp.Services;
using IdentityApp.Helper;
using ApplicationCore.Helpers;
using IdentityApp.Views;
using ApplicationCore.Views;

namespace IdentityApp.Areas.Admin.Controllers
{

	public class UsersController : BaseAdminController
	{
		
		private readonly IUserService userService;

		public UsersController(IUserService userService)
		{
		
			this.userService = userService;
		}

		public async Task<IActionResult> Index(string role = "", string keyword = "", int page = 1, int pageSize = 10)
		{
			IdentityRole selectedRole = null;
			if (!String.IsNullOrEmpty(role)) selectedRole = await userService.GetRoleByNameAsync(role);
			if (selectedRole == null) role = "";

			var users = await userService.FetchUsers(selectedRole, keyword);

			users = users.GetOrdered();

			var pageList = users.GetUsersPagedList(page = 1, pageSize);

			foreach (var item in pageList.ViewList)
			{
				var roles = await userService.GetRolesByUserIdAsync(item.id);
				item.roles = String.Join(",", roles.Select(r => r.Name));
			}


			if (Request.IsAjaxRequest())
			{
				return Ok(pageList);
			}

			bool edit = false;
			var roleOptions = GetRoleOptions(edit);

			ViewData["roles"] = this.ToJsonString(roleOptions);

			ViewData["list"] = this.ToJsonString(pageList);

			return View();
		}

		[HttpGet]
		public IActionResult Create()
		{

			var userModel = new IdentityUserViewModel() { profile=new ProfileViewModel() };

			var model = new IdentityUserEditViewModel { user = userModel , roleOptions = GetRoleOptions()};

			model.roles = new string[] { "Staff" };

			return Ok(model);
		}

		[HttpPost("[area]/[controller]")]
		public async Task<IActionResult> Store([FromBody] IdentityUserEditViewModel model)
		{
			if (!ModelState.IsValid) return BadRequest();

			string userName = model.user.name.Trim();
			string role = model.roles.FirstOrDefault();

			var exist = await userService.GetUserByUserNameAsync(userName);

			if (exist != null)
			{
				ModelState.AddModelError("user.name", "UserName重複了");
				return BadRequest(ModelState);
			}

			
			var profile = new Profile
			{
				Fullname = model.user.profile.fullname,
			};

			var user= await userService.CreateUserAsync(userName, role, profile);

			return Ok(user);


		}

		[HttpGet("[area]/[controller]/{id}/edit")]
		public async Task<IActionResult> Edit(string id)
		{
			var user = userService.GetUserById(id);
			if (user == null) return NotFound();

			var userModel = user.MapIdentityUserViewModel();

			var model = new IdentityUserEditViewModel { user = userModel, roleOptions = GetRoleOptions() };

			var roles = await userService.GetRolesByUserAsync(user);
			model.roles = roles.Select(r => r.Name).ToArray();

			return Ok(model);
			
		}

		[HttpPut("[area]/[controller]/{id}")]
		public async Task<IActionResult> Update(string id, [FromBody] IdentityUserEditViewModel model)
		{
			var user = userService.GetUserById(id);
			if (user == null) return NotFound();

			string username = model.user.name;

			var exist = userService.GetUserByUserName(username);
			if (exist != null && exist.Id != id)
			{
				ModelState.AddModelError("user.name", "UserName重複了");
				return BadRequest(ModelState);
			}

			user = model.user.MapToEntity(user);

			await userService.UpdateUserAsync(user);

			await userService.UpdateUserAsync(user, model.roles);

			return Ok(user);


		}


		List<BaseOption> GetRoleOptions(bool edit=true)
		{
			bool emptyOption = !edit;
			
			var roles = userService.GetAllRoles();
			return roles.ToOptions(emptyOption);
		}

		async Task SetRole(IdentityUserEditViewModel model, ApplicationUser user)
		{
			//foreach (var item in model.roleOptions)
			//{
			//	string role = item.value;
			//	bool inRole = await userManager.IsInRoleAsync(user, role);

			//	if (inRole)
			//	{
			//		model.role = role;
			//		return;
			//	}
			//}
		}

	}
}