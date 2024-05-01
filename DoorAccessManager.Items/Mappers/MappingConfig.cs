using DoorAccessManager.Items.Entities;
using DoorAccessManager.Items.Models.Responses;
using Mapster;

namespace DoorAccessManager.Items.Mappers
{
    public class MappingConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<Door, DoorResponse>()
                .Map(dest => dest.DoorRoles, src => src.DoorRoles.Select(x => x.Role.Name).ToList());

            config.NewConfig<DoorAccessLog, DoorAccessLogResponse>()
                .Map(dest => dest.FullName, src => src.User.Name)
                .Map(dest => dest.Username, src => src.User.Username)
                .Map(dest => dest.DoorName, src => src.Door.Name);
        }
    }
}
