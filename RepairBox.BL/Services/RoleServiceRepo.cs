using RepairBox.BL.DTOs.Role;
using RepairBox.DAL.Entities;
using RepairBox.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepairBox.BL.Services
{
    public interface IRoleServiceRepo
    {
        List<GetRoleDTO?> GetRoles();
        GetRoleDTO? GetRoleById(int id);
        void AddRole(AddRoleDTO roleDTO);
        void UpdateRole(UpdateRoleDTO roleDTO);
        bool DeleteRole(int id);
    }
    public class RoleServiceRepo
    {
        public ApplicationDBContext _context;
        public RoleServiceRepo(ApplicationDBContext context)
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
        public GetRoleDTO? GetRoleById(int id)
        {
            var role = _context.Roles.FirstOrDefault(r => r.Id == id);
            if (role != null)
            {
                var dbRequest = Omu.ValueInjecter.Mapper.Map<GetRoleDTO>(role);
                return dbRequest;
            }
            return null;
        }
        public void AddRole(AddRoleDTO roleDTO)
        {
            var role = new Role
            {
                Name = roleDTO.Name
            };

            _context.Roles.Add(role);
            _context.SaveChanges();

            return;
        }
        public void UpdateRole(UpdateRoleDTO roleDTO)
        {
            var role = _context.Roles.FirstOrDefault(r => r.Id == roleDTO.Id);
            if (role != null)
            {
                role.Name = roleDTO.Name;

                _context.SaveChanges();
            }

            return;
        }
        public bool DeleteRole(int id)
        {
            var role = _context.Roles.FirstOrDefault(r => r.Id == id);
            if (role != null)
            {
                _context.Roles.Remove(role);
                _context.SaveChanges();

                return true;
            }

            return false;
        }
    }
}
