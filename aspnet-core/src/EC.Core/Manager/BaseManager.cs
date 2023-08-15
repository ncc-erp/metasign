using Abp.Application.Services;
using HRMv2.NccCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EC.Manager
{
    public  class BaseManager : ApplicationService
    {
        protected IWorkScope WorkScope { get; set; }

        public BaseManager(IWorkScope workScope)
        {
            WorkScope = workScope;
        }
    }
}
