packages\NuGet.CommandLine.3.4.4-rtm-final\tools\nuget pack Jal.Router\Jal.Router.csproj -Properties "Configuration=Release;Platform=AnyCPU;OutputPath=bin\Release" -Build -IncludeReferencedProjects -OutputDirectory Jal.Router.Nuget

packages\NuGet.CommandLine.3.4.4-rtm-final\tools\nuget pack Jal.Router.Installer\Jal.Router.Installer.csproj -Properties "Configuration=Release;Platform=AnyCPU;OutputPath=bin\Release" -Build -IncludeReferencedProjects -OutputDirectory Jal.Router.Nuget

packages\NuGet.CommandLine.3.4.4-rtm-final\tools\nuget pack Jal.Router.AzureServiceBus\Jal.Router.AzureServiceBus.csproj -Properties "Configuration=Release;Platform=AnyCPU;OutputPath=bin\Release" -Build -IncludeReferencedProjects -OutputDirectory Jal.Router.Nuget

packages\NuGet.CommandLine.3.4.4-rtm-final\tools\nuget pack Jal.Router.LightInject.Installer\Jal.Router.LightInject.Installer.csproj -Properties "Configuration=Release;Platform=AnyCPU;OutputPath=bin\Release" -Build -IncludeReferencedProjects -OutputDirectory Jal.Router.Nuget

packages\NuGet.CommandLine.3.4.4-rtm-final\tools\nuget pack Jal.Router.AzureServiceBus.Installer\Jal.Router.AzureServiceBus.Installer.csproj -Properties "Configuration=Release;Platform=AnyCPU;OutputPath=bin\Release" -Build -IncludeReferencedProjects -OutputDirectory Jal.Router.Nuget

packages\NuGet.CommandLine.3.4.4-rtm-final\tools\nuget pack Jal.Router.AzureServiceBus.LightInject.Installer\Jal.Router.AzureServiceBus.LightInject.Installer.csproj -Properties "Configuration=Release;Platform=AnyCPU;OutputPath=bin\Release" -Build -IncludeReferencedProjects -OutputDirectory Jal.Router.Nuget
pause;