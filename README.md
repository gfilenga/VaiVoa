# Guilherme Rodrigues Filenga
github.com/gfilenga
+55 11 94765-7622
guilherme.filenga@gmail.com


# Construa uma API REST em C# com .NET CORE e Entity Framework Core
## Repositório desse projeto: https://github.com/gfilenga/VaiVoa

> Disclaimer: A arquitetura desse projeto será “inflada” com o objetivo acadêmico, para soluções mais objetivas, no mundo > real, é preciso identificar a necessidade da aplicação e planejar para o futuro.

# Primeiros Passos

### Tecnologias

Utilizaremos algumas praticidades do ecossistema .NET para facilitar na configuração inicial do projeto. 

- [Visual Studio 2019] - Essa IDE é a queridinha dos programadores dotnet, tendo a possibilidade de instalar a versão community que está liberada para uso pessoal em estudos.
- [Entity Framework Core] - É um ORM que irá facilitar a conexão e a geração do banco de dados. E as queries geradas por ele foram reescritas e apresentam ótimas performances.
- [SQL Server] - Utilizaremos ele por meio da instalação do pacote do Entity Framework Core que instalaremos por meio do Package Manager mais a frente.

### Desafio

Escreva um artigo, em formato de blog post sobre um projeto C# com .NET Core. Você deverá descrever o passo-a-passo para a criação de uma API REST que fornece um sistema de geração de número de cartão de crédito virtual.
A API deverá gerar números aleatórios para o pedido de novo cartão. Cada cartão gerado deve estar associado a um email para identificar a pessoa que está utilizando.
Essencialmente são 2 endpoints. Um receberá o email da pessoa e retornará um objeto de resposta com o número do cartão de crédito. E o outro endpoint deverá listar, em ordem de criação, todos os cartões de crédito virtuais de um solicitante (passando seu email como parâmetro).
A implementação deverá ser escrita utilizando C# com .Net Core e Entity Framework Core.

### Criando a solução

Abra o Visual Studio 2019 e procure pelo template de projeto ASP .NET Core Web Api. Ele criará o setup inicial do nosso projeto e agilizará o processo de desenvolvimento.

Escolha a versão 3.1 que é a última versão LTS (long term support) e sem nenhum modelo de autenticação pré-definido.

Crie outros dois projetos de domínio e de infra com o template “Class Library” dentro da mesma solução.

Clique com o botão direito no projeto de API e coloque ele como startup project e adicione o projeto domain e de infra como referências.

Além de, adicionar no projeto de infra o projeto de domain como referência.

> Obs: A camada de domínio deve ser independente e o mais próxima da linguagem ubíqua possível.

### Instalando pacotes
- Dentro do projeto de Infra iremos instalar os seguintes pacotes: 
-- Microsoft.EntityFrameworkCore
-- Microsoft.EntityFrameworkCore.Relational
-- Microsoft.EntityFrameworkCore.SqlServer 
- Dentro do projeto de domínio iremos instalar os seguintes pacotes:
-- FluentValidation.AspNetCore
- Por fim, dentro do projeto da Api:
-- Microsoft.EntityFrameworkCore
-- Microsoft.EntityFrameworkCore.Design
-- Swashbuckle.AspNetCore

### Configurando Swagger

Coloque essas linhas de código no método ConfigureServices e no método Configure da classe startup, respectivamente:
```csharp
services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "VaiVoa.Api", Version = "v1" });
    });
```
```csharp
if (env.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
        app.UseSwagger();
        app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "VaiVoa.Api v1"));
    }
```
### Configurando o contexto

Dentro do projeto de infra, crie uma pasta chamada Context e crie uma classe chamada DataContext.

O contexto irá definir as configurações de criação do nosso banco de dados, o que esquecermos de configurar aqui, o entity framework definirá com valores padrões.

A classe ficará assim:

```csharp
public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        public DbSet<Client> Clients { get; set; }
        public DbSet<CreditCard> CreditCards { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            foreach (var property in modelBuilder.Model.GetEntityTypes()
                .SelectMany(e => e.GetProperties()
                    .Where(p => p.ClrType == typeof(string))))
                property.SetColumnType("varchar(100)");

            foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Cascade;
            }

            base.OnModelCreating(modelBuilder);
        }
    }
```

O primeiro foreach está definindo os campos string como varchar(100) no banco, caso contrário o entity framework core colocaria eles como nvarchar(max).

