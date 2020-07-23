using System.Threading.Tasks;
using CircuitBreaker;
using Models;

namespace Frontend
{
    public class Client
    {
        private readonly CircuitBreaker<Bratkartoffel> _cb;

        public Client()
        {
            _cb = new CircuitBreaker<Bratkartoffel>();
        }

        public async Task<ResponseObject<Bratkartoffel>> GetData()
        {
            return await _cb.GetData();
        }
    }
}
