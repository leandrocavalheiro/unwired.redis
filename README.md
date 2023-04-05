# Unwired Redis

Project developed with the objective of making access to redis even easier. Making it easy to choose which database to record, automated expiration refresh, bulksave and more.

Compatibility: .Net 7<br>

- 🌐 [NuGet Package](https://www.nuget.org/packages/Unwired.Redis)

## 🚀 Using the lib

> #### **Step 1** - Intall Package<br>
>- **Package Manager**
>```bash
>$ Install-Package Unwired.Redis
>```
>- **.Net CLI**
>```bash
>$ dotnet add package Unwired.Redis
>```
 
 >#### Step 2 - AppSettings Configure
>Settings keys for lib usage:
>**URedis**: Settings group for Unwired Redis
>**Password**: If the database has a password for use, it must be entered here.
>**Server**: Address ( Ip:Port ) of the Redis server.
>**SSL**: Active SSL.
>Example:.
>```bash
>"URedis": {
>   "Password": "",
>   "Server": "localhost:6379",
>   "SSL": false
>}
>```

> #### Step 3 - Service Register
>In the project's program.cs, register the service in the ConfigureServices method:
>```bash
>$ IHost host = Host.CreateDefaultBuilder(args)
>   .ConfigureServices(services =>
>   {
>       var configuration = services.BuildServiceProvider().GetService<IConfiguration>();
>       services.AddURedis(configuration);
>       services.AddHostedService<Worker>();
>   })
>   .Build();
>```    

> #### Step 4 - Using the service
>In the constructor of the class that will use the service, inject the URedis by Dependency Injection:
>```bash
>       private readonly IURedis _uRedis;
>       public Worker(IURedis uRedis)
>       {
>           _uRedis = uRedis;
>       }
>```      
>Now the service is available for use anywhere in the class:<br><br>

>Save a registry in Redis
>```bash
> await _uRedis.SaveAsync("myObjectKey", "myObjectValue");
>```
>The SaveAsync structure:<br><br>
`Task<(bool sucess, string codeError, string error)> SaveAsync(string key, object value, string prefixKey = "", uint? expiresInMinutes = 1440, string separatorKeys = "|", int database = -1)`

>- **key**: Key of record to save.
>- **value**: value of record to save.
>- **prefixKey**: In cases where you want to put a prefix to differentiate record types. Default is empty.
>- **expiresInMinutes**: In how many minutes should the registry expire. Null for never expires. Default is 1440 ( 24 hours ).
>- **separatorKeys**: if you want to use a separator between the prefix and the key. Default is |. Example: prefixKey|key.
>- **database**: Database number where the record will be stored.
>