Já o segundo foreach está definindo o comportamento de deleção, que eu coloquei como Cascade. Ou seja, irá deletar as entidades pais mesmo que tenha referências filhas registradas no banco. Portanto, na prática, na nossa aplicação, assim que um cliente for deletado, todos os cartões de créditos pertencentes a ele serão deletados também.

Dentro do appsettings.Development.json definimos a nossa string de conexão com o banco de dados. Ficará assim:

```csharp
{
    ...
    },
    "ConnectionStrings": {
        "connectionString": "Server=localhost,1433;Database=VaiVoa;User ID=sa;Password=1q2w3e4r@#$"
    }
}
```

Não irei entrar em detalhes de como criar uma instância do SQL Server na sua máquina, porém, aqui está um link que irá ajudar, utilizando docker: https://balta.io/blog/sql-server-docker.

A seguir, dentro do projeto de API, dentro do método “configure services” na classe Startup, adicione esse trecho de código:

```csharp
    ...
    services.AddDbContext<DataContext>(opt =>
        opt.UseSqlServer(Configuration.GetConnectionString("connectionString")));
    ...
```

Com o banco de dados resolvido, agora partiremos para o código.


### Camada de domínio
Utilizaremos o conceito de injeção de dependências para este projeto. O .NET possui um ótimo suporte nativo e facilita bastante na produtividade e desacoplamento do código. Portanto, dentro dessa camada, iremos focar na construção de contratos que iremos implementá-los posteriormente nas outras camadas da solução. Além de construir nossas entidades/modelos e validar as requisições e regras de negócio por meio do pacote FluentValidation.

### Notificações
As notificações irão centralizar os erros que forem encontrados em qualquer nível da aplicação. Para isso, nós iremos criar um contrato que irá definir quais métodos teremos dentro dessa classe notificadora, além de criar um modelo para a notificação em si.

Dentro do projeto de domain, você irá criar uma pasta com o nome de Interfaces e dentro dela criar uma interface chamada INotificator. O prefixo “i” é importante para padronizar o que é uma interface e o que não é.

Depois disso, iremos criar uma pasta com o nome de Notifications, que irá armazenar a classe de notificação e a implementação do contrato definido anteriormente.

Com isso feito, o código ficará assim: 

> INotificator
```csharp
public interface INotificator
    {
        bool HasNotification();
        List<Notification> ObtainNotifications();
        void Handle(Notification notificacao);
    }
```

> Notification

```csharp
public class Notification
    {
        public Notification(string mensagem)
        {
            Mensagem = mensagem;
        }

        public string Mensagem { get; }
    }
```

> Notificator
```csharp
public class Notificator : INotificator
    {
        private List<Notification> _notifications;

        public Notificator()
        {
            _notifications = new List<Notification>();
        }

        public void Handle(Notification notificacao)
        {
            _notifications.Add(notificacao);
        }

        public bool HasNotification()
        {
            return _notifications.Any();
        }

        public List<Notification> ObtainNotifications()
        {
            return _notifications;
        }
    }
```

## Entidades/Modelos

Para os modelos, primeiro iremos criar uma classe base com o nome de “Entity”. Essa classe será abstrata (não poderá ser instanciada) e irá definir propriedades bases para as entidades que derivarem dela. Isso facilita a implementação e faz com que repitamos menos código.

A classe ficará assim:

> Entity

```csharp
public abstract class Entity : IEquatable<Entity>
    {
        public Entity()
        {
            Id = Guid.NewGuid();
            Created_at = DateTime.Now;
        }
        public Guid Id { get; set; }
        public DateTime Created_at { get; set; }

        public bool Equals(Entity other)
        {
            return Id == other.Id;
        }
    }
```
> Client

```csharp
public class Client : Entity
    {
        public Client() { }

        public Client(string name, 
            string email, 
            string password, 
            string confirmPassword)
        {
            Name = name;
            Email = email;
            Password = password;
            ConfirmPassword = confirmPassword;
            CreditCards = new List<CreditCard>();
        }

        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }

        // Relacionamentos
        public IList<CreditCard> CreditCards { get; set; }

        public void UpdateClient(string name,string email,string password, string confirmPassword)
        {
            Name = name;
            Email = email;
            Password = password;
            ConfirmPassword = confirmPassword;
        }
    }
```

