namespace CustomersApi.Validators
{
    public static class CuitCuil
    {
        private static readonly int[] Multiplicadores = { 5, 4, 3, 2, 7, 6, 5, 4, 3, 2 };

        public static bool EsValido(string? valor)
        {
            if (string.IsNullOrWhiteSpace(valor))
            {
                return false;
            }

            var digitos = new string(valor.Where(char.IsDigit).ToArray());

            if (digitos.Length != 11)
            {
                return false;
            }

            var suma = 0;
            for (var i = 0; i < Multiplicadores.Length; i++)
            {
                suma += (digitos[i] - '0') * Multiplicadores[i];
            }

            var resto = suma % 11;
            var verificador = resto switch { 0 => 0, 1 => 9, _ => 11 - resto };

            return verificador == digitos[10] - '0';
        }
    }
}
