using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicQueueSimulation
{
    internal abstract class RealTimeObject : IDisposable
    {
        private bool disposed = false;
        public RealTimeObject()
        {
            EventManager.UpdateRealTimeObjects += Update;
        }

        ~RealTimeObject()
        {
            if (disposed) return;
            disposed = true;
            EventManager.UpdateRealTimeObjects -= Update;
        }

        public void Dispose()
        {
            if (disposed) return;
            disposed = true;
            EventManager.UpdateRealTimeObjects -= Update;
        }

        protected virtual void Update(double delta)
        {

        }
    }
}
