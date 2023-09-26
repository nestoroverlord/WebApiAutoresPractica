namespace WebApiAutores.Servicios
{
    public interface IServicio
    {
        Guid ObtenerTransient();
        Guid ObtenerScoped();
        Guid ObtenerSingleton();
        void RealizarTarea();
    }

    public class ServicioA : IServicio
    {
        private readonly ILogger<ServicioA> _logger;
        private readonly ServicioTransient _servicioTransient;
        private readonly ServicioScoped _servicioScoped;
        private readonly ServicioSingleton _servicioSingleton;

        public ServicioA(ILogger<ServicioA> logger,
                         ServicioTransient servicioTransient,
                         ServicioScoped servicioScoped,
                         ServicioSingleton servicioSingleton)
        {
            _logger = logger;
            this._servicioTransient = servicioTransient;
            this._servicioScoped = servicioScoped;
            this._servicioSingleton = servicioSingleton;
        }

        public Guid ObtenerTransient() { return _servicioTransient.guid; } 
        public Guid ObtenerScoped() { return _servicioScoped.guid; } 
        public Guid ObtenerSingleton() { return _servicioSingleton.guid; } 

        public void RealizarTarea()
        {
            //throw new NotImplementedException();
        }
    }
    
    public class ServicioB : IServicio
    {
        public Guid ObtenerScoped()
        {
            throw new NotImplementedException();
        }

        public Guid ObtenerSingleton()
        {
            throw new NotImplementedException();
        }

        public Guid ObtenerTransient()
        {
            throw new NotImplementedException();
        }

        public void RealizarTarea()
        {
            //throw new NotImplementedException();
        }
    }

    public class ServicioTransient 
    {
        public Guid guid = Guid.NewGuid();
    }

    public class ServicioScoped
    {
        public Guid guid = Guid.NewGuid();
    }

    public class ServicioSingleton
    {
        public Guid guid = Guid.NewGuid();
    }
}
