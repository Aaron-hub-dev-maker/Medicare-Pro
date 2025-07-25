using System.Collections.Generic;

namespace Hospital.Services
{
    /// <summary>
    /// Service interface for dashboard/statistics aggregation logic.
    /// </summary>
    public interface IDashboardService
    {
        object GetDashboardData();
    }
} 