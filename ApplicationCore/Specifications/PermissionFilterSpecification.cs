﻿using ApplicationCore.Entities;
using ApplicationCore.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Specifications
{
	public class BasePermissionFilterSpecification : BaseSpecification<Permission>
	{
		public BasePermissionFilterSpecification()
		{
			Criteria = u => !u.Removed;
		}
	}

	public class PermissionFilterSpecification: BasePermissionFilterSpecification
	{
		public PermissionFilterSpecification(string name)
		{
			Criteria = p => p.Name == name;
		}
	}

	public class BaseAppUserFilterSpecification : BaseSpecification<AppUser>
	{
		public BaseAppUserFilterSpecification()
		{
			Criteria = u => !u.Removed && u.Active;

			AddInclude(u => u.UserPermissions);


		}
	}

	public class AppUserFilterSpecification : BaseAppUserFilterSpecification
	{
		public AppUserFilterSpecification(string keyword)
		{
			var compiled = Criteria.Compile();
			Criteria = u => compiled(u) && u.Name.CaseInsensitiveContains(keyword);

		}
	}

	public class BaseUserPermissionFilterSpecification : BaseSpecification<UserPermission>
	{
		public BaseUserPermissionFilterSpecification()
		{
			Criteria = u => !u.Removed;
		}
	}
	public class UserPermissionFilterSpecification : BaseUserPermissionFilterSpecification
	{
		public UserPermissionFilterSpecification(int userId, int permissionId)
		{
			var compiled = Criteria.Compile();
			Criteria = up => up.UserId == userId && up.PermissionId == permissionId;
		}

		public UserPermissionFilterSpecification(AppUser user)
		{
			var compiled = Criteria.Compile();
			Criteria = up => up.UserId == user.Id;
		}

		public UserPermissionFilterSpecification(Permission permission)
		{
			var compiled = Criteria.Compile();
			Criteria = up => up.PermissionId == permission.Id;
		}

		public UserPermissionFilterSpecification(AppUser user, IList<int> permissionIds)
		{
			var compiled = Criteria.Compile();
			Criteria = up => up.UserId == user.Id && permissionIds.Contains(up.PermissionId);
		}
	}
}