> CreditCard
O método que irá gerar o número de cartão de crédito, que estamos utilizando no construtor da classe é o GenerateCreditCardNumber()
```csharp
public class CreditCard : Entity
    {
        public CreditCard() { }

        public CreditCard(int securityCode, 
            Guid clientId)
        {
            Number = GenerateCreditCardNumber();
            ValidThru = DateTime.Now.AddDays(2555);
            SecurityCode = securityCode;
            ClientId = clientId;
        }

        public string Number { get; set; }
        public DateTime ValidThru { get; set; }
        public int SecurityCode{ get; set; }

        // Relacionamentos
        public Guid ClientId { get; set; }
        public Client Client { get; set; }

        private string GenerateCreditCardNumber()
        {
            string creditCardNumber = "4";
            for (int i = 0; i < 15; i++)
            {
                Random rnd = new Random();
                var number = rnd.Next(1, 10);
                creditCardNumber += number.ToString();
            }

            return creditCardNumber;
        }
    }
```

Após configurar os modelos e o contexto, para criar migrações e gerar o banco nós iremos rodar os seguintes comandos, dentro do projeto de infra:

> dotnet ef migrations add InitialCreate --startup-project ../VaiVoa.Api/

Depois, com as migrações criadas, geramos o banco com o seguinte comando:

> dotnet ef database update --startup-project ../VaiVoa.Api/

Com os modelos criados, agora partiremos para a validação dos campos. Para isso, usaremos o FluentValidation. Para maiores detalhes sobre como configurar e outros tipos de validações, fica aqui a documentação dele: https://docs.fluentvalidation.net/en/latest/aspnet.html

## FluentValidation
Dentro da pasta de Models, iremos criar uma outra pasta chamada Validations e dentro dessa criaremos outras duas classes: ClientValidator e CreditCardValidator.

Com essa abordagem é possível centralizar todas as validações em um só lugar e deixa a responsabilidade das Models menor e sem poluição visual com as famosas data annotations.

> ClientValidator
Na validação do cliente ficou definido que o nome deve ter entre 2 a 40 caracteres, o email deve ser válido e os campos password e confirmPassword devem ser iguais.

```csharp
public class ClientValidator : AbstractValidator<Client>
    {
        public ClientValidator()
        {
            RuleFor(client => client.Name)
               .NotEmpty().WithMessage("O campo {PropertyName} precisa ser fornecido")
               .Length(2, 40).WithMessage("Digite um {PropertyName} com no mínimo 2 e no máximo 40 caracteres");

            RuleFor(client => client.Email)
                .NotEmpty().WithMessage("Digite um {PropertyName}")
                .EmailAddress().WithMessage("Digite um {PropertyName} válido");

            RuleFor(client => client.Password)
                .NotEmpty().WithMessage("O campo {PropertyName} precisa ser fornecido");

            RuleFor(client => client.ConfirmPassword)
                .Equal(client => client.Password).WithMessage("{PropertyName} e {ComparisonValue} não batem");
        }
    }
```
> CreditCardValidator
Na validação do número cartão de crédito vamos validar se o primeiro dígito é 4, o que significa que é um cartão de crédito da Visa e também se o código de segurança é um número entre 100 e 999.
Já na validação da data de validade, só estamos verificando se não é uma data passada, o que significaria que o cartão já está vencido. 

```csharp
public class CreditCardValidator : AbstractValidator<CreditCard>
    {
        public CreditCardValidator()
        {
            RuleFor(cc => cc.Number).
                NotEmpty().WithMessage("Digite um número de cartão")
                .Must(c => c.Substring(0,1) == "4").WithMessage("O número do cartão deve começar com 4 (Visa)")
                .Length(16).WithMessage("{PropertyName} deve ter {PropertyValue} caracteres");

            RuleFor(cc => cc.SecurityCode)
                .LessThan(999).WithMessage("Digite um {PropertyName} válido")
                .GreaterThan(100).WithMessage("Digite um {PropertyName} válido")
                .NotNull().WithMessage("Digite um {PropertyName}");

            RuleFor(cc => cc.ValidThru)
                .NotEmpty().WithMessage("Preencha a data de expiração")
                .GreaterThan(DateTime.Now).WithMessage("{PropertyName} deve ser maior que a data atual");
        }
    }
```

## Repositórios
Agora vamos criar o contrato que irá definir o nosso repositório base, esse repositório base irá trazer uma diversidade de métodos que poderão ser usados por todos os repositórios da aplicação.

