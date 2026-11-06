using Microsoft.AspNetCore.Mvc;
namespace CustomersApi.Controllers
{
    public abstract class LifeTimeMarker
    {
        public string Id { get; } = Guid.NewGuid().ToString("N")[..6];
    }

    public class SingletonMarker : LifeTimeMarker { }
    public class ScopedMarker : LifeTimeMarker { }
    public class TransientMarker : LifeTimeMarker { }

    public class ConsumerA
    {
        private readonly SingletonMarker _singleton;
        private readonly ScopedMarker _scoped;
        private readonly TransientMarker _transient;
        public ConsumerA(SingletonMarker singleton, ScopedMarker scoped, TransientMarker transient)
        {
            _singleton = singleton;
            _scoped = scoped;
            _transient = transient;
        }

        public object Ids => new
        {
            Singleton = _singleton.Id,
            Scoped = _scoped.Id,
            Transient = _transient.Id
        };
    }

    public class ConsumerB
    {
        private readonly SingletonMarker _singleton;
        private readonly ScopedMarker _scoped;
        private readonly TransientMarker _transient;
        public ConsumerB(SingletonMarker singleton, ScopedMarker scoped, TransientMarker transient)
        {
            _singleton = singleton;
            _scoped = scoped;
            _transient = transient;
        }

        public object Ids => new
        {
            Singleton = _singleton.Id,
            Scoped = _scoped.Id,
            Transient = _transient.Id
        };
    }

    [ApiController]
    [Route("api/[controller]")]
    public class LifetimeMarkerController : ControllerBase
    {
        private readonly ConsumerA _consumerA;
        private readonly ConsumerB _consumerB;
        public LifetimeMarkerController(ConsumerA consumerA, ConsumerB consumerB)
        {
            _consumerA = consumerA;
            _consumerB = consumerB;
        }
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new { a = _consumerA.Ids, b = _consumerB.Ids });
        }
    }
}
