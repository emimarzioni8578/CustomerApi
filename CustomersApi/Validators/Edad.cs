namespace CustomersApi.Validators
{
    public static class Edad
    {
        public static int Calcular(DateOnly nacimiento, DateOnly hoy)
        {
            var edad = hoy.Year - nacimiento.Year;

            if (nacimiento > hoy.AddYears(-edad))
            {
                edad--;
            }

            return edad;
        }
    }
}
