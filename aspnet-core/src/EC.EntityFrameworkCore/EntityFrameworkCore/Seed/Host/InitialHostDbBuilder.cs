namespace EC.EntityFrameworkCore.Seed.Host
{
    public class InitialHostDbBuilder
    {
        private readonly ECDbContext _context;

        public InitialHostDbBuilder(ECDbContext context)
        {
            _context = context;
        }

        public void Create()
        {
            new DefaultEditionCreator(_context).Create();
            new DefaultLanguagesCreator(_context).Create();
            new HostRoleAndUserCreator(_context).Create();
            new DefaultSettingsCreator(_context).Create();
            new HostEmailTemplateCreator(_context).Create();

            _context.SaveChanges();
        }
    }
}
