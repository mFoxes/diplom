using System.Reflection;
using AutoMapper;
using GrandmaApi.Models;
using LdapConnector;

namespace GrandmaApi.Mappers;

public class AppMappingProfile : Profile
{
    public AppMappingProfile()
    {
        var types = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .Where(a => typeof(IMapping).IsAssignableFrom(a) && a.IsClass);
        foreach (var type in types)
        {
            Activator.CreateInstance(type, this);
        }
    }
}