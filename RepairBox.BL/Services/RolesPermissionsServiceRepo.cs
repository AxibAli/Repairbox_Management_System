﻿using RepairBox.BL.DTOs.Role;
using RepairBox.DAL.Entities;
using RepairBox.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RepairBox.BL.DTOs.Permission;
using RepairBox.BL.DTOs.Resource;
using RepairBox.BL.DTOs.User;
using RepairBox.BL.DTOs.Priority;

namespace RepairBox.BL.Services
{
    public interface IRolesPermissionsServiceRepo
    {
        List<GetRoleDTO?> GetRoles();
        List<GetAllPermissionsDTO?> GetAllPermissions();
        GetPermissionDTO? GetPermissionById(int id);
        void CreateRole(AddRoleDTO roleDTO);
        void CreatePermission(AddPermissionDTO permissionDTO);
        void ModifyRole(UpdateRoleDTO roleDTO);
        void ModifyPermission(UpdatePermissionDTO permissionDTO);
        bool DeleteRole(int id);
        bool DeletePermission(int id);
    }
    public class RolesPermissionsServiceRepo : IRolesPermissionsServiceRepo
    {
        public ApplicationDBContext _context;
        public RolesPermissionsServiceRepo(ApplicationDBContext context)
        {
            _context = context;
        }
        public List<GetRoleDTO?> GetRoles()
        {
            List<GetRoleDTO> roles = new List<GetRoleDTO>();
            var roleList = _context.Roles.ToList();
            if (roleList != null)
            {
                roleList.ForEach(role => roles.Add(Omu.ValueInjecter.Mapper.Map<GetRoleDTO>(role)));
                return roles;
            }
            return null;
        }
        public List<GetAllPermissionsDTO?> GetAllPermissions()
        {
            List<GetAllPermissionsDTO> permissions = new List<GetAllPermissionsDTO>();
            var permissionList = _context.Permissions.ToList();
            if (permissionList != null)
            {
                permissionList.ForEach(role => permissions.Add(Omu.ValueInjecter.Mapper.Map<GetAllPermissionsDTO>(role)));
                return permissions;
            }
            return null;
        }
        public GetPermissionDTO? GetPermissionById(int id)
        {
            var permission = _context.Permissions.FirstOrDefault(p => p.Id == id);

            if(permission == null) { return null; }

            return Omu.ValueInjecter.Mapper.Map<GetPermissionDTO>(permission);
        }
        public void CreateRole(AddRoleDTO roleDTO)
        {
            var role = new UserRole
            {
                Name = roleDTO.Name
            };

            _context.Roles.Add(role);
            _context.SaveChanges();

            foreach (var permissionDTO in roleDTO.Permissions) 
            {
                var userRole_Permission = new UserRole_Permission
                {
                    RoleId = role.Id,
                    PermissionId = permissionDTO.Id
                };
                _context.UserRole_Permissions.Add(userRole_Permission);
                _context.SaveChanges();
            }

            return;
        }
        public void CreatePermission(AddPermissionDTO permissionDTO)
        {
            var permission = new Permission
            {
                Name = permissionDTO.Name
            };

            _context.Permissions.Add(permission);
            _context.SaveChanges();

            foreach(var resourceName in permissionDTO.ResourceNames)
            {
                var resource = new Resource
                {
                    Name = resourceName,
                    PermissionId = permission.Id
                };

                _context.Resources.Add(resource);
                _context.SaveChanges();
            }
        }
        public void ModifyRole(UpdateRoleDTO roleDTO)
        {
            var role = _context.Roles.FirstOrDefault(r => r.Id == roleDTO.Id);
            if (role != null)
            {
                role.Name = roleDTO.Name;

                var userRole_Permissions_ToRemove = _context.UserRole_Permissions.Where(urp => urp.RoleId == roleDTO.Id);
                _context.UserRole_Permissions.RemoveRange(userRole_Permissions_ToRemove);

                foreach (var permissionDTO in roleDTO.Permissions)
                {
                    var userRole_Permission = new UserRole_Permission
                    {
                        RoleId = role.Id,
                        PermissionId = permissionDTO.Id
                    };
                    _context.UserRole_Permissions.Add(userRole_Permission);
                }

                _context.SaveChanges();
            }

            return;
        }
        public void ModifyPermission(UpdatePermissionDTO permissionDTO)
        {
            var permission = _context.Permissions.FirstOrDefault(p => p.Id == permissionDTO.Id);
            if(permission != null)
            {
                permission.Name = permissionDTO.Name;

                var resouce_ToRemove = _context.Resources.Where(r => r.PermissionId == permissionDTO.Id);
                _context.Resources.RemoveRange(resouce_ToRemove);

                foreach(var resourceName in permissionDTO.ResourceNames)
                {
                    var resource = new Resource
                    {
                        Name = resourceName,
                        PermissionId = permissionDTO.Id
                    };

                    _context.Resources.Add(resource);
                }

                _context.SaveChanges();
            }

            return;
        }
        public bool DeleteRole(int id)
        {
            var role = _context.Roles.FirstOrDefault(r => r.Id == id);
            if (role != null)
            {
                //_context.Roles.Remove(role);
                role.IsActive = false;
                role.IsDeleted = true;

                _context.SaveChanges();

                return true;
            }

            return false;
        }
        public bool DeletePermission(int id)
        {
            var permission = _context.Permissions.FirstOrDefault(p => p.Id == id);
            if(permission != null)
            {
                //_context.Permissions.Remove(permission);
                permission.IsActive = false;
                permission.IsDeleted = true;

                _context.SaveChanges();

                return true;
            }

            return false;
        }
        public bool isUserAuthorized(GetUserDTO userLoginSession, GetResourceDTO resourceDTO)
        {
            return false;
        }
    }
}
