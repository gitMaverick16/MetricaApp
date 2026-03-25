namespace OrdersApp.Infrastructure.Configuration
{
    public sealed class SeedSettings
    {
        public const string SectionName = "Seed";

        public bool Enabled { get; init; }

        /// <summary>
        /// Usuarios a crear si no existen (por email). Vacío si el seed está desactivado.
        /// </summary>
        public List<SeedUserEntry> Users { get; init; } = [];
    }
}