Métodos mais específicos poderão ser implementados posteriormente, fora do repositório base. Dentro do projeto de domain, dentro da pasta interfaces, crie a classe IRepository com o seguinte código: 

> IRepository
```csharp
public interface IRepository<TEntity> : IDisposable where TEntity : Entity
    {
        Task Create(TEntity entity);
        Task<TEntity> GetByIdNoTracking(Guid id);
        Task<List<TEntity>> GetAll();
        Task Update(TEntity entity);
        Task Delete(Guid id);
        Task<IEnumerable<TEntity>> Search(Expression<Func<TEntity, bool>> predicate);
        Task<int> SaveChanges();
    }
```
Além disso, vamos criar dentro da mesma pasta, as interfaces IClientRepository e a ICreditCardRepository. 

>IClientRepository
```csharp
public interface IClientRepository : IRepository<Client> { }
```
> ICreditCardRepository
```csharp
public interface ICreditCardRepository : IRepository<CreditCard>
    {
        Task<IEnumerable<CreditCard>> GetAllByEmail(string email);
    }
```
Agora, dentro do projeto de infra, iremos criar uma pasta chamada Repositories e dentro dela criaremos uma classe chamada Repository.

Essa classe será a implementação desse contrato que acabamos de criar.

>Repository
Essa classe também será abstrata e os repositórios dos outros modelos irão derivar dela.

```csharp
public abstract class Repository<TEntity> : IRepository<TEntity> where TEntity : Entity, new()
    {
        protected readonly DataContext Context;

        protected readonly DbSet<TEntity> DbSet;

        public Repository(DataContext context)
        {
            Context = context;
            DbSet = context.Set<TEntity>();
        }

        public virtual async Task Create(TEntity entity)
        {
            DbSet.Add(entity);
            await SaveChanges();
        }

        public virtual async Task<List<TEntity>> GetAll()
        {
            return await DbSet.AsNoTracking().ToListAsync();
        }

        public virtual async Task<TEntity> GetByIdNoTracking(Guid id)
        {
            return await DbSet.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<TEntity>> Search(Expression<Func<TEntity, bool>> predicate)
        {
            return await DbSet.AsNoTracking().Where(predicate).ToListAsync();
        }
        public virtual async Task Update(TEntity entity)
        {
            DbSet.Update(entity);
            await SaveChanges();
        }

        public virtual async Task Delete(Guid id)
        {
            DbSet.Remove(new TEntity { Id = id });
            await SaveChanges();
        }

        public async Task<int> SaveChanges()
        {
            return await Context.SaveChangesAsync();
        }

        public void Dispose()
        {
            Context?.Dispose();
        }
    }
```
E depois, dentro do projeto de infra, crie a implementação dos dois outros contratos:

>ClientRepository
```csharp
public class ClientRepository : Repository<Client>, IClientRepository
    {
        public ClientRepository(DataContext context) : base(context) { }
    }
```

>CreditCardRepository
```csharp
public class CreditCardRepository : Repository<CreditCard>, ICreditCardRepository
    {
        public CreditCardRepository(DataContext context) : base(context) { }

        public async Task<IEnumerable<CreditCard>> GetAllByEmail(string email)
        {
            return await Context.CreditCards.AsNoTracking().Where(c => c.Client.Email == email)
                                            .OrderBy(c => c.Created_at).ToListAsync();
        }
    }
```
> Obs: Não esqueça de passar uma instância do contexto para a classe base, por meio do construtor.

Agora iremos, dentro da camada de domínio, criar os contratos do serviço e implementá-los.

É importante dentro da camada de serviço nós termos os métodos que irão fazer alguma alteração no banco de dados. Dessa forma, conseguimos implementar as regras de negócios necessárias de forma organizada e limpa.

Vamos começar, dentro do projeto de domínio, na pasta interfaces, crie duas interfaces a IClientService e a ICreditCardService.

>IClientService

```csharp
public interface IClientService : IDisposable
    {
        Task Create(Client client);
        Task Update(Client client);
        Task<bool> Delete(Guid id);
    }
```
> ICreditCardService
```csharp
public interface ICreditCardService : IDisposable
    {
        Task Create(CreditCard creditCard);
    }
```
Agora, vamos começar a criar a implementação desses 2 contratos, porém, antes criaremos a classe base dos serviços, que engloba o que terá de comum nas 2 implementações.

