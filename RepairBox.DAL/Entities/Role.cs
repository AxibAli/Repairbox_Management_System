﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepairBox.DAL.Entities
{
    public class Role : Base
    {
        public string Name { get; set; }
        public ICollection<User> User { get; set; }
    }
}
