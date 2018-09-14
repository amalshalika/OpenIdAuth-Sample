using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ToDoTask.Models
{
    public class AddressInfo
    {
        public AddressInfo(string address)
        {
            Address = address;
        }
        public string Address { get; private set; } = string.Empty;

    }
}
