using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicQueueSimulation
{
    internal abstract class RealTimeObject
    {
        public RealTimeObject()
        {
            EventManager.UpdateRealTimeObjects += Update;
        }

        ~RealTimeObject()
        {
            EventManager.UpdateRealTimeObjects -= Update;
        }

        protected virtual void Update(double delta)
        {

        }
    }
}
