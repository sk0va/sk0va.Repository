using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Sample.Core;
using Skova.Repository.Abstractions;

namespace Sample.App
{
    internal class CompanyHostService : IHostedService
    {
        public CompanyHostService(IServiceProvider provider)
        {
            Provider = provider;
        }

        public IServiceProvider Provider { get; }

        public async Task StartAsync(CancellationToken ct = default)
        {
            using var scope = Provider.CreateScope();

            var context = scope.ServiceProvider.GetRequiredService<CompanyDbContext>();

            context.Database.EnsureCreated();

            var personService = scope.ServiceProvider.GetRequiredService<PersonService>();

            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

            var person = new Person
            {
                Id = Guid.NewGuid(),
                PersonAge = 20,
                PersonName = "Adam"
            };

            await personService.CreateAsync(person, ct);

            var byNameAndAge = await personService.GetByAgeAndNameAsync(person.PersonAge, person.PersonName, ct);
            var byId = await personService.GetByIdAsync(person.Id, ct);

            await personService.UpdateNameAsync(person.Id, "Eva", ct);
            await personService.DeleteAsync(person.Id, ct);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}