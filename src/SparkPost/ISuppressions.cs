using System.Collections.Generic;
using System.Threading.Tasks;

namespace SparkPost
{
    public interface ISuppressions
    {
        Task<ListSuppressionResponse> List();
        Task<ListSuppressionResponse> List(SuppressionsQuery supppressionsQuery);
        Task<ListSuppressionResponse> List(string url);
        Task<ListSuppressionResponse> Retrieve(string email);
        Task<UpdateSuppressionResponse> CreateOrUpdate(IEnumerable<string> emails);
        Task<UpdateSuppressionResponse> CreateOrUpdate(IEnumerable<Suppression> suppressions);
        Task<bool> Delete(string email);
    }
}