Dentro da camada de domínio, crie uma pasta chamada Services e dentro dessa pasta crie uma classe abstrata chamada BaseService.

A classe ficará assim:

> BaseService
Ela irá receber uma instância da classe INotificator, por meio de injeção de dependência e oferecerá alguns métodos de suporte para que possamos realizar as validações necessárias.
```csharp
public abstract class BaseService
    {
        private readonly INotificator _notificator;

        protected BaseService(INotificator notificator)
        {
            _notificator = notificator;
        }

        protected void Notificar(ValidationResult validationResult)
        {
            foreach (var error in validationResult.Errors)
            {
                Notificar(error.ErrorMessage);
            }
        }

        protected void Notificar(string mensagem)
        {
            _notificator.Handle(new Notification(mensagem));
        }

        protected bool ExecutarValidacao<TV, TE>(TV validacao, TE entidade) where TV : AbstractValidator<TE> where TE : Entity
        {
            var validator = validacao.Validate(entidade);

            if (validator.IsValid) return true;

            Notificar(validator);

            return false;
        }
    }
```
Agora vamos criar a implementação dos contratos de IClientService e ICreditCardService, dentro da pasta Services, na camada de domínio.
>ClientService
```csharp
public class ClientService : BaseService, IClientService
    {
        private readonly IClientRepository _clientRepository;
        public ClientService(INotificator notificator, 
            IClientRepository clientRepository) : base(notificator)
        {
            _clientRepository = clientRepository;
        }

        public async Task Create(Client client)
        {
            if (!ExecutarValidacao(new ClientValidator(), client)) return;

            if (_clientRepository.Search(c => c.Email == client.Email).Result.Any())
            {
                Notificar("Email já cadastrado!");
                return;
            }

            await _clientRepository.Create(client);
        }
             
        public async Task Update(Client client)
        {
            if (!ExecutarValidacao(new ClientValidator(), client)) return;

            await _clientRepository.Update(client);
        }

        public async Task<bool> Delete(Guid id)
        {
            await _clientRepository.Delete(id);
            return true;
        }
        public void Dispose()
        {
            _clientRepository?.Dispose();
        }
    }
```

> CreditCardService
```csharp
public class CreditCardService : BaseService, ICreditCardService
    {
        private readonly ICreditCardRepository _creditCardRepository;
        public CreditCardService(INotificator notificator, 
            ICreditCardRepository creditCardRepository) : base(notificator)
        {
            _creditCardRepository = creditCardRepository;
        }

        public async Task Create(CreditCard creditCard)
        {
            if (!ExecutarValidacao(new CreditCardValidator(), creditCard)) return;

            await _creditCardRepository.Create(creditCard);
        }
        public void Dispose()
        {
            _creditCardRepository?.Dispose();
        }
    }
```
Com a camada de domínio e de infra praticamente prontas, vamos passar para a parte da API.
Antes de qualquer coisa, vamos resolver a injeção de dependências geradas. Para isso, dentro da camada de API, crie uma pasta chamada Configuration e crie uma classe com o nome [DependencyInjectionConfig]. 

A classe ficará assim:

```csharp
public static class DependencyInjectionConfig
    {
        public static IServiceCollection ResolveDependencies(this IServiceCollection services)
        {
            services.AddScoped<DataContext>();
            // Client
            services.AddScoped<IClientRepository, ClientRepository>();
            services.AddScoped<IClientService, ClientService>();
            // Credit Card
            services.AddScoped<ICreditCardRepository, CreditCardRepository>();
            services.AddScoped<ICreditCardService, CreditCardService>();
            //Notificator
            services.AddScoped<INotificator, Notificator>();

            return services;
        }
    }
```
Dessa forma, mantemos todas as resoluções das dependências em um só local e não espalhado na classe de Startup.
Agora, para colocar essa classe em funcionamento, simplesmente adicione esta linha de código dentro do método [ConfigureServices] da classe [Startup]:

```csharp
services.ResolveDependencies();
```
Pronto, todas as nossas dependências estão resolvidas.

Agora vamos partir para criação das nossas controllers. Para não perder o costume, criaremos a base controller, que também irá implementar tudo em comum que as próximas controllers irão precisar.

Dentro da camada de API, crie uma pasta chamada Controllers e dentro dela crie uma classe chamada MainController.

