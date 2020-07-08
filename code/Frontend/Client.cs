using System.Threading.Tasks;
using Frontend;
using Models;

namespace WebFrontEnd
{
    public class Client
    {
        private readonly CircuitBreaker<Bratkartoffel> _cb;

        public Client()
        {
            _cb = new CircuitBreaker<Bratkartoffel>();
        }

        public async Task<Bratkartoffel> GetData()
        {
            return await _cb.GetData();
        }
    }
}
