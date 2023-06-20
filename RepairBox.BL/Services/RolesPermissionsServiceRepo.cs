using RepairBox.BL.DTOs.Role;
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

namespace RepairBox.BL.Services
{
    public interface IRolesPermissionsServiceRepo
    {
        List<GetRoleDTO?> GetRoles();
        List<GetPermissionDTO?> GetPermissions();
        void CreateRole(AddRoleDTO roleDTO, List<GetPermissionDTO> permissionDTOs);
        void ModifyRole(UpdateRoleDTO roleDTO, List<GetPermissionDTO> permissionDTOs);
        bool DeleteRole(int id);
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
        public List<GetPermissionDTO?> GetPermissions()
        {
            List<GetPermissionDTO> permissions = new List<GetPermissionDTO>();
            var permissionList = _context.Permissions.ToList();
            if (permissionList != null)
            {
                permissionList.ForEach(role => permissions.Add(Omu.ValueInjecter.Mapper.Map<GetPermissionDTO>(role)));
                return permissions;
            }
            return null;
        }
        public void CreateRole(AddRoleDTO roleDTO, List<GetPermissionDTO> permissionDTOs)
        {
            var role = new UserRole
            {
                Name = roleDTO.Name
            };

            _context.Roles.Add(role);
            _context.SaveChanges();

            foreach (var permissionDTO in permissionDTOs) 
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
        public void ModifyRole(UpdateRoleDTO roleDTO, List<GetPermissionDTO> permissionDTOs)
        {
            var role = _context.Roles.FirstOrDefault(r => r.Id == roleDTO.Id);
            if (role != null)
            {
                role.Name = roleDTO.Name;

                var userRole_Permissions_ToRemove = _context.UserRole_Permissions.Where(urp => urp.RoleId == roleDTO.Id);
                _context.UserRole_Permissions.RemoveRange(userRole_Permissions_ToRemove);

                foreach (var permissionDTO in permissionDTOs)
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
        public void AddPermission(AddPermissionDTO permissionDTO, List<AddResourceDTO> resourceDTOs)
        {
            var permission = new Permission
            {
                Name = permissionDTO.Name,
            };
            _context.Permissions.Add(permission);
            _context.SaveChanges();

            foreach(var resourceDTO in resourceDTOs)
            {
                var resource = new Resource
                {
                    Name = resourceDTO.Name,
                    PermissionId = permission.Id
                };

                _context.Resources.Add(resource);
                _context.SaveChanges();
            }
        }
        public bool isUserAuthorized(GetUserDTO userLoginSession, GetResourceDTO resourceDTO)
        {
            return false;
        }
    }
}