>MainController
```csharp
[ApiController]
    public abstract class MainController : ControllerBase
    {
        private readonly INotificator _notificator;

        protected MainController(INotificator notificator)
        {
            _notificator = notificator;
        }

        protected bool ValidOperation()
        {
            return !_notificator.HasNotification();
        }

        protected ActionResult CustomResponse(object result = null)
        {
            if (ValidOperation())
            {
                return Ok(new
                {
                    success = true,
                    data = result
                });
            }

            return BadRequest(new
            {
                success = false,
                errors = _notificator.ObtainNotifications().Select(n => n.Mensagem)
            });
        }

        protected ActionResult CustomResponse(ModelStateDictionary modelState)
        {
            if (!modelState.IsValid) NotificarErroModelInvalida(modelState);
            return CustomResponse();
        }

        protected void NotificarErroModelInvalida(ModelStateDictionary modelState)
        {
            var erros = modelState.Values.SelectMany(e => e.Errors);
            foreach (var erro in erros)
            {
                var errorMsg = erro.Exception == null ? erro.ErrorMessage : erro.Exception.Message;
                NotificarErro(errorMsg);
            }
        }
        protected void NotificarErro(string mensagem)
        {
            _notificator.Handle(new Notification(mensagem));
        }
    }
```
Agora que temos nossa controller base, vamos criar as controllers de cartão de crédito e de cliente.

Dentro da pasta Controllers, crie as controllers com os nomes ClientController e CreditCardController.

> CreditCardController
```csharp
[Route("api/v1/credit-card")]
    public class CreditCardController : MainController
    {
        private readonly ICreditCardRepository _creditCardRepository;
        private readonly ICreditCardService _creditCardService;
        private readonly IClientRepository _clientRepository;

        public CreditCardController(INotificator notificator,
            ICreditCardRepository creditCardRepository,
            ICreditCardService creditCardService, 
            IClientRepository clientRepository) : base(notificator)
        {
            _creditCardRepository = creditCardRepository;
            _creditCardService = creditCardService;
            _clientRepository = clientRepository;
        }
        [HttpPost]
        public async Task<ActionResult<CreateCreditCardCommand>> Create(string email,
            CreateCreditCardCommand command)
        {
            if (!ModelState.IsValid) return CustomResponse(ModelState);
            
            if (!_clientRepository.Search(c => c.Email == email).Result.Any()) return NotFound();
           
            var creditCard = new CreditCard(command.SecurityCode, command.ClientId);

            await _creditCardService.Create(creditCard);

            return CustomResponse(creditCard);
        }

        [HttpGet]
        public async Task<IEnumerable<CreditCard>> ListAllByEmail(string email)
        {
            return await _creditCardRepository.GetAllByEmail(email);
        }
    }
```
>ClientController - Crud
```csharp
[Route("api/v1/clients")]
    public class ClientController : MainController
    {
        private readonly IClientRepository _clientRepository;
        private readonly IClientService _clientService;

        public ClientController(INotificator notificator,
            IClientRepository clientRepository,
            IClientService clientService) : base(notificator)
        {
            _clientRepository = clientRepository;
            _clientService = clientService;
        }

        [HttpPost]
        public async Task<ActionResult<CreateClientCommand>> Create(CreateClientCommand command)
        {
            if (!ModelState.IsValid) return CustomResponse(ModelState);

            var client = new Client(command.Name, command.Email, command.Password, command.ConfirmPassword);

            await _clientService.Create(client);

            return CustomResponse(client);
        }

        [HttpGet]
        public async Task<IEnumerable<Client>> List()
        {
            return await _clientRepository.GetAll();
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<UpdateClientCommand>> Update(Guid id,
            UpdateClientCommand command)
        {
            if (!ModelState.IsValid) return CustomResponse(ModelState);

            if (id != command.Id) return BadRequest(command);

            var client = await _clientRepository.GetByIdNoTracking(id);

            client.UpdateClient(command.Name, command.Email, command.Password, command.ConfirmPassword);
            
            await _clientService.Update(client);

            return CustomResponse(client);
        }

        [HttpDelete("{id:guid}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            var client = await _clientRepository.GetByIdNoTracking(id);

            if (client == null) return NotFound();

            await _clientService.Delete(id);

            return CustomResponse();
        }
    }
```

link do arquivo pdf desse "blog post": https://drive.google.com/file/d/1pk6qX1IavU5BpKCTweBATWuvyXUTdtUl/view?usp=sharing
